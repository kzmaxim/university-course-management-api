using Microsoft.AspNetCore.Identity;
using UniversityCourseManagementApi.Common;
using UniversityCourseManagementApi.Models;

namespace UniversityCourseManagementApi.Data;

public static class IdentityDataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var role in AppRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        await EnsureUserAsync(
            userManager,
            userName: "admin",
            email: "admin@university.kz",
            password: "Admin123!",
            roles: [AppRoles.Admin]);

        await EnsureUserAsync(
            userManager,
            userName: "teacher.elena",
            email: "teacher.elena@university.kz",
            password: "Teacher123!",
            roles: [AppRoles.Teacher],
            teacherId: 1);

        await EnsureUserAsync(
            userManager,
            userName: "teacher.marat",
            email: "teacher.marat@university.kz",
            password: "Teacher123!",
            roles: [AppRoles.Teacher],
            teacherId: 2);

        await EnsureUserAsync(
            userManager,
            userName: "student.alex",
            email: "alex.smith@student.kz",
            password: "Student123!",
            roles: [AppRoles.Student],
            studentId: 1);

        await EnsureUserAsync(
            userManager,
            userName: "student.aigerim",
            email: "aigerim.nurpeisova@student.kz",
            password: "Student123!",
            roles: [AppRoles.Student],
            studentId: 2);
    }

    private static async Task EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        string userName,
        string email,
        string password,
        IReadOnlyList<string> roles,
        int? teacherId = null,
        int? studentId = null)
    {
        var user = await userManager.FindByNameAsync(userName);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                EmailConfirmed = true,
                TeacherId = teacherId,
                StudentId = studentId
            };

            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to create user {userName}: {string.Join(", ", createResult.Errors.Select(error => error.Description))}");
            }
        }
        else
        {
            user.TeacherId = teacherId;
            user.StudentId = studentId;
            await userManager.UpdateAsync(user);
        }

        var currentRoles = await userManager.GetRolesAsync(user);
        foreach (var role in roles.Where(role => !currentRoles.Contains(role)))
        {
            await userManager.AddToRoleAsync(user, role);
        }
    }
}
