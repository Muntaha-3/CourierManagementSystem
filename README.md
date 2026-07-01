<div align="center">

# 🚚 Courier Management System

### A web-based courier &amp; parcel tracking system built with ASP.NET Core MVC

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)
![C%23](https://img.shields.io/badge/C%23-95.5%25-239120?style=for-the-badge&logo=csharp)
![EntityFramework](https://img.shields.io/badge/Entity%20Framework-Core-3B3B3B?style=for-the-badge)
![License](https://img.shields.io/badge/License-Academic-green?style=for-the-badge)

</div>

---

## 📖 About The Project

**Courier Management System** is a full-stack web application developed using **ASP.NET Core MVC**, designed to streamline the process of booking, tracking, and managing courier parcels. The system allows admins to manage customers, parcels, and deliveries through a clean and organized dashboard, built as part of coursework at the **Institute of Data Science, UET Lahore**.

## ✨ Features

- 📦 Parcel booking and management
- 🔍 Real-time parcel tracking by tracking ID
- 👤 Customer registration and management
- 🔐 Secure admin login and authentication
- 📊 Dashboard with delivery statistics
- 🗂️ Organized MVC architecture (Controllers, Models, Views)
- 💾 Database integration using Entity Framework Core

---

## 🛠️ Tech Stack

| Layer | Technology |
|-------|------------|
| **Backend** | ASP.NET Core MVC (C#) |
| **Frontend** | HTML5, CSS3, Bootstrap, jQuery |
| **Database** | SQL Server + Entity Framework Core |
| **IDE** | Visual Studio 2022 |
| **Version Control** | Git & GitHub |

---

## 📁 Project Structure

```
CourierManagementSystem/
├── Controllers/        # MVC Controllers (business logic)
├── Data/                # DbContext & database configuration
├── Migrations/          # EF Core migration files
├── Models/               # Entity/data models
├── Views/                 # Razor views (UI pages)
├── wwwroot/                # Static files (CSS, JS, images, libraries)
├── Properties/               # Project launch settings
├── Program.cs                  # Application entry point
├── appsettings.json               # Configuration settings
└── CourierManagementSystem.csproj    # Project file
```

---

## 🚀 Getting Started

### Prerequisites
- Visual Studio 2022 (or later)
- .NET 8.0 SDK
- SQL Server

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/Muntaha-3/CourierManagementSystem.git
   ```

2. **Open the project**
   Open `CourierManagementSystem.slnx` in Visual Studio

3. **Configure the database**
   Update your connection string in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER;Database=CourierDB;Trusted_Connection=True;"
   }
   ```

4. **Apply migrations**
   ```bash
   Update-Database
   ```

5. **Run the project**
   Press `F5` or click **Run** in Visual Studio

---

## 👥 Contributors

| Name | GitHub |
|------|--------|
| Muntaha | [@Muntaha-3](https://github.com/Muntaha-3) |

---

## 📄 License

This project was developed for academic purposes at the **Institute of Data Science (IDS), University of Engineering and Technology (UET), Lahore**.

---

<div align="center">

⭐ If you found this project helpful, consider giving it a star!

</div>
