# MinhaPrimeiraApi 🚀

This project is a robust and scalable RESTful API built with .NET Core, designed to manage products and categories, handle user authentication, and provide a solid foundation for e-commerce or inventory management applications. It leverages modern architectural patterns like the Repository and Unit of Work to ensure maintainability and testability. The API includes features like JWT authentication, pagination, filtering, and global exception handling, making it a comprehensive solution for backend development.

## 🌟 Key Features

- **User Authentication & Authorization:** Secure your API with JWT-based authentication, including login, registration, and refresh token functionality.
- **Product & Category Management:** Create, read, update, and delete products and categories with ease.
- **Pagination:** Efficiently handle large datasets with paged endpoints for products and categories.
- **Filtering:** Filter products by price and categories by name to find exactly what you need.
- **Global Exception Handling:** Consistent error responses with detailed error information for debugging.
- **Repository Pattern:** Decoupled data access layer for easy switching of data sources.
- **Unit of Work:** Manage database transactions efficiently and ensure data consistency.
- **AutoMapper Integration:** Simplify object-to-object mapping between domain models and DTOs.
- **Swagger/OpenAPI Support:** Interactive API documentation for easy exploration and testing.
- **Custom Logging:** Detailed logging for monitoring and troubleshooting.
- **Environment Variable Configuration:** Securely manage sensitive information using `.env` files.

## 🛠️ Tech Stack

- **Backend:**
    - .NET 7
    - ASP.NET Core Web API
    - C#
- **Database:**
    - MySQL (using Entity Framework Core)
- **ORM:**
    - Entity Framework Core
- **Authentication:**
    - JWT (JSON Web Tokens)
    - BCrypt.Net (Password Hashing)
- **Object Mapping:**
    - AutoMapper
- **Dependency Injection:**
    - ASP.NET Core built-in DI container
- **Configuration:**
    - `appsettings.json`
    - DotNetEnv (for `.env` file support)
- **API Documentation:**
    - Swagger/OpenAPI
- **Logging:**
    - Microsoft.Extensions.Logging
- **Other:**
    - Newtonsoft.Json
    - System.IdentityModel.Tokens.Jwt
    - System.Security.Cryptography
    - Microsoft.AspNetCore.Mvc
    - Microsoft.AspNetCore.Authorization
    - Microsoft.AspNetCore.Identity
    - Microsoft.EntityFrameworkCore
    - Microsoft.Extensions.Configuration
    - X.PagedList
    - X.PagedList.EF

## 📦 Getting Started

### Prerequisites

- [.NET 7 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
- [MySQL Server](https://www.mysql.com/downloads/)
- [Visual Studio](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/) with C# extension

### Installation

1.  Clone the repository:

    ```bash
    git clone <repository-url>
    cd MinhaPrimeiraApi
    ```

2.  Create a `.env` file in the `MinhaPrimeiraApi` directory and configure the database connection string:

    ```
    DATABASE_URL=Server=localhost;Port=3306;Database=YourDatabaseName;Uid=YourUsername;Pwd=YourPassword;
    ```

    Replace `YourDatabaseName`, `YourUsername`, and `YourPassword` with your MySQL credentials.

3.  Install the necessary .NET dependencies:

    ```bash
    dotnet restore
    ```

4.  Apply Entity Framework Core migrations to create the database schema:

    ```bash
    dotnet ef database update --project MinhaPrimeiraApi.Infra
    ```

### Running Locally

1.  Navigate to the `MinhaPrimeiraApi` directory:

    ```bash
    cd MinhaPrimeiraApi
    ```

2.  Run the application:

    ```bash
    dotnet run
    ```

3.  The API will be accessible at `https://localhost:<port>`, where `<port>` is the port number configured in `launchSettings.json` (typically 5001 for HTTPS).

4.  Access Swagger UI at `https://localhost:<port>/swagger` to explore the API endpoints.

## 📂 Project Structure

```
MinhaPrimeiraApi/
├── MinhaPrimeiraApi.sln                  # Solution file
├── MinhaPrimeiraApi/                     # Main API project
│   ├── Controllers/                      # API Controllers
│   │   ├── AuthController.cs
│   │   ├── CategoriesController.cs
│   │   ├── ProductsController.cs
│   ├── Extensions/                       # Extension methods
│   │   ├── ApiExecptionMiddlewareExtensions.cs
│   │   ├── ServiceCollectionExtensions.cs
│   ├── Filters/                          # Custom filters
│   │   ├── ApiExceptionFilter.cs
│   ├── Logging/                          # Custom logging implementation
│   ├── Program.cs                        # Entry point of the application
│   ├── appsettings.json                  # Configuration settings
│   ├── appsettings.Development.json
│   ├── .env                              # Environment variables
│   └── ...
├── MinhaPrimeiraApi.Services/            # Business logic and service implementations
│   ├── Services/
│   │   ├── TokenService.cs
│   │   └── ...
│   └── ...
├── MinhaPrimeiraApi.Infra/               # Infrastructure-related code (data access)
│   ├── Context/                          # Database context
│   │   ├── AppDbContext.cs
│   │   └── ...
│   ├── Repository/                       # Repositories
│   │   ├── CategoriesRepository.cs
│   │   ├── ProductsRepository.cs
│   │   ├── UnitOfWork.cs
│   │   ├── UserRepository.cs
│   │   └── ...
│   └── ...
├── MinhaPrimeiraApi.Domain/              # Domain models and interfaces
│   ├── DTOs/                             # Data Transfer Objects
│   │   ├── CategoryDTO.cs
│   │   ├── ProductDTO.cs
│   │   ├── ...
│   ├── Interface/                        # Interfaces for repositories and services
│   │   ├── ICategoryRepository.cs
│   │   ├── IProductsRepository.cs
│   │   ├── IUnitOfWork.cs
│   │   ├── IUserRepository.cs
│   │   ├── ITokenService.cs
│   │   └── ...
│   ├── Models/                            # Domain models
│   │   ├── Category.cs
│   │   ├── Product.cs
│   │   ├── User.cs
│   │   ├── Role.cs
│   │   ├── ...
│   ├── Models/Pagination/                 # Pagination related models
│   │   ├── CategoriesParameters.cs
│   │   ├── ProductsParameters.cs
│   │   ├── PagedList.cs
│   │   └── ...
│   └── ...
└── Solution Items/
    └── compose.yaml                      # Docker Compose file
```

## 📸 Screenshots

(Add screenshots of your application here to showcase its functionality and UI.)

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1.  Fork the repository.
2.  Create a new branch for your feature or bug fix.
3.  Make your changes and commit them with descriptive messages.
4.  Push your changes to your fork.
5.  Submit a pull request to the main repository.

## 📝 License

This project is licensed under the [MIT License](LICENSE).

## 📬 Contact

If you have any questions or suggestions, feel free to contact me at [Your Email].

## 💖 Thanks

Thank you for checking out this project! I hope it helps you in your development endeavors.

This is written by [readme.ai](https://readme-generator-phi.vercel.app/).
