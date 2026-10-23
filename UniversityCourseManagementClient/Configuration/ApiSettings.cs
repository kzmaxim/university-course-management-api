namespace UniversityCourseManagementClient.Configuration;

public class ApiSettings
{
    public const string SectionName = "UniversityCourseApi";

    public string BaseUrl { get; set; } = "http://localhost:5278";
}
