# Product & Supplier Manager

A professional-grade full-stack web application built with a Blazor WebAssembly frontend and an ASP.NET Core 8 Web API backend. 
This system provides comprehensive functionality for managing products and their suppliers, featuring an automated audit trail 
for stock movements, JWT-based security, and a fully containerized environment.

---

## Key Features

-   **User Authentication**: Secure registration and login system using JWT (JSON Web Tokens) with local storage persistence.
-   **Role-Based Access Control**: Strict differentiation between Admin and User roles. Admins have full CRUD access, while regular users are restricted to read-only views.
-   **Product & Supplier Management**: Full lifecycle management including pagination, name-based search, and category filtering.
-   **Stock Movement History (Audit Trail)**: Automatically tracks every change in inventory levels. Logs the quantity changed, the action type (Restock, Manual Update, etc.), the user who performed it, and a timestamp.
-   **Export to CSV**: Admins can export the entire product inventory to a CSV file for external reporting in Excel.
-   **Docker Ready**: Fully containerized using Docker Compose, including a persistent SQL Server database volume.
-   **Automated Unit Testing**: Robust test suite using xUnit and Moq ensuring the reliability of core business services and API controllers.

---

## System Architecture

**The application follows a Clean Architecture pattern across four distinct projects:**

- **ServerApp (Backend)**: ASP.NET Core Web API handling business logic, the Repository pattern, EF Core database access, and Identity management.
- **ClientApp (Frontend)**: Blazor WebAssembly providing a responsive, logic-driven UI that consumes the API using a secure HttpClient pipeline.
- **SharedApp (Core)**: A central library containing unified DTOs (Data Transfer Objects), Domain Models, and Validation rules used by both the client and server.
- **InventorySystem.Tests (QA)**: An xUnit testing project that utilizes Moq to isolate and verify the behavior of Services and Controllers.


---

## Tech Stack

- **Frontend**: Blazor WebAssembly
- **Backend**: ASP.NET Core 8 Web API
- **Testing**: xUnit, Moq
- **Containerization**: Docker & Docker Compose
- **Database**: SQL Server
- **ORM**: Entity Framework Core
- **Authentication**: ASP.NET Identity with JWT
- **Validation**: FluentValidation
- **Logging**: Serilog


---

### Getting Started

**Prerequisites**

- .NET 8 SDK
- Docker Desktop (Recommended)
- SQL Server (If running locally without Docker)
   

---

### Setup Instructions (The Docker Way - Recommended)


1. **Clone the repository**

        git clone <https://github.com/NompumeleloMbhense/InventoryManagementSystem>
        cd InventoryManagementSystem

2. **Configure Secrets**
Create a .env file in the root directory and add the following:

        # Create a .env file and fill in your own secure values:
        DB_PASSWORD=your_strong_password_here
        JWT_KEY=your_very_long_secret_key_here
        ADMIN_EMAIL=admin@yourdomain.com
        ADMIN_PASSWORD=your_admin_password_here

3. **Run the Application**

        docker-compose up --build

Access the app at http://localhost:5050.


---

### Setup Instructions (The Local Way)

**1. Configure Connection String**
Update ServerApp/appsettings.json with your local SQL Server instance details.

**2.Run Tests**

    dotnet test

**3. Launch the Projects**
Run the ServerApp and ClientApp projects simultaneously using Visual Studio or dotnet run.


---

### Challenges & How I Overcame Them

**Challenge: Refactoring for Testability**
- **Problem**: I initially used FluentValidation’s ValidateAndThrowAsync extension method. While it made the code shorter, it was nearly impossible to mock in unit tests, leading to confusing NullReferenceExceptions.
- **Solution**: I refactored the Controllers to use Explicit Validation. By calling await _validator.ValidateAsync(dto) and manually throwing the exception, the code became much easier to mock and test.

**Challenge: Resolving Build Ambiguity (RZ9999)**
- **Problem**: Nesting <AuthorizeView> tags inside the Router and Layout caused the compiler to crash because it couldn't differentiate between multiple context variables.
- **Solution**: Switched to a C# Logic Approach in the main layout and nav menu. By using @if (isAuthenticated) and @if (isAdmin) in the @code block, I removed the variable naming conflict entirely.

**Challenge: DTO Unification**
- **Problem**: Maintaining separate models in the ClientApp and SharedApp caused frequent "sync" bugs.
- **Solution**: Deleted duplicate models and unified everything into the SharedApp. I refactored them from positional records to property-based records to support Blazor's two-way data binding and seamless JSON serialization.

--- 

### Future Improvements

- **Refresh Tokens**: Implement automated token renewal to prevent users from being logged out during active sessions.
- **Advanced Filtering**: Add multi-column sorting and date-range filters for the Stock Movement History
- **Product Images**: Integrate Azure Blob Storage or local volume storage for product thumbnails.
- **Integration Testing**: Add WebApplicationFactory tests to verify the full API-to-Database pipeline.


---

### Images

**Log In**

<img width="800" height="361" alt="Login" src="https://github.com/user-attachments/assets/18e5def2-aacb-4fd5-a37b-6dad9e28a76f" />


**Dashboard**

<img width="800" height="365" alt="Dashboard" src="https://github.com/user-attachments/assets/f1cbde6e-0271-4118-acaf-3e85de054603" />


**Products List**

<img width="800" height="362" alt="ProductsList" src="https://github.com/user-attachments/assets/e1b706d5-3729-4c84-af62-a60273ea3c79" />


**Add Product**

<img width="800" height="364" alt="AddProduct" src="https://github.com/user-attachments/assets/ed22b114-a359-4c11-9ce0-8d4472c8c578" />


**Search Product**

<img width="800" height="364" alt="SearchProduct" src="https://github.com/user-attachments/assets/29e1005f-83fd-406f-968c-af1c63810f7b" />


**Product Details**

<img width="1913" height="866" alt="ProductDetails" src="https://github.com/user-attachments/assets/7c6d625d-9e5d-4634-893c-3ddae8170d43" />


**Update Product**

<img width="800" height="360" alt="EditProduct" src="https://github.com/user-attachments/assets/248ecc9c-a745-460e-b384-24874d222540" />


**Delete Product**

<img width="800" height="365" alt="DeleteProduct" src="https://github.com/user-attachments/assets/24feb5df-9bf1-4086-bdd3-d2d7c5b0efc0" />


**Suppliers List**

<img width="1908" height="862" alt="SuppliersList" src="https://github.com/user-attachments/assets/8fb70006-d384-4f7e-997b-09f33bb5bb32" />


**Supplier Details**

<img width="1910" height="860" alt="SupplierDetails" src="https://github.com/user-attachments/assets/d48601ed-e91f-4bfe-b918-a1c5eacd3fc8" />


**Delete Supplier**

<img width="800" height="360" alt="DeleteSupplier" src="https://github.com/user-attachments/assets/806616e0-80b2-4870-8d1f-daa61c445cef" />


**input Errors**

<img width="1897" height="863" alt="InputErrors" src="https://github.com/user-attachments/assets/adc4cd56-830d-4ae3-ae6e-8a7af859b328" />


---

### License

This project is licensed under the MIT License.

---

### Contact

Developed by Nompumelelo.
Email: nsmbhense6@gmail.com

---
