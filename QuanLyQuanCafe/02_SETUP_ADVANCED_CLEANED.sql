-- ========================================
-- FILE 3: SETUP ADVANCED - VPD, OLS, AUDIT (CLEANED)
-- ========================================
-- Chạy file này với user: CAFEAPP/Cafe@123
-- Sau khi đã chạy file 01_SETUP_CORE.sql
-- ========================================

SET SERVEROUTPUT ON;
SET VERIFY OFF;

PROMPT ========================================
PROMPT FILE 3: SETUP ADVANCED - VPD, OLS, AUDIT (CLEANED)
PROMPT ========================================
PROMPT 
PROMPT Bước này sẽ:
PROMPT 1. Tạo package crypto đơn giản (chỉ 3 functions)
PROMPT 2. Cài đặt VPD (Virtual Private Database)
PROMPT 3. Mô ph�?ng OLS (Oracle Label Security)
PROMPT 4. Tạo audit triggers
PROMPT 
PROMPT ========================================

PROMPT *** BƯ�?C 1: PACKAGE MÃ HÓA �?ƠN GIẢN ***

PROMPT 1.1: Tạo package crypto_v2 (simplified - chỉ IV encryption)...
CREATE OR REPLACE PACKAGE pkg_crypto_v2 AS
  -- Tạo dữ liệu ngẫu nhiên
  FUNCTION gen_random_raw(p_length NUMBER) RETURN RAW;
  
  -- Mã hóa với IV trả v�?
  FUNCTION aes_encrypt_with_iv(
    p_text VARCHAR2, 
    p_key RAW, 
    p_iv OUT RAW
  ) RETURN RAW;
  
  -- Giải mã với IV cụ thể
  FUNCTION aes_decrypt_with_iv(
    p_encrypted RAW, 
    p_key RAW, 
    p_iv RAW
  ) RETURN VARCHAR2;
  
END pkg_crypto_v2;
/

CREATE OR REPLACE PACKAGE BODY pkg_crypto_v2 AS
  
  FUNCTION gen_random_raw(p_length NUMBER) RETURN RAW IS
    v_random RAW(2000);
  BEGIN
    v_random := SYS.DBMS_CRYPTO.RANDOMBYTES(p_length);
    RETURN v_random;
  END;

  FUNCTION aes_encrypt_with_iv(
    p_text VARCHAR2, 
    p_key RAW, 
    p_iv OUT RAW
  ) RETURN RAW IS
    v_encrypted RAW(2000);
  BEGIN
    -- Tạo IV ngẫu nhiên
    p_iv := gen_random_raw(16);
    
    -- Mã hóa
    v_encrypted := SYS.DBMS_CRYPTO.ENCRYPT(
      UTL_RAW.CAST_TO_RAW(p_text),
      SYS.DBMS_CRYPTO.ENCRYPT_AES256 + SYS.DBMS_CRYPTO.CHAIN_CBC + SYS.DBMS_CRYPTO.PAD_PKCS5,
      p_key, 
      p_iv
    );
    
    RETURN v_encrypted;
  END;

  FUNCTION aes_decrypt_with_iv(
    p_encrypted RAW, 
    p_key RAW, 
    p_iv RAW
  ) RETURN VARCHAR2 IS
  BEGIN
    RETURN UTL_RAW.CAST_TO_VARCHAR2(
      SYS.DBMS_CRYPTO.DECRYPT(
        p_encrypted,
        SYS.DBMS_CRYPTO.ENCRYPT_AES256 + SYS.DBMS_CRYPTO.CHAIN_CBC + SYS.DBMS_CRYPTO.PAD_PKCS5,
        p_key, 
        p_iv
      )
    );
  END;
  
END pkg_crypto_v2;
/

PROMPT  ✓ Tạo package pkg_crypto_v2 (simplified)

PROMPT *** BƯ�?C 2: CÀI �?ẶT VPD (VIRTUAL PRIVATE DATABASE) ***

PROMPT 2.1: Tạo bảng lưu context...
BEGIN
  EXECUTE IMMEDIATE 'DROP TABLE SESSION_CONTEXT';
EXCEPTION WHEN OTHERS THEN NULL;
END;
/

CREATE GLOBAL TEMPORARY TABLE SESSION_CONTEXT (
  SESSION_ID NUMBER DEFAULT SYS_CONTEXT('USERENV', 'SESSIONID'),
  CURRENT_MANV NUMBER,
  CURRENT_ROLE VARCHAR2(30),
  CURRENT_USERNAME VARCHAR2(50),
  LAST_UPDATE DATE DEFAULT SYSDATE,
  IP_ADDRESS VARCHAR2(50),
  LOGIN_TIME DATE DEFAULT SYSDATE
) ON COMMIT PRESERVE ROWS;

PROMPT  ✓ Tạo bảng SESSION_CONTEXT

PROMPT 2.2: Tạo package quản lý context...
CREATE OR REPLACE PACKAGE pkg_context AS
  -- Thiết lập context
  PROCEDURE set_manv(p_manv NUMBER);
  PROCEDURE set_role(p_role VARCHAR2);
  PROCEDURE set_username(p_username VARCHAR2);
  PROCEDURE set_ip(p_ip VARCHAR2);
  
  -- Xóa context
  PROCEDURE clear_context;
  
  -- Lấy thông tin context
  FUNCTION get_manv RETURN NUMBER;
  FUNCTION get_role RETURN VARCHAR2;
  FUNCTION get_username RETURN VARCHAR2;
  FUNCTION get_ip RETURN VARCHAR2;
  
  -- Kiểm tra quy�?n
  FUNCTION has_role(p_role VARCHAR2) RETURN BOOLEAN;
  FUNCTION is_admin RETURN BOOLEAN;
  FUNCTION is_manager RETURN BOOLEAN;
  FUNCTION is_staff RETURN BOOLEAN;
  
  -- Log context
  PROCEDURE log_context_change(p_action VARCHAR2);
  
