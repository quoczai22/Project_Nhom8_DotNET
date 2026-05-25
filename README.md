<div align="center">

<img src="https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 8.0"/>
<img src="https://img.shields.io/badge/WPF-MVVM-3f51b5?style=for-the-badge&logo=windows&logoColor=white" alt="WPF MVVM"/>
<img src="https://img.shields.io/badge/SQL%20Server-EF%20Core-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" alt="SQL Server"/>
<img src="https://img.shields.io/badge/CI%2FCD-GitHub%20Actions-2088FF?style=for-the-badge&logo=githubactions&logoColor=white" alt="CI/CD"/>

<br/><br/>

# 🖥️ Quản Lý Linh Kiện Máy Tính 

> Ứng dụng desktop quản lý linh kiện máy tính toàn diện, xây dựng bằng **WPF .NET 8.0** theo kiến trúc **MVVM**, kết nối **SQL Server** qua **Entity Framework Core**.

**Đồ án cuối kỳ — Môn Công nghệ .NET Chiều T3 T7-T12**

<br/>

[![Download .exe](https://img.shields.io/github/v/release/quoczai22/Project_Nhom8_DotNET?style=flat-square&label=⬇️%20Tải%20file%20.exe&color=3f51b5&logoColor=white)](https://github.com/quoczai22/Project_Nhom8_DotNET/releases/latest)
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
- [Hướng dẫn cài đặt](#-hướng-dẫn-cài-đặt)
- [Thông tin nhóm](#-thông-tin-nhóm)

---

## 🚀 Giới thiệu

**Quản Lý Linh Kiện Máy Tính** là ứng dụng desktop được xây dựng nhằm hỗ trợ các cửa hàng linh kiện máy tính trong việc quản lý hàng hóa, xuất hóa đơn và theo dõi khách hàng — nhân viên một cách hiệu quả.

Ứng dụng cung cấp giao diện trực quan, hỗ trợ **chế độ sáng/tối**, tích hợp thanh toán **MoMo qua QR**, và có thể chạy ngay từ file `.exe` mà không cần cài đặt phức tạp.

---

## ✨ Tính năng nổi bật

### 🔐 Xác thực & Phân quyền
- Đăng nhập / Đăng xuất an toàn
- Phân quyền theo vai trò (Admin / Nhân viên)

### 📦 Quản lý Linh kiện
- Xem danh sách, thêm, sửa, xóa linh kiện
- Phân loại theo **Loại linh kiện**
- Tìm kiếm & lọc nhanh
- Cảnh báo **tồn kho thấp** tự động

### 🧾 Hóa đơn & Thanh toán
- Tạo hóa đơn bán hàng
- Xuất hóa đơn ra file & in trực tiếp
- Thanh toán **MoMo** qua quét mã QR *(demo)*

### 📥 Phiếu Nhập Kho
- Tạo phiếu nhập linh kiện từ nhà cung cấp
- Chọn nhân viên thực hiện & ngày nhập
- Thêm nhiều linh kiện với số lượng và đơn giá nhập tùy chỉnh
- Tự động **cộng số lượng tồn kho** khi lưu phiếu
- Tự động **trừ số lượng tồn kho** khi xóa phiếu
- Lọc phiếu nhập theo khoảng thời gian & từ khóa
- Thống kê: tổng phiếu, tổng chi phí, số lượng linh kiện nhập, phiếu trong tháng
- Xuất phiếu nhập ra file `.txt`

### 👥 Quản lý nhân sự & Khách hàng
- CRUD Nhân viên, Khách hàng
- Tra cứu lịch sử giao dịch

### 🎨 Giao diện
- Chuyển đổi **Light Mode / Dark Mode**
- Màu chủ đạo `#3f51b5` (Material Indigo)
- Thiết kế theo chuẩn Material Design

---

## 🛠️ Công nghệ sử dụng

| Thành phần | Công nghệ |
|---|---|
| Ngôn ngữ | C# 12 |
| Framework UI | WPF (.NET 8.0) |
| Kiến trúc | MVVM (Model-View-ViewModel) |
| ORM | Entity Framework Core 8 |
| Database | Microsoft SQL Server |
| Thiết kế DB | T-SQL |
| Thanh toán | MoMo Payment API (QR demo) |
| CI/CD | GitHub Actions (auto build & release `.exe`) |
| Report / Print | Xuất PDF / In hóa đơn |

---

## 🏗️ Kiến trúc hệ thống

```
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
│ Data Binding / Command
┌───────────────────────────────────────────────▼──────────────────────────────────────────────────┐
│                                           VIEWMODEL                                              │
├──────────────────────────────────────────────────────────────────────────────────────────────────┤
│ - BaseViewModel                    - LoginViewModel              - LoaiLinhKienViewModel         │
│ - RelayCommand                     - MainViewModel               - HoaDonViewModel               │
│ - ISearchable                      - TrangChuViewModel           - KhachHangViewModel            │
│                                    - LinhKienViewModel           - NhanVienViewModel             │
│                                    - PhieuNhapViewModel          - ThemPhieuNhapDialogViewModel  │
└───────────────────────────────────────────────┬──────────────────────────────────────────────────┘
│
┌───────────────────────────────────────────────▼──────────────────────────────────────────────────┐
│                                             MODEL                                                │
├──────────────────────────────────────────────────────────────────────────────────────────────────┤
│ - QL_LinhKien_PC_Context           - LinhKien                    - NhanVien                      │
│ - QL_LinhKien_PC_Context_Functions - LoaiLk                      - TaiKhoan                      │
│ - QL_LinhKien_PC_ContextProcedures - NhaSanXuat                  - ThongKeBanHang                │
│ - IQL_LinhKien_PC_ContextProcedures- HoaDon                      - ThongKeHang                   │
│ - DbContextExtensions              - ChiTietHd                   - sp_BaoCaoTonKhoResult         │
│ - DataProvider                     - PhieuNhap                   - MomoExecuteResponseModel      │
│                                    - ChiTietPn                   - MomoResponse                  │
│                                    - KhachHang                                                   │
└───────────────────────────────────────────────┬──────────────────────────────────────────────────┘
│ Entity Framework Core
┌───────────────────────────────────────────────▼──────────────────────────────────────────────────┐
│                                       SQL SERVER DATABASE                                        │
└──────────────────────────────────────────────────────────────────────────────────────────────────┘

```

---

## 📁 Cấu trúc thư mục

```
📦 Repository Root
│
├── 📂 CSDL/                                        # Script T-SQL thiết kế database
│   └── *QuanLyLinhKienMayTinh_NET.sql              # Tạo bảng, stored procedures, functions
│
└── 📂 Project_Nhom8_DotNET/                        # Source code WPF
    │
    ├── 📂 Helpers/
    │   └── Multipasswordconverter.cs               # Converter hỗ trợ PasswordBox binding
    │
    ├── 📂 Images/
    │   ├── avatar.png
    │   └── logo_momo.png
    │
    ├── 📂 Models/                                  # EF Core entities + DbContext
    │   ├── QL_LinhKien_PC_Context.cs               # DbContext chính
    │   ├── QL_LinhKien_PC_Context.Functions.cs     # DB Functions mapping
    │   ├── QL_LinhKien_PC_ContextProcedures.cs     # Stored Procedures mapping
    │   ├── IQL_LinhKien_PC_ContextProcedures.cs    # Interface cho procedures
    │   ├── DbContextExtensions.cs
    │   ├── DataProvider.cs
    │   │
    │   ├── LinhKien.cs                             # Entity: Linh kiện
    │   ├── LoaiLk.cs                               # Entity: Loại linh kiện
    │   ├── NhaSanXuat.cs                           # Entity: Nhà sản xuất
    │   ├── HoaDon.cs                               # Entity: Hóa đơn
    │   ├── ChiTietHd.cs                            # Entity: Chi tiết hóa đơn
    │   ├── PhieuNhap.cs                            # Entity: Phiếu nhập
    │   ├── ChiTietPn.cs                            # Entity: Chi tiết phiếu nhập
    │   ├── KhachHang.cs                            # Entity: Khách hàng
    │   ├── NhanVien.cs                             # Entity: Nhân viên
    │   ├── TaiKhoan.cs                             # Entity: Tài khoản đăng nhập
    │   ├── ThongKeBanHang.cs                       # Model: Thống kê bán hàng
    │   ├── ThongKeHang.cs                          # Model: Thống kê hàng
    │   ├── sp_BaoCaoTonKhoResult.cs                # Model: Kết quả báo cáo tồn kho
    │   ├── MomoExecuteResponseModel.cs             # Model: Response MoMo
    │   └── MomoResponse.cs                         # Model: Dữ liệu MoMo
    │
    ├── 📂 Services/
    │   ├── IMomoService.cs                         # Interface thanh toán MoMo
    │   ├── MomoService.cs                          # Xử lý API MoMo (QR, callback)
    │   └── AutoIDServices.cs                       # Tự động sinh ID
    │
    ├── 📂 Themes/                                  # Giao diện sáng / tối
    │   ├── ThemeLight.xaml
    │   └── ThemeDark.xaml
    │
    ├── 📂 ViewModels/                              # MVVM – logic & data binding
    │   ├── BaseViewModel.cs                        # Base với INotifyPropertyChanged
    │   ├── RelayCommand.cs                         # ICommand implementation
    │   ├── ISearchable.cs                          # Interface tìm kiếm dùng chung
    │   ├── LoginViewModel.cs
    │   ├── MainViewModel.cs
    │   ├── TrangChuViewModel.cs
    │   ├── LinhKienViewModel.cs
    │   ├── LoaiLinhKienViewModel.cs
    │   ├── HoaDonViewModel.cs
    │   ├── PhieuNhapViewModel.cs                   # ViewModel quản lý phiếu nhập
    │   ├── ThemPhieuNhapDialogViewModel.cs         # ViewModel dialog tạo phiếu nhập
    │   ├── KhachHangViewModel.cs
    │   └── NhanVienViewModel.cs
    │
    ├── 📂 Views/                                   # XAML – giao diện người dùng
    │   ├── LoginView.xaml                          # Màn hình đăng nhập
    │   ├── MainWindow.xaml                         # Cửa sổ chính (navigation)
    │   ├── TrangChuView.xaml                       # Trang chủ / Dashboard
    │   ├── LinhKienView.xaml                       # Quản lý linh kiện
    │   ├── LoaiLinhKienView.xaml                   # Quản lý loại linh kiện
    │   ├── HoaDonView.xaml                         # Quản lý hóa đơn
    │   ├── PhieuNhapView.xaml                      # Quản lý phiếu nhập kho
    │   ├── KhachHangView.xaml                      # Quản lý khách hàng
    │   ├── NhanVienView.xaml                       # Quản lý nhân viên
    │   ├── MomoPaymentView.xaml                    # Màn hình thanh toán MoMo QR
    │   ├── ThongBaoTonKhoWindow.xaml               # Cửa sổ cảnh báo tồn kho
    │   │
    │   ├── ChonPhuongThucDialog.xaml               # Chọn phương thức thanh toán
    │   ├── ThemHoaDonDialog.xaml                   # Hộp thoại thêm hóa đơn
    │   ├── SuaHoaDonDialog.xaml                    # Hộp thoại sửa hóa đơn
    │   ├── ThemPhieuNhapDialog.xaml                # Hộp thoại tạo phiếu nhập kho
    │   ├── ThemSuaLinhKienDialog.xaml
    │   ├── ThemSuaLoaiLinhKienDialog.xaml
    │   ├── ThemSuaKhachHangDialog.xaml
    │   └── ThemSuaNhanVienDialog.xaml
    │
    ├── 📄 App.xaml
    ├── 📄 App.xaml.cs
    └── 📄 Project_Nhom8_DotNET.csproj
```

---

## ⚙️ Hướng dẫn cài đặt

### ▶️ Cách 1 — Chạy file `.exe` (khuyến nghị)

1. Vào mục **[Releases](https://github.com/quoczai22/Project_Nhom8_DotNET/releases/latest)** của repo
2. Tải file `QuanLyLinhKien-Setup.exe` về máy
3. Chạy file và làm theo hướng dẫn cài đặt
4. Mở ứng dụng và đăng nhập

> **Yêu cầu:** Máy cần cài sẵn [.NET 8.0 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) và có SQL Server.

---

### 🛠️ Cách 2 — Build từ source code

#### Yêu cầu hệ thống

| Công cụ | Phiên bản tối thiểu |
|---|---|
| Visual Studio | 2022 (v17.8+) |
| .NET SDK | 8.0 |
| SQL Server | 2022 |
| SQL Server Management Studio | Tùy chọn |

#### Các bước thực hiện

**1. Clone repository**

```bash
git clone https://github.com/quoczai22/Project_Nhom8_DotNET.git
cd Project_Nhom8_DotNET
```

**2. Cấu hình Connection String**

Cài đặt phần dataprovider qua trang web Kteam: https://howkteam.vn/course/lap-trinh-phan-mem-quan-ly-kho-wpf-mvvm/class-dataprovider-trong-phan-mem-quan-ly-kho-wpf-mvvm-2651

```
DataProvider (
    private DataProvider()
    {
        _supabaseConnStr = "Server=db.pmkwulshpbpugvphzvwk.supabase.co;Port=5432;Database=postgres;User Id=postgres;Password=Kienquoc@1704;";
        _localConnStr = "Data Source=(localdbb)\\MSSQLLocalDB;Initial Catalog=QL_LinhKien_PC_NET;Integrated Security=True;TrustServerCertificate=True;Encrypt=False";
    }
)
```

**3. Tạo Database và Migration**

Mở **Package Manager Console** trong Visual Studio:

```powershell
Update-Database
```

Hoặc dùng .NET CLI:

```bash
dotnet ef database update
```

**4. Chạy ứng dụng**

```bash
dotnet run
```

Hoặc nhấn **F5** trong Visual Studio.

---

## 👨‍💻 Thông tin nhóm

**Nhóm 8 — Môn Công nghệ .NET**

| Vai trò | Họ và tên | MSSV |
|:---:|---|:---:|
| 👑 Trưởng nhóm | Trịnh Hữu Kiến Quốc | 2001240399 |
| 👨‍💻 Thành viên | Nguyễn Nhật Minh Quân | 2001240388 |
| 👨‍💻 Thành viên | Lương Văn Quan | 2001240384 |
| 👨‍💻 Thành viên | Ngụy Hạo Nhiên | 2001240341 |

---

<div align="center">

Made with ❤️ by **Nhóm 8** &nbsp;·&nbsp; Môn Công nghệ .NET

</div>
