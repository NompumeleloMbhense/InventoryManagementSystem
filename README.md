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

# The application follows a Clean Architecture pattern across four distinct projects:

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

# Prerequisites

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