END pkg_context;
/

CREATE OR REPLACE PACKAGE BODY pkg_context AS
  
  PROCEDURE set_manv(p_manv NUMBER) IS
    v_count NUMBER;
  BEGIN
    SELECT COUNT(*) INTO v_count 
    FROM SESSION_CONTEXT 
    WHERE SESSION_ID = SYS_CONTEXT('USERENV', 'SESSIONID');
    
    IF v_count > 0 THEN
      UPDATE SESSION_CONTEXT 
      SET CURRENT_MANV = p_manv, LAST_UPDATE = SYSDATE
      WHERE SESSION_ID = SYS_CONTEXT('USERENV', 'SESSIONID');
    ELSE
      INSERT INTO SESSION_CONTEXT (CURRENT_MANV) VALUES (p_manv);
    END IF;
    COMMIT;
  END;

  PROCEDURE set_role(p_role VARCHAR2) IS
    v_count NUMBER;
  BEGIN
    SELECT COUNT(*) INTO v_count 
    FROM SESSION_CONTEXT 
    WHERE SESSION_ID = SYS_CONTEXT('USERENV', 'SESSIONID');
    
    IF v_count > 0 THEN
      UPDATE SESSION_CONTEXT 
      SET CURRENT_ROLE = p_role, LAST_UPDATE = SYSDATE
      WHERE SESSION_ID = SYS_CONTEXT('USERENV', 'SESSIONID');
    ELSE
      INSERT INTO SESSION_CONTEXT (CURRENT_ROLE) VALUES (p_role);
    END IF;
    COMMIT;
  END;
  
  PROCEDURE set_username(p_username VARCHAR2) IS
    v_count NUMBER;
  BEGIN
    SELECT COUNT(*) INTO v_count 
    FROM SESSION_CONTEXT 
    WHERE SESSION_ID = SYS_CONTEXT('USERENV', 'SESSIONID');
    
    IF v_count > 0 THEN
      UPDATE SESSION_CONTEXT 
      SET CURRENT_USERNAME = p_username, LAST_UPDATE = SYSDATE
      WHERE SESSION_ID = SYS_CONTEXT('USERENV', 'SESSIONID');
    ELSE
      INSERT INTO SESSION_CONTEXT (CURRENT_USERNAME) VALUES (p_username);
    END IF;
    COMMIT;
  END;
  
  PROCEDURE set_ip(p_ip VARCHAR2) IS
    v_count NUMBER;
  BEGIN
    SELECT COUNT(*) INTO v_count 
    FROM SESSION_CONTEXT 
    WHERE SESSION_ID = SYS_CONTEXT('USERENV', 'SESSIONID');
    
    IF v_count > 0 THEN
      UPDATE SESSION_CONTEXT 
      SET IP_ADDRESS = p_ip, LAST_UPDATE = SYSDATE
      WHERE SESSION_ID = SYS_CONTEXT('USERENV', 'SESSIONID');
    ELSE
      INSERT INTO SESSION_CONTEXT (IP_ADDRESS) VALUES (p_ip);
    END IF;
    COMMIT;
  END;

  PROCEDURE clear_context IS
  BEGIN
    DELETE FROM SESSION_CONTEXT 
    WHERE SESSION_ID = SYS_CONTEXT('USERENV', 'SESSIONID');
    COMMIT;
  END;

  FUNCTION get_manv RETURN NUMBER IS
    v_manv NUMBER;
  BEGIN
    SELECT CURRENT_MANV INTO v_manv 
    FROM SESSION_CONTEXT 
    WHERE SESSION_ID = SYS_CONTEXT('USERENV', 'SESSIONID');
    RETURN v_manv;
  EXCEPTION
    WHEN NO_DATA_FOUND THEN RETURN NULL;
  END;

  FUNCTION get_role RETURN VARCHAR2 IS
    v_role VARCHAR2(30);
  BEGIN
    SELECT CURRENT_ROLE INTO v_role 
    FROM SESSION_CONTEXT 
    WHERE SESSION_ID = SYS_CONTEXT('USERENV', 'SESSIONID');
    RETURN v_role;
  EXCEPTION
    WHEN NO_DATA_FOUND THEN RETURN NULL;
  END;
  
  FUNCTION get_username RETURN VARCHAR2 IS
    v_username VARCHAR2(50);
  BEGIN
    SELECT CURRENT_USERNAME INTO v_username 
    FROM SESSION_CONTEXT 
    WHERE SESSION_ID = SYS_CONTEXT('USERENV', 'SESSIONID');
    RETURN v_username;
  EXCEPTION
    WHEN NO_DATA_FOUND THEN RETURN NULL;
  END;
  
  FUNCTION get_ip RETURN VARCHAR2 IS
    v_ip VARCHAR2(50);
  BEGIN
    SELECT IP_ADDRESS INTO v_ip 
    FROM SESSION_CONTEXT 
    WHERE SESSION_ID = SYS_CONTEXT('USERENV', 'SESSIONID');
    RETURN v_ip;
  EXCEPTION
    WHEN NO_DATA_FOUND THEN RETURN NULL;
  END;
  
  FUNCTION has_role(p_role VARCHAR2) RETURN BOOLEAN IS
    v_current_role VARCHAR2(30);
  BEGIN
    v_current_role := get_role;
    RETURN v_current_role = p_role;
  END;
  
  FUNCTION is_admin RETURN BOOLEAN IS
  BEGIN
    RETURN has_role('ADMIN');
  END;
  
  FUNCTION is_manager RETURN BOOLEAN IS
  BEGIN
    RETURN has_role('QUANLY');
  END;
  
  FUNCTION is_staff RETURN BOOLEAN IS
  BEGIN
    RETURN has_role('NHANVIEN') OR has_role('THUKHO');
  END;
  
  PROCEDURE log_context_change(p_action VARCHAR2) IS
    PRAGMA AUTONOMOUS_TRANSACTION;
  BEGIN
    INSERT INTO AUDIT_LOG(USERNAME, ACTION_TYPE, TABLE_NAME, RECORD_ID, ACTION_TIME, IP_ADDRESS)
    VALUES (get_username(), 'CONTEXT_' || p_action, 'SESSION_CONTEXT', 
            TO_CHAR(SYS_CONTEXT('USERENV', 'SESSIONID')), SYSDATE, get_ip());
    COMMIT;
  END;
  
