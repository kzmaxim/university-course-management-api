using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;
using UniversityCourseManagementClient.Configuration;
using UniversityCourseManagementClient.Models;

namespace UniversityCourseManagementClient.Services;

public class UniversityCourseApiClient : IUniversityCourseApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;
    private readonly ILogger<UniversityCourseApiClient> _logger;

    public UniversityCourseApiClient(HttpClient httpClient, IOptions<ApiSettings> settings, ILogger<UniversityCourseApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpClient.BaseAddress = new Uri(settings.Value.BaseUrl.TrimEnd('/') + "/");
    }

    public async Task<(IReadOnlyList<CourseListItem> Courses, string? Error)> GetCoursesAsync(
        CancellationToken cancellationToken = default)
    {
        var (data, error) = await SendAsync<IReadOnlyList<CourseListItem>>("api/v1/courses", cancellationToken);
        return (data ?? Array.Empty<CourseListItem>(), error);
    }

    public Task<(CourseListItem? Course, string? Error)> GetCourseAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        SendAsync<CourseListItem>($"api/v1/courses/{id}", cancellationToken);

    private async Task<(T? Data, string? Error)> SendAsync<T>(string path, CancellationToken cancellationToken)
    {
        try
        {
            using var response = await _httpClient.GetAsync(path, cancellationToken);
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(body))
            {
                return (default, "The API returned an empty response.");
            }

            var envelope = JsonSerializer.Deserialize<ApiResponse<T>>(body, JsonOptions);
            if (envelope is null)
            {
                return (default, "Unable to parse the API response.");
            }

            if (!response.IsSuccessStatusCode || !envelope.IsSuccess)
            {
                var message = envelope.ErrorMessage ?? $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}";
                _logger.LogWarning("API request to {Path} failed: {Message}", path, message);
                return (default, message);
            }

            return (envelope.Result, null);
        }
        catch (HttpRequestException exception)
        {
            _logger.LogWarning(exception, "Connection error while calling {Path}", path);
            return (default, "The API is unavailable. Check that the server is running and the base URL is correct.");
        }
        catch (TaskCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(exception, "Timeout while calling {Path}", path);
            return (default, "The API request timed out.");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unexpected error while calling {Path}", path);
            return (default, "An unexpected error occurred while contacting the API.");
        }
    }
}
