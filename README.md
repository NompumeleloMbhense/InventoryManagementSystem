# Product & Supplier Manager

A web application built with **Blazor WebAssembly** and **ASP.NET Core** to manage products and suppliers efficiently. Users can register, log in, view, add, edit, and delete products and suppliers. Authentication is handled using **JWT**, and authorization is role-based.

---

## Features

- **User Authentication**
  - Register new users
  - Login with JWT token
  - Role-based access (Admin and User)
  - Logout functionality

- **Product Management**
  - View a paginated list of products
  - Search and filter products by name or category
  - Add, edit, and delete products (Admin privileges)
  
- **Supplier Management**
  - View a paginated list of suppliers
  - Add, edit, and delete suppliers (Admin privileges)

- **Frontend**
  - Blazor WebAssembly pages for Products and Suppliers
  - Responsive design with Bootstrap

- **Backend**
  - ASP.NET Core Web API
  - Entity Framework Core for database operations
  - SQL Server database
  - Identity for user management
  - JWT authentication for securing API endpoints
  - Seeded initial data (Admin, User, products, suppliers)

---

## Tech Stack

- **Frontend:** Blazor WebAssembly
- **Backend:** ASP.NET Core 8 Web API
- **Database:** SQL Server
- **ORM:** Entity Framework Core
- **Authentication:** ASP.NET Identity with JWT
- **Validation:** FluentValidation
- **Logging:** Serilog
- **Version Control:** Git

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- SQL Server
- Visual Studio 2022 / VS Code
- Node.js & npm (optional, if you want to manage client-side packages)

---

### Setup Instructions

1. **Clone the repository**

git clone <your-repo-url>
cd ProductSupplierManager

2. **Configure the database connection**

Update the connection string in appsettings.json:
"ConnectionStrings": {
  "InventoryDBConnection": "Server=YOUR_SERVER_NAME;Database=InventoryDB;Trusted_Connection=True;MultipleActiveResultSets=true"
}

3. **Apply Migrations and Seed Data**

The application automatically applies migrations and seeds initial data (Admin, User, products, suppliers) on startup.

4. **Run the Application**

    - Open the solution in Visual Studio and run the project.
    - The Blazor client runs on http://localhost:5065 by default.

5. **Access the App**

    - Home page: /
    - Products page: /products (requires login)
    - Suppliers page: /suppliers (requires login)
    - Register a new user: /register
    - Login: /login

---

### User Accounts

The app seeds initial users:

- Admin
    - Email: admin@example.com
    - Password: Admin123!
- User
    - Email: user@example.com
    - Password: User123!

---

### Usage Flow

1. Visit the Home page (/) to see the welcome screen.
2. Register as a new user or log in with an existing account.
3. Once logged in:
    - Access Products and Suppliers pages.
    - Admin users can add, edit, or delete products and suppliers.
4. Logout using the Logout button, which will clear your token and require re-login to access protected pages.

---

### Future Improvements

    - Add refresh tokens to avoid frequent logins.
    - Implement role-based UI, showing or hiding buttons based on the user role.
    - Add unit and integration tests for API endpoints.
    - Add sorting and advanced filtering for products and suppliers.
    - Enhance UI with more modern styling and mobile responsiveness.

---

### License

This project is licensed under the MIT License.

---

### Contact

Developed by Nompumelelo.
Email: nsmbhense6@gmail.com

---