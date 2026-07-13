# README - Ứng dụng Quản lý Quán Cafe

## Mục đích
Đây là README hướng dẫn sử dụng ứng dụng Quản lý Quán Cafe được xây dựng bằng C# Windows Forms. File này chỉ hướng dẫn cách cài đặt, chạy và dùng ứng dụng, không phải hướng dẫn cho thư viện Oracle.

---

## 1. Cấu trúc SQL trong project

### Có thư mục SQL không?
- Trong project hiện tại không có thư mục riêng tên `SQL`.
- Các file `.sql` nằm trực tiếp trong thư mục:
  - `QuanLyQuanCafe(Update)\QuanLyQuanCafe`

### Các file SQL cần chạy
Thứ tự chạy các file SQL:
1. `00_SETUP_SYS.sql`
2. `01_SETUP_CORE.sql`
3. `02_SETUP_ADVANCED_CLEANED.sql`
4. `03_SETUP_CRYPTO_PROFILE.sql`
5. `04_ADD_MISSING_PROCEDURES.sql`

> Mặc dù yêu cầu chỉ 4 file, project hiện có 5 file SQL khởi tạo. Nên chạy đủ cả 5 file theo đúng thứ tự trên.

---

## 2. Xác định project

### File solution / project
- File solution: `Nhom12_PhanMemQuanLyQuanCafe.sln`
- File project: `Nhom12_PhanMemQuanLyQuanCafe.csproj`

### Công nghệ sử dụng
- Ngôn ngữ: C#
- Loại ứng dụng: Windows Forms (WinForms)
- Framework: .NET Framework 4.8

---

## 3. Các màn hình / form chính

### Form đăng nhập
- `FormDangNhap`

### Form chính sau khi đăng nhập
- `FormMain`

### Các form chức năng
- `FormBanHang` - Bán hàng, tạo hóa đơn, thêm món, thanh toán
- `FormQuanLyMenu` - Quản lý thực đơn, thêm/sửa/ngừng bán món
- `FormQuanLyKho` - Quản lý kho, quản lý nguyên liệu, nhập hàng
- `FormQuanLyTaiKhoan` - Quản lý tài khoản người dùng
- `FormThongKe` - Thống kê doanh thu và món bán chạy
- `FormAuditLog` - Xem audit log thay đổi dữ liệu
- `FormOLS` - Mô phỏng Oracle Label Security (OLS) cho nguyên liệu
- `FormLichSuDangNhap` - Xem lịch sử đăng nhập

---

## 4. Role / loại người dùng

### Role có trong code và SQL
- `ADMIN`
- `QUANLY`
- `NHANVIEN`

### Role được sử dụng rõ ràng trong ứng dụng
- `ADMIN`: được xử lý đầy đủ trong code để mở menu quản trị
- `NHANVIEN`: được xử lý để cho phép chức năng bán hàng, kho, menu, thống kê

### Ghi chú
- Role `QUANLY` xuất hiện trong dữ liệu SQL, nhưng mã C# hiện tại không xử lý role này riêng biệt như `ADMIN` hoặc `NHANVIEN`.
- Do đó, nếu đăng nhập bằng `QUANLY`, quyền hiển thị trong FormMain có thể phụ thuộc vào logic của database và VPD, chứ không phải xác định rõ bằng code form.

---

## 5. Tài khoản mặc định

Trong file `01_SETUP_CORE.sql`, có các tài khoản mẫu sau:
- `admin` / `Admin@123` -> role `ADMIN`
- `quanly` / `Quanly@123` -> role `QUANLY`
- `phucvu` / `Phucvu@123` -> role `NHANVIEN`

> Lưu ý: Các tài khoản này được tạo trong script SQL. Nếu bạn chạy lại script trên môi trường khác, có thể cần tạo lại dữ liệu mẫu.

---

## 6. Hướng dẫn sử dụng ứng dụng

### Bước 1: Chạy file SQL
1. Mở Oracle SQL Developer hoặc công cụ tương tự.
2. Kết nối đến Oracle Database.
3. Chạy các file SQL theo thứ tự:
   - `00_SETUP_SYS.sql`
   - `01_SETUP_CORE.sql`
   - `02_SETUP_ADVANCED_CLEANED.sql`
   - `03_SETUP_CRYPTO_PROFILE.sql`
   - `04_ADD_MISSING_PROCEDURES.sql`