END pkg_context;
/

PROMPT  ✓ Tạo package pkg_context

PROMPT 2.3: Cập nhật function dang_nhap cho VPD...
CREATE OR REPLACE FUNCTION dang_nhap_v2(
  p_username IN VARCHAR2,
  p_password IN VARCHAR2,
  p_ip IN VARCHAR2
) RETURN VARCHAR2 AS
  PRAGMA AUTONOMOUS_TRANSACTION;
  v_salt RAW(64);
  v_hash RAW(2000);
  v_stored RAW(2000);
  v_locked_until DATE;
  v_fails NUMBER;
  v_role VARCHAR2(30);
  v_manv NUMBER;
  v_password_expired CHAR(1);
BEGIN
  SELECT SALT, PASSWORD_HASH, LOCKED_UNTIL, LOGIN_FAILS, ROLE_CODE, MANV,
         CASE WHEN PASSWORD_EXPIRES_AT < SYSDATE THEN 'Y' ELSE 'N' END
    INTO v_salt, v_stored, v_locked_until, v_fails, v_role, v_manv, v_password_expired
    FROM TAIKHOAN WHERE USERNAME = p_username;

  IF v_locked_until IS NOT NULL AND v_locked_until > SYSDATE THEN
    INSERT INTO LICHSU_DANGNHAP(USERNAME, MANV, HOATDONG, THOIGIAN, IP_ADDRESS, GHI_CHU)
    VALUES (p_username, v_manv, 'LOGIN_FAIL', SYSDATE, p_ip, 'Tai khoan bi khoa');
    COMMIT;
    RETURN 'LOCKED';
  END IF;
  
  IF v_password_expired = 'Y' THEN
    INSERT INTO LICHSU_DANGNHAP(USERNAME, MANV, HOATDONG, THOIGIAN, IP_ADDRESS, GHI_CHU)
    VALUES (p_username, v_manv, 'LOGIN_FAIL', SYSDATE, p_ip, 'Mat khau da het han');
    COMMIT;
    RETURN 'EXPIRED';
  END IF;

  v_hash := SYS.DBMS_CRYPTO.HASH(
    UTL_RAW.CAST_TO_RAW(p_password || RAWTOHEX(v_salt)),
    SYS.DBMS_CRYPTO.HASH_SH256
  );

  IF v_hash = v_stored THEN
    UPDATE TAIKHOAN SET LAST_LOGIN_AT = SYSDATE, LOGIN_FAILS = 0, LOCKED_UNTIL = NULL 
    WHERE USERNAME = p_username;
    
    INSERT INTO LICHSU_DANGNHAP(USERNAME, MANV, HOATDONG, THOIGIAN, IP_ADDRESS, GHI_CHU)
    VALUES (p_username, v_manv, 'LOGIN_SUCCESS', SYSDATE, p_ip, 'Dang nhap thanh cong');
    
    -- SET CONTEXT CHO VPD
    pkg_context.set_manv(v_manv);
    pkg_context.set_role(v_role);
    pkg_context.set_username(p_username);
    pkg_context.set_ip(p_ip);
    pkg_context.log_context_change('LOGIN');
    
    COMMIT;
    RETURN 'OK:' || v_role;
  ELSE
    v_fails := v_fails + 1;
    UPDATE TAIKHOAN SET LOGIN_FAILS = v_fails WHERE USERNAME = p_username;
    
    INSERT INTO LICHSU_DANGNHAP(USERNAME, MANV, HOATDONG, THOIGIAN, IP_ADDRESS, GHI_CHU)
    VALUES (p_username, v_manv, 'LOGIN_FAIL', SYSDATE, p_ip, 'Sai mat khau');
    
    IF v_fails >= 5 THEN
      UPDATE TAIKHOAN SET LOCKED_UNTIL = SYSDATE + (15/1440) WHERE USERNAME = p_username;
    END IF;
    
    COMMIT;
    RETURN 'FAIL';
  END IF;
EXCEPTION
  WHEN NO_DATA_FOUND THEN
    COMMIT;
    RETURN 'NOUSER';
END;
/

PROMPT  ✓ Tạo function dang_nhap_v2 với VPD context

