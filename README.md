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
