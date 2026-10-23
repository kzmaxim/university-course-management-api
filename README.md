# University Course Management API

**Ткачев Максим**

REST Web API на ASP.NET Core для управления студентами, преподавателями, курсами и записями на курсы.

## Модуль 1

Базовая REST-архитектура: CRUD, Repository, DTO, AutoMapper, PostgreSQL, единый формат ответа и обработка ошибок.

### Архитектура

```
Client
  ↓ HTTP
Controller
  ↓
Service / Repository
  ↓
Entity Framework Core
  ↓
PostgreSQL
```

Контроллеры принимают HTTP-запросы и возвращают единый `ReturnResult<T>`. Бизнес-правила находятся в сервисах. Репозитории работают с `ApplicationDbContext`. Сущности БД не отдаются клиенту напрямую: для API используются DTO и AutoMapper.

### Запуск

1. PostgreSQL (локально или через Docker):

```bash
docker compose up -d
```

2. Строка подключения по умолчанию: `Host=localhost;Port=5433;Database=university_courses;Username=postgres;Password=postgres` (порт 5433, чтобы не конфликтовать с локальным PostgreSQL).

3. Запуск API:

```bash
dotnet run --project UniversityCourseManagementApi
```

При старте приложение применяет миграции. Тестовые данные добавляются через EF `HasData`.

Swagger UI: [http://localhost:5278/swagger](http://localhost:5278/swagger)

Дополнительный SQL-скрипт тестовых данных: `scripts/seed.sql`.

### Endpoints

| Method | URL | Описание |
|--------|-----|----------|
| GET | `/api/students` | Список студентов (с курсами) |
| GET | `/api/students/{id}` | Студент по ID вместе с курсами |
| POST | `/api/students` | Создать студента |
| PUT | `/api/students/{id}` | Изменить студента |
| DELETE | `/api/students/{id}` | Удалить студента |
| GET | `/api/teachers` | Список преподавателей |
| GET | `/api/teachers/{id}` | Преподаватель по ID |
| POST | `/api/teachers` | Создать преподавателя |
| PUT | `/api/teachers/{id}` | Изменить преподавателя |
| DELETE | `/api/teachers/{id}` | Удалить преподавателя |
| GET | `/api/courses` | Список курсов. Фильтры: `search`, `teacherId`, `minCredits`, `maxCredits` |
| GET | `/api/courses/{id}` | Курс по ID |
| POST | `/api/courses` | Создать курс |
| PUT | `/api/courses/{id}` | Изменить курс |
| DELETE | `/api/courses/{id}` | Удалить курс |
| GET | `/api/enrollments` | Список записей |
| GET | `/api/enrollments/{id}` | Запись по ID |
| POST | `/api/enrollments` | Записать студента на курс |
| PUT | `/api/enrollments/{id}/grade` | Изменить оценку студента |
| DELETE | `/api/enrollments/{id}` | Снять студента с курса |

### Формат ответа

Успех:

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "result": {},
  "errorCode": null,
  "errorMessage": null,
  "traceId": "..."
}
```

Ошибка:

```json
{
  "statusCode": 404,
  "isSuccess": false,
  "result": null,
  "errorCode": "STUDENT_NOT_FOUND",
  "errorMessage": "Student not found",
  "traceId": "..."
}
```

### Бизнес-правила

- нельзя записать несуществующего студента или записать на несуществующий курс;
- нельзя дважды записать одного студента на один курс;
- курс нельзя привязать к несуществующему преподавателю;
- запрос несуществующего объекта возвращает `404`, а не успешный ответ.

### Dependency Injection

`DbContext`, репозитории и сервисы зарегистрированы как **Scoped**: один экземпляр на HTTP-запрос, общий `DbContext` внутри запроса, без удержания соединения между запросами. AutoMapper регистрируется как Singleton.

### Валидация

DTO помечены атрибутами `Data Annotations` (`[Required]`, `[EmailAddress]`, `[MaxLength]`, `[Range]`). Ошибки модели возвращают `400 Bad Request` с `errorCode: VALIDATION_ERROR` и текстом из `InvalidModelStateResponseFactory` в `Program.cs`.

### Логирование

Используется встроенный `ILogger<T>`:

| Уровень | Где | Что пишется |
|---------|-----|-------------|
| **Information** | `*Service` | создание, изменение и удаление сущностей; запись на курс; смена оценки |
| **Warning** | `*Service` | запрос несуществующего объекта; нарушения бизнес-правил (дубликат записи, несуществующий студент/курс/преподаватель) |
| **Warning** | `ExceptionHandlingMiddleware` | ожидаемые ошибки (`AppException`) |
| **Error** | `ExceptionHandlingMiddleware` | непредвиденные исключения (полный stack trace только в лог, клиенту — `INTERNAL_SERVER_ERROR`) |

Уровни по умолчанию задаются в `appsettings.json` (`Default: Information`). При защите смотрите вывод консоли при `dotnet run`.

### Обработка ошибок

`ExceptionHandlingMiddleware` перехватывает исключения и отдаёт клиенту `ReturnResult` без stack trace и SQL. Централизованная валидация модели настроена в `Program.cs`.

---

## Модуль 2

Модернизация API: безопасный доступ, ASP.NET Core Identity, JWT, роли, CORS, версионирование и внешний MVC-клиент. Функциональность модуля 1 сохранена; изменения описаны ниже.

### Состав решения

| Проект | Назначение |
|--------|------------|
| `UniversityCourseManagementApi` | REST API, PostgreSQL, Identity, JWT |
| `UniversityCourseManagementClient` | MVC-клиент (список и детали курсов через HTTP) |

### Архитектура (с учётом безопасности)

```
Client (Browser / Postman / MVC app)
  ↓ HTTP + CORS
  ↓ Authorization: Bearer (JWT)
