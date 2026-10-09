-- Test data dump for University Course Management API.
-- Apply after migrations, or skip if the app already seeded HasData.

INSERT INTO "Teachers" ("Id", "FirstName", "LastName", "Email", "Department")
VALUES
    (1, 'Elena', 'Ivanova', 'elena.ivanova@university.kz', 'Software Engineering'),
    (2, 'Marat', 'Suleimenov', 'marat.suleimenov@university.kz', 'Information Systems')
ON CONFLICT ("Id") DO NOTHING;

INSERT INTO "Students" ("Id", "FirstName", "LastName", "Email", "BirthDate", "CreatedAt")
VALUES
    (1, 'Alex', 'Smith', 'alex.smith@student.kz', '2004-03-12', '2026-01-15 08:00:00+00'),
    (2, 'Aigerim', 'Nurpeisova', 'aigerim.nurpeisova@student.kz', '2003-11-02', '2026-01-15 08:00:00+00'),
    (3, 'Daniyar', 'Omarov', 'daniyar.omarov@student.kz', '2005-07-21', '2026-01-15 08:00:00+00')
ON CONFLICT ("Id") DO NOTHING;

INSERT INTO "Courses" ("Id", "Name", "Description", "Credits", "TeacherId", "CreatedAt")
VALUES
    (1, 'Web Services Development', 'REST APIs with ASP.NET Core', 5, 1, '2026-01-15 08:00:00+00'),
    (2, 'Database Systems', 'Relational databases and ORM', 4, 2, '2026-01-15 08:00:00+00'),
    (3, 'Software Architecture', 'Layered architecture and design patterns', 3, 1, '2026-01-15 08:00:00+00')
ON CONFLICT ("Id") DO NOTHING;

INSERT INTO "Enrollments" ("Id", "StudentId", "CourseId", "EnrollmentDate", "Grade")
VALUES
    (1, 1, 1, '2026-01-15 08:00:00+00', 87.50),
    (2, 2, 1, '2026-01-15 08:00:00+00', NULL)
ON CONFLICT ("Id") DO NOTHING;

SELECT setval(pg_get_serial_sequence('"Teachers"', 'Id'), (SELECT MAX("Id") FROM "Teachers"));
SELECT setval(pg_get_serial_sequence('"Students"', 'Id'), (SELECT MAX("Id") FROM "Students"));
SELECT setval(pg_get_serial_sequence('"Courses"', 'Id'), (SELECT MAX("Id") FROM "Courses"));
SELECT setval(pg_get_serial_sequence('"Enrollments"', 'Id'), (SELECT MAX("Id") FROM "Enrollments"));
