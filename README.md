# 🍽️ Hệ Thống Quản Lý Nhà Hàng (Web API)

Đây là mã nguồn Đồ án cuối kỳ môn **Lập trình Cơ sở dữ liệu**. Dự án xây dựng một hệ thống Web API phục vụ quy trình quản lý nhà hàng thực tế, bao gồm: Quản lý bàn ăn, gọi món (Order), đồng bộ hóa với nhà bếp, thanh toán, và thống kê doanh thu.

Hệ thống được thiết kế theo **Kiến trúc 3 Lớp (3-Layer Architecture)** và tuân thủ các tiêu chuẩn của **RESTful API** với định dạng giao tiếp **JSON**.

---

## 🛠️ Công Nghệ & Phiên Bản Phần Mềm Sử Dụng

Dự án được phát triển và kiểm thử trên các môi trường phần mềm sau:

* **IDE Phát triển:** Visual Studio 2022
* **Nền tảng Backend:** .NET Core (C#)
* **Hệ quản trị CSDL:** Microsoft SQL Server (2019 / 2022)
* **Công nghệ Truy xuất Dữ liệu:**
  * **Entity Framework Core:** Áp dụng cho các thao tác CRUD cơ bản và tự động tạo Database bằng EF Migrations.
  * **ADO.NET (Mô hình phi kết nối):** Dùng `SqlDataAdapter` và `DataTable` để truy xuất dữ liệu lớn từ Stored Procedure phục vụ xuất báo cáo.
  * **LINQ:** Xử lý, truy vấn và ánh xạ dữ liệu thành các DTO.
* **Frontend (Giao diện):** HTML5, CSS3, Bootstrap 5.0.2, jQuery (AJAX).

---

## 🏗️ Kiến Trúc Hệ Thống

Hệ thống áp dụng kiến trúc 3 lớp phân tách trách nhiệm rõ ràng:
1. **Presentation Layer (`Controllers`):** Chứa các Web API (RESTful), nhận và trả dữ liệu JSON.
2. **Business Logic Layer (`BLL` / `Services`):** Xử lý quy tắc nghiệp vụ, tính toán doanh thu, giờ chờ món.
3. **Data Access Layer (`DAL` / `Repositories`):** Tương tác trực tiếp với SQL Server.

---

## 🗄️ Lập Trình Cơ Sở Dữ Liệu (T-SQL)

Các xử lý phức tạp được đẩy xuống tầng CSDL thông qua T-SQL nhằm tối ưu hóa hiệu suất, bao gồm:

* **Stored Procedures & Transactions:**
  * `sp_TaoDonHang`: Tạo đơn hàng và thêm chi tiết món (Dùng lệnh `MERGE`). Cấu hình bọc `BEGIN TRAN / COMMIT` an toàn.
  * `sp_ThanhToanHoaDon`: Xử lý chốt hóa đơn, giải phóng bàn. Có bắt lỗi `ROLLBACK TRAN`.
  * `sp_ThongKeDoanhThu` & `sp_DoanhThuTheoNhom`: Trích xuất số liệu biểu đồ.
* **Views:**
  * `vw_DanhSachMonBep`: Lọc danh sách món đang chờ/đang nấu cho màn hình Đầu bếp.
  * `vw_Top10MonBanChay`: Thống kê xếp hạng món ăn.
* **Functions:** * `fn_TinhPhutCho`: Hàm tính thời gian thực tế khách đã chờ món.
* **Triggers:**
  * `trg_ThongBaoMonXong`: Tự động kích hoạt khi Đầu bếp bấm "Xong", chèn log vào bảng thông báo cho Phục vụ.

---

## 🚀 Hướng Dẫn Cài Đặt & Chạy Dự Án

**Bước 1: Clone mã nguồn về máy**
```bash
git clone [https://github.com/Phulsagi/QuanLyNhaHang.git](https://github.com/Phulsagi/QuanLyNhaHang.git)
