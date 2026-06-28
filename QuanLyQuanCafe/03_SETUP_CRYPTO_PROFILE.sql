-- ========================================
-- FILE 4: SETUP CRYPTO NÂNG CAO & PROFILE
-- ========================================
-- Ch?y file này v?i user: CAFE_APP/Cafe@123
-- Sau khi ?ã ch?y file 02_SETUP_ADVANCED.sql
-- ========================================

SET SERVEROUTPUT ON;
SET VERIFY OFF;

PROMPT ========================================
PROMPT FILE 4: SETUP CRYPTO NANG CAO & PROFILE
PROMPT ========================================
PROMPT 
PROMPT Buoc nay se:
PROMPT 1. Ma hoa bat doi xung (RSA) 
PROMPT 2. Cai dat FGA (Fine-Grained Auditing) 
PROMPT 3. Quan ly profile va session 
PROMPT 4. Ma hoa lai (Hybrid) 
PROMPT 
PROMPT ========================================

PROMPT *** BUOC 1: MA HOA BAT DOI XUNG (RSA) NÂNG CAO ***

PROMPT 1.1: Tao bang luu khoa RSA...
BEGIN
  EXECUTE IMMEDIATE 'DROP TABLE RSA_KEYS CASCADE CONSTRAINTS';
EXCEPTION WHEN OTHERS THEN NULL;
END;
/

CREATE TABLE RSA_KEYS (
  KEY_ID NUMBER PRIMARY KEY,
  KEY_NAME VARCHAR2(100) UNIQUE NOT NULL,
  KEY_TYPE VARCHAR2(20) DEFAULT 'RSA', -- RSA, ECC
  PUBLIC_KEY CLOB NOT NULL,
  PRIVATE_KEY_ENC RAW(2000), -- Private key ???c mã hóa b?ng AES
  KEY_SIZE NUMBER DEFAULT 2048,
  CREATED_AT DATE DEFAULT SYSDATE,
  CREATED_BY VARCHAR2(100),
  KEY_PURPOSE VARCHAR2(50),
  IS_ACTIVE CHAR(1) DEFAULT 'Y',
  EXPIRY_DATE DATE DEFAULT SYSDATE + 365,
  CONSTRAINT chk_rsa_active CHECK (IS_ACTIVE IN ('Y', 'N'))
);

CREATE SEQUENCE seq_rsa_keys START WITH 1 INCREMENT BY 1;

BEGIN
  EXECUTE IMMEDIATE 'DROP TABLE DIGITAL_SIGNATURES CASCADE CONSTRAINTS';
EXCEPTION WHEN OTHERS THEN NULL;
END;
/

CREATE TABLE DIGITAL_SIGNATURES (
  SIGNATURE_ID NUMBER PRIMARY KEY,
  TABLE_NAME VARCHAR2(50) NOT NULL,
  RECORD_ID VARCHAR2(100) NOT NULL,
  DATA_HASH RAW(2000) NOT NULL,
  SIGNATURE RAW(2000) NOT NULL,
  KEY_ID NUMBER NOT NULL,
  SIGNED_BY VARCHAR2(100),
  SIGNED_AT DATE DEFAULT SYSDATE,
  VERIFIED CHAR(1) DEFAULT 'N',
  VERIFIED_AT DATE,
  VERIFIED_BY VARCHAR2(100),
  CONSTRAINT fk_sig_key FOREIGN KEY (KEY_ID) REFERENCES RSA_KEYS(KEY_ID),
  CONSTRAINT chk_sig_verified CHECK (VERIFIED IN ('Y', 'N'))
);

CREATE SEQUENCE seq_signatures START WITH 1 INCREMENT BY 1;

-- B?ng l?u tr? khóa mã hóa (Key Vault mô ph?ng)
BEGIN
  EXECUTE IMMEDIATE 'DROP TABLE KEY_VAULT CASCADE CONSTRAINTS';
EXCEPTION WHEN OTHERS THEN NULL;
END;
/

CREATE TABLE KEY_VAULT (
  KEY_ID NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  KEY_NAME VARCHAR2(100) UNIQUE NOT NULL,
  KEY_TYPE VARCHAR2(20), -- AES, RSA, HMAC
  KEY_VALUE_ENC RAW(2000), -- Khóa ???c mã hóa
  KEY_IV RAW(16),
  MASTER_KEY_ID NUMBER, -- Khóa chính ?? gi?i mã
  CREATED_AT DATE DEFAULT SYSDATE,
  CREATED_BY VARCHAR2(100),
  KEY_PURPOSE VARCHAR2(50),
  IS_ACTIVE CHAR(1) DEFAULT 'Y',
  LAST_USED DATE
) TABLESPACE SECURE_DATA; -- L?u ? tablespace an toàn

PROMPT  Da tao bang RSA keys va Key Vault

PROMPT 1.2: Tao package ma hoa bat doi xung nang cao...
CREATE OR REPLACE PACKAGE pkg_asymmetric AS
  
  -- ==== RSA KEY MANAGEMENT ====
  -- Tao cap khoa RSA (mo phong nang cao)
  PROCEDURE generate_rsa_keypair(
    p_key_name VARCHAR2,
    p_key_purpose VARCHAR2 DEFAULT 'GENERAL',
    p_key_size NUMBER DEFAULT 2048,
    p_key_id OUT NUMBER
  );
  
  -- Vo hieu hoa khoa
  PROCEDURE deactivate_key(p_key_id NUMBER);
  
  -- Kiem tra khoa con hieu luc
  FUNCTION is_key_valid(p_key_id NUMBER) RETURN BOOLEAN;
  
  -- ==== DIGITAL SIGNATURES ====
  -- Tao chu ky so cho du lieu
  PROCEDURE sign_data(
    p_table_name VARCHAR2,
    p_record_id VARCHAR2,
    p_data CLOB,
    p_key_id NUMBER,
    p_signature_id OUT NUMBER
  );
  
  -- Xac thuc chu ky so
  FUNCTION verify_signature(
    p_signature_id NUMBER
  ) RETURN BOOLEAN;
  
  -- Xac thuc chu ky bat dong bo (cho ung dung)
  PROCEDURE verify_signature_async(
    p_signature_id NUMBER,
    p_verifier VARCHAR2
  );
  
  -- ==== KEY WRAPPING ====
  -- Key wrapping: Ma hoa khoa AES bang RSA
  FUNCTION wrap_aes_key(
    p_aes_key RAW,
    p_rsa_key_id NUMBER
  ) RETURN RAW;
  
  -- Key unwrapping: Giai ma khoa AES
  FUNCTION unwrap_aes_key(
    p_wrapped_key RAW,
    p_rsa_key_id NUMBER
  ) RETURN RAW;
  
  -- ==== KEY VAULT ====
  -- Luu khoa vao Key Vault
  PROCEDURE store_key_in_vault(
    p_key_name VARCHAR2,
    p_key_type VARCHAR2,
    p_key_value RAW,
    p_purpose VARCHAR2
  );
  
  -- Lay khoa tu Key Vault
  FUNCTION retrieve_key_from_vault(
    p_key_name VARCHAR2
  ) RETURN RAW;
  
  -- ==== UTILITY FUNCTIONS ====
  -- Tao hash cho du lieu
  FUNCTION generate_hash(
    p_data CLOB,
    p_algorithm VARCHAR2 DEFAULT 'SHA256'
  ) RETURN RAW;
  
  -- Kiem tra tinh toan ven cua du lieu
  FUNCTION verify_data_integrity(
    p_data CLOB,
    p_expected_hash RAW
  ) RETURN BOOLEAN;
  
END pkg_asymmetric;
/

