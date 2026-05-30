<div align="center">

<img src="https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 8.0"/>
<img src="https://img.shields.io/badge/WPF-MVVM-3f51b5?style=for-the-badge&logo=windows&logoColor=white" alt="WPF MVVM"/>
<img src="https://img.shields.io/badge/SQL%20Server-EF%20Core-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" alt="SQL Server"/>
<img src="https://img.shields.io/badge/Schema-dbo-orange?style=for-the-badge" alt="Schema dbo"/>

<br/><br/>

# 🖥️ Ứng Dụng Quản Lý Linh Kiện Máy Tính

> Ứng dụng desktop quản lý linh kiện máy tính toàn diện, xây dựng bằng **WPF .NET 8.0** theo kiến trúc **MVVM**, kết nối **SQL Server** qua **Entity Framework Core**.



**Đồ án cuối kỳ — Môn Công nghệ .NET Chiều T3 T7-T12**
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

```text
┌──────────────────────────────────────────────────────────────────────────────────────────────────┐
│                                           VIEW (XAML)                                            │
├──────────────────────────────────────────────────────────────────────────────────────────────────┤
│ - LoginView                        - MomoPaymentView             - ThemSuaLinhKienDialog         │
│ - MainWindow                       - ThongBaotonKhoWindow        - ThemSuaLoaiLinhKienDialog     │
│ - TrangChuView                     - ChonPhuongThucDialog        - ThemSuaKhachHangDialog        │
│ - LinhKienView                     - ThemHoaDonDialog            - ThemSuaNhanVienDialog         │
│ - LoaiLinhKienView                 - SuaHoaDonDialog             - ThemPhieuNhapDialog           │
│ - HoaDonView                       - KhachHangView                                               │
│ - PhieuNhapView                    - NhanVienView                                                │
└───────────────────────────────────────────────┬──────────────────────────────────────────────────┘
                                                │ Data Binding / Command (RelayCommand)
┌───────────────────────────────────────────────▼──────────────────────────────────────────────────┐
│                                           VIEWMODEL                                              │
├──────────────────────────────────────────────────────────────────────────────────────────────────┤
│ - BaseViewModel (INotifyPropertyChanged)    - LoginViewModel              - LoaiLinhKienViewModel│
│ - RelayCommand                              - MainViewModel               - HoaDonViewModel      │
│ - ISearchable (Interface tìm kiếm RAM)      - TrangChuViewModel           - KhachHangViewModel   │
│                                             - LinhKienViewModel           - NhanVienViewModel    │
│                                             - PhieuNhapViewModel          - BaoCaoViewModel      │
└───────────────────────────────────────────────┬──────────────────────────────────────────────────┘
                                                │ Entity Framework Core 8 (AsNoTracking / Include)
┌───────────────────────────────────────────────▼──────────────────────────────────────────────────┐
│                                       MODEL (SCHEMA: DBO)                                        │
├──────────────────────────────────────────────────────────────────────────────────────────────────┤
│ - QL_LinhKien_PC_Context                    - LinhKien                    - NhanVien             │
│ - QL_LinhKien_PC_Context.Functions          - LoaiLk                      - TaiKhoan             │
│ - QL_LinhKien_PC_ContextProcedures          - NhaSanXuat                  - ThongKeBanHang       │
│ - DataProvider (Kết nối dữ liệu)            - HoaDon / ChiTietHd          - ThongKeHang          │
│                                             - PhieuNhap / ChiTietPn       - sp_BaoCaoTonKhoResult│
└───────────────────────────────────────────────┬──────────────────────────────────────────────────┘
                                                │ T-SQL Driver Connection
┌───────────────────────────────────────────────▼──────────────────────────────────────────────────┐
│                                      MICROSOFT SQL SERVER                                        │
└──────────────────────────────────────────────────────────────────────────────────────────────────┘
📦 Repository Root
│
├── 📂 CSDL/                                        # Phân hệ lưu trữ script dữ liệu
│   └── QuanLyLinhKienMayTinh_NET.sql               # Script khởi tạo Database, bảng hệ thống, SP và Function
│
└── 📂 Project_Nhom8_DotNET/                        # Mã nguồn ứng dụng WPF chính
    │
    ├── 📂 Helpers/
    │   └── Multipasswordconverter.cs               # Hỗ trợ chuyển đổi giá trị password mã hóa cho PasswordBox
    │
    ├── 📂 Images/
    │   ├── avatar.png                              # Ảnh giao diện người dùng đại diện
    │   └── logo_momo.png                           # Biểu tượng phục vụ phân hệ MoMo API
    │
    ├── 📂 Models/                                  # Tầng định nghĩa thực thể và ngữ cảnh CSDL
    │   ├── QL_LinhKien_PC_Context.cs               # Lớp DbContext chính quản lý thực thể dữ liệu
    │   ├── QL_LinhKien_PC_Context.Functions.cs     # Khai báo cấu trúc ánh xạ Function hệ thống (fn_DoanhThuTheoThang)
    │   ├── QL_LinhKien_PC_ContextProcedures.cs     # Khai báo cấu trúc ánh xạ Stored Procedure (sp_BaoCaoTonKho)
    │   ├── DataProvider.cs                         # Khởi tạo Singleton quản lý chuỗi kết nối
    │   └── [Cơ số các file C# Entity tương ứng với các bảng: LinhKien, HoaDon, NhanVien...]
    │
    ├── 📂 Services/
    │   ├── IMomoService.cs                         # Định nghĩa cổng giao tiếp dịch vụ thanh toán trực tuyến
    │   ├── MomoService.cs                          # Xử lý chi tiết thuật toán băm chuỗi SHA256 và điều hướng API
    │   └── AutoIDServices.cs                       # Thuật toán tự sinh các mã ID tăng dần (HD, PN...)
    │
    ├── 📂 Themes/                                  # Hệ thống quản lý tài nguyên giao diện
    │   ├── ThemeLight.xaml                         # Khai báo màu sắc, kiểu dáng nền sáng
    │   └── ThemeDark.xaml                          # Khai báo màu sắc, kiểu dáng nền tối (Dark mode)
    │
    ├── 📂 ViewModels/                              # Tầng trung gian điều hướng và xử lý logic nghiệp vụ
    │   ├── BaseViewModel.cs                        # Triển khai gốc cho cơ chế thông báo đổi thuộc tính interface
    │   ├── RelayCommand.cs                         # Thiết lập các hành vi đóng gói Command
    │   ├── ISearchable.cs                          # Giao diện hỗ trợ bộ lọc RAM đồng nhất cho các danh mục
    │   └── [Các ViewModel tương ứng cho từng màn hình chức năng phụ trách]
    │
    ├── 📂 Views/                                   # Tầng hiển thị giao diện người dùng (gồm các XAML và Dialog)
    │   ├── LoginView.xaml                          # Màn hình thực hiện đăng nhập hệ thống
    │   ├── MainWindow.xaml                         # Khung ứng dụng chính tích hợp Sidebar điều hướng
    │   └── [Các phân hệ hiển thị: HoaDonView, PhieuNhapView, LinhKienView, KhachHangView...]
    │
    ├── 📄 App.xaml                                 # Điểm khởi tạo tài nguyên và nạp Theme mặc định của ứng dụng
    ├── 📄 App.xaml.cs                              # Điểm kích hoạt khởi chạy đầu tiên của toàn bộ chương trình WPF
    └── 📄 Project_Nhom8_DotNET.csproj              # File quản lý cấu hình project và các gói thư viện NuGet đi kèm
    ⚙️ Hướng dẫn cài đặt & Cấu hình
Cách 1: Sử dụng bộ cài đặt đóng gói (.exe)
Truy cập vào phân mục Releases của kho lưu trữ.

Tải tệp tin bộ cài đặt dạng thực thi có tên QuanLyLinhKien-Setup.exe về máy tính cục bộ.

Chạy tệp tin cài đặt và thực hiện theo luồng chỉ dẫn trên màn hình.

Yêu cầu hệ thống: Máy trạm cần nạp sẵn môi trường nền tảng .NET 8.0 Desktop Runtime và trỏ kết nối được đến máy chủ SQL Server.

Cách 2: Biên dịch trực tiếp từ mã nguồn hệ thống
1. Clone mã nguồn về máy tính

Bash
git clone [https://github.com/quoczai22/Project_Nhom8_DotNET.git](https://github.com/quoczai22/Project_Nhom8_DotNET.git)
cd Project_Nhom8_DotNET
2. Khởi tạo Cơ sở dữ liệu bằng T-SQL (Khuyên dùng)
Dự án sử dụng cơ sở dữ liệu Microsoft SQL Server chạy trực tiếp trên máy cục bộ (Local).

Mở SQL Server Management Studio (SSMS).

Mở file script CSDL/QuanLyLinhKienMayTinh_NET.sql.

Nhấn Execute (F5) để tự động khởi tạo CSDL, bảng, dữ liệu mẫu, Stored Procedures và Functions.

3. Cấu hình Chuỗi kết nối (Connection String)
Mở file Models/DataProvider.cs trong Visual Studio và điều chỉnh biến chứa chuỗi kết nối (ConnectionString) khớp với tên Server của máy tính bạn:

C#
private DataProvider()
{
    // Chuỗi kết nối SQL Server cục bộ (Sửa lại Data Source theo tên Server của máy bạn nếu cần)
    _localConnStr = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=QL_LinhKien_PC_NET;Integrated Security=True;TrustServerCertificate=True;Encrypt=False";
}
4. Thực thi chạy ứng dụng
Mở Package Manager Console trong Visual Studio và cập nhật cấu trúc cơ sở dữ liệu:

PowerShell
Update-Database
Sau đó khởi chạy chương trình bằng dòng lệnh:

Bash
dotnet run
Hoặc nhấn tổ hợp phím F5 trực tiếp trong môi trường Microsoft Visual Studio.
## 👨‍💻 Thông tin nhóm



**Nhóm 8 — Môn Công nghệ .NET**



| Vai trò | Họ và tên | MSSV |

|:---:|---|:---:|

| 👑 Trưởng nhóm | Trịnh Hữu Kiến Quốc | 2001240399 |

| 👨‍💻 Thành viên | Nguyễn Nhật Minh Quân | 2001240388 |

| 👨‍💻 Thành viên | Lương Văn Quan | 2001240384 |

| 👨‍💻 Thành viên | Ngụy Hạo Nhiên | 2001240341 |



---
👨‍💻 Thông tin nhóm thực hiện
Ứng dụng được thiết kế, phát triển và hoàn thiện bởi tập thể thành viên Nhóm 8 — Lớp học phần Công nghệ .NET (Năm học 2025 - 2026):

👑 Trịnh Hữu Kiến Quốc (MSSV: 2001240399) — Nhóm trưởng phụ trách Kiến trúc, Bảo mật & Tích hợp API.

👨‍💻 Nguyễn Nhật Minh Quân (MSSV: 2001240388) — Thành viên phụ trách Thiết kế & Lập trình Cơ sở dữ liệu nâng cao.

👨‍💻 Lương Văn Quan (MSSV: 2001240384) — Thành viên phụ trách Xây dựng Giao diện UI/UX & Style Resource.

👨‍💻 Ngụy Hạo Nhiên (MSSV: 2001240341) — Thành viên phụ trách Phát triển Logic Nghiệp vụ & Ràng buộc ViewModel.

Made with ❤️ by Nhóm 8 — Khoa CNTT Toàn bộ nội dung tài liệu báo cáo phản ánh đúng kết quả thực hiện đồ án chính thức (Năm học 2025-2026).
