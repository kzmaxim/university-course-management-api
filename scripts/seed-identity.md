# Test users (created on API startup)

| User | Password | Role | Linked entity |
|------|----------|------|----------------|
| `admin` | `Admin123!` | Admin | — |
| `teacher.elena` | `Teacher123!` | Teacher | Teacher id 1 |
| `teacher.marat` | `Teacher123!` | Teacher | Teacher id 2 |
| `student.alex` | `Student123!` | Student | Student id 1 |
| `student.aigerim` | `Student123!` | Student | Student id 2 |

Registration via `POST /api/auth/register` assigns the **Student** role only (no admin elevation).

Role changes: `POST /api/admin/users/roles` (Admin JWT required).
