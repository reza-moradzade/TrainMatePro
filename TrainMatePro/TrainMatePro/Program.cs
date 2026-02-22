using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TrainMatePro.Data;
using TrainMatePro.Middleware;
using TrainMatePro.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// تنظیم Swagger با پشتیبانی JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TrainMatePro API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IExerciseApiService, ExerciseApiService>();  // تغییر به Singleton
builder.Services.AddScoped<IExerciseSyncService, ExerciseSyncService>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddLogging();
builder.Services.AddScoped<IWorkoutService, WorkoutService>();

// JWT Authentication
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"] ?? "YourSecretKeyHere_MakeItLongAndComplex_123456789");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// CORS برای فرانت Vue
builder.Services.AddCors(options =>
{
    options.AddPolicy("VueApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// استفاده از CORS
app.UseCors("VueApp");

// میدلور سفارشی JWT
app.UseMiddleware<JwtMiddleware>();

// تنظیم مسیر فایل‌های استاتیک برای GIFها
app.UseStaticFiles(); // این برای wwwroot

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ایجاد دیتابیس و کاربر پیش‌فرض
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();

    // ایجاد کاربر مربی پیش‌فرض
    if (!dbContext.Users.Any())
    {
        var coach = new TrainMatePro.Models.User
        {
            Email = "qc.reza@gmail.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("reza123456"),
            FullName = "رضا مرادزاده",
            Role = "coach",
            Department = "بدنسازی"
        };

        dbContext.Users.Add(coach);
        dbContext.SaveChanges();

        Console.WriteLine("✅ Coach user created: qc.reza@gmail.com");
    }

    // ✅ همگام‌سازی حرکات
    try
    {
        var syncService = scope.ServiceProvider.GetRequiredService<IExerciseSyncService>();
        var count = await syncService.SyncExercisesFromApiAsync();
        Console.WriteLine($"✅ Synced {count} exercises to database");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error syncing exercises: {ex.Message}");
    }
}

app.Run();