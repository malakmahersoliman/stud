# Stud — Student & Department Management

Full-stack application to manage **students** and **departments**: create, read, update, and delete records, with students linked to a department.

## Repositories

| Part | Repository / location |
|------|------------------------|
| **Frontend** | [github.com/malakmahersoliman/studentFrontEnd](https://github.com/malakmahersoliman/studentFrontEnd) |
| **Backend** | This repo (`stud/` project) |

Clone the frontend:

```powershell
git clone https://github.com/malakmahersoliman/studentFrontEnd.git
cd studentFrontEnd
npm install
```

---

## Tech stack

| Layer | Technologies |
|-------|----------------|
| Backend | ASP.NET Core 10, EF Core, SQL Server, MediatR, FluentValidation, Swagger |
| Frontend | Angular 21, standalone components, signals, HttpClient, reactive forms |

**URLs (local development)**

| App | URL |
|-----|-----|
| API | `http://localhost:5275/api` |
| Swagger | `http://localhost:5275/swagger` |
| Angular | `http://localhost:4200` (use `4201` if port 4200 is busy) |

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (LTS) + npm
- SQL Server (local instance; connection string in `stud/appsettings.json`)
- EF Core tools (first time only): `dotnet tool install --global dotnet-ef`

---

## Quick start

**1. Backend** (from this repo, `stud` folder)

```powershell
cd stud
dotnet run --launch-profile http
```

**2. Frontend** (GitHub repo or local copy)

```powershell
cd studentFrontEnd
npm start
```

**3. Browser**

- Departments: `/departments`
- Students: `/students`

Create at least one **department** before adding **students**.

---

## Project structure

### Backend (`stud/`)

```
stud/
├── Domain/                 # Student, Department entities
├── Data/
│   ├── AppDbContext.cs
│   └── Configurations/     # Fluent API
├── DTOs/
│   ├── Department/
│   └── Student/
├── Feature/                # MediatR commands & queries
│   ├── Department/
│   └── Student/
├── Controllers/
├── Common/Behaviors/       # Validation pipeline
├── Migrations/
├── Program.cs
└── appsettings.json
```

### Frontend ([studentFrontEnd](https://github.com/malakmahersoliman/studentFrontEnd))

```
src/app/
├── models/
├── services/               # HTTP + signals
├── components/             # navbar
├── pages/
│   ├── departments/
│   ├── department-details/
│   └── students/
├── app.routes.ts
└── app.config.ts

src/environments/environment.ts   # apiUrl → backend
```

---

## Features

- List, add, edit, and delete **departments**
- List, add, edit, and delete **students** (with department dropdown)
- View one department and its students (`/departments/:id`)
- API documented with Swagger in Development

---

## API endpoints

| Method | Department | Student |
|--------|------------|---------|
| GET | `/api/department` | `/api/student` |
| GET | `/api/department/{id}` | `/api/student/{id}` |
| POST | `/api/department` | `/api/student` |
| PUT | `/api/department/{id}` | `/api/student/{id}` |
| DELETE | `/api/department/{id}` | `/api/student/{id}` |

---

## Database

- Catalog: **StudTest** (configurable in `appsettings.json` → `ConnectionStrings:DefaultConnection`)
- Deleting a department removes its students (cascade)
- Migrations run on startup in Development (`Database.Migrate()` in `Program.cs`)

---

## Implementation guide

Build the project **layer by layer**. Register services in **`Program.cs`** (backend) and **`app.config.ts`** (frontend) as you go — not all at the end.

### Backend

| Step | What to build | `Program.cs` |
|------|---------------|--------------|
| 1 | **Entities** — `Domain/Student.cs`, `Domain/Department.cs` | — |
| 2 | **AppDbContext** — `DbSet`s, `ApplyConfigurationsFromAssembly` | `AddDbContext<AppDbContext>` + connection string in `appsettings.json` |
| 3 | **Fluent API** — `Data/Configurations/*Configuration.cs` | — |
| 4 | **Migration** — `dotnet ef migrations add InitialCreate` | `Database.Migrate()` after `Build()` |
| 5 | **DTOs** — request/response per entity | — |
| 6 | **Features** — MediatR handlers (see below) | `AddMediatR`, optional `AddValidatorsFromAssembly` + `ValidationBehavior`, `UseExceptionHandler` |
| 7 | **Controllers** — `DepartmentController`, `StudentController` | `AddControllers`, `MapControllers`, Swagger in Development |
| 8 | **Angular client** | `AddCors` (`4200`, `4201`), `UseCors`, skip `UseHttpsRedirection` in Development |

**Features — build in this order for each entity**

Commands (write):

1. **Create** — command + handler  
2. **Update** — command + handler  
3. **Delete** — command + handler  

Queries (read):

4. **GetAll** — query + handler  
5. **GetById** — query + handler (department: include `Students`)

Folders follow: `Feature/{Entity}/Command/{Action}/` and `Feature/{Entity}/Queries/{Action}/`.

Handlers use `AppDbContext`; controllers only call `_mediator.Send(...)`.

### Frontend

| Step | What to build | `app.config.ts` |
|------|---------------|-----------------|
| 1 | **Models** — `department.model.ts`, `student.model.ts` | — |
| 2 | **Environment** — `apiUrl: 'http://localhost:5275/api'` | — |
| 3 | **Services** — CRUD methods, signals for lists | `provideHttpClient()` |
| 4 | **Components** — e.g. `navbar` | — |
| 5 | **Pages** — departments, department-details, students + `app.routes.ts` | `provideRouter(routes)` |

---

## Configuration notes

**Frontend API URL** (`src/environments/environment.ts`):

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5275/api',
};
```

**CORS** must allow the port Angular uses (`Program.cs` → `AllowAngular` policy).

---

## Troubleshooting

| Problem | What to do |
|---------|------------|
| `Unable to load departments` in UI | Start the API; use `http` profile on port **5275**; refresh the page |
| `MSB3027` / `stud.exe` locked | Stop the running API (Shift+F5 or end **stud** process), then rebuild |
| API works in browser but not from Angular | Check CORS port (`4200` / `4201`); avoid HTTPS redirect in Development |
| Empty student list after delete department | Expected if cascade removed students; list reloads from API |

---

## Author

Malak Maher Soliman — frontend: [studentFrontEnd](https://github.com/malakmahersoliman/studentFrontEnd)
