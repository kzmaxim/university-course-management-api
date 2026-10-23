namespace UniversityCourseManagementApi.Configuration;

public class CorsSettings
{
    public const string SectionName = "Cors";
    public const string PolicyName = "ClientApplications";

    public string[] AllowedOrigins { get; set; } = [];
}