PROMPT 2.4: Tạo VPD policy functions...
CREATE OR REPLACE FUNCTION vpd_hoadon_policy(
  p_schema VARCHAR2,
  p_object VARCHAR2
) RETURN VARCHAR2 AS
  v_role VARCHAR2(30);
  v_manv NUMBER;
  v_predicate VARCHAR2(2000);
BEGIN
  v_role := pkg_context.get_role();
  v_manv := pkg_context.get_manv();
  
  IF v_role = 'ADMIN' THEN
    RETURN NULL;
  END IF;
  
  IF v_role = 'QUANLY' THEN
    RETURN NULL;
  END IF;
  
  IF v_role = 'NHANVIEN' AND v_manv IS NOT NULL THEN
    v_predicate := 'MANV = ' || v_manv;
    RETURN v_predicate;
  END IF;
  
  IF v_role = 'THUKHO' THEN
    RETURN '1=0';
  END IF;
  
  RETURN '1=0';
END;
/

CREATE OR REPLACE FUNCTION vpd_phieunhap_policy(
  p_schema VARCHAR2,
  p_object VARCHAR2
) RETURN VARCHAR2 AS
  v_role VARCHAR2(30);
  v_manv NUMBER;
BEGIN
  v_role := pkg_context.get_role();
  v_manv := pkg_context.get_manv();
  
  IF v_role IN ('ADMIN', 'QUANLY') THEN
    RETURN NULL;
  END IF;
  
  IF v_role = 'THUKHO' AND v_manv IS NOT NULL THEN
    RETURN 'MANV = ' || v_manv;
  END IF;
  
  IF v_role = 'NHANVIEN' THEN
    RETURN '1=0';
  END IF;
  
  RETURN '1=0';
END;
/

CREATE OR REPLACE FUNCTION vpd_nguyenlieu_policy(
  p_schema VARCHAR2,
  p_object VARCHAR2
) RETURN VARCHAR2 AS
  v_role VARCHAR2(30);
  v_predicate VARCHAR2(2000);
BEGIN
  v_role := pkg_context.get_role();
  
  IF v_role = 'ADMIN' THEN
    RETURN NULL;
  END IF;
  
  IF v_role = 'QUANLY' THEN
    RETURN 'SECURITY_LEVEL IN (''PUBLIC'', ''INTERNAL'')';
  END IF;
  
  IF v_role = 'THUKHO' THEN
    RETURN 'SECURITY_LEVEL IN (''PUBLIC'', ''INTERNAL'')';
  END IF;
  
  IF v_role = 'NHANVIEN' THEN
    RETURN 'SECURITY_LEVEL = ''PUBLIC''';
  END IF;
  
  RETURN '1=0';
END;
/

CREATE OR REPLACE FUNCTION vpd_nhanvien_policy(
  p_schema VARCHAR2,
  p_object VARCHAR2
) RETURN VARCHAR2 AS
  v_role VARCHAR2(30);
  v_manv NUMBER;
BEGIN
  v_role := pkg_context.get_role();
  v_manv := pkg_context.get_manv();
  
  IF v_role = 'ADMIN' THEN
    RETURN NULL;
  END IF;
  
  IF v_role = 'QUANLY' THEN
    RETURN '1=1';
  END IF;
  
  IF v_role IN ('NHANVIEN', 'THUKHO') AND v_manv IS NOT NULL THEN
    RETURN 'MANV = ' || v_manv;
  END IF;
  
  RETURN '1=0';
END;
/

PROMPT  ✓ Tạo 4 VPD policy functions

PROMPT 2.5: Tạo view an toàn cho từng role...
CREATE OR REPLACE VIEW v_nhanvien_admin AS
SELECT 
  MANV,
  HOTEN,
  EMAIL,
  SODT,
  NGAYSINH,
  TRANGTHAI,
  NGAYTAO
FROM NHANVIEN;
/

CREATE OR REPLACE VIEW v_nhanvien_quanly AS
SELECT 
  MANV,
  HOTEN,
  EMAIL,
  SODT,
  TRANGTHAI,
  NGAYTAO
FROM NHANVIEN;
/

CREATE OR REPLACE VIEW v_nhanvien_staff AS
SELECT 
  MANV,
  HOTEN,
  EMAIL,
  SODT,
  TRANGTHAI
FROM NHANVIEN
WHERE MANV = pkg_context.get_manv();
/

PROMPT  ✓ Tạo VPD views

PROMPT *** BƯ�?C 3: MÔ PH�?NG OLS (ORACLE LABEL SECURITY) ***

PROMPT 3.1: Tạo view mô ph�?ng OLS...
CREATE OR REPLACE VIEW v_nguyenlieu_admin AS
SELECT 
  MANL,
  TENNL,
  DONVI,
  SOLUONGTON,
  SECURITY_LEVEL,
  MANCC_CHINH,
  CASE SECURITY_LEVEL
    WHEN 'PUBLIC' THEN 'Cong khai'
    WHEN 'INTERNAL' THEN 'Noi bo'
    WHEN 'CONFIDENTIAL' THEN 'Mat'
    ELSE 'Khong xác đinh'
  END AS MUC_DO_BAO_MAT,
  'FULL_ACCESS' AS ACCESS_LEVEL
FROM NGUYENLIEU;
/

CREATE OR REPLACE VIEW v_nguyenlieu_quanly AS
SELECT 
  MANL,
  TENNL,
  DONVI,
  SOLUONGTON,
  SECURITY_LEVEL,
  MANCC_CHINH,
  'Noi bo��' AS MUC_DO_BAO_MAT,
  'LIMITED_ACCESS' AS ACCESS_LEVEL
