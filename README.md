# 🧺 Folded n' Wear

### *Book now — and let us wash your worries away!* ✨

**Folded n' Wear** is a robust Laundry Management System built with **ASP.NET Core 8.0**. It streamlines the laundry process by connecting customers with efficient booking tools while providing administrators with powerful oversight on orders, payments, and monthly reports.

---

## 🚀 Features

### 👤 For Customers

* **Easy Booking System**: Schedule your laundry drop-offs and pickups online.
* **Service Options**:
* 👕 **Regular Laundry**: Everyday clothes (T-shirts, pants, dresses).
* 🛌 **Heavy Items**: Blankets, towels, linens, and comforters.
* ⚡ **Rush Service**: Same-day processing for those laundry emergencies!


* **Smart Scheduling**: The system automatically checks daily capacity (max 10kg/day) to prevent overbooking.
* **Order Tracking**: View the status of your current and past bookings in real-time.
* **Payment History**: Keep track of your balances and payment status.

### 🛡️ For Administrators

* **Dashboard Control**: Manage Users, Orders, and Payments from a central hub.
* **Order Management**: Update status (Received, Processing, Completed) and handle cancellations.
* **Financial Reports**: Automatically generate **Monthly Reports** to track:
* 💰 Total Earnings
* ⚖️ Total Kilos Processed


* **User Management**: View customer details and manage accounts.

---

## 💸 Services & Pricing

| Service | Description | Rate |
| --- | --- | --- |
| **Regular Laundry** | Everyday wear, light items. | **₱28.00 / kilo** |
| **Heavy Items** | Blankets, towels, linens. | **₱40.00 / kilo** |
| **Comforters** | Heavy comforters, stuffed toys. | **₱65.00 / kilo** |
| **Rush Service** | Expedited processing. | **Double the total** |

---

## 🛠️ Tech Stack

* **Framework**: [ASP.NET Core 8.0 MVC](https://dotnet.microsoft.com/)
* **Database**: MySQL (via `Pomelo.EntityFrameworkCore.MySql`)
* **ORM**: Entity Framework Core
* **Frontend**: HTML5, CSS3, Bootstrap 5.3.2
* **Authentication**: ASP.NET Core Identity / Session-based Auth

---

## ⚙️ How to Run

1. **Clone the repository**:
```bash
git clone https://github.com/yourusername/folded-n-wear.git

```


2. **Configure the Database**:
* Ensure MySQL is running.
* Update your connection string in `appsettings.json`.


3. **Apply Migrations**:
Open your terminal in the project folder and run:
```bash
dotnet ef database update

```


4. **Run the Application**:
```bash
dotnet run

```


5. **Open in Browser**:
Navigate to `https://localhost:7154` (or the port specified in your launch settings).

---

## 📸 Snapshots

* **Home Page**: Welcoming interface with session-based personalization.
* **Services**: Clear pricing tables for transparency.
* **Admin Panel**: Comprehensive tools for business management.

---

