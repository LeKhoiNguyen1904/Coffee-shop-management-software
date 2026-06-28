-- ========================================
-- FILE: ADD MISSING OLS PROCEDURES
-- ========================================
-- This file adds missing procedures for OLS functionality
-- Run as: C##CAFE_APP/Cafe@123
-- ========================================

SET SERVEROUTPUT ON;
SET VERIFY OFF;

PROMPT ========================================
PROMPT ADDING MISSING OLS PROCEDURES
PROMPT ========================================

PROMPT Creating procedure: them_nguyenlieu_secure...
CREATE OR REPLACE PROCEDURE them_nguyenlieu_secure(
  p_tennl IN VARCHAR2,
  p_donvi IN VARCHAR2,
  p_soluongton IN DECIMAL,
  p_security_level IN VARCHAR2 DEFAULT 'PUBLIC',
  p_mancc_chinh IN NUMBER DEFAULT NULL
)
AS
  v_manl NUMBER;
BEGIN
  -- Get next MANL from sequence
  SELECT NVL(MAX(MANL), 0) + 1 INTO v_manl FROM NGUYENLIEU;
  
  -- Insert new nguyenlieu
  INSERT INTO NGUYENLIEU(
    MANL,
    TENNL,
    DONVI,
    SOLUONGTON,
    MANCC_CHINH,
    SECURITY_LEVEL
  ) VALUES (
    v_manl,
    p_tennl,
    p_donvi,
    p_soluongton,
    p_mancc_chinh,
    p_security_level
  );
  
  COMMIT;
  
  DBMS_OUTPUT.PUT_LINE(' Da them nguyen lieu: ' || p_tennl || ' (Level: ' || p_security_level || ')');
  
EXCEPTION
  WHEN DUP_VAL_ON_INDEX THEN
    RAISE_APPLICATION_ERROR(-20001, 'Lỗi thêm nguyên liệu: Tên nguyên liệu đã tồn tại');
  WHEN OTHERS THEN
    RAISE_APPLICATION_ERROR(-20002, 'Lỗi thêm nguyên liệu: ' || SQLERRM);
END them_nguyenlieu_secure;
/

PROMPT Creating procedure: them_mon_secure (fix duplicate issue)...
CREATE OR REPLACE PROCEDURE them_mon_secure(
  p_tenmon IN VARCHAR2,
  p_dongia IN DECIMAL,
  p_motat IN VARCHAR2 DEFAULT NULL,
  p_trangthai IN VARCHAR2 DEFAULT 'ACTIVE'
)
AS
  v_mamon NUMBER;
  v_count NUMBER;
BEGIN
  -- Check if mon already exists
  SELECT COUNT(*) INTO v_count 
  FROM MON 
  WHERE LOWER(TENMON) = LOWER(p_tenmon);
  
  IF v_count > 0 THEN
    RAISE_APPLICATION_ERROR(-20003, 'Lỗi thêm món: Tên món đã tồn tại');
  END IF;
  
  -- Get next MAMON
  SELECT NVL(MAX(MAMON), 0) + 1 INTO v_mamon FROM MON;
  
  -- Insert new mon
  INSERT INTO MON(
    MAMON,
    TENMON,
    DONGIA,
    MOTAT,
    TRANGTHAI
  ) VALUES (
    v_mamon,
    p_tenmon,
    p_dongia,
    p_motat,
    p_trangthai
  );
  
  COMMIT;
  
  DBMS_OUTPUT.PUT_LINE(' Da them mon: ' || p_tenmon);
  
EXCEPTION
  WHEN OTHERS THEN
    RAISE_APPLICATION_ERROR(-20004, 'Lỗi thêm món: ' || SQLERRM);
END them_mon_secure;
/

PROMPT Creating function: get_nguyenlieu_by_role...
CREATE OR REPLACE FUNCTION get_nguyenlieu_by_role
RETURN SYS_REFCURSOR
AS
  v_cursor SYS_REFCURSOR;
  v_role VARCHAR2(30);
BEGIN
  -- Try to get role from context, default to PUBLIC if not set
  BEGIN
    v_role := pkg_context.get_role();
  EXCEPTION WHEN OTHERS THEN
    v_role := 'NHANVIEN';
  END;
  
  IF v_role = 'ADMIN' THEN
    OPEN v_cursor FOR SELECT * FROM NGUYENLIEU;
  ELSIF v_role = 'QUANLY' THEN
    OPEN v_cursor FOR 
      SELECT * FROM NGUYENLIEU 
      WHERE SECURITY_LEVEL IN ('PUBLIC', 'INTERNAL');
  ELSIF v_role = 'THUKHO' THEN
    OPEN v_cursor FOR SELECT * FROM NGUYENLIEU;
  ELSE
    OPEN v_cursor FOR 
      SELECT * FROM NGUYENLIEU 
      WHERE SECURITY_LEVEL = 'PUBLIC';
  END IF;
  
  RETURN v_cursor;
EXCEPTION
  WHEN OTHERS THEN
    -- Return empty cursor if error
    OPEN v_cursor FOR SELECT * FROM NGUYENLIEU WHERE 1=0;
    RETURN v_cursor;