CREATE OR REPLACE PACKAGE BODY pkg_asymmetric AS
  
  -- ==== RSA KEY MANAGEMENT ====
  PROCEDURE generate_rsa_keypair(
    p_key_name VARCHAR2,
    p_key_purpose VARCHAR2 DEFAULT 'GENERAL',
    p_key_size NUMBER DEFAULT 2048,
    p_key_id OUT NUMBER
  ) IS
    v_public_key CLOB;
    v_private_key_raw RAW(2000);
    v_private_key_enc RAW(2000);
    v_master_key RAW(32);
    v_iv RAW(16);
  BEGIN
    -- Tao ID moi
    p_key_id := seq_rsa_keys.NEXTVAL;
    
    -- Mo phong tao khoa RSA (trong thuc te se dung DBMS_CRYPTO hoac thu vien ngoai)
    -- Tao public key (mo phong)
    v_public_key := '-----BEGIN RSA PUBLIC KEY-----' || CHR(10) ||
                   'MIIBCgKCAQEA' || 
                   RAWTOHEX(SYS.DBMS_CRYPTO.RANDOMBYTES(128)) || 
                   CHR(10) ||
                   '-----END RSA PUBLIC KEY-----';
    
    -- Tao private key (mo phong)
    v_private_key_raw := SYS.DBMS_CRYPTO.RANDOMBYTES(256);
    
    -- Ma hoa private key bang AES (Key Encryption Key)
    -- Lay master key tu config
    SELECT CONFIG_VAL INTO v_master_key 
    FROM APP_CONFIG 
    WHERE CONFIG_KEY = 'AES_KEY';
    
    v_iv := SYS.DBMS_CRYPTO.RANDOMBYTES(16);
    v_private_key_enc := SYS.DBMS_CRYPTO.ENCRYPT(
      v_private_key_raw,
      SYS.DBMS_CRYPTO.ENCRYPT_AES256 + SYS.DBMS_CRYPTO.CHAIN_CBC + SYS.DBMS_CRYPTO.PAD_PKCS5,
      v_master_key,
      v_iv
    );
    
    -- Luu khoa vao bang RSA_KEYS
    INSERT INTO RSA_KEYS(
      KEY_ID, KEY_NAME, KEY_TYPE, PUBLIC_KEY, 
      PRIVATE_KEY_ENC, KEY_SIZE, CREATED_BY, KEY_PURPOSE
    ) VALUES (
      p_key_id, p_key_name, 'RSA', v_public_key,
      v_private_key_enc, p_key_size, USER, p_key_purpose
    );
    
    -- Luu IV vao Key Vault
    store_key_in_vault(
      p_key_name || '_IV',
      'AES',
      v_iv,
      'RSA_PRIVATE_KEY_IV_' || p_key_id
    );
    
    COMMIT;
    
    DBMS_OUTPUT.PUT_LINE(' Da tao cap khoa RSA: ' || p_key_name || 
                        ' (ID: ' || p_key_id || ', Size: ' || p_key_size || ')');
  END;
  
  PROCEDURE deactivate_key(p_key_id NUMBER) IS
  BEGIN
    UPDATE RSA_KEYS 
    SET IS_ACTIVE = 'N',
        EXPIRY_DATE = SYSDATE
    WHERE KEY_ID = p_key_id;
    
    COMMIT;
    
    DBMS_OUTPUT.PUT_LINE(' Da vo hieu hoa khoa ID: ' || p_key_id);
  END;
  
  FUNCTION is_key_valid(p_key_id NUMBER) RETURN BOOLEAN IS
    v_is_active CHAR(1);
    v_expiry_date DATE;
  BEGIN
    SELECT IS_ACTIVE, EXPIRY_DATE 
    INTO v_is_active, v_expiry_date
    FROM RSA_KEYS 
    WHERE KEY_ID = p_key_id;
    
    RETURN v_is_active = 'Y' AND (v_expiry_date IS NULL OR v_expiry_date > SYSDATE);
  EXCEPTION
    WHEN NO_DATA_FOUND THEN
      RETURN FALSE;
  END;
  
  -- ==== DIGITAL SIGNATURES ====
  PROCEDURE sign_data(
    p_table_name VARCHAR2,
    p_record_id VARCHAR2,
    p_data CLOB,
    p_key_id NUMBER,
    p_signature_id OUT NUMBER
  ) IS
    v_data_hash RAW(2000);
    v_signature RAW(2000);
    v_private_key_enc RAW(2000);
    v_master_key RAW(32);
    v_iv RAW(16);
    v_private_key_raw RAW(2000);
  BEGIN
    -- Kiem tra khoa con hieu luc
    IF NOT is_key_valid(p_key_id) THEN
      RAISE_APPLICATION_ERROR(-20010, 'Khoa RSA khong con hieu luc');
    END IF;
    
    -- Tao ID chu ky moi
    p_signature_id := seq_signatures.NEXTVAL;
    
    -- Buoc 1: Hash du lieu
    v_data_hash := generate_hash(p_data, 'SHA256');
    
    -- Buoc 2: Lay private key da ma hoa
    SELECT PRIVATE_KEY_ENC INTO v_private_key_enc
    FROM RSA_KEYS WHERE KEY_ID = p_key_id;
    
    -- Buoc 3: Giai ma private key (mo phong)
    -- Trong thuc te can lay IV tu Key Vault
    v_master_key := retrieve_key_from_vault('MASTER_KEY');
    IF v_master_key IS NULL THEN
      SELECT CONFIG_VAL INTO v_master_key 
      FROM APP_CONFIG WHERE CONFIG_KEY = 'AES_KEY';
    END IF;
    
    -- Mo phong giai ma
    v_private_key_raw := SYS.DBMS_CRYPTO.RANDOMBYTES(256);
    
    -- Buoc 4: Tao chu ky (mo phong: hash cua hash + private key)
    v_signature := SYS.DBMS_CRYPTO.HASH(
      UTL_RAW.CONCAT(v_data_hash, v_private_key_raw),
      SYS.DBMS_CRYPTO.HASH_SH256
    );
    
    -- Buoc 5: Luu chu ky
    INSERT INTO DIGITAL_SIGNATURES(
      SIGNATURE_ID, TABLE_NAME, RECORD_ID, DATA_HASH, 
      SIGNATURE, KEY_ID, SIGNED_BY
    ) VALUES (
      p_signature_id, p_table_name, p_record_id, v_data_hash,
      v_signature, p_key_id, USER
    );
    
    COMMIT;
    
    DBMS_OUTPUT.PUT_LINE(' Da ky du lieu: ' || p_table_name || 
                        '[' || p_record_id || ']' ||
                        ' voi signature ID: ' || p_signature_id);
  END;
  
  FUNCTION verify_signature(
    p_signature_id NUMBER
  ) RETURN BOOLEAN IS
    v_signature RAW(2000);
    v_data_hash RAW(2000);
    v_key_id NUMBER;
    v_public_key CLOB;
    v_computed_sig RAW(2000);
    v_verification_result BOOLEAN;
  BEGIN
    -- Lay thong tin chu ky
    SELECT SIGNATURE, DATA_HASH, KEY_ID
    INTO v_signature, v_data_hash, v_key_id
    FROM DIGITAL_SIGNATURES
    WHERE SIGNATURE_ID = p_signature_id;
    
    -- Lay public key
    SELECT PUBLIC_KEY INTO v_public_key 
    FROM RSA_KEYS 
    WHERE KEY_ID = v_key_id AND IS_ACTIVE = 'Y';
    
    -- Xac thuc (mo phong nang cao)
    -- Trong thuc te: giai ma voi public key va so sanh
    v_computed_sig := SYS.DBMS_CRYPTO.HASH(
      UTL_RAW.CONCAT(v_data_hash, UTL_RAW.CAST_TO_RAW(v_public_key)),
      SYS.DBMS_CRYPTO.HASH_SH256
    );
    
    v_verification_result := (v_computed_sig = v_signature);
    
    -- Cap nhat trang thai
    IF v_verification_result THEN
      UPDATE DIGITAL_SIGNATURES 
      SET VERIFIED = 'Y',
          VERIFIED_AT = SYSDATE,
          VERIFIED_BY = USER
      WHERE SIGNATURE_ID = p_signature_id;
    ELSE
      UPDATE DIGITAL_SIGNATURES 
      SET VERIFIED = 'N'
      WHERE SIGNATURE_ID = p_signature_id;
    END IF;
    
    COMMIT;
    
    RETURN v_verification_result;
  EXCEPTION
    WHEN OTHERS THEN
      RETURN FALSE;
  END;
  
  PROCEDURE verify_signature_async(
    p_signature_id NUMBER,
    p_verifier VARCHAR2
  ) IS
    v_verified BOOLEAN;
  BEGIN
    v_verified := verify_signature(p_signature_id);
    
    IF v_verified THEN
      DBMS_OUTPUT.PUT_LINE('Chu ky ' || p_signature_id || 
                          ' da duoc xac thuc boi ' || p_verifier);
    ELSE
      DBMS_OUTPUT.PUT_LINE('Chu ky ' || p_signature_id || 
                          ' KHONG hop le (xac thuc boi ' || p_verifier || ')');
    END IF;
  END;
  
  -- ==== KEY WRAPPING ====
  FUNCTION wrap_aes_key(
    p_aes_key RAW,
    p_rsa_key_id NUMBER
  ) RETURN RAW IS
    v_public_key CLOB;
    v_wrapped_key RAW(2000);
  BEGIN
    -- Lay public key
    SELECT PUBLIC_KEY INTO v_public_key 
    FROM RSA_KEYS 
    WHERE KEY_ID = p_rsa_key_id AND IS_ACTIVE = 'Y';
    
    -- Mo phong wrap: Hash(AES_key + RSA_public_key + timestamp)
    v_wrapped_key := SYS.DBMS_CRYPTO.HASH(
      UTL_RAW.CONCAT(
        p_aes_key,
        UTL_RAW.CONCAT(
          UTL_RAW.CAST_TO_RAW(v_public_key),
          UTL_RAW.CAST_TO_RAW(TO_CHAR(SYSDATE, 'YYYYMMDDHH24MISS'))
        )
      ),
      SYS.DBMS_CRYPTO.HASH_SH512
    );
    
    RETURN v_wrapped_key;
  END;
  
  FUNCTION unwrap_aes_key(
    p_wrapped_key RAW,
    p_rsa_key_id NUMBER
  ) RETURN RAW IS
    v_master_key RAW(32);
  BEGIN
    -- Trong thuc te: Giai ma bang private key
    -- Mo phong: Tra ve key ngau nhien (de test)
    
    -- Lay master key de "giai ma"
    SELECT CONFIG_VAL INTO v_master_key 
    FROM APP_CONFIG WHERE CONFIG_KEY = 'AES_KEY';
    
    -- Mo phong unwrap: Hash(wrapped_key + master_key)
    RETURN SYS.DBMS_CRYPTO.HASH(
      UTL_RAW.CONCAT(p_wrapped_key, v_master_key),
      SYS.DBMS_CRYPTO.HASH_SH256
    );
  END;
  
  -- ==== KEY VAULT ====
  PROCEDURE store_key_in_vault(
    p_key_name VARCHAR2,
    p_key_type VARCHAR2,
    p_key_value RAW,
    p_purpose VARCHAR2
  ) IS
    v_master_key RAW(32);
    v_iv RAW(16);
    v_encrypted_key RAW(2000);
  BEGIN
    -- Lay master key
    SELECT CONFIG_VAL INTO v_master_key 
    FROM APP_CONFIG WHERE CONFIG_KEY = 'AES_KEY';
    
    -- Tao IV
    v_iv := SYS.DBMS_CRYPTO.RANDOMBYTES(16);
    
    -- Ma hoa khoa
    v_encrypted_key := SYS.DBMS_CRYPTO.ENCRYPT(
      p_key_value,
      SYS.DBMS_CRYPTO.ENCRYPT_AES256 + SYS.DBMS_CRYPTO.CHAIN_CBC + SYS.DBMS_CRYPTO.PAD_PKCS5,
      v_master_key,
      v_iv
    );
    
    -- Luu vao Key Vault
    INSERT INTO KEY_VAULT(
      KEY_NAME, KEY_TYPE, KEY_VALUE_ENC, KEY_IV,
      MASTER_KEY_ID, CREATED_BY, KEY_PURPOSE
    ) VALUES (
      p_key_name, p_key_type, v_encrypted_key, v_iv,
      1, -- Master key ID (hardcode cho don gian)
      USER, p_purpose
    );
    
    COMMIT;
    
    DBMS_OUTPUT.PUT_LINE(' Da luu khoa vao Key Vault: ' || p_key_name);
  END;
  
  FUNCTION retrieve_key_from_vault(
    p_key_name VARCHAR2
  ) RETURN RAW IS
    v_encrypted_key RAW(2000);
    v_iv RAW(16);
    v_master_key RAW(32);
    v_decrypted_key RAW(2000);
  BEGIN
    -- Lay khoa da ma hoa va IV
    SELECT KEY_VALUE_ENC, KEY_IV 
    INTO v_encrypted_key, v_iv
    FROM KEY_VAULT 
    WHERE KEY_NAME = p_key_name 
      AND IS_ACTIVE = 'Y'
      AND (LAST_USED IS NULL OR LAST_USED > SYSDATE - 30);
    
    -- Lay master key
    SELECT CONFIG_VAL INTO v_master_key 
    FROM APP_CONFIG WHERE CONFIG_KEY = 'AES_KEY';
    
    -- Giai ma
    v_decrypted_key := SYS.DBMS_CRYPTO.DECRYPT(
      v_encrypted_key,
      SYS.DBMS_CRYPTO.ENCRYPT_AES256 + SYS.DBMS_CRYPTO.CHAIN_CBC + SYS.DBMS_CRYPTO.PAD_PKCS5,
      v_master_key,
      v_iv
    );
    
    -- Cap nhat last_used
    UPDATE KEY_VAULT 
    SET LAST_USED = SYSDATE 
    WHERE KEY_NAME = p_key_name;
    
    COMMIT;
    
    RETURN v_decrypted_key;
  EXCEPTION
    WHEN NO_DATA_FOUND THEN
      RETURN NULL;
    WHEN OTHERS THEN
      RETURN NULL;
  END;
  
  -- ==== UTILITY FUNCTIONS ====
  FUNCTION generate_hash(
    p_data CLOB,
    p_algorithm VARCHAR2 DEFAULT 'SHA256'
  ) RETURN RAW IS
    v_hash RAW(2000);
    v_algorithm NUMBER;
  BEGIN
    -- Chon thuat toan
    IF p_algorithm = 'SHA256' THEN
      v_algorithm := SYS.DBMS_CRYPTO.HASH_SH256;
    ELSIF p_algorithm = 'SHA512' THEN
      v_algorithm := SYS.DBMS_CRYPTO.HASH_SH512;
    ELSIF p_algorithm = 'MD5' THEN
      v_algorithm := SYS.DBMS_CRYPTO.HASH_MD5;
    ELSE
      v_algorithm := SYS.DBMS_CRYPTO.HASH_SH256;
    END IF;
    
    -- Tao hash
    v_hash := SYS.DBMS_CRYPTO.HASH(
      UTL_RAW.CAST_TO_RAW(p_data),
      v_algorithm
    );
    
    RETURN v_hash;
  END;
  
  FUNCTION verify_data_integrity(
    p_data CLOB,
    p_expected_hash RAW
  ) RETURN BOOLEAN IS
    v_actual_hash RAW(2000);
  BEGIN
    v_actual_hash := generate_hash(p_data, 'SHA256');
    RETURN v_actual_hash = p_expected_hash;
  END;
  
END pkg_asymmetric;
/

PROMPT 1.3: Tao procedure ky hoa don...
CREATE OR REPLACE PROCEDURE ky_hoadon(
  p_mahd NUMBER,
  p_key_id NUMBER
) AS
  v_data CLOB;
  v_signature_id NUMBER;
  v_hoadon_info VARCHAR2(4000);