FROM NGUYENLIEU
WHERE SECURITY_LEVEL IN ('PUBLIC', 'INTERNAL');
/

CREATE OR REPLACE VIEW v_nguyenlieu_nhanvien AS
SELECT 
  MANL,
  TENNL,
  DONVI,
  SOLUONGTON,
  'PUBLIC' AS SECURITY_LEVEL,
  MANCC_CHINH,
  'Cng khai' AS MUC_DO_BAO_MAT,
  'RESTRICTED_ACCESS' AS ACCESS_LEVEL
FROM NGUYENLIEU
WHERE SECURITY_LEVEL = 'PUBLIC';
/

CREATE OR REPLACE VIEW v_nguyenlieu_thukho AS
SELECT 
  MANL,
  TENNL,
  DONVI,
  SOLUONGTON,
  CASE 
    WHEN SECURITY_LEVEL = 'CONFIDENTIAL' THEN 'INTERNAL'
    ELSE SECURITY_LEVEL
  END AS SECURITY_LEVEL,
  MANCC_CHINH,
  CASE 
    WHEN SECURITY_LEVEL = 'CONFIDENTIAL' THEN 'Noi bo��'
    WHEN SECURITY_LEVEL = 'INTERNAL' THEN 'Noi bo��'
    ELSE 'Cong khai'
  END AS MUC_DO_BAO_MAT,
  'INVENTORY_ACCESS' AS ACCESS_LEVEL
FROM NGUYENLIEU;
/

PROMPT 3.2: Tạo function ch�?n view theo role...
CREATE OR REPLACE FUNCTION get_nguyenlieu_by_role
RETURN SYS_REFCURSOR
AS
  v_cursor SYS_REFCURSOR;
  v_role VARCHAR2(30);
BEGIN
  v_role := pkg_context.get_role();
  
  IF v_role = 'ADMIN' THEN
    OPEN v_cursor FOR SELECT * FROM v_nguyenlieu_admin;
  ELSIF v_role = 'QUANLY' THEN
    OPEN v_cursor FOR SELECT * FROM v_nguyenlieu_quanly;
  ELSIF v_role = 'THUKHO' THEN
    OPEN v_cursor FOR SELECT * FROM v_nguyenlieu_thukho;
  ELSIF v_role = 'NHANVIEN' THEN
    OPEN v_cursor FOR SELECT * FROM v_nguyenlieu_nhanvien;
  ELSE
    OPEN v_cursor FOR SELECT * FROM v_nguyenlieu_nhanvien;
  END IF;
  
  RETURN v_cursor;
END;
/

CREATE OR REPLACE FUNCTION get_nhanvien_by_role
RETURN SYS_REFCURSOR
AS
  v_cursor SYS_REFCURSOR;
  v_role VARCHAR2(30);
BEGIN
  v_role := pkg_context.get_role();
  
  IF v_role = 'ADMIN' THEN
    OPEN v_cursor FOR SELECT * FROM v_nhanvien_admin;
  ELSIF v_role = 'QUANLY' THEN
    OPEN v_cursor FOR SELECT * FROM v_nhanvien_quanly;
  ELSE
    OPEN v_cursor FOR SELECT * FROM v_nhanvien_staff;
  END IF;
  
  RETURN v_cursor;
END;
/

PROMPT  ✓ Mô ph�?ng OLS bằng view

PROMPT *** BƯ�?C 4: TẠO AUDIT TRIGGERS ***

PROMPT 4.1: Tạo package helper cho audit...
CREATE OR REPLACE PACKAGE pkg_audit AS
  PROCEDURE log_action(
    p_table_name VARCHAR2,
    p_action_type VARCHAR2,
    p_record_id VARCHAR2,
    p_old_value CLOB DEFAULT NULL,
    p_new_value CLOB DEFAULT NULL
  );
  
  PROCEDURE log_fga(
    p_table_name VARCHAR2,
    p_policy_name VARCHAR2,
    p_sql_text CLOB
  );
  
  FUNCTION get_client_ip RETURN VARCHAR2;
  FUNCTION get_username RETURN VARCHAR2;
  FUNCTION get_session_info RETURN VARCHAR2;
  
  PROCEDURE generate_audit_report(
    p_from_date DATE DEFAULT SYSDATE - 30,
    p_to_date DATE DEFAULT SYSDATE
  );
  
END pkg_audit;
/

