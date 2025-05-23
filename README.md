# 🎬 Movie App Web API - ASP.NET Core 9

This project is a RESTful Web API built using **ASP.NET Core 9**, designed to manage and serve data for a movie application. It allows users and clients (such as web or mobile frontends) to interact with a backend movie database efficiently and securely.

---

## 🧰 Technologies Used

- **ASP.NET Core 9**
- **Entity Framework Core**
- **SQL Server / SQLite**
- **Swagger / Swashbuckle**
- **AutoMapper**
- **JWT Authentication (Optional)**
- **LINQ and Async/Await**

---

## 📦 Features

### 🎥 Movie Management (CRUD)
- **Create**: Add new movies with title, description, release year, genre, duration, rating, etc.
- **Read**: Retrieve all movies or a specific movie by ID.
- **Update**: Modify movie details.
- **Delete**: Remove movies from the database.

### 🔍 Filtering & Searching
- Filter movies by:
  - Title (partial/full match)
  - Genre
  - Release year
  - Minimum rating
- Sort by popularity, release date, or title.
- Pagination for large movie lists.

### 👤 User Authentication & Authorization (Optional)
- User registration and login.
- Secure endpoints using **JWT (JSON Web Tokens)**.
- Role-based access (e.g., admin vs. regular user).

### 🗂️ Data Persistence
- Database integration using **Entity Framework Core**.
- Code-first or database-first approach.
- Migrations to update schema easily.

### 📄 API Documentation
- Integrated **Swagger UI** for testing and understanding available endpoints.
- Clear schema definitions and example requests/responses.

### ⚙️ Other Functionalities
- **Dapper** for clean and simple object mapping between DTOs and entities.
- Global exception handling.
- Logging using built-in or third-party libraries.
- CORS configuration to support frontend integration.