BEGIN
  -- Lay thong tin hoa don
  SELECT 
    'MAHD=' || MAHD || 
    ',MANV=' || MANV || 
    ',MABAN=' || MABAN ||
    ',MAKH=' || NVL(TO_CHAR(MAKH), 'NULL') ||
    ',TONGTIEN=' || TONGTIEN || 
    ',TRANGTHAI=' || TRANGTHAI ||
    ',NGAYLAP=' || TO_CHAR(NGAYLAP, 'YYYY-MM-DD HH24:MI:SS')
  INTO v_hoadon_info
  FROM HOADON
  WHERE MAHD = p_mahd;
  
  -- Lay chi tiet hoa don
  SELECT v_hoadon_info || ',CHITIET={' || 
         LISTAGG('MON=' || m.TENMON || ',SL=' || ct.SOLUONG || ',GIA=' || ct.DONGIA, ';') 
         WITHIN GROUP (ORDER BY ct.MAMON) || '}'
  INTO v_data
  FROM CHITIET_HD ct
  JOIN MON m ON ct.MAMON = m.MAMON
  WHERE ct.MAHD = p_mahd;
  
  -- Ky du lieu
  pkg_asymmetric.sign_data('HOADON', TO_CHAR(p_mahd), v_data, p_key_id, v_signature_id);
  
  -- Cap nhat audit
  pkg_audit.log_action('HOADON', 'DIGITAL_SIGN', TO_CHAR(p_mahd), NULL, 
                      'Signature ID: ' || v_signature_id);
  
  DBMS_OUTPUT.PUT_LINE(' Da ky hoa don ' || p_mahd || 
                      ' voi signature ID: ' || v_signature_id);
EXCEPTION
  WHEN OTHERS THEN
    DBMS_OUTPUT.PUT_LINE(' Loi khi ky hoa don: ' || SQLERRM);
END;
/

-- Procedure ky phieu nhap
CREATE OR REPLACE PROCEDURE ky_phieunhap(
  p_mapn NUMBER,
  p_key_id NUMBER
) AS
  v_data CLOB;
  v_signature_id NUMBER;
BEGIN
  -- Tao chuoi du lieu tu phieu nhap
  SELECT 
    'MAPN=' || MAPN || 
    ',MANCC=' || MANCC || 
    ',MANV=' || MANV ||
    ',TONGTIEN=' || TONGTIEN || 
    ',NGAYNHAP=' || TO_CHAR(NGAYNHAP, 'YYYY-MM-DD HH24:MI:SS')
  INTO v_data
  FROM PHIEUNHAP
  WHERE MAPN = p_mapn;
  
  -- Ky du lieu
  pkg_asymmetric.sign_data('PHIEUNHAP', TO_CHAR(p_mapn), v_data, p_key_id, v_signature_id);
  
  DBMS_OUTPUT.PUT_LINE(' Da ky phieu nhap ' || p_mapn || 
                      ' voi signature ID: ' || v_signature_id);
END;
/

PROMPT  Da tao package RSA nang cao

PROMPT *** BUOC 2: CAI DAT FGA (FINE-GRAINED AUDITING) NÂNG CAO ***

PROMPT 2.1: Tao trigger FGA cho truy cap du lieu nha?y cam...
-- Trigger cho truy c?p d? li?u mã hóa
CREATE OR REPLACE TRIGGER trg_fga_sensitive_data_access
AFTER INSERT OR UPDATE OR DELETE ON SENSITIVE_DATA
FOR EACH ROW
DECLARE
  PRAGMA AUTONOMOUS_TRANSACTION;
  v_action VARCHAR2(20);
  v_details VARCHAR2(1000);
BEGIN
  IF INSERTING THEN
    v_action := 'INSERT_SENSITIVE';
    v_details := 'Data type: ' || :NEW.DATA_TYPE || ', Created by: ' || :NEW.CREATED_BY;
  ELSIF UPDATING THEN
    v_action := 'UPDATE_SENSITIVE';
    v_details := 'Data type: ' || :NEW.DATA_TYPE || ', Modified by: ' || USER;
  ELSE
    v_action := 'DELETE_SENSITIVE';
    v_details := 'Data type: ' || :OLD.DATA_TYPE || ', Deleted by: ' || USER;
  END IF;
  
  pkg_audit.log_fga(
    'SENSITIVE_DATA',
    'fga_sensitive_data_access',
    v_action || ' - ' || v_details
  );
END;
/

-- Trigger cho truy c?p ngoài gi? hành chính
CREATE OR REPLACE TRIGGER trg_fga_after_hours_access
BEFORE INSERT OR UPDATE OR DELETE ON HOADON
DECLARE
  PRAGMA AUTONOMOUS_TRANSACTION;
  v_current_hour NUMBER;
BEGIN
  -- L?y gi? hi?n t?i
  v_current_hour := TO_NUMBER(TO_CHAR(SYSDATE, 'HH24'));
  
  -- N?u ngoài gi? hành chính (20h - 6h)
  IF v_current_hour >= 20 OR v_current_hour < 6 THEN
    pkg_audit.log_fga(
      'HOADON',
      'fga_after_hours_access',
      'Access detected after hours: ' || TO_CHAR(SYSDATE, 'YYYY-MM-DD HH24:MI:SS') ||
      ' by user: ' || USER
    );
  END IF;
EXCEPTION
  WHEN OTHERS THEN NULL;
END;
/

-- Trigger cho truy c?p thông tin nhân viên
CREATE OR REPLACE TRIGGER trg_fga_employee_info_access
AFTER SELECT ON NHANVIEN
FOR EACH ROW
DECLARE
  PRAGMA AUTONOMOUS_TRANSACTION;
  v_query VARCHAR2(4000);
BEGIN
  -- Ch? log n?u truy c?p thông tin nh?y c?m
  IF :OLD.CCCD_ENC IS NOT NULL OR :OLD.DIACHI_ENC IS NOT NULL THEN
    -- L?y câu l?nh SQL ?ang th?c thi (mô ph?ng)
    v_query := 'SELECT * FROM NHANVIEN WHERE MANV = ' || :OLD.MANV;
    
    pkg_audit.log_fga(
      'NHANVIEN',
      'fga_employee_sensitive_info',
      'Accessed encrypted employee info: MANV=' || :OLD.MANV || 
      ', User: ' || USER || ', Time: ' || TO_CHAR(SYSDATE, 'HH24:MI:SS')
    );
  END IF;
EXCEPTION
  WHEN OTHERS THEN NULL;
END;
/

PROMPT 2.2: Tao package quan ly FGA...
CREATE OR REPLACE PACKAGE pkg_fga_management AS
  
  -- Thêm policy FGA m?i
  PROCEDURE add_fga_policy(
    p_policy_name VARCHAR2,
    p_table_name VARCHAR2,
    p_condition VARCHAR2,
    p_audit_column VARCHAR2 DEFAULT NULL
  );
  
  -- Kích ho?t/vô hi?u hóa policy
  PROCEDURE toggle_fga_policy(
    p_policy_name VARCHAR2,
    p_enable BOOLEAN
  );
  
  -- Xem report FGA
  PROCEDURE generate_fga_report(
    p_from_date DATE DEFAULT SYSDATE - 7,
    p_to_date DATE DEFAULT SYSDATE
  );
  
  -- Phân tích hành vi ?áng ng?
  PROCEDURE analyze_suspicious_activity;
  
  -- D?n d?p log c?
  PROCEDURE cleanup_old_fga_logs(p_days_to_keep NUMBER DEFAULT 90);
  
END pkg_fga_management;
/

CREATE OR REPLACE PACKAGE BODY pkg_fga_management AS
  
  PROCEDURE add_fga_policy(
    p_policy_name VARCHAR2,
    p_table_name VARCHAR2,
    p_condition VARCHAR2,
    p_audit_column VARCHAR2 DEFAULT NULL
  ) IS
  BEGIN
    -- Trong Oracle th?c t? dùng DBMS_FGA.ADD_POLICY
    -- ? ?ây mô ph?ng b?ng trigger
    DBMS_OUTPUT.PUT_LINE('Mo phong them FGA policy: ' || p_policy_name);
    DBMS_OUTPUT.PUT_LINE('  Table: ' || p_table_name);
    DBMS_OUTPUT.PUT_LINE('  Condition: ' || p_condition);
    
    -- Ghi log vào b?ng FGA policies (mô ph?ng)
    INSERT INTO FGA_LOG(USERNAME, TABLE_NAME, POLICY_NAME, SQL_TEXT, CLIENT_IP)
    VALUES (USER, 'SYSTEM', 'ADD_FGA_POLICY', 
            'ADD POLICY ' || p_policy_name || ' ON ' || p_table_name,
            SYS_CONTEXT('USERENV', 'IP_ADDRESS'));
    
    COMMIT;
  END;
  
  PROCEDURE toggle_fga_policy(
    p_policy_name VARCHAR2,
    p_enable BOOLEAN
  ) IS
    v_status VARCHAR2(20);
  BEGIN
    IF p_enable THEN
      v_status := 'ENABLED';
    ELSE
      v_status := 'DISABLED';
    END IF;
    
    DBMS_OUTPUT.PUT_LINE('FGA Policy ' || p_policy_name || ': ' || v_status);
    
    -- Ghi log
    INSERT INTO FGA_LOG(USERNAME, TABLE_NAME, POLICY_NAME, SQL_TEXT, CLIENT_IP)
    VALUES (USER, 'SYSTEM', 'TOGGLE_FGA_POLICY', 
            v_status || ' POLICY ' || p_policy_name,
            SYS_CONTEXT('USERENV', 'IP_ADDRESS'));
    
    COMMIT;
  END;
  
  PROCEDURE generate_fga_report(
    p_from_date DATE DEFAULT SYSDATE - 7,
    p_to_date DATE DEFAULT SYSDATE
  ) IS
    v_total_events NUMBER;
    v_suspicious_events NUMBER;
    v_top_user VARCHAR2(100);
    v_top_table VARCHAR2(100);
  BEGIN
    DBMS_OUTPUT.PUT_LINE('=== FGA AUDIT REPORT (' || 
                         TO_CHAR(p_from_date, 'DD/MM/YYYY') || ' - ' ||
                         TO_CHAR(p_to_date, 'DD/MM/YYYY') || ') ===');
    
    -- T?ng s? s? ki?n
    SELECT COUNT(*) INTO v_total_events
    FROM FGA_LOG
    WHERE TIMESTAMP BETWEEN p_from_date AND p_to_date;
    
    DBMS_OUTPUT.PUT_LINE('Tong so su kien FGA: ' || v_total_events);
    DBMS_OUTPUT.PUT_LINE('----------------------------------------');
    
    -- S? ki?n ?áng ng?
    SELECT COUNT(*) INTO v_suspicious_events
    FROM FGA_LOG
    WHERE TIMESTAMP BETWEEN p_from_date AND p_to_date
      AND (POLICY_NAME LIKE '%SENSITIVE%' 
           OR POLICY_NAME LIKE '%AFTER_HOURS%'
           OR SQL_TEXT LIKE '%DELETE%');
    
    DBMS_OUTPUT.PUT_LINE('Su kien dang ngo: ' || v_suspicious_events);
    
    -- Top user
    BEGIN
      SELECT USERNAME INTO v_top_user
      FROM (
        SELECT USERNAME, COUNT(*) as event_count
        FROM FGA_LOG
        WHERE TIMESTAMP BETWEEN p_from_date AND p_to_date
        GROUP BY USERNAME
        ORDER BY COUNT(*) DESC
      ) WHERE ROWNUM = 1;
      
      DBMS_OUTPUT.PUT_LINE('User co nhieu su kien nhat: ' || v_top_user);
    EXCEPTION
      WHEN NO_DATA_FOUND THEN
        v_top_user := 'None';
    END;
    
    -- Top table
    BEGIN
      SELECT TABLE_NAME INTO v_top_table
      FROM (
        SELECT TABLE_NAME, COUNT(*) as event_count
        FROM FGA_LOG
        WHERE TIMESTAMP BETWEEN p_from_date AND p_to_date
        GROUP BY TABLE_NAME
        ORDER BY COUNT(*) DESC
      ) WHERE ROWNUM = 1;
      
      DBMS_OUTPUT.PUT_LINE('Table duoc giam sat nhieu nhat: ' || v_top_table);
    EXCEPTION
      WHEN NO_DATA_FOUND THEN
        v_top_table := 'None';
    END;
    
    DBMS_OUTPUT.PUT_LINE('----------------------------------------');
    
    -- Chi ti?t s? ki?n g?n ?ây
    DBMS_OUTPUT.PUT_LINE('5 SU KIEN GAN DAY NHAT:');
    FOR rec IN (
      SELECT ID, USERNAME, POLICY_NAME, 
             SUBSTR(SQL_TEXT, 1, 50) as SQL_SHORT,
             TO_CHAR(TIMESTAMP, 'DD/MM HH24:MI') as TIME
      FROM FGA_LOG
      WHERE TIMESTAMP BETWEEN p_from_date AND p_to_date
      ORDER BY TIMESTAMP DESC
      FETCH FIRST 5 ROWS ONLY
    ) LOOP
      DBMS_OUTPUT.PUT_LINE('  ' || rec.TIME || ' | ' || rec.USERNAME || 
                          ' | ' || rec.POLICY_NAME || ' | ' || rec.SQL_SHORT);
    END LOOP;
    
  END;
  
  PROCEDURE analyze_suspicious_activity IS
    v_midnight_access NUMBER;
    v_sensitive_access NUMBER;
    v_multiple_failures NUMBER;
  BEGIN
    DBMS_OUTPUT.PUT_LINE('=== PHAN TICH HANH VI DANG NGO ===');
    
    -- Truy c?p n?a ?êm (00:00 - 05:00)
    SELECT COUNT(*) INTO v_midnight_access
    FROM FGA_LOG
    WHERE TO_CHAR(TIMESTAMP, 'HH24') BETWEEN '00' AND '05'
      AND TIMESTAMP > SYSDATE - 7;
    
    IF v_midnight_access > 0 THEN
      DBMS_OUTPUT.PUT_LINE('CANH BAO: Co ' || v_midnight_access || 
                          ' truy cap vao nua dem (00h-05h) trong 7 ngay qua');
    END IF;
    
    -- Truy c?p d? li?u nh?y c?m
    SELECT COUNT(*) INTO v_sensitive_access
    FROM FGA_LOG
    WHERE POLICY_NAME LIKE '%SENSITIVE%'
      AND TIMESTAMP > SYSDATE - 1;
    
    IF v_sensitive_access > 10 THEN
      DBMS_OUTPUT.PUT_LINE('CANH BAO: Nhieu truy cap du lieu nhay cam (' || 
                          v_sensitive_access || ' trong 24h)');
    END IF;
    
    -- Nhi?u l?n th?t b?i ??ng nh?p
    SELECT COUNT(*) INTO v_multiple_failures
    FROM (
      SELECT USERNAME, COUNT(*) as fail_count
      FROM LICHSU_DANGNHAP
      WHERE HOATDONG = 'LOGIN_FAIL'
        AND THOIGIAN > SYSDATE - 1/24 -- 1 gi? qua
      GROUP BY USERNAME
      HAVING COUNT(*) >= 3
    );
    
    IF v_multiple_failures > 0 THEN
      DBMS_OUTPUT.PUT_LINE('CANH BAO: Co tai khoan dang nhap that bai nhieu lan');
    END IF;
    
    IF v_midnight_access = 0 AND v_sensitive_access <= 10 AND v_multiple_failures = 0 THEN
      DBMS_OUTPUT.PUT_LINE('Khong phat hien hoat dong dang ngo.');
    END IF;
    
  END;
  
  PROCEDURE cleanup_old_fga_logs(p_days_to_keep NUMBER DEFAULT 90) IS
    v_deleted_count NUMBER;
  BEGIN
    DELETE FROM FGA_LOG
    WHERE TIMESTAMP < SYSDATE - p_days_to_keep;
    
    v_deleted_count := SQL%ROWCOUNT;
    
    COMMIT;
    
    DBMS_OUTPUT.PUT_LINE('Da xoa ' || v_deleted_count || 
                        ' ban ghi FGA log cu hon ' || p_days_to_keep || ' ngay');
  END;
  