4. Đảm bảo các file chạy thành công và không có lỗi.

### Bước 2: Mở project trong Visual Studio
1. Mở Visual Studio.
2. Chọn `Open Project/Solution`.
3. Mở file solution: `Nhom12_PhanMemQuanLyQuanCafe.sln`.

### Bước 3: Build và chạy
1. Trong Visual Studio, chọn `Build Solution` hoặc `Rebuild Solution`.
2. Nếu cần restore package, chờ cho Visual Studio hoàn tất.
3. Chạy ứng dụng bằng `Start` hoặc `Ctrl+F5`.
4. Ứng dụng sẽ mở màn hình đăng nhập.

### Bước 4: Đăng nhập
1. Sau khi chạy, cửa sổ `FormDangNhap` sẽ hiện.
2. Nhập username/password.
3. Nếu đăng nhập thành công, ứng dụng sẽ mở `FormMain`.

### Bước 5: Sử dụng các chức năng

#### Nếu đăng nhập bằng `ADMIN`
- Quản lý tài khoản: mở `FormQuanLyTaiKhoan`.
- Xem lịch sử đăng nhập: mở `FormLichSuDangNhap`.
- Xem audit log: mở `FormAuditLog`.
- Quản lý OLS: mở `FormOLS`.
- Quản lý bán hàng: mở `FormBanHang`.
- Quản lý thực đơn: mở `FormQuanLyMenu`.
- Quản lý kho: mở `FormQuanLyKho`.
- Xem thống kê: mở `FormThongKe`.

#### Nếu đăng nhập bằng `NHANVIEN`
- Bán hàng: `FormBanHang`.
- Quản lý thực đơn: `FormQuanLyMenu`.
- Quản lý kho / nhập hàng: `FormQuanLyKho`.
- Xem thống kê bán hàng: `FormThongKe`.

#### Nếu đăng nhập bằng `QUANLY`
- Tồn tại trong dữ liệu mẫu nhưng ứng dụng không xử lý role này rõ ràng bằng code.
- Quyền truy cập có thể khác nhau tùy cấu hình database và VPD, nên ưu tiên sử dụng `ADMIN` hoặc `NHANVIEN` để kiểm tra tính năng.

---

## 7. Chức năng chính của ứng dụng

### Quản lý bán hàng
- Tạo hóa đơn mới.
- Thêm món vào hóa đơn.
- Thanh toán hóa đơn.
- Xem hóa đơn của nhân viên đang đăng nhập.

### Quản lý thực đơn
- Hiển thị danh sách món.
- Thêm món mới với tên, loại, giá.
- Sửa thông tin món.
- Ngừng bán món.

### Quản lý kho
- Hiển thị nguyên liệu và tồn kho.
- Lọc nguyên liệu theo nhà cung cấp.
- Nhập hàng và cập nhật tồn kho.

### Quản lý tài khoản
- Tạo tài khoản mới cho nhân viên.
- Chọn role `ADMIN` hoặc `NHANVIEN`.
- Hiển thị danh sách tài khoản và trạng thái đăng nhập.

### Thống kê
- Thống kê doanh thu theo ngày.
- Thống kê doanh thu theo món.
- Hiển thị top món bán chạy.

### Audit / lịch sử
- Xem audit log hành động dữ liệu.
- Lọc audit theo bảng, hành động, ngày.
- Xem lịch sử đăng nhập.

### OLS / bảo mật dữ liệu
- Mô phỏng truy cập nguyên liệu theo mức bảo mật PUBLIC / INTERNAL / CONFIDENTIAL.
- ADMIN có thể thêm nguyên liệu và cập nhật mức bảo mật.
- NHANVIEN có thể nhìn dữ liệu phù hợp quyền.

---

## 8. Lưu ý
- Nếu có lỗi đăng nhập, kiểm tra lại dữ liệu trong Oracle và tài khoản mặc định.
- Nếu ứng dụng không vào được form chính, kiểm tra lại việc chạy đầy đủ các file SQL.
- Ứng dụng cần Oracle Database sẵn sàng và có user kết nối được.
