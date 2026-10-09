# University Course Management API

REST Web API на ASP.NET Core для управления студентами, преподавателями, курсами и записями на курсы.

## Архитектура

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

## Запуск

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

## Endpoints

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
| PUT | `/api/enrollments/{id}` | Изменить оценку |
| DELETE | `/api/enrollments/{id}` | Снять студента с курса |

## Формат ответа

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

## Бизнес-правила

- нельзя записать несуществующего студента или записать на несуществующий курс;
- нельзя дважды записать одного студента на один курс;
- курс нельзя привязать к несуществующему преподавателю;
- запрос несуществующего объекта возвращает `404`, а не успешный ответ.

## Dependency Injection

`DbContext`, репозитории и сервисы зарегистрированы как **Scoped**: один экземпляр на HTTP-запрос, общий `DbContext` внутри запроса, без удержания соединения между запросами. AutoMapper регистрируется как Singleton.