END pkg_fga_management;
/

PROMPT 2.3: Tao view xem FGA audit nang cao...
CREATE OR REPLACE VIEW v_fga_audit_detailed AS
SELECT 
  f.ID,
  f.USERNAME,
  f.TABLE_NAME,
  f.POLICY_NAME,
  SUBSTR(f.SQL_TEXT, 1, 500) AS SQL_TEXT_FULL,
  TO_CHAR(f.TIMESTAMP, 'YYYY-MM-DD HH24:MI:SS') AS AUDIT_TIME,
  f.CLIENT_IP,
  CASE 
    WHEN f.POLICY_NAME LIKE '%SENSITIVE%' THEN 'HIGH'
    WHEN f.POLICY_NAME LIKE '%AFTER_HOURS%' THEN 'MEDIUM'
    ELSE 'LOW'
  END AS RISK_LEVEL,
  CASE 
    WHEN TO_CHAR(f.TIMESTAMP, 'HH24') BETWEEN '20' AND '06' THEN 'Y'
    ELSE 'N'
  END AS AFTER_HOURS_FLAG
FROM FGA_LOG f
ORDER BY f.TIMESTAMP DESC;
/

-- View th?ng kê FGA
CREATE OR REPLACE VIEW v_fga_statistics AS
SELECT 
  TRUNC(TIMESTAMP) AS AUDIT_DATE,
  COUNT(*) AS TOTAL_EVENTS,
  SUM(CASE WHEN POLICY_NAME LIKE '%SENSITIVE%' THEN 1 ELSE 0 END) AS SENSITIVE_EVENTS,
  SUM(CASE WHEN TO_CHAR(TIMESTAMP, 'HH24') BETWEEN '20' AND '06' THEN 1 ELSE 0 END) AS AFTER_HOURS_EVENTS,
  COUNT(DISTINCT USERNAME) AS UNIQUE_USERS,
  COUNT(DISTINCT TABLE_NAME) AS UNIQUE_TABLES
FROM FGA_LOG
WHERE TIMESTAMP > SYSDATE - 30
GROUP BY TRUNC(TIMESTAMP)
ORDER BY AUDIT_DATE DESC;
/

PROMPT  Da cai dat FGA nang cao

PROMPT *** BUOC 3: QUAN LY PROFILE VA SESSION NÂNG CAO ***

PROMPT 3.1: Tao bang quan ly session nang cao...
BEGIN
  EXECUTE IMMEDIATE 'DROP TABLE SESSION_TRACKING CASCADE CONSTRAINTS';
EXCEPTION WHEN OTHERS THEN NULL;
END;
/

CREATE TABLE SESSION_TRACKING (
  SESSION_ID NUMBER PRIMARY KEY,
  USERNAME VARCHAR2(100),
  MANV NUMBER,
  ROLE_CODE VARCHAR2(30),
  SID NUMBER,
  SERIAL# NUMBER,
  LOGIN_TIME DATE DEFAULT SYSDATE,
  LAST_ACTIVITY DATE DEFAULT SYSDATE,
  IP_ADDRESS VARCHAR2(50),
  HOST VARCHAR2(200),
  TERMINAL VARCHAR2(200),
  PROGRAM VARCHAR2(200),
  MODULE VARCHAR2(200),
  ACTION VARCHAR2(200),
  STATUS VARCHAR2(20) DEFAULT 'ACTIVE',
  SESSION_TIMEOUT_MINUTES NUMBER DEFAULT 30,
  CONSTRAINT fk_session_nv FOREIGN KEY (MANV) REFERENCES NHANVIEN(MANV),
  CONSTRAINT fk_session_role FOREIGN KEY (ROLE_CODE) REFERENCES PHANQUYEN(ROLE_CODE)
);

CREATE SEQUENCE seq_session START WITH 1 INCREMENT BY 1;

-- B?ng session history
BEGIN
  EXECUTE IMMEDIATE 'DROP TABLE SESSION_HISTORY CASCADE CONSTRAINTS';
EXCEPTION WHEN OTHERS THEN NULL;
END;
/

CREATE TABLE SESSION_HISTORY (
  HISTORY_ID NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  SESSION_ID NUMBER,
  USERNAME VARCHAR2(100),
  ACTION_TYPE VARCHAR2(50), -- LOGIN, LOGOUT, TIMEOUT, KILLED
  ACTION_TIME DATE DEFAULT SYSDATE,
  IP_ADDRESS VARCHAR2(50),
  DURATION_MINUTES NUMBER,
  REASON VARCHAR2(500)
);

PROMPT 3.2: Tao bang luu cau hinh profile nang cao...
BEGIN
  EXECUTE IMMEDIATE 'DROP TABLE USER_PROFILES CASCADE CONSTRAINTS';
EXCEPTION WHEN OTHERS THEN NULL;
END;
/

CREATE TABLE USER_PROFILES (
  PROFILE_ID NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  PROFILE_NAME VARCHAR2(50) UNIQUE NOT NULL,
  USERNAME VARCHAR2(50),
  ROLE_CODE VARCHAR2(30),
  MAX_SESSIONS NUMBER DEFAULT 5,
  IDLE_TIMEOUT_MINUTES NUMBER DEFAULT 60,
  PASSWORD_LIFE_DAYS NUMBER DEFAULT 90,
  FAILED_LOGIN_LIMIT NUMBER DEFAULT 5,
  PASSWORD_LOCK_TIME_MINUTES NUMBER DEFAULT 15,
  PASSWORD_REUSE_LIMIT NUMBER DEFAULT 5,
  PASSWORD_COMPLEXITY VARCHAR2(100) DEFAULT 'MEDIUM',
  CREATED_AT DATE DEFAULT SYSDATE,
  IS_ACTIVE CHAR(1) DEFAULT 'Y',
  CONSTRAINT fk_profile_user FOREIGN KEY (USERNAME) REFERENCES TAIKHOAN(USERNAME),
  CONSTRAINT fk_profile_role FOREIGN KEY (ROLE_CODE) REFERENCES PHANQUYEN(ROLE_CODE),
  CONSTRAINT chk_profile_active CHECK (IS_ACTIVE IN ('Y', 'N'))
);

-- Thêm d? li?u m?u
INSERT INTO USER_PROFILES (PROFILE_NAME, USERNAME, ROLE_CODE, MAX_SESSIONS, IDLE_TIMEOUT_MINUTES, 
                          PASSWORD_LIFE_DAYS, FAILED_LOGIN_LIMIT, PASSWORD_COMPLEXITY)
VALUES ('ADMIN_PROFILE', 'admin', 'ADMIN', 5, 60, 90, 5, 'STRONG');

INSERT INTO USER_PROFILES (PROFILE_NAME, USERNAME, ROLE_CODE, MAX_SESSIONS, IDLE_TIMEOUT_MINUTES, 
                          PASSWORD_LIFE_DAYS, FAILED_LOGIN_LIMIT, PASSWORD_COMPLEXITY)
VALUES ('MANAGER_PROFILE', 'quanly', 'QUANLY', 3, 45, 60, 5, 'MEDIUM');

INSERT INTO USER_PROFILES (PROFILE_NAME, ROLE_CODE, MAX_SESSIONS, IDLE_TIMEOUT_MINUTES, 
                          PASSWORD_LIFE_DAYS, FAILED_LOGIN_LIMIT, PASSWORD_COMPLEXITY)
VALUES ('STAFF_PROFILE', 'NHANVIEN', 2, 30, 45, 3, 'MEDIUM');

INSERT INTO USER_PROFILES (PROFILE_NAME, ROLE_CODE, MAX_SESSIONS, IDLE_TIMEOUT_MINUTES, 
                          PASSWORD_LIFE_DAYS, FAILED_LOGIN_LIMIT, PASSWORD_COMPLEXITY)
VALUES ('INVENTORY_PROFILE', 'THUKHO', 2, 30, 45, 3, 'MEDIUM');

COMMIT;

