# ============ Stage 1: Build ============
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# بننسخ ملفات الـ csproj الأول بس عشان Docker يعمل Cache للـ restore
# ولو غيرت كود من غير ما تغير الـ dependencies، الـ build هيبقى أسرع بكتير
COPY Ewan.Domain/Ewan.Domain.csproj Ewan.Domain/
COPY Ewan.Application/Ewan.Application.csproj Ewan.Application/
COPY Ewan.Infrastructure/Ewan.Infrastructure.csproj Ewan.Infrastructure/
COPY Ewan.API/Ewan.API.csproj Ewan.API/

RUN dotnet restore Ewan.API/Ewan.API.csproj

# دلوقتي ننسخ باقي الكود ونعمل Build فعلي
COPY . .
RUN dotnet publish Ewan.API/Ewan.API.csproj -c Release -o /app/publish --no-restore

# ============ Stage 2: Runtime ============
# نستخدم صورة الـ Runtime بس (مش الـ SDK) عشان الحجم النهائي يبقى أصغر بكتير
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Railway بيحدد البورت تلقائي عن طريق متغير PORT وقت التشغيل (مش وقت الـ Build)
# فمحتاجين نستخدم Shell عشان نقرأ قيمته لحظة تشغيل الـ Container، مش ENV ثابتة
EXPOSE 8080
ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-8080} dotnet Ewan.API.dll"]