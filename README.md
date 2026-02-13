# Product & Supplier Manager

A web application built with **Blazor WebAssembly** and **ASP.NET Core** to manage products and suppliers efficiently. 
Users can register, log in, view, add, edit, and delete products and suppliers. 
Authentication is handled using **JWT**, and authorization is role-based.

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

### Challenges & How I Overcame Them

**Challenge:**
- My navbar did not update automatically after login or logout. I initially tried using manual state tracking with events.
**Solution:**
- I replaced manual boolean tracking with <AuthorizeView>, which listens to authentication state changes automatically. After
implementing NotifyAuthenticationStateChanged() correctly in my provider, the UI updated instantly without manual refreshes.
**What I Learned:**
- AuthorizeView reacts to authentication state
- Proper separation of concerns simplifies UI logic
- Blazor’s built-in authorization system should be leveraged instead of reinvented

**Challenge:**
  - Initially, validation logic can easily become cluttered inside controllers or components, making code harder to maintain and test.
**Solution:**
  - I implemented FluentValidation to centralize validation rules in dedicated validator classes.
**What I learned:**
  - Keeps validation logic separate from business logic
  - Reusable across endpoints

**Challenge:**
  - At first, it was tempting to return database entities directly from the API to the frontend.
  - However, this tightly couples the database model to the client and exposes internal structure unnecessarily.
**Solution:**
  - I introduced DTOs (Data Transfer Objects) to separate:
  - Database models (Entities)
  - API contract (DTOs)
  - UI models (Client-side models)
**What I learned:**
- DTOs enforce clean separation of concerns and protect the API boundary. They make the system more scalable and secure.

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