Controller  →  [Authorize] / [AllowAnonymous]
  ↓
Service (бизнес-правила + проверка владения ресурсом)
  ↓
Repository  →  EF Core  →  PostgreSQL
```

### Запуск

1. PostgreSQL: `docker compose up -d`

2. Локальные настройки API (JWT-ключ не хранится в Git):

```bash
cp UniversityCourseManagementApi/appsettings.Development.example.json \
   UniversityCourseManagementApi/appsettings.Development.json
```

Либо переменная окружения `Jwt__Key` (минимум 32 символа).

3. API:

```bash
dotnet run --project UniversityCourseManagementApi
```

Swagger: [http://localhost:5278/swagger](http://localhost:5278/swagger) (документы **v1** и **v2**, JWT Bearer).

4. MVC-клиент:

```bash
dotnet run --project UniversityCourseManagementClient
```

Клиент: [http://localhost:5180](http://localhost:5180). Адрес API: `UniversityCourseManagementClient/appsettings.json` → `UniversityCourseApi:BaseUrl`.

При старте API применяются миграции Identity и создаются роли/тестовые пользователи (`Data/IdentityDataSeeder.cs`, см. `scripts/seed-identity.md`).

### Аутентификация и авторизация

- **Аутентификация** — «кто вы?» (`POST /api/auth/login` → JWT с идентификатором, именем и ролями).
- **Авторизация** — «что вам можно?» (`[Authorize(Roles = ...)]`, политики, проверки в сервисах по `teacher_id` / `student_id` в claims).

JWT в заголовке `Authorization: Bearer <token>`. Неверный или просроченный токен → **401**; недостаточно прав → **403** (формат `ReturnResult`).

| Method | URL | Доступ |
|--------|-----|--------|
| POST | `/api/auth/register` | Anonymous (роль Student) |
| POST | `/api/auth/login` | Anonymous |
| GET | `/api/auth/me` | JWT |
| POST | `/api/admin/users/roles` | Admin |

Тестовые учётные записи: [scripts/seed-identity.md](scripts/seed-identity.md).

### Защита endpoints (кратко)

- Публичное чтение: списки/детали курсов (`/api/v1/courses`, `/api/v2/courses` или `/api/courses` → v1), преподаватели.
- Мутации студентов, преподавателей, курсов и записей — с JWT; большинство изменений — роль **Admin**.
- Записи и оценки: фильтрация по роли; преподаватель меняет оценки только на своих курсах; студент видит только свои записи.

### Версии API

Все ресурсы доступны в **v1** и **v2** с префиксом `/api/v{version}/`:

| Ресурс | v1 / v2 (пример) |
|--------|------------------|
| Auth | `/api/v1/auth/login`, `/api/v2/auth/me` |
| Students | `/api/v1/students`, `/api/v2/students/{id}` |
| Teachers | `/api/v1/teachers` |
| Courses | `/api/v1/courses`, `/api/v2/courses/{id}` |
| Enrollments | `/api/v1/enrollments` |
| Admin | `/api/v1/admin/users/roles` |

Для **студентов, преподавателей, записей и auth** контракт v1 и v2 совпадает (те же DTO и поведение). Различие v2 задано для **получения курсов**:

| Version | GET courses | Контракт |
|---------|-------------|----------|
| v1 | `/api/v1/courses`, `/api/v1/courses/{id}` | `CourseDto` (как в модуле 1) |
| v2 | `/api/v2/courses`, `/api/v2/courses/{id}` | `CourseV2Dto` (преподаватель подробнее, число записанных) |

POST/PUT/DELETE курсов одинаковы в v1 и v2 (ответ — `CourseDto`), только **Admin**.

**Совместимость модуля 1:** запросы без версии в URL (`/api/students`, `/api/courses`, …) middleware переписывает в `/api/v1/...`.

OpenAPI/Swagger — отдельные документы **v1** и **v2** для всех контроллеров.

### CORS

Именованная политика `ClientApplications`, origins из `Cors:AllowedOrigins` (`http://localhost:5180`, `http://localhost:5181`).

### Прохождение запроса

```
HTTP Request
  → ExceptionHandlingMiddleware
  → LegacyApiVersionRewriteMiddleware (/api/... → /api/v1/...)
  → CORS
  → Authentication (JWT Bearer → ClaimsPrincipal)
  → Authorization ([Authorize] / роли / 401|403 handler)
  → Controller
  → Service (TeacherId/StudentId для записей и оценок)
  → Repository → DB
  → ReturnResult JSON
```

### Проверка сценариев (примеры)

```bash
# Анонимный список курсов
curl -s http://localhost:5278/api/v1/courses | head

# Анонимное создание курса → 401
curl -s -o /dev/null -w "%{http_code}\n" -X POST http://localhost:5278/api/courses \
  -H "Content-Type: application/json" -d '{"name":"X","credits":3,"teacherId":1}'

# Login → JWT
curl -s -X POST http://localhost:5278/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"userName":"admin","password":"Admin123!"}'

# v1 и v2
curl -s http://localhost:5278/api/v1/courses/1
curl -s http://localhost:5278/api/v2/courses/1
```

### Безопасность

- Пароли и JWT не логируются.
- Ключ подписи JWT — конфигурация / user secrets / env, не репозиторий.
- Регистрация не выдаёт Admin; назначение ролей — только Admin API.
