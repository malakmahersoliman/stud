# Stud — Student & Department Management

Full-stack app for managing **students** and **departments**.

| Part | Location | Stack |
|------|----------|--------|
| Backend | `stud/` | ASP.NET Core 10, EF Core, MediatR, SQL Server |
| Frontend | `C:\Users\malak\OneDrive\Documents\Angular\stud-client` | Angular 21, signals, HttpClient |

**API:** `http://localhost:5275/api`  
**Angular:** `http://localhost:4200` (or `4201` if another app uses 4200)

---

## How to run

**Backend**
```powershell
cd stud
dotnet run --launch-profile http
```

**Frontend**
```powershell
cd C:\Users\malak\OneDrive\Documents\Angular\stud-client
npm start
```

Swagger (development): `http://localhost:5275/swagger`

---

## Implementation steps

Follow this order when adding or rebuilding the project. **`Program.cs` is updated in small steps** as each layer is added — do not leave all registration until the end.

### Backend (`stud/`)

#### 1. Domain entities

Create the core models and their relationship.

- `Domain/Student.cs` — `Id`, `Name`, `Navname`, `DepartmentId`, navigation to `Department`
- `Domain/Department.cs` — `Id`, `Name`, collection of `Students`

**`Program.cs`:** nothing new yet.

---

#### 2. `AppDbContext`

- `Data/AppDbContext.cs` — inherit `DbContext`
- Add `DbSet<Department>` and `DbSet<Student>`
- In `OnModelCreating`, call `ApplyConfigurationsFromAssembly` so Fluent API configs are picked up

**`appsettings.json`:** add `ConnectionStrings:DefaultConnection` (SQL Server).

**`Program.cs` — register services:**
```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

---

#### 3. Fluent API configuration

One configuration class per entity (table rules, keys, lengths, relationships).

- `Data/Configurations/DepartmentConfiguration.cs`
- `Data/Configurations/StudentConfiguration.cs`

Example: required `Name`, max length, foreign key `DepartmentId`, cascade delete from department to students.

**`Program.cs`:** no change (configs are applied inside `AppDbContext`).

---

#### 4. Migration

```powershell
cd stud
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**`Program.cs` — after `var app = builder.Build();`:**
```csharp
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}
```

---

#### 5. DTOs

Request/response shapes for the API (do not expose EF entities from controllers).

- `DTOs/Department/` — `DepartmentRequestDto`, `DepartmentResponseDto`
- `DTOs/Student/` — `StudentRequestDto`, `StudentResponseDto`

**`Program.cs`:** no change.

---

#### 6. Features (CQRS with MediatR)

Organize by entity: **Command** (write) or **Query** (read). Build **one command or query at a time** — class + handler, then test.

**`Program.cs` — when you add the first feature (before handlers can run):**
```csharp
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
```

**Optional — FluentValidation + pipeline behavior** (`Common/Behaviors/ValidationBehavior.cs`):
```csharp
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
```

**`Program.cs` — exception handling (after MediatR / validation):**
```csharp
app.UseExceptionHandler(/* map ValidationException, KeyNotFoundException, etc. */);
```

**Department**

| Step | Folder | What to build |
|------|--------|----------------|
| Command | `Feature/Department/Command/CreateDepartment/` | `CreateDepartmentCommand` + handler |
| Command | `Feature/Department/Command/UpdateDepartment/` | `UpdateDepartmentCommand` + handler |
| Command | `Feature/Department/Command/DeleteDepartment/` | `DeleteDepartmentCommand` + handler |
| Query | `Feature/Department/Queries/GetAllDepartments/` | `GetAllDepartmentsQuery` + handler |
| Query | `Feature/Department/Queries/GetDepartmentById/` | `GetDepartmentByIdQuery` + handler (include students) |

**Student**

| Step | Folder | What to build |
|------|--------|----------------|
| Command | `Feature/Student/Command/CreateStudent/` | `CreateStudentCommand` + handler |
| Command | `Feature/Student/Command/UpdateStudent/` | `UpdateStudentCommand` + handler |
| Command | `Feature/Student/Command/DeleteStudent/` | `DeleteStudentCommand` + handler |
| Query | `Feature/Student/Queries/GetAllStudents/` | `GetAllStudentsQuery` + handler |
| Query | `Feature/Student/Queries/GetStudentById/` | `GetStudentByIdQuery` + handler |

New handlers are picked up automatically by `RegisterServicesFromAssembly` — no extra line in `Program.cs` per handler.

---

#### 7. Controllers

Thin API layer — inject `IMediator`, send commands/queries, return HTTP results.

- `Controllers/DepartmentController.cs` — `GET`, `GET {id}`, `POST`, `PUT {id}`, `DELETE {id}`
- `Controllers/StudentController.cs` — same pattern

Route: `[Route("api/[controller]")]` → `/api/department`, `/api/student`.

**`Program.cs` — register and map (if not already):**
```csharp
builder.Services.AddControllers();
// ...
app.MapControllers();
```

**`Program.cs` — Swagger (development):**
```csharp
builder.Services.AddSwaggerGen(/* ... */);
// after Build:
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(/* ... */);
}
```

---

#### 8. Frontend integration (CORS + HTTP pipeline)

When the Angular app will call the API:

**`Program.cs` — CORS:**
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://localhost:4201")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

app.UseCors("AllowAngular");
```

**`Program.cs` — HTTPS (local dev):** skip redirect so `http://localhost:5275` works from the browser:
```csharp
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
```

---

#### `Program.cs` checklist (summary)

| Step | What you add in `Program.cs` |
|------|------------------------------|
| 2 – DbContext | `AddDbContext<AppDbContext>` |
| 4 – Migration | `Database.Migrate()` on startup |
| 6 – Features | `AddMediatR`, optional validators + `ValidationBehavior`, `UseExceptionHandler` |
| 7 – Controllers | `AddControllers`, `MapControllers`, Swagger |
| 8 – Angular | `AddCors`, `UseCors`, conditional `UseHttpsRedirection` |

---

### Frontend (`stud-client/`)

Same idea: register providers in **`app.config.ts`** as each layer appears.

#### 1. Models

TypeScript interfaces matching API DTOs.

- `models/department.model.ts` — `Department`, `DepartmentRequest`
- `models/student.model.ts` — `Student`, `StudentRequest`

**`app.config.ts`:** no change.

---

#### 2. Environment

- `environments/environment.ts` — `apiUrl: 'http://localhost:5275/api'`

**`app.config.ts`:** no change.

---

#### 3. Services

HTTP + **signals** for list state.

- `services/department.service.ts` — load, create, update, delete
- `services/student.service.ts` — load, create, update, delete

**`app.config.ts` — register HttpClient:**
```typescript
import { provideHttpClient } from '@angular/common/http';

export const appConfig: ApplicationConfig = {
  providers: [
    provideHttpClient(),
    // ...
  ],
};
```

---

#### 4. Components

Reusable UI.

- `components/navbar/` — links to Departments and Students

**`app.config.ts`:** no change (components are imported where used).

---

#### 5. Pages + routing

Smart components: lists, forms, edit, delete.

- `pages/departments/`
- `pages/department-details/`
- `pages/students/`
- `app.routes.ts` — lazy-loaded routes

**`app.config.ts` — register router:**
```typescript
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(),
  ],
};
```

---

#### `app.config.ts` checklist (summary)

| Step | What you add |
|------|----------------|
| 3 – Services | `provideHttpClient()` |
| 5 – Pages | `provideRouter(routes)` |

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

Default catalog: **StudTest** (`appsettings.json` → `DefaultConnection`).

Create departments before students; each student needs a valid `departmentId`.
