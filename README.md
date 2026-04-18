# Product & Supplier Manager

A full-stack web application built with a Blazor WebAssembly frontend and an ASP.NET Core Web API backend. 
This system provides comprehensive functionality for managing products and their suppliers,
featuring JWT-based user authentication and role-based authorization.

---

## Key Features

-   **User Authentication**: Secure registration and login system using JWT (JSON Web Tokens).
-   **Role-Based Access Control**: Differentiates between `Admin` and `User` roles. Admins have full CRUD access, while regular users have read-only permissions.
-   **Product Management**: Admins can create, read, update, and delete products. The product list is paginated and includes search and filtering capabilities.
-   **Supplier Management**: Admins can manage supplier information. The system supports full CRUD operations, pagination, and search for suppliers.
-   **Dashboard**: A welcoming dashboard for authenticated users that provides a quick overview of total products and suppliers.
-   **Clean Architecture**: The solution is separated into three distinct projects (`ServerApp`, `ClientApp`, `SharedApp`) for maintainability and separation of concerns.
-   **Database Seeding**: The application automatically seeds the database with initial data (roles, users, products, suppliers) on startup for easy setup and testing.
-   **Automated Unit Testing**: High-coverage test suite ensuring the reliability of business logic and API reliability.

---

## System Architecture

The application is structured into three main projects to enforce a clean separation of concerns:

-   **`ServerApp`**: An ASP.NET Core Web API project that serves as the backend. It handles:
    -   API endpoints for authentication, products, and suppliers.
    -   Business logic and data access using the Repository pattern.
    -   Database management with Entity Framework Core.
    -   User and role management with ASP.NET Core Identity.
    -   JWT generation and validation for securing the API.

-   **`ClientApp`**: A Blazor WebAssembly project that provides the user interface. It is responsible for:
    -   Rendering all UI components and pages.
    -   Making HTTP requests to the `ServerApp` API.
    -   Managing client-side authentication state and token storage in the browser's local storage.
    -   Providing a responsive user experience.

-   **`SharedApp`**: A .NET class library containing code shared between the `ServerApp` and `ClientApp`. This includes:
    -   Domain models (`Product`, `Supplier`).
    -   Data Transfer Objects (DTOs) for API communication.
    -   Validation rules using FluentValidation to ensure consistency on both client and server.
      
- **InventorySystem.Tests**: An xUnit testing project that utilizes Moq to isolate and verify the behavior of Services and Controllers.

---

## Tech Stack

- **Frontend:** Blazor WebAssembly
- **Backend:** ASP.NET Core 8 Web API
- **Testing:** xUnit, Moq
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
- Nesting <AuthorizeView> tags caused the compiler to crash because it couldn't differentiate between multiple context variables.

**Solution:**
- Switched to a C# Logic Approach in the UI. By using @if (isAuthenticated) and @if (isAdmin), I removed the variable naming
  conflict entirely.

**What I Learned:**
-  Sometimes standard C# logic is more robust than specialized framework tags, especially in complex layouts.

**Challenge:**
  - I was maintaining separate models in the ClientApp and SharedApp.

**Solution:**
  - Deleted duplicate models and unified everything into the SharedApp. I refactored them from positional records to
    property-based records to support Blazor's two-way data binding.


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
    - Add sorting and advanced filtering for products and suppliers.
    - Audit Logs: Track which users made specific changes to inventory stock.
    - Enhance UI with more modern styling and mobile responsiveness.

---

### Images

<img width="800" height="360" alt="AdminLogin" src="https://github.com/user-attachments/assets/43a94b17-a137-421c-abe8-23213e64b53f" />




<img width="800" height="364" alt="Dashboard" src="https://github.com/user-attachments/assets/13591dad-6ba9-4a69-a3ac-34784ceaafe2" />




<img width="800" height="362" alt="SearchProduct" src="https://github.com/user-attachments/assets/144a7174-535e-4616-994d-43f32122f0f5" />




<img width="800" height="364" alt="AddProduct" src="https://github.com/user-attachments/assets/7f5e08cc-3e27-4830-ad1e-2e74abd0b694" />




<img width="800" height="364" alt="ProductDetails" src="https://github.com/user-attachments/assets/fb3a621f-3726-4e94-b142-0e09ce25508e" />




<img width="800" height="363" alt="UpdateProduct" src="https://github.com/user-attachments/assets/9086d39f-72b4-47bd-956c-2e39e576dcd7" />




<img width="800" height="365" alt="DeleteProduct" src="https://github.com/user-attachments/assets/d3145f7e-0dff-4985-a535-679150b9387e" />




<img width="800" height="361" alt="DeleteSupplierWithProducts" src="https://github.com/user-attachments/assets/ed68983c-8cff-46d0-876d-daa7b297b844" />


---

### License

This project is licensed under the MIT License.

---

### Contact

Developed by Nompumelelo.
Email: nsmbhense6@gmail.com

---
