<!-- Capsule Render Banner -->
![Header](https://capsule-render.vercel.app/api?type=waving&color=3f51b5&height=220&section=header&text=PC%20Parts%20Manager&fontSize=50&fontColor=ffffff&fontAlignY=38&desc=Hệ%20thống%20quản%20lý%20linh%20kiện%20máy%20tính%20thông%20minh&descAlignY=60&descFontColor=c5cae9)

<div align="center">

![Version](https://img.shields.io/badge/version-2.1.0-3f51b5?style=for-the-badge&logo=semantic-release)
![Build](https://img.shields.io/badge/build-passing-4caf50?style=for-the-badge&logo=github-actions)
![TypeScript](https://img.shields.io/badge/TypeScript-5.0-3178c6?style=for-the-badge&logo=typescript)
![React](https://img.shields.io/badge/React-18-61dafb?style=for-the-badge&logo=react)
![License](https://img.shields.io/badge/license-MIT-ff9800?style=for-the-badge)

</div>

---

## 📋 Mục lục

- [✨ Tính năng](#-tính-năng)
- [📸 Giao diện](#-giao-diện)
- [🛠️ Công nghệ](#️-công-nghệ)
- [🚀 Cài đặt](#-cài-đặt)
- [⚙️ Cấu hình](#️-cấu-hình)
- [📖 Hướng dẫn sử dụng](#-hướng-dẫn-sử-dụng)
- [🤝 Đóng góp](#-đóng-góp)
- [📄 License](#-license)

---

## ✨ Tính năng

| Tính năng | Mô tả |
|---|---|
| 📦 **Quản lý kho** | Theo dõi CPU, GPU, RAM, SSD, Mainboard theo thời gian thực |
| 🔔 **Cảnh báo tồn kho** | Tự động thông báo khi linh kiện sắp hết hàng |
| 📊 **Dashboard thống kê** | Biểu đồ xuất nhập, doanh thu, top sản phẩm |
| 🔍 **Tìm kiếm nâng cao** | Lọc theo danh mục, thương hiệu, giá, tình trạng |
| 📤 **Xuất báo cáo** | Export Excel, PDF — lịch sử giao dịch chi tiết |
| 👥 **Phân quyền** | Admin, Nhân viên kho, Kế toán |

---

## 📸 Giao diện

> _Screenshot hoặc GIF demo tại đây_

```
┌─────────────────────────────────────────────┐
│  🖥️  PC Parts Manager          [+ Thêm mới] │
├──────────────┬──────────┬────────┬──────────┤
│ Tên linh kiện│ Danh mục │Tồn kho │Trạng thái│
├──────────────┼──────────┼────────┼──────────┤
│ i9-13900K    │ CPU      │  14    │ ✅ Còn   │
│ RTX 4080     │ GPU      │   3    │ ⚠️ Thấp  │
│ Samsung 990  │ SSD      │   0    │ ❌ Hết   │
└──────────────┴──────────┴────────┴──────────┘
```

---

## 🛠️ Công nghệ

<div align="center">

![React](https://img.shields.io/badge/React_18-20232A?style=flat-square&logo=react&logoColor=61DAFB)
![TypeScript](https://img.shields.io/badge/TypeScript-007ACC?style=flat-square&logo=typescript&logoColor=white)
![Material UI](https://img.shields.io/badge/Material_UI-3f51b5?style=flat-square&logo=mui&logoColor=white)
![Node.js](https://img.shields.io/badge/Node.js-43853D?style=flat-square&logo=node.js&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=flat-square&logo=postgresql&logoColor=white)
![Chart.js](https://img.shields.io/badge/Chart.js-FF6384?style=flat-square&logo=chart.js&logoColor=white)

</div>

---

## 🚀 Cài đặt

```bash
# 1. Clone repository
git clone https://github.com/yourname/pc-parts-manager.git
cd pc-parts-manager

# 2. Cài đặt dependencies
npm install

# 3. Cấu hình môi trường
cp .env.example .env

# 4. Chạy database migration
npm run migrate

# 5. Khởi động ứng dụng
npm run dev
# → http://localhost:3000
```

---

## ⚙️ Cấu hình

Tạo file `.env` từ `.env.example` và điền thông tin:

```env
# Database
DATABASE_URL=postgresql://user:password@localhost:5432/pcparts

# App
PORT=3000
JWT_SECRET=your_secret_key

# Email (cảnh báo tồn kho)
SMTP_HOST=smtp.gmail.com
SMTP_USER=your@gmail.com
```

---

## 📖 Hướng dẫn sử dụng

<details>
<summary><b>📦 Thêm linh kiện mới</b></summary>

1. Vào mục **Quản lý kho** → nhấn **+ Thêm mới**
2. Điền tên, danh mục, nhà sản xuất, giá nhập, số lượng
3. Đặt ngưỡng cảnh báo tồn kho tối thiểu
4. Nhấn **Lưu**

</details>

<details>
<summary><b>📊 Xem báo cáo & xuất file</b></summary>

1. Vào mục **Báo cáo** → chọn khoảng thời gian
2. Chọn loại báo cáo: Tồn kho / Doanh thu / Xuất nhập
3. Nhấn **Xuất Excel** hoặc **Xuất PDF**

</details>

---

## 🤝 Đóng góp

Mọi đóng góp đều được chào đón! Vui lòng:

1. Fork repo này
2. Tạo branch mới (`git checkout -b feature/ten-tinh-nang`)
3. Commit thay đổi (`git commit -m 'feat: thêm tính năng X'`)
4. Push lên branch (`git push origin feature/ten-tinh-nang`)
5. Mở Pull Request

---

## 📄 License

Phân phối theo giấy phép **MIT**. Xem file [LICENSE](LICENSE) để biết thêm.

---

<!-- Footer wave -->
![Footer](https://capsule-render.vercel.app/api?type=waving&color=3f51b5&height=120&section=footer)

<div align="center">
  <sub>Made with ❤️ by <a href="https://github.com/yourname">yourname</a></sub>
</div>