PROMPT 3.3: Tao package quan ly session nang cao...
CREATE OR REPLACE PACKAGE pkg_session_mgmt AS
  
  -- ==== SESSION MANAGEMENT ====
  -- Dang ky session moi
  PROCEDURE register_session(
    p_username VARCHAR2,
    p_manv NUMBER,
    p_role_code VARCHAR2,
    p_session_id OUT NUMBER
  );
  
  -- Cap nhat hoat dong session
  PROCEDURE update_activity(p_session_id NUMBER);
  
  -- Dong session
  PROCEDURE terminate_session(
    p_session_id NUMBER,
    p_reason VARCHAR2 DEFAULT 'MANUAL'
  );
  
  -- Kiem tra session timeout
  PROCEDURE check_session_timeouts;
  
  -- ==== SESSION MONITORING ====
  -- Lay danh sach session dang hoat dong
  PROCEDURE list_active_sessions;
  
  -- Kiem tra so luong session cua user
  FUNCTION count_user_sessions(p_username VARCHAR2) RETURN NUMBER;
  
  -- Kiem tra user co vuot qua gioi han session khong
  FUNCTION is_user_over_session_limit(p_username VARCHAR2) RETURN BOOLEAN;
  
  -- Lay thong tin session hien tai
  PROCEDURE get_current_session_info;
  
  -- ==== SESSION SECURITY ====
  -- Kiem tra session co hop le khong
  FUNCTION is_session_valid(p_session_id NUMBER) RETURN BOOLEAN;
  
  -- Force logout tat ca session cua user
  PROCEDURE force_logout_user(p_username VARCHAR2);
  
  -- Kiem tra IP co duoc phep khong
  FUNCTION is_ip_allowed(p_ip VARCHAR2, p_username VARCHAR2) RETURN BOOLEAN;
  
  -- ==== REPORTING ====
  -- Tao bao cao session
  PROCEDURE generate_session_report(
    p_from_date DATE DEFAULT SYSDATE - 7,
    p_to_date DATE DEFAULT SYSDATE
  );
  
  -- Phan tich hoat dong dang ngo
  PROCEDURE analyze_suspicious_sessions;
  
END pkg_session_mgmt;
/

CREATE OR REPLACE PACKAGE BODY pkg_session_mgmt AS
  
  PROCEDURE register_session(
    p_username VARCHAR2,
    p_manv NUMBER,
    p_role_code VARCHAR2,
    p_session_id OUT NUMBER
  ) IS
    v_profile_max_sessions NUMBER;
    v_current_sessions NUMBER;
    v_ip VARCHAR2(50);
  BEGIN
    -- Kiem tra IP
    v_ip := SYS_CONTEXT('USERENV', 'IP_ADDRESS');
    IF NOT is_ip_allowed(v_ip, p_username) THEN
      RAISE_APPLICATION_ERROR(-20020, 'IP address not allowed: ' || v_ip);
    END IF;
    
    -- Kiem tra gioi han session
    SELECT MAX_SESSIONS INTO v_profile_max_sessions
    FROM USER_PROFILES 
    WHERE (USERNAME = p_username OR ROLE_CODE = p_role_code)
      AND IS_ACTIVE = 'Y'
      AND ROWNUM = 1;
    
    v_current_sessions := count_user_sessions(p_username);
    
    IF v_current_sessions >= v_profile_max_sessions THEN
      RAISE_APPLICATION_ERROR(-20021, 'User has reached maximum session limit: ' || 
                                     v_profile_max_sessions);
    END IF;
    
    -- Tao session moi
    p_session_id := seq_session.NEXTVAL;
    
    INSERT INTO SESSION_TRACKING(
      SESSION_ID, USERNAME, MANV, ROLE_CODE,
      LOGIN_TIME, LAST_ACTIVITY, 
      IP_ADDRESS, HOST, TERMINAL, PROGRAM, MODULE,
      STATUS, SESSION_TIMEOUT_MINUTES
    ) VALUES (
      p_session_id, p_username, p_manv, p_role_code,
      SYSDATE, SYSDATE,
      v_ip,
      SYS_CONTEXT('USERENV', 'HOST'),
      SYS_CONTEXT('USERENV', 'TERMINAL'),
      SYS_CONTEXT('USERENV', 'MODULE'),
      SYS_CONTEXT('USERENV', 'ACTION'),
      'ACTIVE', 30 -- Default timeout
    );
    
    -- Ghi history
    INSERT INTO SESSION_HISTORY(SESSION_ID, USERNAME, ACTION_TYPE, IP_ADDRESS)
    VALUES (p_session_id, p_username, 'LOGIN', v_ip);
    
    COMMIT;
    
    DBMS_OUTPUT.PUT_LINE('Registered session ' || p_session_id || 
                        ' for user ' || p_username);
  EXCEPTION
    WHEN OTHERS THEN
      -- Fallback: simple session registration
      p_session_id := seq_session.NEXTVAL;
      INSERT INTO SESSION_TRACKING(SESSION_ID, USERNAME, MANV, STATUS)
      VALUES (p_session_id, p_username, p_manv, 'ACTIVE');
      COMMIT;
  END;
  
  PROCEDURE update_activity(p_session_id NUMBER) IS
    v_timeout_minutes NUMBER;
  BEGIN
    UPDATE SESSION_TRACKING 
    SET LAST_ACTIVITY = SYSDATE 
    WHERE SESSION_ID = p_session_id;
    
    -- Lay timeout setting
    SELECT SESSION_TIMEOUT_MINUTES INTO v_timeout_minutes
    FROM SESSION_TRACKING
    WHERE SESSION_ID = p_session_id;
    
    -- Kiem tra timeout
    IF v_timeout_minutes IS NOT NULL THEN
      UPDATE SESSION_TRACKING
      SET STATUS = 'INACTIVE'
      WHERE SESSION_ID = p_session_id
        AND LAST_ACTIVITY < SYSDATE - (v_timeout_minutes/1440);
    END IF;
    
    COMMIT;
  END;
  
  PROCEDURE terminate_session(
    p_session_id NUMBER,
    p_reason VARCHAR2 DEFAULT 'MANUAL'
  ) IS
    v_username VARCHAR2(100);
    v_login_time DATE;
    v_duration_minutes NUMBER;
  BEGIN
    -- Lay thong tin session
    SELECT USERNAME, LOGIN_TIME
    INTO v_username, v_login_time
    FROM SESSION_TRACKING
    WHERE SESSION_ID = p_session_id;
    
    -- Tinh thoi gian session
    v_duration_minutes := ROUND((SYSDATE - v_login_time) * 24 * 60, 2);
    
    -- Cap nhat session
    UPDATE SESSION_TRACKING
    SET STATUS = 'TERMINATED',
        LAST_ACTIVITY = SYSDATE
    WHERE SESSION_ID = p_session_id;
    
    -- Ghi history
    INSERT INTO SESSION_HISTORY(
      SESSION_ID, USERNAME, ACTION_TYPE, 
      ACTION_TIME, IP_ADDRESS, DURATION_MINUTES, REASON
    ) VALUES (
      p_session_id, v_username, 'LOGOUT',
      SYSDATE, 
      (SELECT IP_ADDRESS FROM SESSION_TRACKING WHERE SESSION_ID = p_session_id),
      v_duration_minutes, p_reason
    );
    
    COMMIT;
    
    DBMS_OUTPUT.PUT_LINE('Terminated session ' || p_session_id || 
                        ' for user ' || v_username || 
                        ' (Duration: ' || v_duration_minutes || ' minutes)');
  END;
  
  PROCEDURE check_session_timeouts IS
    CURSOR c_timeout_sessions IS
      SELECT SESSION_ID, USERNAME, LAST_ACTIVITY, SESSION_TIMEOUT_MINUTES
      FROM SESSION_TRACKING
      WHERE STATUS = 'ACTIVE'
        AND SESSION_TIMEOUT_MINUTES IS NOT NULL
        AND LAST_ACTIVITY < SYSDATE - (SESSION_TIMEOUT_MINUTES/1440);
    
    v_count NUMBER := 0;
  BEGIN
    FOR rec IN c_timeout_sessions LOOP
      terminate_session(rec.SESSION_ID, 'TIMEOUT');
      v_count := v_count + 1;
    END LOOP;
    
    IF v_count > 0 THEN
      DBMS_OUTPUT.PUT_LINE('Terminated ' || v_count || ' timed-out sessions');
    END IF;
  END;
  
  PROCEDURE list_active_sessions IS
  BEGIN
    DBMS_OUTPUT.PUT_LINE('=== ACTIVE SESSIONS (' || TO_CHAR(SYSDATE, 'DD/MM/YYYY HH24:MI') || ') ===');
    DBMS_OUTPUT.PUT_LINE('ID     User            Role        Login Time         Last Activity     IP Address');
    DBMS_OUTPUT.PUT_LINE('------ --------------- ---------- ------------------ ----------------- ---------------');
    
    FOR rec IN (
      SELECT SESSION_ID, USERNAME, ROLE_CODE, 
             TO_CHAR(LOGIN_TIME, 'DD/MM HH24:MI') AS LOGIN_TIME,
             TO_CHAR(LAST_ACTIVITY, 'DD/MM HH24:MI') AS LAST_ACTIVITY,
             IP_ADDRESS,
             ROUND((SYSDATE - LAST_ACTIVITY) * 24 * 60, 0) AS MINUTES_IDLE
      FROM SESSION_TRACKING
      WHERE STATUS = 'ACTIVE'
      ORDER BY LOGIN_TIME DESC
    ) LOOP
      DBMS_OUTPUT.PUT_LINE(
        RPAD(rec.SESSION_ID, 7) ||
        RPAD(rec.USERNAME, 16) ||
        RPAD(NVL(rec.ROLE_CODE, 'N/A'), 11) ||
        RPAD(rec.LOGIN_TIME, 18) ||
        RPAD(rec.LAST_ACTIVITY, 18) ||
        RPAD(NVL(rec.IP_ADDRESS, 'N/A'), 15) ||
        CASE WHEN rec.MINUTES_IDLE > 5 THEN ' [IDLE:' || rec.MINUTES_IDLE || 'm]' END
      );
    END LOOP;
    
    DBMS_OUTPUT.PUT_LINE('Total: ' || count_user_sessions(NULL) || ' active sessions');
  END;
  
  FUNCTION count_user_sessions(p_username VARCHAR2) RETURN NUMBER IS
    v_count NUMBER;
  BEGIN
    IF p_username IS NULL THEN
      SELECT COUNT(*) INTO v_count
      FROM SESSION_TRACKING
      WHERE STATUS = 'ACTIVE';
    ELSE
      SELECT COUNT(*) INTO v_count
      FROM SESSION_TRACKING
      WHERE USERNAME = p_username 
        AND STATUS = 'ACTIVE';
    END IF;
    
    RETURN v_count;
  END;
  
  FUNCTION is_user_over_session_limit(p_username VARCHAR2) RETURN BOOLEAN IS
    v_current_sessions NUMBER;
    v_max_sessions NUMBER;
    v_role_code VARCHAR2(30);
  BEGIN
    v_current_sessions := count_user_sessions(p_username);
    
    -- Lay role cua user
    SELECT ROLE_CODE INTO v_role_code
    FROM TAIKHOAN
    WHERE USERNAME = p_username;
    
    -- Lay gioi han session tu profile
    SELECT MAX_SESSIONS INTO v_max_sessions
    FROM USER_PROFILES 
    WHERE (USERNAME = p_username OR ROLE_CODE = v_role_code)
      AND IS_ACTIVE = 'Y'
      AND ROWNUM = 1;
    
    RETURN v_current_sessions >= v_max_sessions;
  EXCEPTION
    WHEN NO_DATA_FOUND THEN
      RETURN FALSE;
  END;
  
  PROCEDURE get_current_session_info IS
    v_sid NUMBER;
    v_serial NUMBER;
  BEGIN
    v_sid := SYS_CONTEXT('USERENV', 'SID');
    v_serial := SYS_CONTEXT('USERENV', 'SERIAL#');
    
    DBMS_OUTPUT.PUT_LINE('=== CURRENT SESSION INFO ===');
    DBMS_OUTPUT.PUT_LINE('User: ' || USER);
    DBMS_OUTPUT.PUT_LINE('Session ID: ' || SYS_CONTEXT('USERENV', 'SESSIONID'));
    DBMS_OUTPUT.PUT_LINE('SID/Serial#: ' || v_sid || '/' || v_serial);
    DBMS_OUTPUT.PUT_LINE('IP Address: ' || SYS_CONTEXT('USERENV', 'IP_ADDRESS'));
    DBMS_OUTPUT.PUT_LINE('Host: ' || SYS_CONTEXT('USERENV', 'HOST'));
    DBMS_OUTPUT.PUT_LINE('Terminal: ' || SYS_CONTEXT('USERENV', 'TERMINAL'));
    DBMS_OUTPUT.PUT_LINE('Module: ' || SYS_CONTEXT('USERENV', 'MODULE'));
    DBMS_OUTPUT.PUT_LINE('Action: ' || SYS_CONTEXT('USERENV', 'ACTION'));
    DBMS_OUTPUT.PUT_LINE('Database: ' || SYS_CONTEXT('USERENV', 'DB_NAME'));
  END;
  
  FUNCTION is_session_valid(p_session_id NUMBER) RETURN BOOLEAN IS
    v_status VARCHAR2(20);
    v_last_activity DATE;
    v_timeout_minutes NUMBER;
  BEGIN
    SELECT STATUS, LAST_ACTIVITY, SESSION_TIMEOUT_MINUTES
    INTO v_status, v_last_activity, v_timeout_minutes
    FROM SESSION_TRACKING
    WHERE SESSION_ID = p_session_id;
    
    IF v_status != 'ACTIVE' THEN
      RETURN FALSE;
    END IF;
    
    -- Kiem tra timeout
    IF v_timeout_minutes IS NOT NULL AND 
       v_last_activity < SYSDATE - (v_timeout_minutes/1440) THEN
      RETURN FALSE;
    END IF;
    
    RETURN TRUE;
  EXCEPTION
    WHEN NO_DATA_FOUND THEN
      RETURN FALSE;
  END;
  
  PROCEDURE force_logout_user(p_username VARCHAR2) IS
    CURSOR c_user_sessions IS
      SELECT SESSION_ID
      FROM SESSION_TRACKING
      WHERE USERNAME = p_username
        AND STATUS = 'ACTIVE';
    
    v_count NUMBER := 0;
  BEGIN
    FOR rec IN c_user_sessions LOOP
      terminate_session(rec.SESSION_ID, 'FORCE_LOGOUT');
      v_count := v_count + 1;
    END LOOP;
    
    DBMS_OUTPUT.PUT_LINE('Force logged out ' || v_count || 
                        ' sessions for user ' || p_username);
  END;
  
  FUNCTION is_ip_allowed(p_ip VARCHAR2, p_username VARCHAR2) RETURN BOOLEAN IS
    v_role_code VARCHAR2(30);
  BEGIN
    -- Lay role cua user
    SELECT ROLE_CODE INTO v_role_code
    FROM TAIKHOAN
    WHERE USERNAME = p_username;
    
    -- Simple IP filtering logic
    -- Trong thuc te co the dung bang IP_WHITELIST
    
    -- Admin co the tu bat ky IP nao
    IF v_role_code = 'ADMIN' THEN
      RETURN TRUE;
    END IF;
    
    -- Cac user khac: kiem tra IP local hoac trusted
    IF p_ip LIKE '192.168.%' OR 
       p_ip LIKE '10.%' OR
       p_ip = '127.0.0.1' OR
       p_ip = 'localhost' THEN
      RETURN TRUE;
    END IF;
    
    -- Ghi log neu IP khong duoc phep
    INSERT INTO FGA_LOG(USERNAME, TABLE_NAME, POLICY_NAME, SQL_TEXT, CLIENT_IP)
    VALUES (p_username, 'SYSTEM', 'IP_FILTER', 
            'Access attempt from non-allowed IP: ' || p_ip,
            p_ip);
    
    COMMIT;
    
    RETURN FALSE;
  EXCEPTION
    WHEN OTHERS THEN
      RETURN TRUE; -- Default allow on error
  END;
  
  PROCEDURE generate_session_report(
    p_from_date DATE DEFAULT SYSDATE - 7,
    p_to_date DATE DEFAULT SYSDATE
  ) IS
    v_total_sessions NUMBER;
    v_avg_duration NUMBER;
    v_busiest_day VARCHAR2(20);
    v_most_active_user VARCHAR2(100);
  BEGIN
    DBMS_OUTPUT.PUT_LINE('=== SESSION REPORT (' || 
                         TO_CHAR(p_from_date, 'DD/MM/YYYY') || ' - ' ||
                         TO_CHAR(p_to_date, 'DD/MM/YYYY') || ') ===');
    
    -- Tong so session
    SELECT COUNT(*) INTO v_total_sessions
    FROM SESSION_HISTORY
    WHERE ACTION_TIME BETWEEN p_from_date AND p_to_date
      AND ACTION_TYPE = 'LOGIN';
    
    DBMS_OUTPUT.PUT_LINE('Tong so session: ' || v_total_sessions);
    
    -- Thoi gian trung binh
    SELECT ROUND(AVG(DURATION_MINUTES), 1) INTO v_avg_duration
    FROM SESSION_HISTORY
    WHERE ACTION_TIME BETWEEN p_from_date AND p_to_date
      AND ACTION_TYPE = 'LOGOUT'
      AND DURATION_MINUTES IS NOT NULL;
    
    DBMS_OUTPUT.PUT_LINE('Thoi gian session trung binh: ' || v_avg_duration || ' phut');
    
    -- Ngay nhieu session nhat
    BEGIN
      SELECT TO_CHAR(TRUNC(ACTION_TIME), 'DD/MM/YYYY') INTO v_busiest_day
      FROM (
        SELECT TRUNC(ACTION_TIME) as day, COUNT(*) as session_count
        FROM SESSION_HISTORY
        WHERE ACTION_TIME BETWEEN p_from_date AND p_to_date
          AND ACTION_TYPE = 'LOGIN'
        GROUP BY TRUNC(ACTION_TIME)
        ORDER BY COUNT(*) DESC
      ) WHERE ROWNUM = 1;
      
      DBMS_OUTPUT.PUT_LINE('Ngay nhieu session nhat: ' || v_busiest_day);
    EXCEPTION
      WHEN NO_DATA_FOUND THEN
        v_busiest_day := 'None';
    END;
    
    -- User nhieu session nhat
    BEGIN
      SELECT USERNAME INTO v_most_active_user
      FROM (
        SELECT USERNAME, COUNT(*) as session_count
        FROM SESSION_HISTORY
        WHERE ACTION_TIME BETWEEN p_from_date AND p_to_date
          AND ACTION_TYPE = 'LOGIN'
        GROUP BY USERNAME
        ORDER BY COUNT(*) DESC
      ) WHERE ROWNUM = 1;
      
      DBMS_OUTPUT.PUT_LINE('User nhieu session nhat: ' || v_most_active_user);
    EXCEPTION
      WHEN NO_DATA_FOUND THEN
        v_most_active_user := 'None';
    END;
    
    DBMS_OUTPUT.PUT_LINE('----------------------------------------');
    
    -- Top 5 session dai nhat
    DBMS_OUTPUT.PUT_LINE('TOP 5 SESSION DAI NHAT:');
    FOR rec IN (
      SELECT USERNAME, DURATION_MINUTES, REASON,
             TO_CHAR(ACTION_TIME, 'DD/MM HH24:MI') as END_TIME
      FROM SESSION_HISTORY
      WHERE ACTION_TYPE = 'LOGOUT'
        AND DURATION_MINUTES IS NOT NULL
        AND ACTION_TIME BETWEEN p_from_date AND p_to_date
      ORDER BY DURATION_MINUTES DESC
      FETCH FIRST 5 ROWS ONLY
    ) LOOP
      DBMS_OUTPUT.PUT_LINE('  ' || rec.USERNAME || ': ' || 
                          rec.DURATION_MINUTES || ' phut' ||
                          ' (Ket thuc: ' || rec.END_TIME || ', Ly do: ' || rec.REASON || ')');
    END LOOP;
    
  END;
  
  PROCEDURE analyze_suspicious_sessions IS
    v_long_sessions NUMBER;
    v_multiple_ip_sessions NUMBER;
    v_after_hours_sessions NUMBER;
  BEGIN
    DBMS_OUTPUT.PUT_LINE('=== PHAN TICH SESSION DANG NGO ===');
    
    -- Session qua dai (> 8 gio)
    SELECT COUNT(*) INTO v_long_sessions
    FROM SESSION_HISTORY
    WHERE ACTION_TYPE = 'LOGOUT'
      AND DURATION_MINUTES > 480 -- 8 hours
      AND ACTION_TIME > SYSDATE - 1;
    
    IF v_long_sessions > 0 THEN
      DBMS_OUTPUT.PUT_LINE('CANH BAO: Co ' || v_long_sessions || 
                          ' session dai hon 8 gio trong 24h qua');
    END IF;
    
    -- User dang nhap tu nhieu IP khac nhau
    SELECT COUNT(DISTINCT USERNAME) INTO v_multiple_ip_sessions
    FROM (
      SELECT USERNAME, COUNT(DISTINCT IP_ADDRESS) as ip_count
      FROM SESSION_HISTORY
      WHERE ACTION_TYPE = 'LOGIN'
        AND ACTION_TIME > SYSDATE - 1/24 -- 1 gi? qua
      GROUP BY USERNAME
      HAVING COUNT(DISTINCT IP_ADDRESS) > 1
    );
    
    IF v_multiple_ip_sessions > 0 THEN
      DBMS_OUTPUT.PUT_LINE('CANH BAO: Co user dang nhap tu nhieu IP khac nhau');
    END IF;
    
    -- Session ngoai gio hanh chinh
    SELECT COUNT(*) INTO v_after_hours_sessions
    FROM SESSION_HISTORY
    WHERE ACTION_TYPE = 'LOGIN'
      AND TO_CHAR(ACTION_TIME, 'HH24') BETWEEN '20' AND '06'
      AND ACTION_TIME > SYSDATE - 1;
    
    IF v_after_hours_sessions > 0 THEN
      DBMS_OUTPUT.PUT_LINE('CANH BAO: Co ' || v_after_hours_sessions || 
                          ' session ngoai gio hanh chinh trong 24h qua');
    END IF;
    
    IF v_long_sessions = 0 AND v_multiple_ip_sessions = 0 AND v_after_hours_sessions = 0 THEN
      DBMS_OUTPUT.PUT_LINE('Khong phat hien session dang ngo.');
    END IF;
    
  END;
  
