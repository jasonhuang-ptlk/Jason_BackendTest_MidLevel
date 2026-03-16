using Jason_BackendTest_MidLevel.Helpers;
using Jason_BackendTest_MidLevel.Repositories;
using Jason_BackendTest_MidLevel.Repositories.Interfaces;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// ── 服務注冊 ──────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "MyOffice ACPD API",
        Version     = "v1",
        Description = "Jason_BackendTest_MidLevel — MyOffice_ACPD CRUD RESTful API"
    });

    // 讀取 XML 文件，讓 Swagger 顯示每個 API 的說明
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    // 啟用 SwaggerRequestExample / SwaggerResponseExample
    options.ExampleFilters();

    // 啟用 [SwaggerOperation] 等 Annotation
    options.EnableAnnotations();
});

// 注冊所有 IExamplesProvider（掃描同 Assembly 下的範例類別）
builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

// IHttpContextAccessor：LogHelper 讀取 Jason_Debug 標頭用
builder.Services.AddHttpContextAccessor();

// 注入 LogHelper 與 Repository
builder.Services.AddScoped<ILogHelper, LogHelper>();
builder.Services.AddScoped<IMyOfficeAcpdRepository, MyOfficeAcpdRepository>();

// ── 管線設定 ──────────────────────────────────────────────────
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyOffice ACPD API v1");
        c.RoutePrefix = string.Empty; // F5 直接開啟 Swagger UI（不需 /swagger 前綴）
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
