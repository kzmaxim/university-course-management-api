using UniversityCourseManagementClient.Configuration;
using UniversityCourseManagementClient.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection(ApiSettings.SectionName));
builder.Services.AddHttpClient<IUniversityCourseApiClient, UniversityCourseApiClient>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Courses}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