END pkg_session_mgmt;
/

PROMPT 3.4: Tao job tu dong kiem tra session...
BEGIN
  -- Job kiem tra session timeout (moi 5 phut)
  DBMS_SCHEDULER.CREATE_JOB(
    job_name        => 'CHECK_SESSION_TIMEOUT_JOB',
    job_type        => 'PLSQL_BLOCK',
    job_action      => 'BEGIN pkg_session_mgmt.check_session_timeouts; END;',
    start_date      => SYSTIMESTAMP,
    repeat_interval => 'FREQ=MINUTELY; INTERVAL=5',
    enabled         => TRUE,
    comments        => 'Auto-check for session timeouts every 5 minutes'
  );
  
  DBMS_OUTPUT.PUT_LINE('Da tao job kiem tra session timeout');
EXCEPTION
  WHEN OTHERS THEN
    DBMS_OUTPUT.PUT_LINE('Khong the tao job: ' || SQLERRM);
END;
/

PROMPT  Da tao profile va session management nang cao

PROMPT *** BUOC 4: MA HOA LAI (HYBRID ENCRYPTION) HOAN THIEN ***

PROMPT 4.1: Tao bang luu file dinh kem nang cao...
BEGIN
  EXECUTE IMMEDIATE 'DROP TABLE HOADON_ATTACHMENTS CASCADE CONSTRAINTS';
EXCEPTION WHEN OTHERS THEN NULL;
END;
/

