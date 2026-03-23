# 🚗 Car Marketplace API (.NET 8)

A scalable Web API built using **.NET 8** and **Clean Architecture**, designed for a car-selling platform similar to Cars24 / Spinny.

---

## 📌 Overview

This system supports:

- Sellers listing cars
- Users browsing and bidding on cars
- Admins monitoring and analyzing platform activity

Built with **performance-first approach using Dapper** and clean separation of concerns.

---

## 🏗️ Architecture

Clean Architecture ensures maintainability, scalability, and testability.

### 📂 Structure


src/
├── Api/ # Controllers, Middleware, Entry point
├── Application/ # Business logic, Interfaces, DTOs
├── Domain/ # Entities, Enums, Core rules
├── Infrastructure/ # Dapper, Repositories, External services


---

## ⚙️ Tech Stack

- .NET 8 Web API
- Dapper
- SQL Server
- JWT Authentication
- REST APIs

---

## 👥 Roles

- **Admin**
- **User (Buyer)**
- **Seller**

---

## 👨‍💼 Admin Features

- View all cars
- Monitor platform activity
- View insights (date-wise analytics)
- Manage users and sellers
- View transactions

---

## 👤 User (Buyer) Features

- Register / Login
- Browse cars
- View details
- Place bids
- Track bid status
- Make payment

### Flow


Available → Bid →
Accepted → Booked → Paid → Sold
Rejected → Available


---

## 🧑‍🔧 Seller Features

- Register / Login
- List cars
- View bids
- Accept / Reject bids
- Update status

### Flow


Available → Bid Received →
Accept → Booked → Paid → Sold
Reject → Available
Cancel → Available


---

## 🚘 Car Status

| Status     | Description |
|------------|------------|
| Available  | Open for bidding |
| Booked     | Bid accepted |
| Sold       | Payment completed |
| Cancelled  | Booking cancelled |

---

## 🗂️ Naming Conventions

- **PascalCase** → Classes, Methods  
- **camelCase** → Variables  
- **Async suffix** → Async methods  

### Examples

| Type       | Example |
|------------|--------|
| Controller | CarController |
| Service    | CarService |
| Interface  | ICarService |
| Repository | CarRepository |
| DTO        | CarDto |

---

## 🗄️ Data Access (Dapper)

- High performance
- Full SQL control
- Repository pattern used

```csharp
public async Task<IEnumerable<Car>> GetAvailableCarsAsync()
{
    var query = "SELECT * FROM Cars WHERE Status = @Status";
    return await _dbConnection.QueryAsync<Car>(query, new { Status = "Available" });
}



🔐 Authentication
JWT-based authentication
Role-based authorization:
Admin
Seller
User
🧩 Modules
Authentication
Car Management
Bidding System
Payment Flow
Admin Insights