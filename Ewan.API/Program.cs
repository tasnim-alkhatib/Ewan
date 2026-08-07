using Ewan.Application.Interfaces;
using Ewan.Infrastructure.Persistence;
using Ewan.Infrastructure.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ============ 1) Database ============
builder.Services.AddDbContext<EwanDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ============ 2) Services (DI) ============
builder.Services.AddScoped<IBannerService, BannerService>();
builder.Services.AddScoped<IFileStorageService, AzureBlobStorageService>();
// سيب باقي الـ Services هنا لما تضيفها: IOfferService, IServiceItemService, IInquiryService...

builder.Services.AddValidatorsFromAssembly(typeof(Ewan.Application.DTOs.Banners.UpsertBannerRequestValidator).Assembly);
builder.Services.AddFluentValidationAutoValidation();

// ============ 3) JWT Authentication (لوحة التحكم) ============
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!))
        };
    });
builder.Services.AddAuthorization();

// ============ 4) CORS (عشان الفرونت React يقدر يكلم الـ API) ============
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",   // Vite dev server
                "http://localhost:3000",   // لو زميلتك مستخدمة CRA
                "https://ewan-hr-admin.up.railway.app", // لوحة التحكم على Railway - عدّل الاسم حسب اللي Railway هيديهولك فعليًا
                "https://ewan-hr-web.up.railway.app"    // الموقع العام على Railway - نفس الملاحظة
                                                        // لما يبقى عندكوا دومين حقيقي، ضيفه هنا وسيب رابط railway.app لحد ما تتأكدوا إن كل حاجة اتنقلت
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// الـ Enums هتترجع كـ نص (زي "HomeSlider") بدل رقم (زي 1) في كل الـ Responses
// ده أسهل بكتير على الفرونت، وبيظهر أوضح في Scalar
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// ============ 5) OpenAPI (الأساس اللي Scalar بيعرض منه) ============
// ده الـ Endpoint المدمج في .NET 9 نفسه، بيولد وثيقة /openapi/v1.json تلقائي
builder.Services.AddOpenApi();

var app = builder.Build();

// تطبيق أي Migration جديدة تلقائيًا وقت تشغيل السيرفر - عملي جدًا في مرحلة البداية
// من غير CI/CD Pipeline. لاحقًا لما يبقى عندكوا Staging/Production منفصلين، الأفضل
// تشغّل الـ Migrations كخطوة منفصلة في الـ Pipeline بدل ما تحصل تلقائي هنا
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EwanDbContext>();
    db.Database.Migrate();
}

// ============ 6) تفعيل Scalar UI ============
// شغالة بس في Development عادة، لو عايزها تفضل شغالة في الإنتاج شيل الـ if
// ملحوظة مؤقتة: طول ما لسه معندكوش دومين وبيئات منفصلة (Staging/Production)،
// سايبين Scalar شغالة دايمًا عشان زميلتك تقدر توصلها من رابط azurewebsites.net مباشرة.
// لما تفصلوا البيئات بعدين، رجّع الشرط ده لـ: if (app.Environment.IsDevelopment())
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options
        .WithTitle("Ewan HR API")
        .WithTheme(ScalarTheme.Purple)
        .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Fetch);
});
// الواجهة هتفتح على: https://your-app-name.up.railway.app/scalar/v1

app.UseHttpsRedirection();
app.UseCors("FrontendPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();