CREATE TABLE HOADON_ATTACHMENTS (
  ATTACHMENT_ID NUMBER PRIMARY KEY,
  MAHD NUMBER NOT NULL,
  FILE_NAME VARCHAR2(200),
  FILE_TYPE VARCHAR2(50),
  FILE_SIZE NUMBER, -- Bytes
  FILE_DATA_ENC BLOB,
  ENCRYPTION_ALGORITHM VARCHAR2(50) DEFAULT 'AES256+RSA',
  SESSION_KEY_ENC RAW(2000), -- Session key wrapped v?i RSA
  RSA_KEY_ID NUMBER,
  IV RAW(16),
  HMAC_KEY RAW(64), -- Key cho HMAC verification
  HMAC_VALUE RAW(2000), -- HMAC c?a d? li?u
  COMPRESSED CHAR(1) DEFAULT 'N',
  CREATED_AT DATE DEFAULT SYSDATE,
  CREATED_BY VARCHAR2(100),
  LAST_ACCESSED DATE,
  ACCESS_COUNT NUMBER DEFAULT 0,
  CONSTRAINT fk_attach_hd FOREIGN KEY (MAHD) REFERENCES HOADON(MAHD),
  CONSTRAINT fk_attach_rsa FOREIGN KEY (RSA_KEY_ID) REFERENCES RSA_KEYS(KEY_ID),
  CONSTRAINT chk_attach_compressed CHECK (COMPRESSED IN ('Y', 'N'))
) TABLESPACE SECURE_DATA;

CREATE SEQUENCE seq_attachments START WITH 1 INCREMENT BY 1;

-- B?ng l?u metadata file
BEGIN
  EXECUTE IMMEDIATE 'DROP TABLE FILE_METADATA CASCADE CONSTRAINTS';
EXCEPTION WHEN OTHERS THEN NULL;
END;
/

CREATE TABLE FILE_METADATA (
  METADATA_ID NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  ATTACHMENT_ID NUMBER NOT NULL,
  KEY_NAME VARCHAR2(100),
  KEY_VALUE VARCHAR2(4000),
  IS_ENCRYPTED CHAR(1) DEFAULT 'N',
  CREATED_AT DATE DEFAULT SYSDATE,
  CONSTRAINT fk_metadata_attach FOREIGN KEY (ATTACHMENT_ID) 
    REFERENCES HOADON_ATTACHMENTS(ATTACHMENT_ID)
);

PROMPT 4.2: Tao package ma hoa lai hoan thien...
CREATE OR REPLACE PACKAGE pkg_hybrid_encryption AS
  
  -- ==== FILE ENCRYPTION ====
  -- Ma hoa file lon bang hybrid encryption
  PROCEDURE encrypt_file_hybrid(
    p_mahd NUMBER,
    p_file_name VARCHAR2,
    p_file_data CLOB,
    p_file_type VARCHAR2 DEFAULT 'TEXT',
    p_compress CHAR(1) DEFAULT 'N',
    p_rsa_key_id NUMBER,
    p_attachment_id OUT NUMBER
  );
  
  -- Ma hoa file binary (BLOB)
  PROCEDURE encrypt_blob_hybrid(
    p_mahd NUMBER,
    p_file_name VARCHAR2,
    p_file_data BLOB,
    p_file_type VARCHAR2,
    p_rsa_key_id NUMBER,
    p_attachment_id OUT NUMBER
  );
  
  -- ==== FILE DECRYPTION ====
  -- Giai ma file
  FUNCTION decrypt_file(
    p_attachment_id NUMBER
  ) RETURN CLOB;
  
  -- Giai ma file binary
  FUNCTION decrypt_blob(
    p_attachment_id NUMBER
  ) RETURN BLOB;
  
  -- ==== INTEGRITY VERIFICATION ====
  -- Tinh HMAC cho du lieu
  FUNCTION calculate_hmac(
    p_data RAW,
    p_key RAW
  ) RETURN RAW;
  
  -- Xac thuc tinh toan ven
  FUNCTION verify_integrity(
    p_attachment_id NUMBER
  ) RETURN BOOLEAN;
  
  -- ==== KEY MANAGEMENT ====
  -- Tao session key ngau nhien
  FUNCTION generate_session_key RETURN RAW;
  
  -- Wrap session key voi RSA
  FUNCTION wrap_session_key(
    p_session_key RAW,
    p_rsa_key_id NUMBER
  ) RETURN RAW;
  
  -- Unwrap session key
  FUNCTION unwrap_session_key(
    p_wrapped_key RAW,
    p_rsa_key_id NUMBER
  ) RETURN RAW;
  
  -- ==== FILE OPERATIONS ====
  -- Log file access
  PROCEDURE log_file_access(p_attachment_id NUMBER);
  
  -- Xem thong tin file
  PROCEDURE get_file_info(p_attachment_id NUMBER);
  
  -- Xoa file (secure delete)
  PROCEDURE secure_delete_file(p_attachment_id NUMBER);
  
END pkg_hybrid_encryption;
/

