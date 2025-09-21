# DevOps Control Center

> A custom Blazor + .NET 9 dashboard for Azure DevOps with role-based access control, audit logging, and fine-grained permissions. Extends DevOps APIs with extra guardrails so teams can control who can trigger builds, manage releases, and approve deployments.

![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)

---

## 🚀 Features
- 🔐 Individual account authentication with custom roles
- 📊 Blazor dashboard UI with real-time updates
- ⚙️ Integration with Azure DevOps REST APIs
- ✅ Guardrails for builds, releases, and deployments
- 📝 Audit logging for compliance

---

## 📦 Tech Stack
- .NET 9 / ASP.NET Core  
- Blazor Server  
- EF Core with SQL  
- Azure DevOps REST API  
- SignalR for real-time updates  

---

## 📄 License
This project is licensed under the [MIT License](./LICENSE).

## 🛠 Getting Started

Follow these steps to run the project locally.

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) (or update the connection string)
- Git

### Clone the Repository
```bash
git clone https://github.com/timsouthard/DevOpsControlCenter.git
cd DevOpsControlCenter
```
### Configure the Database

Update **appsettings.json** with your SQL connection string (defaults to LocalDB).

Apply the EF migrations:

```bash
dotnet ef database update --project DevOpsControlCenter.Infrastructure
```

### Run the Application

```bash
dotnet run --project DevOpsControlCenter.Web
```

The app will start on:
👉 [https://localhost:5001](https://localhost:5001)

### Default Accounts

For demo purposes, the database is seeded with:

**Admin**

* Email: `admin@demo.com`
* Password: `Admin123!`

**User**

* Email: `user@demo.com`
* Password: `User123!`