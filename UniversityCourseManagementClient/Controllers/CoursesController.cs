using Microsoft.AspNetCore.Mvc;
using UniversityCourseManagementClient.Services;

namespace UniversityCourseManagementClient.Controllers;

public class CoursesController : Controller
{
    private readonly IUniversityCourseApiClient _apiClient;

    public CoursesController(IUniversityCourseApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var (courses, error) = await _apiClient.GetCoursesAsync(cancellationToken);
        ViewBag.Error = error;
        return View(courses ?? Array.Empty<UniversityCourseManagementClient.Models.CourseListItem>());
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var (course, error) = await _apiClient.GetCourseAsync(id, cancellationToken);
        if (course is null)
        {
            ViewBag.Error = error ?? "Course not found.";
            return View();
        }

        return View(course);
    }
}