CREATE OR REPLACE PACKAGE BODY pkg_hybrid_encryption AS
  
  -- ==== FILE ENCRYPTION ====
  PROCEDURE encrypt_file_hybrid(
    p_mahd NUMBER,
    p_file_name VARCHAR2,
    p_file_data CLOB,
    p_file_type VARCHAR2 DEFAULT 'TEXT',
    p_compress CHAR(1) DEFAULT 'N',
    p_rsa_key_id NUMBER,
    p_attachment_id OUT NUMBER
  ) IS
    v_session_key RAW(32);
    v_iv RAW(16);
    v_encrypted_data BLOB;
    v_wrapped_key RAW(2000);
    v_hmac_key RAW(64);
    v_hmac_value RAW(2000);
    v_file_raw RAW(32767);
    v_compressed_raw RAW(32767);
  BEGIN
    -- Tao ID attachment
    p_attachment_id := seq_attachments.NEXTVAL;
    
    -- Buoc 1: Tao session key ngau nhien
    v_session_key := generate_session_key();
    
    -- Buoc 2: Tao IV
    v_iv := SYS.DBMS_CRYPTO.RANDOMBYTES(16);
    
    -- Buoc 3: Chuyen CLOB sang RAW
    v_file_raw := UTL_RAW.CAST_TO_RAW(DBMS_LOB.SUBSTR(p_file_data, 32767, 1));
    
    -- Buoc 4: Neu can, nen du lieu
    IF p_compress = 'Y' THEN
      -- Mo phong compression (trong thuc te dung UTL_COMPRESS)
      v_compressed_raw := v_file_raw; -- Gi? s? không nén
    ELSE
      v_compressed_raw := v_file_raw;
    END IF;
    
    -- Buoc 5: Ma hoa du lieu bang AES
    v_encrypted_data := SYS.DBMS_CRYPTO.ENCRYPT(
      v_compressed_raw,
      SYS.DBMS_CRYPTO.ENCRYPT_AES256 + SYS.DBMS_CRYPTO.CHAIN_CBC + SYS.DBMS_CRYPTO.PAD_PKCS5,
      v_session_key,
      v_iv
    );
    
    -- Buoc 6: Ma hoa session key bang RSA (key wrapping)
    v_wrapped_key := wrap_session_key(v_session_key, p_rsa_key_id);
    
    -- Buoc 7: Tao HMAC key va tinh HMAC
    v_hmac_key := SYS.DBMS_CRYPTO.RANDOMBYTES(64);
    v_hmac_value := calculate_hmac(v_compressed_raw, v_hmac_key);
    
    -- Buoc 8: Luu tat ca vao database
    INSERT INTO HOADON_ATTACHMENTS(
      ATTACHMENT_ID, MAHD, FILE_NAME, FILE_TYPE, FILE_SIZE,
      FILE_DATA_ENC, ENCRYPTION_ALGORITHM, SESSION_KEY_ENC,
      RSA_KEY_ID, IV, HMAC_KEY, HMAC_VALUE, COMPRESSED,
      CREATED_AT, CREATED_BY
    ) VALUES (
      p_attachment_id, p_mahd, p_file_name, p_file_type,
      DBMS_LOB.GETLENGTH(v_encrypted_data),
      v_encrypted_data, 'AES256+RSA', v_wrapped_key,
      p_rsa_key_id, v_iv, v_hmac_key, v_hmac_value, p_compress,
      SYSDATE, USER
    );
    
    -- Buoc 9: Luu metadata
    INSERT INTO FILE_METADATA(ATTACHMENT_ID, KEY_NAME, KEY_VALUE)
    VALUES (p_attachment_id, 'ORIGINAL_SIZE', TO_CHAR(DBMS_LOB.GETLENGTH(v_file_raw)));
    
    INSERT INTO FILE_METADATA(ATTACHMENT_ID, KEY_NAME, KEY_VALUE)
    VALUES (p_attachment_id, 'ENCRYPTION_TIME', TO_CHAR(SYSDATE, 'YYYY-MM-DD HH24:MI:SS'));
    
    COMMIT;
    
    DBMS_OUTPUT.PUT_LINE('Da ma hoa file: ' || p_file_name || 
                        ' (Attachment ID: ' || p_attachment_id || ')');
  END;
  
  PROCEDURE encrypt_blob_hybrid(
    p_mahd NUMBER,
    p_file_name VARCHAR2,
    p_file_data BLOB,
    p_file_type VARCHAR2,
    p_rsa_key_id NUMBER,
    p_attachment_id OUT NUMBER
  ) IS
    v_session_key RAW(32);
    v_iv RAW(16);
    v_encrypted_data BLOB;
    v_wrapped_key RAW(2000);
    v_hmac_key RAW(64);
    v_hmac_value RAW(2000);
    v_blob_raw RAW(32767);
  BEGIN
    p_attachment_id := seq_attachments.NEXTVAL;
    
    -- Tao session key
    v_session_key := generate_session_key();
    
    -- Tao IV
    v_iv := SYS.DBMS_CRYPTO.RANDOMBYTES(16);
    
    -- Lay BLOB data (dau tien 32KB)
    v_blob_raw := DBMS_LOB.SUBSTR(p_file_data, 32767, 1);
    
    -- Ma hoa
    v_encrypted_data := SYS.DBMS_CRYPTO.ENCRYPT(
      v_blob_raw,
      SYS.DBMS_CRYPTO.ENCRYPT_AES256 + SYS.DBMS_CRYPTO.CHAIN_CBC + SYS.DBMS_CRYPTO.PAD_PKCS5,
      v_session_key,
      v_iv
    );
    
    -- Wrap session key
    v_wrapped_key := wrap_session_key(v_session_key, p_rsa_key_id);
    
    -- HMAC
    v_hmac_key := SYS.DBMS_CRYPTO.RANDOMBYTES(64);
    v_hmac_value := calculate_hmac(v_blob_raw, v_hmac_key);
    
    -- Luu
    INSERT INTO HOADON_ATTACHMENTS(
      ATTACHMENT_ID, MAHD, FILE_NAME, FILE_TYPE, FILE_SIZE,
      FILE_DATA_ENC, ENCRYPTION_ALGORITHM, SESSION_KEY_ENC,
      RSA_KEY_ID, IV, HMAC_KEY, HMAC_VALUE,
      CREATED_AT, CREATED_BY
    ) VALUES (
      p_attachment_id, p_mahd, p_file_name, p_file_type,
      DBMS_LOB.GETLENGTH(p_file_data),
      v_encrypted_data, 'AES256+RSA', v_wrapped_key,
      p_rsa_key_id, v_iv, v_hmac_key, v_hmac_value,
      SYSDATE, USER
    );
    
    COMMIT;
    
    DBMS_OUTPUT.PUT_LINE('Da ma hoa BLOB file: ' || p_file_name);
  END;
  
  -- ==== FILE DECRYPTION ====
  FUNCTION decrypt_file(
    p_attachment_id NUMBER
  ) RETURN CLOB IS
    v_encrypted_data BLOB;
    v_session_key_enc RAW(2000);
    v_iv RAW(16);
    v_hmac_key RAW(64);
    v_hmac_value RAW(2000);
    v_rsa_key_id NUMBER;
    v_session_key RAW(32);
    v_decrypted_raw RAW(32767);
    v_clob CLOB;
    v_integrity_ok BOOLEAN;
  BEGIN
    -- Lay du lieu da ma hoa
    SELECT FILE_DATA_ENC, SESSION_KEY_ENC, IV, HMAC_KEY, HMAC_VALUE, RSA_KEY_ID
    INTO v_encrypted_data, v_session_key_enc, v_iv, v_hmac_key, v_hmac_value, v_rsa_key_id
    FROM HOADON_ATTACHMENTS
    WHERE ATTACHMENT_ID = p_attachment_id;
    
    -- Buoc 1: Unwrap session key
    v_session_key := unwrap_session_key(v_session_key_enc, v_rsa_key_id);
    
    -- Buoc 2: Giai ma du lieu
    v_decrypted_raw := SYS.DBMS_CRYPTO.DECRYPT(
      v_encrypted_data,
      SYS.DBMS_CRYPTO.ENCRYPT_AES256 + SYS.DBMS_CRYPTO.CHAIN_CBC + SYS.DBMS_CRYPTO.PAD_PKCS5,
      v_session_key,
      v_iv
    );
    
    -- Buoc 3: Kiem tra tinh toan ven
    v_integrity_ok := (calculate_hmac(v_decrypted_raw, v_hmac_key) = v_hmac_value);
    
    IF NOT v_integrity_ok THEN
      RAISE_APPLICATION_ERROR(-20030, 'File integrity check failed!');
    END IF;
    
    -- Buoc 4: Chuyen RAW sang CLOB
    DBMS_LOB.CREATETEMPORARY(v_clob, TRUE);
    DBMS_LOB.WRITEAPPEND(v_clob, UTL_RAW.LENGTH(v_decrypted_raw), v_decrypted_raw);
    
    -- Buoc 5: Log access
    log_file_access(p_attachment_id);
    
    RETURN v_clob;
  EXCEPTION
    WHEN OTHERS THEN
      RETURN EMPTY_CLOB();
  END;
  
  FUNCTION decrypt_blob(
    p_attachment_id NUMBER
  ) RETURN BLOB IS
    v_encrypted_data BLOB;
    v_session_key_enc RAW(2000);
    v_iv RAW(16);
    v_rsa_key_id NUMBER;
    v_session_key RAW(32);
    v_decrypted_blob BLOB;
  BEGIN
    SELECT FILE_DATA_ENC, SESSION_KEY_ENC, IV, RSA_KEY_ID
    INTO v_encrypted_data, v_session_key_enc, v_iv, v_rsa_key_id
    FROM HOADON_ATTACHMENTS
    WHERE ATTACHMENT_ID = p_attachment_id;
    
    -- Unwrap session key
    v_session_key := unwrap_session_key(v_session_key_enc, v_rsa_key_id);
    
    -- Giai ma
    v_decrypted_blob := SYS.DBMS_CRYPTO.DECRYPT(
      v_encrypted_data,
      SYS.DBMS_CRYPTO.ENCRYPT_AES256 + SYS.DBMS_CRYPTO.CHAIN_CBC + SYS.DBMS_CRYPTO.PAD_PKCS5,
      v_session_key,
      v_iv
    );
    
    -- Log access
    log_file_access(p_attachment_id);
    
    RETURN v_decrypted_blob;
  EXCEPTION
    WHEN OTHERS THEN
      RETURN EMPTY_BLOB();
  END;
  
  -- ==== INTEGRITY VERIFICATION ====
  FUNCTION calculate_hmac(
    p_data RAW,
    p_key RAW
  ) RETURN RAW IS
  BEGIN
    RETURN SYS.DBMS_CRYPTO.MAC(
      p_data,
      SYS.DBMS_CRYPTO.HMAC_SH256,
      p_key
    );
  END;
  
  FUNCTION verify_integrity(
    p_attachment_id NUMBER
  ) RETURN BOOLEAN IS
    v_encrypted_data BLOB;
    v_session_key_enc RAW(2000);
    v_iv RAW(16);
    v_hmac_key RAW(64);
    v_hmac_value RAW(2000);
    v_rsa_key_id NUMBER;
    v_session_key RAW(32);
    v_decrypted_raw RAW(32767);
    v_calculated_hmac RAW(2000);
  BEGIN
    SELECT FILE_DATA_ENC, SESSION_KEY_ENC, IV, HMAC_KEY, HMAC_VALUE, RSA_KEY_ID
    INTO v_encrypted_data, v_session_key_enc, v_iv, v_hmac_key, v_hmac_value, v_rsa_key_id
    FROM HOADON_ATTACHMENTS
    WHERE ATTACHMENT_ID = p_attachment_id;
    
    -- Unwrap session key
    v_session_key := unwrap_session_key(v_session_key_enc, v_rsa_key_id);
    
    -- Giai ma
    v_decrypted_raw := SYS.DBMS_CRYPTO.DECRYPT(
      v_encrypted_data,
      SYS.DBMS_CRYPTO.ENCRYPT_AES256 + SYS.DBMS_CRYPTO.CHAIN_CBC + SYS.DBMS_CRYPTO.PAD_PKCS5,
      v_session_key,
      v_iv
    );
    
    -- Tinh HMAC va so sanh
    v_calculated_hmac := calculate_hmac(v_decrypted_raw, v_hmac_key);
    
    RETURN v_calculated_hmac = v_hmac_value;
  EXCEPTION
    WHEN OTHERS THEN
      RETURN FALSE;
  END;
  
  -- ==== KEY MANAGEMENT ====
  FUNCTION generate_session_key RETURN RAW IS
  BEGIN
    RETURN SYS.DBMS_CRYPTO.RANDOMBYTES(32); -- AES-256 key
  END;
  
  FUNCTION wrap_session_key(
    p_session_key RAW,
    p_rsa_key_id NUMBER
  ) RETURN RAW IS
  BEGIN
    -- Su dung package pkg_asymmetric
    RETURN pkg_asymmetric.wrap_aes_key(p_session_key, p_rsa_key_id);
  END;
  
  FUNCTION unwrap_session_key(
    p_wrapped_key RAW,
    p_rsa_key_id NUMBER
  ) RETURN RAW IS
  BEGIN
    RETURN pkg_asymmetric.unwrap_aes_key(p_wrapped_key, p_rsa_key_id);
  END;
  
  -- ==== FILE OPERATIONS ====
  PROCEDURE log_file_access(p_attachment_id NUMBER) IS
  BEGIN
    UPDATE HOADON_ATTACHMENTS
    SET LAST_ACCESSED = SYSDATE,
        ACCESS_COUNT = ACCESS_COUNT + 1
    WHERE ATTACHMENT_ID = p_attachment_id;
    
    COMMIT;
  END;
  
  PROCEDURE get_file_info(p_attachment_id NUMBER) IS
    v_file_name VARCHAR2(200);
    v_file_type VARCHAR2(50);
    v_file_size NUMBER;
    v_created_at DATE;
    v_access_count NUMBER;
    v_integrity_ok BOOLEAN;
  BEGIN
    SELECT FILE_NAME, FILE_TYPE, FILE_SIZE, CREATED_AT, ACCESS_COUNT
    INTO v_file_name, v_file_type, v_file_size, v_created_at, v_access_count
    FROM HOADON_ATTACHMENTS
    WHERE ATTACHMENT_ID = p_attachment_id;
    
    -- Kiem tra tinh toan ven
    v_integrity_ok := verify_integrity(p_attachment_id);
    
    DBMS_OUTPUT.PUT_LINE('=== FILE INFORMATION ===');
    DBMS_OUTPUT.PUT_LINE('ID: ' || p_attachment_id);
    DBMS_OUTPUT.PUT_LINE('Name: ' || v_file_name);
    DBMS_OUTPUT.PUT_LINE('Type: ' || v_file_type);
    DBMS_OUTPUT.PUT_LINE('Size: ' || v_file_size || ' bytes');
    DBMS_OUTPUT.PUT_LINE('Created: ' || TO_CHAR(v_created_at, 'YYYY-MM-DD HH24:MI:SS'));
    DBMS_OUTPUT.PUT_LINE('Access count: ' || v_access_count);
    DBMS_OUTPUT.PUT_LINE('Integrity: ' || CASE WHEN v_integrity_ok THEN 'OK' ELSE 'COMPROMISED' END);
  END;
  
  PROCEDURE secure_delete_file(p_attachment_id NUMBER) IS
    v_encrypted_data BLOB;
  BEGIN
    -- Lay BLOB data
    SELECT FILE_DATA_ENC INTO v_encrypted_data
    FROM HOADON_ATTACHMENTS
    WHERE ATTACHMENT_ID = p_attachment_id;
    
    -- Xoa metadata truoc
    DELETE FROM FILE_METADATA WHERE ATTACHMENT_ID = p_attachment_id;
    
    -- Xoa file (Oracle se tu dong xoa BLOB)
    DELETE FROM HOADON_ATTACHMENTS WHERE ATTACHMENT_ID = p_attachment_id;
    
    COMMIT;
    
    DBMS_OUTPUT.PUT_LINE('Da xoa file attachment ID: ' || p_attachment_id);
    
    -- Ghi audit log
    pkg_audit.log_action('HOADON_ATTACHMENTS', 'DELETE', 
                         TO_CHAR(p_attachment_id), NULL, 'Secure delete');
  END;
  
END pkg_hybrid_encryption;
/

PROMPT 4.3: Tao view xem file dinh kem...
CREATE OR REPLACE VIEW v_hoadon_attachments AS
SELECT 
  a.ATTACHMENT_ID,
  a.MAHD,
  h.TONGTIEN,
  h.TRANGTHAI,
  a.FILE_NAME,
  a.FILE_TYPE,
  a.FILE_SIZE,
  a.ENCRYPTION_ALGORITHM,
  k.KEY_NAME AS RSA_KEY_NAME,
  TO_CHAR(a.CREATED_AT, 'YYYY-MM-DD HH24:MI:SS') AS CREATED_AT,
  a.CREATED_BY,
  a.LAST_ACCESSED,
  a.ACCESS_COUNT,
  CASE 
    WHEN a.LAST_ACCESSED IS NULL THEN 'NEVER'
    ELSE TO_CHAR(a.LAST_ACCESSED, 'YYYY-MM-DD HH24:MI')
  END AS LAST_ACCESS_TIME
FROM HOADON_ATTACHMENTS a
JOIN HOADON h ON a.MAHD = h.MAHD
JOIN RSA_KEYS k ON a.RSA_KEY_ID = k.KEY_ID
ORDER BY a.CREATED_AT DESC;
/

-- View thong ke file attachment
CREATE OR REPLACE VIEW v_attachment_statistics AS
SELECT 
  FILE_TYPE,
  COUNT(*) AS FILE_COUNT,
  SUM(FILE_SIZE) AS TOTAL_SIZE,
  ROUND(AVG(FILE_SIZE), 2) AS AVG_SIZE,
  MIN(CREATED_AT) AS FIRST_FILE,
  MAX(CREATED_AT) AS LAST_FILE,
  SUM(ACCESS_COUNT) AS TOTAL_ACCESSES
FROM HOADON_ATTACHMENTS
GROUP BY FILE_TYPE
ORDER BY FILE_COUNT DESC;
/

PROMPT ========================================
PROMPT HOAN THANH FILE 4: SETUP CRYPTO & PROFILE!
PROMPT 
PROMPT ========================================
PROMPT HE THONG DA HOAN TAT CAI DAT
PROMPT ========================================
PROMPT 
PROMPT TAI KHOAN MAC DINH:
PROMPT 1. admin / Admin@123 (ADMIN)
PROMPT 2. quanly / Quanly@123 (QUANLY)
PROMPT 3. phucvu / Phucvu@123 (NHANVIEN)
PROMPT 4. thukho / Thukho@123 (THUKHO)
PROMPT 
PROMPT CHUC MUNG! HE THONG DA SAN SANG SU DUNG.
PROMPT ========================================

-- Kiem tra tong quan
SELECT 'RSA Keys' as Item, COUNT(*) as Count FROM RSA_KEYS
UNION ALL
SELECT 'FGA Logs', COUNT(*) FROM FGA_LOG
UNION ALL
SELECT 'Session Tracking', COUNT(*) FROM SESSION_TRACKING
UNION ALL
SELECT 'User Profiles', COUNT(*) FROM USER_PROFILES
UNION ALL
SELECT 'Attachments', COUNT(*) FROM HOADON_ATTACHMENTS
UNION ALL
SELECT 'Digital Signatures', COUNT(*) FROM DIGITAL_SIGNATURES;