CREATE OR REPLACE PACKAGE BODY pkg_audit AS
  
  PROCEDURE log_action(
    p_table_name VARCHAR2,
    p_action_type VARCHAR2,
    p_record_id VARCHAR2,
    p_old_value CLOB DEFAULT NULL,
    p_new_value CLOB DEFAULT NULL
  ) IS
    PRAGMA AUTONOMOUS_TRANSACTION;
    v_username VARCHAR2(100);
    v_ip VARCHAR2(50);
  BEGIN
    v_username := get_username();
    v_ip := get_client_ip();
    
    INSERT INTO AUDIT_LOG(
      USERNAME,
      ACTION_TYPE,
      TABLE_NAME,
      RECORD_ID,
      OLD_VALUE,
      NEW_VALUE,
      ACTION_TIME,
      IP_ADDRESS
    ) VALUES (
      v_username,
      p_action_type,
      p_table_name,
      p_record_id,
      p_old_value,
      p_new_value,
      SYSDATE,
      v_ip
    );
    COMMIT;
  END;
  
  PROCEDURE log_fga(
    p_table_name VARCHAR2,
    p_policy_name VARCHAR2,
    p_sql_text CLOB
  ) IS
    PRAGMA AUTONOMOUS_TRANSACTION;
    v_username VARCHAR2(100);
    v_ip VARCHAR2(50);
  BEGIN
    v_username := get_username();
    v_ip := get_client_ip();
    
    INSERT INTO FGA_LOG(
      USERNAME,
      TABLE_NAME,
      POLICY_NAME,
      SQL_TEXT,
      TIMESTAMP,
      CLIENT_IP
    ) VALUES (
      v_username,
      p_table_name,
      p_policy_name,
      p_sql_text,
      SYSDATE,
      v_ip
    );
    COMMIT;
  END;
  
  FUNCTION get_client_ip RETURN VARCHAR2 IS
  BEGIN
    RETURN SYS_CONTEXT('USERENV', 'IP_ADDRESS');
  END;
  
  FUNCTION get_username RETURN VARCHAR2 IS
    v_ctx_username VARCHAR2(100);
  BEGIN
    v_ctx_username := pkg_context.get_username();
    IF v_ctx_username IS NOT NULL THEN
      RETURN v_ctx_username;
    END IF;
    RETURN SYS_CONTEXT('USERENV', 'SESSION_USER');
  END;
  
  FUNCTION get_session_info RETURN VARCHAR2 IS
  BEGIN
    RETURN 'SID: ' || SYS_CONTEXT('USERENV', 'SID') ||
           ', SessionID: ' || SYS_CONTEXT('USERENV', 'SESSIONID') ||
           ', Host: ' || SYS_CONTEXT('USERENV', 'HOST');
  END;
  
  PROCEDURE generate_audit_report(
    p_from_date DATE DEFAULT SYSDATE - 30,
    p_to_date DATE DEFAULT SYSDATE
  ) IS
    v_count_audit NUMBER;
    v_count_fga NUMBER;
  BEGIN
    SELECT COUNT(*) INTO v_count_audit
    FROM AUDIT_LOG
    WHERE ACTION_TIME BETWEEN p_from_date AND p_to_date;
    
    SELECT COUNT(*) INTO v_count_fga
    FROM FGA_LOG
    WHERE TIMESTAMP BETWEEN p_from_date AND p_to_date;
    
    DBMS_OUTPUT.PUT_LINE('=== B�?O C�?O AUDIT (' || 
                         TO_CHAR(p_from_date, 'DD/MM/YYYY') || ' - ' ||
                         TO_CHAR(p_to_date, 'DD/MM/YYYY') || ') ===');
    DBMS_OUTPUT.PUT_LINE('Tổng số bản ghi Audit Log: ' || v_count_audit);
    DBMS_OUTPUT.PUT_LINE('Tổng số bản ghi FGA Log: ' || v_count_fga);
    DBMS_OUTPUT.PUT_LINE('----------------------------------------');
    
    DBMS_OUTPUT.PUT_LINE('TOP USERS (Audit Log):');
    FOR rec IN (
      SELECT USERNAME, COUNT(*) as ACTION_COUNT
      FROM AUDIT_LOG
      WHERE ACTION_TIME BETWEEN p_from_date AND p_to_date
      GROUP BY USERNAME
      ORDER BY COUNT(*) DESC
      FETCH FIRST 5 ROWS ONLY
    ) LOOP
      DBMS_OUTPUT.PUT_LINE('  ' || rec.USERNAME || ': ' || rec.ACTION_COUNT || ' actions');
    END LOOP;
    
    DBMS_OUTPUT.PUT_LINE('----------------------------------------');
    DBMS_OUTPUT.PUT_LINE('TOP TABLES (Audit Log):');
    FOR rec IN (
      SELECT TABLE_NAME, COUNT(*) as ACTION_COUNT
      FROM AUDIT_LOG
      WHERE ACTION_TIME BETWEEN p_from_date AND p_to_date
      GROUP BY TABLE_NAME
      ORDER BY COUNT(*) DESC
      FETCH FIRST 5 ROWS ONLY
    ) LOOP
      DBMS_OUTPUT.PUT_LINE('  ' || rec.TABLE_NAME || ': ' || rec.ACTION_COUNT || ' actions');
    END LOOP;
    
  END;
  
END pkg_audit;
/

PROMPT 4.2-4.7: Tạo 6 audit triggers...
CREATE OR REPLACE TRIGGER trg_audit_nhanvien
AFTER INSERT OR UPDATE OR DELETE ON NHANVIEN
FOR EACH ROW
DECLARE
  v_action VARCHAR2(20);
  v_old_val CLOB;
  v_new_val CLOB;
