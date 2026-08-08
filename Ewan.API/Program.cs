using Ewan.Application.Interfaces;
using Ewan.Infrastructure.Persistence;
using Ewan.Infrastructure.Services;
using Ewan.Domain.Enums;
using FluentValidation;
using Ewan.Application.DTOs.Auth;
using Ewan.Domain.Entities;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
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
builder.Services.AddScoped<IOfferService, OfferService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
// سيب باقي الـ Services هنا لما تضيفها: IServiceItemService, IInquiryService...

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

// Railway (وأي منصة مشابهة) بتستقبل الطلبات بـ HTTPS من بره، وبتوصّلها للسيرفر بـ HTTP عادي من جوه.
// من غير السطر ده، السيرفر هيفتكر إنه شغال على HTTP بس، وScalar هيكتب رابط http:// غلط.
// لازم نمسح قائمة "البروكسيات المعروفة" الافتراضية، لأنها بترفض أي Header جاي من بروكسي
// مش مسجل فيها مسبقًا، وRailway مش مسجل فيها.
var forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
forwardedHeadersOptions.KnownNetworks.Clear();
forwardedHeadersOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedHeadersOptions);

// تطبيق أي Migration جديدة تلقائيًا وقت تشغيل السيرفر - عملي جدًا في مرحلة البداية
// من غير CI/CD Pipeline. لاحقًا لما يبقى عندكوا Staging/Production منفصلين، الأفضل
// تشغّل الـ Migrations كخطوة منفصلة في الـ Pipeline بدل ما تحصل تلقائي هنا
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EwanDbContext>();
    db.Database.Migrate();

    // لو معندناش أي مستخدم في لوحة التحكم لسه، نعمل SuperAdmin أولي تلقائيًا
    // من غير ده معندكيش أي طريقة تسجلي دخول بيها أول مرة
    if (!db.AdminUsers.Any())
    {
        var seedEmail = builder.Configuration["SeedAdmin:Email"];
        var seedPassword = builder.Configuration["SeedAdmin:Password"];

        if (!string.IsNullOrWhiteSpace(seedEmail) && !string.IsNullOrWhiteSpace(seedPassword))
        {
            var admin = new AdminUser
            {
                FullName = "Super Admin",
                Email = seedEmail.ToLower(),
                Role = AdminRole.SuperAdmin
            };
            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<Ewan.Domain.Entities.AdminUser>();
            admin.PasswordHash = hasher.HashPassword(admin, seedPassword);

            db.AdminUsers.Add(admin);
            db.SaveChanges();
        }
    }
}

// ============ 6) تفعيل Scalar UI ============
// شغالة بس في Development عادة، لو عايزها تفضل شغالة في الإنتاج شيل الـ if
// ملحوظة مؤقتة: طول ما لسه معندكوش دومين وبيئات منفصلة (Staging/Production)،
// سايبين Scalar شغالة دايمًا عشان زميلتك تقدر توصلها من رابط Railway مباشرة.
// لما تفصلوا البيئات بعدين، رجّع الشرط ده لـ: if (app.Environment.IsDevelopment())
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options
        .WithTitle("Ewan HR API")
        .WithTheme(ScalarTheme.Purple)
        .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Fetch);

    // تحديد الرابط الصح يدويًا (مش اعتمادًا على تخمين البروتوكول من الـ Request)
    // خدي بالك: لازم يكون عندك متغير PublicBaseUrl في Railway Variables بقيمة
    // https://ewan-production.up.railway.app (بدون / في الآخر)
    var publicBaseUrl = builder.Configuration["PublicBaseUrl"];
    if (!string.IsNullOrWhiteSpace(publicBaseUrl))
    {
        options.AddServer(publicBaseUrl, "Production");
    }
});
// الواجهة هتفتح على: https://your-app-name.up.railway.app/scalar/v1

app.UseHttpsRedirection();
app.UseCors("FrontendPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();