END get_nguyenlieu_by_role;
/

PROMPT Creating function: get_security_level_statistics...
CREATE OR REPLACE FUNCTION get_security_level_statistics
RETURN SYS_REFCURSOR
AS
  v_cursor SYS_REFCURSOR;
BEGIN
  OPEN v_cursor FOR
  SELECT 
    SECURITY_LEVEL,
    COUNT(*) AS SO_LUONG,
    SUM(SOLUONGTON) AS TONG_TON_KHO,
    CASE 
      WHEN SECURITY_LEVEL = 'PUBLIC' THEN 'Công khai'
      WHEN SECURITY_LEVEL = 'INTERNAL' THEN 'Nội bộ'
      WHEN SECURITY_LEVEL = 'CONFIDENTIAL' THEN 'Mật'
      ELSE 'Không xác định'
    END AS MUC_DO_BAO_MAT
  FROM NGUYENLIEU
  GROUP BY SECURITY_LEVEL
  ORDER BY 
    CASE 
      WHEN SECURITY_LEVEL = 'PUBLIC' THEN 1
      WHEN SECURITY_LEVEL = 'INTERNAL' THEN 2
      WHEN SECURITY_LEVEL = 'CONFIDENTIAL' THEN 3
      ELSE 4
    END;
  
  RETURN v_cursor;
EXCEPTION
  WHEN OTHERS THEN
    -- Return empty cursor if error
    OPEN v_cursor FOR 
    SELECT 'PUBLIC' SECURITY_LEVEL, 0 SO_LUONG, 0 TONG_TON_KHO, 'Công khai' MUC_DO_BAO_MAT 
    FROM DUAL WHERE 1=0;
    RETURN v_cursor;
END get_security_level_statistics;
/

PROMPT Creating procedure: update_security_level...
CREATE OR REPLACE PROCEDURE update_security_level(
  p_manl IN NUMBER,
  p_security_level IN VARCHAR2
)
AS
BEGIN
  UPDATE NGUYENLIEU
  SET SECURITY_LEVEL = p_security_level
  WHERE MANL = p_manl;
  
  IF SQL%ROWCOUNT = 0 THEN
    RAISE_APPLICATION_ERROR(-20005, 'Nguyên liệu không tồn tại');
  END IF;
  
  COMMIT;
  
  DBMS_OUTPUT.PUT_LINE(' Da cap nhat security level cho MANL: ' || p_manl);
  
EXCEPTION
  WHEN OTHERS THEN
    RAISE_APPLICATION_ERROR(-20006, 'Lỗi cập nhật: ' || SQLERRM);
END update_security_level;
/

PROMPT Creating function: get_security_level...
CREATE OR REPLACE FUNCTION get_security_level(p_manl IN NUMBER)
RETURN VARCHAR2
AS
  v_level VARCHAR2(20);
BEGIN
  SELECT SECURITY_LEVEL INTO v_level
  FROM NGUYENLIEU
  WHERE MANL = p_manl;
  
  RETURN v_level;
EXCEPTION
  WHEN NO_DATA_FOUND THEN
    RETURN NULL;
  WHEN OTHERS THEN
    RETURN NULL;
END get_security_level;
/

PROMPT Creating procedure: dang_xuat (logout)...
CREATE OR REPLACE PROCEDURE dang_xuat(
  p_username IN VARCHAR2,
  p_ip IN VARCHAR2 DEFAULT NULL
)
AS
  PRAGMA AUTONOMOUS_TRANSACTION;
BEGIN
  -- Log logout action to audit log if table exists
  BEGIN
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
      p_username,
      'LOGOUT',
      'SESSION',
      p_username,
      'Logged in',
      'Logged out',
      SYSDATE,
      p_ip
    );
    COMMIT;
  EXCEPTION WHEN OTHERS THEN
    NULL; -- Ignore if audit table doesn't exist
  END;
  
  -- Clear session context
  BEGIN
    DELETE FROM SESSION_CONTEXT 
    WHERE CURRENT_USERNAME = p_username;
    COMMIT;
  EXCEPTION WHEN OTHERS THEN
    NULL; -- Ignore if session context table doesn't exist
  END;
  
  DBMS_OUTPUT.PUT_LINE(' User ' || p_username || ' logged out successfully');
  
EXCEPTION
  WHEN OTHERS THEN
    RAISE_APPLICATION_ERROR(-20099, 'Lỗi đăng xuất: ' || SQLERRM);
END dang_xuat;
/

PROMPT Creating function: get_next_mamon...
CREATE OR REPLACE FUNCTION get_next_mamon
RETURN NUMBER
AS
  v_next NUMBER;
BEGIN
  SELECT NVL(MAX(MAMON), 0) + 1 INTO v_next FROM MON;
  RETURN v_next;
EXCEPTION
  WHEN OTHERS THEN
    RETURN 1;
END get_next_mamon;
/

PROMPT ========================================
PROMPT COMPLETED: All missing procedures created!
PROMPT ========================================
