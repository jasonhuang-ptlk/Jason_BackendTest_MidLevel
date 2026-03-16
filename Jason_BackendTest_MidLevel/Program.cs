using Jason_BackendTest_MidLevel.Helpers;
using Jason_BackendTest_MidLevel.Repositories;
using Jason_BackendTest_MidLevel.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ── 服務注冊 ──────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// IHttpContextAccessor：LogHelper 需要讀取 HTTP 請求標頭（Jason_Debug）
builder.Services.AddHttpContextAccessor();

// 注入 LogHelper 與 Repository
builder.Services.AddScoped<ILogHelper, LogHelper>();
builder.Services.AddScoped<IMyOfficeAcpdRepository, MyOfficeAcpdRepository>();

// ── 管線設定 ──────────────────────────────────────────────────
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
