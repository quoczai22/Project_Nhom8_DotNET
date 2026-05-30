<div align="center">

<img src="https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 8.0"/>
<img src="https://img.shields.io/badge/WPF-MVVM-3f51b5?style=for-the-badge&logo=windows&logoColor=white" alt="WPF MVVM"/>
<img src="https://img.shields.io/badge/SQL%20Server-EF%20Core-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" alt="SQL Server"/>
<img src="https://img.shields.io/badge/Schema-dbo-orange?style=for-the-badge" alt="Schema dbo"/>

<br/><br/>

# 🖥️ Ứng Dụng Quản Lý Linh Kiện Máy Tính

> Đồ án cuối kỳ học phần Công nghệ .NET — Lớp: HUIT_dotNET-WPF.HK2.2526
> Khoa Công nghệ Thông tin — Trường Đại học Công Thương Thành phố Hồ Chí Minh (HUIT)

<br/>

[![Download Setup](https://img.shields.io/github/v/release/quoczai22/Project_Nhom8_DotNET?style=flat-square&label=⬇️%20Tải%20Bộ%20Cài%20Đặt&color=3f51b5)](https://github.com/quoczai22/Project_Nhom8_DotNET/releases/latest)
[![GitHub stars](https://img.shields.io/github/stars/quoczai22/Project_Nhom8_DotNET?style=flat-square&color=3f51b5)](https://github.com/quoczai22/Project_Nhom8_DotNET/stargazers)
[![GitHub issues](https://img.shields.io/github/issues/quoczai22/Project_Nhom8_DotNET?style=flat-square&color=3f51b5)](https://github.com/quoczai22/Project_Nhom8_DotNET/issues)

</div>

---

## 📋 Mục lục

- [Giới thiệu](#-giới-thiệu)
- [Tính năng nổi bật](#-tính-năng-nổi-bật)
- [Công nghệ sử dụng](#-công-nghệ-sử-dụng)
- [Kiến trúc hệ thống](#-kiến-trúc-hệ-thống)
- [Cấu trúc thư mục](#-cấu-trúc-thư-mục)
- [Hướng dẫn cài đặt & Cấu hình](#-hướng-dẫn-cài-đặt--cấu-hình)
- [Bảng phân công nhiệm vụ](#-bảng-phân-con-nhiệm-vụ)
- [Đối chiếu tiêu chí chấm điểm](#-đối-chiếu-tiêu-chí-chấm-điểm)
- [Thông tin nhóm thực hiện](#-thông-tin-nhóm-thực-hiện)

---

## 🚀 Giới thiệu

Hoạt động kinh doanh linh kiện máy tính có đặc thù dữ liệu thay đổi liên tục về sản phẩm, nhà sản xuất, khách hàng, nhân viên, nhập kho và bán hàng. Ứng dụng **Quản Lý Linh Kiện Máy Tính** được Nhóm 8 xây dựng nhằm giải quyết triệt để các vấn đề sai lệch tồn kho, khó tra cứu giao dịch và tối ưu hóa quy trình phân quyền vận hành cửa hàng.

Ứng dụng được phát triển hoàn chỉnh trên nền tảng **WPF .NET 8.0**, tuân thủ nghiêm ngặt mô hình **MVVM sạch** giúp tách biệt giao diện và logic xử lý, kết nối cơ sở dữ liệu **SQL Server (hướng Database First)** thông qua **Entity Framework Core 8**.

---

## ✨ Tính năng nổi bật

### 🔐 1. Xác thực & Phân quyền Hệ thống (Database-Level Security)
- Hệ thống định nghĩa 4 nhóm quyền chính: *Quản lý toàn bộ, Thu ngân, Chăm sóc khách hàng, và Nhân viên kho*.
- Giao diện tự động ẩn/hiện các menu chức năng dựa theo quyền của tài khoản lưu trong `LuuTrangThai`.
- Nhân viên đã nghỉ việc (cờ `DaNghiViec = true` hoặc `DaNghiViec = 1`) sẽ bị khóa tài khoản hệ thống ngay lập tức.

### 📦 2. Quản lý Thực thể nâng cao (Advanced CRUD)
- Hỗ trợ đầy đủ nghiệp vụ cho các phân hệ: *Linh Kiện, Loại Linh Kiện, Nhà Sản Xuất, Nhân Viên, và Khách Hàng*.
- Chức năng tìm kiếm áp dụng bộ lọc nâng cao trên **`ICollectionView.Filter`**, thực hiện tính toán trực tiếp trên RAM, đem lại phản hồi tức thì và không làm nghẽn đường truyền SQL Server khi gõ phím.

### 🧾 3. Luồng nghiệp vụ Hóa đơn bán hàng
- Mã hóa đơn tự động sinh tăng dần theo định dạng chuẩn (`HD001`, `HD002`,...).
- Tự động kiểm tra số lượng tồn kho thực tế và đưa ra cảnh báo ngăn chặn nếu vượt quá số lượng hiện có.
- Toàn bộ quá trình lưu dữ liệu (`HoaDon` và hàng loạt `ChiTietHd`) được bọc trong một **Database Transaction bất đồng bộ**, tự động trừ số lượng tồn kho khi thành công và hủy bỏ (`Rollback`) toàn bộ nếu xảy ra lỗi giữa chừng.

### 📥 4. Luồng nghiệp vụ Phiếu Nhập Kho
- Hỗ trợ tạo phiếu nhập từ phía nhà sản xuất, tính toán chi phí theo thời gian thực dựa trên số lượng và đơn giá nhập.
- Áp dụng cơ chế Transaction đối xứng: Tự động **cộng số lượng tồn kho** khi lưu phiếu nhập thành công.
- Tích hợp **chốt chặn kiểm tra thông minh khi xóa phiếu nhập**: Hệ thống đối chiếu lượng tồn kho hiện tại trước khi trừ bớt kho, ngăn chặn tuyệt đối lỗi âm kho nếu hàng hóa đó đã được bán lẻ cho khách từ trước.

### 📊 5. Thống kê, Dashboard & Cảnh báo thông minh
- Dashboard trang chủ hiển thị các thẻ số liệu tổng quan trực quan.
- Biểu đồ đường phân tích doanh thu theo từng quý giai đoạn 2023 - 2026 thông qua việc thiết lập ánh xạ (`mapping`) dữ liệu trực tiếp từ hàm chuyên sâu **`fn_DoanhThuTheoThang`** dưới SQL Server.
- Biểu đồ tròn phân bổ nhân sự theo chức vụ (`LiveCharts.Wpf`).
- Tích hợp chuông thông báo kết nối thủ tục **`sp_BaoCaoTonKho`** — tự động quét và hiển thị toàn bộ linh kiện có số lượng tồn kho thấp dưới 10 đơn vị trong một cửa sổ riêng.

### 💸 6. Tích hợp cổng thanh toán trực tuyến MoMo
- Cho phép tùy chọn phương thức thanh toán giữa Tiền mặt hoặc Ví điện tử MoMo.
- Gọi API MoMo (môi trường Sandbox) để nhận chuỗi phản hồi, tự động sinh mã QR động kèm đồng hồ đếm ngược thời gian thực (60 giây).
- Áp dụng cơ chế **Polling (mỗi 3 giây)** kết hợp xử lý chữ ký số bảo mật **`HMAC-SHA256`** chuẩn alphabet để đồng bộ trạng thái giao dịch tự động.

### 🖨️ 7. Phân hệ Báo cáo & In ấn chuyên nghiệp (WPF FlowDocument)
Thay vì sử dụng các công cụ bên thứ ba kém ổn định trên môi trường .NET hiện đại, nhóm triển khai trực tiếp bằng công nghệ **`FlowDocument` và `FlowDocumentReader`** của WPF:
- **Báo cáo đơn giản:** In hóa đơn bán hàng đầy đủ thông tin header, bảng chi tiết sản phẩm, tổng tiền, phương thức thanh toán và footer chữ ký.
- **Báo cáo nâng cao:** Báo cáo tổng hợp doanh thu theo hãng sản xuất có tính năng gom nhóm (`grouping`), thống kê chi tiết số lượng đơn, doanh thu, giá bán trung bình và tổng hợp so sánh cuối kỳ.

---

## 🛠️ Công nghệ sử dụng

| Thành phần | Công nghệ / Thư viện | Vai trò trong đồ án |
| :--- | :--- | :--- |
| **Ngôn ngữ** | C# 12 | Ngôn ngữ lập trình logic hệ thống |
| **Framework** | WPF (.NET 8.0) | Nền tảng xây dựng ứng dụng Desktop chính |
| **Kiến trúc UI** | MVVM (Model-View-ViewModel) | Tách biệt hoàn toàn View (XAML) và Logic qua `RelayCommand` |
| **ORM** | Entity Framework Core 8 | Ánh xạ thực thể C#, truy vấn bằng `AsNoTracking` và `Include` |
| **Cơ sở dữ liệu** | Microsoft SQL Server (T-SQL) | Lưu trữ chính (Schema mặc định: `dbo`) |
| **Cấu trúc CSDL** | Stored Procedure & Functions | Xử lý tác vụ tính toán doanh thu và quét tồn kho thấp |
| **Giao diện** | ResourceDictionary động | Quản lý Style hệ thống, hỗ trợ chuyển đổi Light/Dark theme |
| **Biểu đồ** | LiveCharts.Wpf | Vẽ biểu đồ đường doanh thu và biểu đồ tròn nhân sự |
| **Thanh toán** | MoMo Payment API Sandbox | Tích hợp sinh mã QR và xử lý kết quả thanh toán trực tuyến |

---

## 🏗️ Kiến trúc hệ thống
