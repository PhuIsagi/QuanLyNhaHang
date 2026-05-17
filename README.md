# Hệ Thống Quản Lý Nhà Hàng

**Lập trình Cơ sở dữ liệu**. Dự án xây dựng một hệ thống Web API phục vụ quy trình quản lý nhà hàng thực tế, bao gồm: Quản lý bàn ăn, gọi món, đồng bộ hóa với nhà bếp, thanh toán, và thống kê doanh thu.

## Công Nghệ & Phiên Bản Phần Mềm Sử Dụng

* **IDE Phát triển:** Visual Studio 2022
* **Nền tảng Backend:** .NET Core (C#)
* **Hệ quản trị CSDL:** Microsoft SQL Server (2019)
* **Công nghệ Truy xuất Dữ liệu:**
  * **Entity Framework Core:** Áp dụng cho các thao tác CRUD cơ bản và tự động tạo Database bằng EF Migrations.
  * **ADO.NET (Mô hình phi kết nối):** Dùng `SqlDataAdapter` và `DataTable` để truy xuất dữ liệu lớn từ Stored Procedure phục vụ xuất báo cáo.
  * **LINQ:** Xử lý, truy vấn và ánh xạ dữ liệu thành các DTO.
* **Frontend (Giao diện):** HTML5, CSS3, Bootstrap 5.0.2, jQuery.

## Kiến Trúc Hệ Thống

Hệ thống áp dụng kiến trúc 3 lớp:
1. **Presentation Layer (`Controllers`):** Chứa các Web API (RESTful), nhận và trả dữ liệu JSON.
2. **Business Logic Layer (`BLL` / `Services`):** Xử lý quy tắc nghiệp vụ, tính toán doanh thu, giờ chờ món.
3. **Data Access Layer (`DAL` / `Repositories`):** Tương tác trực tiếp với SQL Server.

## Hướng Dẫn Cài Đặt & Triển Khai
Sau khi git clone mã nguồn về máy tính, thực hiện các bước sau để thiết lập cơ sở dữ liệu cho hệ thống:

Lấy file sao lưu: Truy cập vào thư mục gốc của dự án, tìm và mở thư mục mang tên Database, sau đó tìm tệp tin sao lưu dữ liệu QuanLyNhaHang.bak.

Khôi phục trên SSMS: Khởi động công cụ Microsoft SQL Server Management Studio (SSMS).

Thao tác Restore: Nhấp chuột phải vào mục Databases trong cửa sổ Object Explorer, chọn Restore Database..., điều hướng đường dẫn đến tệp tin QuanLyNhaHang.bak.

Tra cứu tài khoản đăng nhập: Sau khi quá trình khôi phục thành công, thực hiện mở và truy vấn bảng dữ liệu nhanvien trực tiếp dưới SQL Server để kiểm tra thông tin danh sách tài khoản, vai trò (PhucVu, Bep, ThuNgan, QuanLy) và mật khẩu tương ứng dùng để đăng nhập.