BEGIN
  IF INSERTING THEN
    v_action := 'INSERT';
    v_new_val := 'MANV=' || :NEW.MANV || ', HOTEN=' || :NEW.HOTEN || ', EMAIL=' || :NEW.EMAIL || ', SODT=' || :NEW.SODT ||', TRANGTHAI=' || :NEW.TRANGTHAI;
    pkg_audit.log_action('NHANVIEN', v_action, TO_CHAR(:NEW.MANV), NULL, v_new_val);
  ELSIF UPDATING THEN
    v_action := 'UPDATE';
    v_old_val := 'MANV=' || :OLD.MANV || ', HOTEN=' || :OLD.HOTEN || ', EMAIL=' || :OLD.EMAIL || ', SODT=' || :OLD.SODT ||', TRANGTHAI=' || :OLD.TRANGTHAI;
    v_new_val := 'MANV=' || :NEW.MANV || ', HOTEN=' || :NEW.HOTEN || ', EMAIL=' || :NEW.EMAIL || ', SODT=' || :NEW.SODT ||', TRANGTHAI=' || :NEW.TRANGTHAI;
    pkg_audit.log_action('NHANVIEN', v_action, TO_CHAR(:NEW.MANV), v_old_val, v_new_val);
  ELSIF DELETING THEN
    v_action := 'DELETE';
    v_old_val := 'MANV=' || :OLD.MANV || ', HOTEN=' || :OLD.HOTEN || ', EMAIL=' || :OLD.EMAIL || ', SODT=' || :OLD.SODT ||', TRANGTHAI=' || :OLD.TRANGTHAI;
    pkg_audit.log_action('NHANVIEN', v_action, TO_CHAR(:OLD.MANV), v_old_val, NULL);
  END IF;
END;
/

CREATE OR REPLACE TRIGGER trg_audit_nguyenlieu
AFTER INSERT OR UPDATE OR DELETE ON NGUYENLIEU
FOR EACH ROW
DECLARE
  v_action VARCHAR2(20);
  v_old_val CLOB;
  v_new_val CLOB;
BEGIN
  IF INSERTING THEN
    v_action := 'INSERT';
    v_new_val := 'MANL=' || :NEW.MANL || ', TENNL=' || :NEW.TENNL || ', SOLUONGTON=' || :NEW.SOLUONGTON ||', SECURITY_LEVEL=' || :NEW.SECURITY_LEVEL;
    pkg_audit.log_action('NGUYENLIEU', v_action, TO_CHAR(:NEW.MANL), NULL, v_new_val);
  ELSIF UPDATING THEN
    v_action := 'UPDATE';
    v_old_val := 'MANL=' || :OLD.MANL || ', TENNL=' || :OLD.TENNL || ', SOLUONGTON=' || :OLD.SOLUONGTON ||', SECURITY_LEVEL=' || :OLD.SECURITY_LEVEL;
    v_new_val := 'MANL=' || :NEW.MANL || ', TENNL=' || :NEW.TENNL || ', SOLUONGTON=' || :NEW.SOLUONGTON ||', SECURITY_LEVEL=' || :NEW.SECURITY_LEVEL;
    pkg_audit.log_action('NGUYENLIEU', v_action, TO_CHAR(:NEW.MANL), v_old_val, v_new_val);
  ELSIF DELETING THEN
    v_action := 'DELETE';
    v_old_val := 'MANL=' || :OLD.MANL || ', TENNL=' || :OLD.TENNL || ', SOLUONGTON=' || :OLD.SOLUONGTON ||', SECURITY_LEVEL=' || :OLD.SECURITY_LEVEL;
    pkg_audit.log_action('NGUYENLIEU', v_action, TO_CHAR(:OLD.MANL), v_old_val, NULL);
  END IF;
END;
/

CREATE OR REPLACE TRIGGER trg_audit_mon
AFTER INSERT OR UPDATE OR DELETE ON MON
FOR EACH ROW
DECLARE
  v_action VARCHAR2(20);
  v_old_val CLOB;
  v_new_val CLOB;
BEGIN
  IF INSERTING THEN
    v_action := 'INSERT';
    v_new_val := 'MAMON=' || :NEW.MAMON || ', TENMON=' || :NEW.TENMON || ', DONGIA=' || :NEW.DONGIA ||', TRANGTHAI=' || :NEW.TRANGTHAI;
    pkg_audit.log_action('MON', v_action, TO_CHAR(:NEW.MAMON), NULL, v_new_val);
  ELSIF UPDATING THEN
    v_action := 'UPDATE';
    v_old_val := 'MAMON=' || :OLD.MAMON || ', TENMON=' || :OLD.TENMON || ', DONGIA=' || :OLD.DONGIA ||', TRANGTHAI=' || :OLD.TRANGTHAI;
    v_new_val := 'MAMON=' || :NEW.MAMON || ', TENMON=' || :NEW.TENMON || ', DONGIA=' || :NEW.DONGIA ||', TRANGTHAI=' || :NEW.TRANGTHAI;
    pkg_audit.log_action('MON', v_action, TO_CHAR(:NEW.MAMON), v_old_val, v_new_val);
  ELSIF DELETING THEN
    v_action := 'DELETE';
    v_old_val := 'MAMON=' || :OLD.MAMON || ', TENMON=' || :OLD.TENMON || ', DONGIA=' || :OLD.DONGIA ||', TRANGTHAI=' || :OLD.TRANGTHAI;
    pkg_audit.log_action('MON', v_action, TO_CHAR(:OLD.MAMON), v_old_val, NULL);
  END IF;
END;
/

CREATE OR REPLACE TRIGGER trg_audit_hoadon
AFTER INSERT OR UPDATE OR DELETE ON HOADON
FOR EACH ROW
DECLARE
  v_action VARCHAR2(20);
  v_old_val CLOB;
  v_new_val CLOB;
