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
    - Add unit and integration tests for API endpoints.
    - Add sorting and advanced filtering for products and suppliers.
    - Enhance UI with more modern styling and mobile responsiveness.

---

### Images 
<img width="1893" height="860" alt="Dashboard" src="https://github.com/user-attachments/assets/0f429631-78d8-4c66-ae0b-077774f4ac27" />
<img width="1888" height="858" alt="ProductsList" src="https://github.com/user-attachments/assets/5f2d6004-cc10-47e8-81a5-fcd13a23c587" />
<img width="1912" height="866" alt="ProductDetails" src="https://github.com/user-attachments/assets/6e335107-ad2d-4cbf-b587-4fad8c3d5e26" />
<img width="1891" height="866" alt="UpdateProduct" src="https://github.com/user-attachments/assets/7e80ad09-0183-4714-8fa3-7c05c44344e4" />
<img width="1892" height="857" alt="SuppliersList" src="https://github.com/user-attachments/assets/50fa3486-c697-4c10-b48e-a368dfa0b7b1" />
<img width="1896" height="865" alt="SupplierDetails" src="https://github.com/user-attachments/assets/1bdbfc5f-895d-474b-8c6f-132049564f7f" />
<img width="1910" height="857" alt="ProductAddedConfirmation" src="https://github.com/user-attachments/assets/fa394729-f963-4b02-9a4d-37609896c944" />
<img width="1907" height="863" alt="DeleteProductConfirmation" src="https://github.com/user-attachments/assets/c673335a-cee3-4e3c-9162-976f31b07f17" />
<img width="1907" height="858" alt="Login" src="https://github.com/user-attachments/assets/f57d6577-e8da-4af2-adc4-7f8b0ed012a6" />


---

### License

This project is licensed under the MIT License.

---

### Contact

Developed by Nompumelelo.
Email: nsmbhense6@gmail.com

---
