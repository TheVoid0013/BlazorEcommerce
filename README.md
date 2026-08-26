# BlazorEcommerce

A simple full-stack e-commerce web app built with **Blazor WebAssembly** on the frontend and **ASP.NET Core Web API** on the backend.

## Features

- Browse featured products and shop by category
- Live search with autocomplete suggestions
- Product details with variant selection (size, color, etc.) and sale pricing
- Shopping cart with quantity updates and item removal
- Place orders
- User registration, login/logout, and password change (JWT-based auth)
- Live cart item counter in the nav bar

## Tech Stack

**Client**
- Blazor WebAssembly (C#)
- Bootstrap 5
- Blazored LocalStorage (auth token & cart persistence)

**Server**
- ASP.NET Core Web API
- Entity Framework Core
- JWT authentication

## Project Structure

```
BlazorEcommerce/
├── Client/          # Blazor WebAssembly app
│   ├── Pages/        # Routable pages (Login, Register, Cart, ProductDetails, etc.)
│   ├── Shared/        # Reusable components & layouts (NavMenu, Search, ProductList, etc.)
│   └── Services/      # HTTP client services (IProductService, ICartService, IAuthService, etc.)
├── Server/           # ASP.NET Core Web API
│   ├── Controllers/   # Auth, Cart, Category, Order, Product
│   └── Services/      # Business logic behind each controller
└── Shared/           # Models shared by Client and Server (Product, CartItem, ServiceResponse, etc.)
```

## Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (8.0 or later)
- A SQL database (e.g. SQL Server / SQLite, depending on your `Server` project's connection string)

### Running the app

1. Clone the repository
   ```bash
   git clone https://github.com/TheVoid0013/BlazorEcommerce.git
   cd BlazorEcommerce
   ```

2. Update the connection string in `Server/appsettings.json` to point to your database.

3. Apply migrations and run the server
   ```bash
   cd Server
   dotnet ef database update
   dotnet run
   ```

4. In a separate terminal, run the client
   ```bash
   cd Client
   dotnet run
   ```

5. Open the client URL shown in the terminal (e.g. `https://localhost:5001`) in your browser.

## API Overview

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/Auth/Register` | Register a new user |
| POST | `/api/Auth/Login` | Log in and receive a JWT |
| POST | `/api/Auth/change-password` | Change the logged-in user's password |
| GET | `/api/Product` | Get all products |
| GET | `/api/Product/{productId}` | Get a single product |
| GET | `/api/Product/featured` | Get featured products |
| GET | `/api/Product/category/{categoryUrl}` | Get products in a category |
| GET | `/api/Product/search/{searchText}/{page}` | Search products (paged) |
| GET | `/api/Product/searchsuggestions/{searchText}` | Get search autocomplete suggestions |
| GET | `/api/Category` | Get all categories |
| GET | `/api/Cart` | Get current cart items |
| POST | `/api/Cart` | Get cart products from a list of cart items (used for guest carts stored locally) |
| POST | `/api/Cart/products` | Get product details for a list of cart items |
| GET | `/api/Cart/count` | Get number of items in the cart |
| POST | `/api/Cart/add` | Add an item to the cart |
| PUT | `/api/Cart/update-quantity` | Update the quantity of a cart item |
| DELETE | `/api/Cart/{productId}/{productTypeId}` | Remove an item from the cart |
| POST | `/api/Order` | Place an order from the current cart |

All responses are wrapped in a `ServiceResponse<T>` object containing `data`, `success`, and `message`.

## License

This project is licensed under the [MIT License](LICENSE).