BEGIN
  IF INSERTING THEN
    v_action := 'INSERT';
    v_new_val := 'MAHD=' || :NEW.MAHD || ', MANV=' || :NEW.MANV || ', TONGTIEN=' || :NEW.TONGTIEN ||', TRANGTHAI=' || :NEW.TRANGTHAI;
    pkg_audit.log_action('HOADON', v_action, TO_CHAR(:NEW.MAHD), NULL, v_new_val);
  ELSIF UPDATING THEN
    v_action := 'UPDATE';
    v_old_val := 'MAHD=' || :OLD.MAHD || ', MANV=' || :OLD.MANV || ', TONGTIEN=' || :OLD.TONGTIEN ||', TRANGTHAI=' || :OLD.TRANGTHAI;
    v_new_val := 'MAHD=' || :NEW.MAHD || ', MANV=' || :NEW.MANV || ', TONGTIEN=' || :NEW.TONGTIEN ||', TRANGTHAI=' || :NEW.TRANGTHAI;
    pkg_audit.log_action('HOADON', v_action, TO_CHAR(:NEW.MAHD), v_old_val, v_new_val);
  ELSIF DELETING THEN
    v_action := 'DELETE';
    v_old_val := 'MAHD=' || :OLD.MAHD || ', MANV=' || :OLD.MANV || ', TONGTIEN=' || :OLD.TONGTIEN ||', TRANGTHAI=' || :OLD.TRANGTHAI;
    pkg_audit.log_action('HOADON', v_action, TO_CHAR(:OLD.MAHD), v_old_val, NULL);
  END IF;
END;
/

CREATE OR REPLACE TRIGGER trg_fga_nguyenlieu_internal
AFTER INSERT OR UPDATE ON NGUYENLIEU
FOR EACH ROW
WHEN (NEW.SECURITY_LEVEL = 'INTERNAL' OR NEW.SECURITY_LEVEL = 'CONFIDENTIAL')
DECLARE
  PRAGMA AUTONOMOUS_TRANSACTION;
  v_action VARCHAR2(50);
BEGIN
  IF INSERTING THEN
    v_action := 'INSERT';
  ELSE
    v_action := 'UPDATE';
  END IF;
  
  pkg_audit.log_fga('NGUYENLIEU', 'fga_nguyenlieu_sensitive',
    v_action || ' nguyên liệu sensitive: ' || :NEW.TENNL || ' (Level: ' || :NEW.SECURITY_LEVEL || ')');
EXCEPTION
  WHEN OTHERS THEN NULL;
END;
/

CREATE OR REPLACE TRIGGER trg_fga_hoadon_tongtien
AFTER INSERT OR UPDATE OR DELETE ON HOADON
FOR EACH ROW
DECLARE
  PRAGMA AUTONOMOUS_TRANSACTION;
  v_action VARCHAR2(50);
  v_mahd NUMBER;
  v_tongtien NUMBER;
BEGIN
  IF DELETING THEN
    v_action := 'DELETE';
    v_mahd := :OLD.MAHD;
    v_tongtien := :OLD.TONGTIEN;
  ELSIF UPDATING THEN
    v_action := 'UPDATE';
    v_mahd := :NEW.MAHD;
    v_tongtien := :NEW.TONGTIEN;
  ELSE
    v_action := 'INSERT';
    v_mahd := :NEW.MAHD;
    v_tongtien := :NEW.TONGTIEN;
  END IF;
  
  IF v_tongtien > 1000000 THEN
    pkg_audit.log_fga('HOADON', 'fga_hoadon_tongtien',
    v_action || ' hóa đơn giá trị cao: HD' || v_mahd || ' (Ti�?n: ' || v_tongtien || ')');
  END IF;
EXCEPTION
  WHEN OTHERS THEN NULL;
END;
/

PROMPT  ✓ Tạo 6 audit trigger và 2 FGA trigger

PROMPT 4.8: Tạo view xem audit log...
CREATE OR REPLACE VIEW v_audit_log AS
SELECT 
  ID,
  USERNAME,
  ACTION_TYPE,
  TABLE_NAME,
  RECORD_ID,
  SUBSTR(OLD_VALUE, 1, 200) AS OLD_VALUE_SHORT,
  SUBSTR(NEW_VALUE, 1, 200) AS NEW_VALUE_SHORT,
  TO_CHAR(ACTION_TIME, 'YYYY-MM-DD HH24:MI:SS') AS ACTION_TIME_STR,
  IP_ADDRESS
FROM AUDIT_LOG
ORDER BY ACTION_TIME DESC;
/

CREATE OR REPLACE VIEW v_fga_log AS
SELECT 
  ID,
  USERNAME,
  TABLE_NAME,
  POLICY_NAME,
  SUBSTR(SQL_TEXT, 1, 300) AS SQL_TEXT_SHORT,
  TO_CHAR(TIMESTAMP, 'YYYY-MM-DD HH24:MI:SS') AS AUDIT_TIME,
  CLIENT_IP
FROM FGA_LOG
ORDER BY TIMESTAMP DESC;
/

PROMPT ========================================
PROMPT ✓ HOÀN THÀNH FILE 3: SETUP ADVANCED (CLEANED)!
PROMPT ========================================

SELECT 'Audit triggers' as Item, COUNT(*) as Count FROM user_triggers 
WHERE trigger_name LIKE 'TRG_AUDIT_%' OR trigger_name LIKE 'TRG_FGA_%'
UNION ALL
SELECT 'VPD functions', COUNT(*) FROM user_procedures 
WHERE object_name LIKE 'VPD_%' OR object_name LIKE 'PKG_CONTEXT%'
UNION ALL
SELECT 'OLS views', COUNT(*) FROM user_views 
WHERE view_name LIKE 'V_NGUYENLIEU_%' OR view_name LIKE 'V_NHANVIEN_%'
UNION ALL
SELECT 'Crypto packages', COUNT(*) FROM user_objects 
WHERE object_name LIKE 'PKG_CRYPTO%';
