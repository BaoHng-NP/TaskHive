using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Net.payOS;
using System.Text;
using System.Text.Json.Serialization;
using TaskHive.API;
using TaskHive.API.Hubs;
using TaskHive.Repository;
using TaskHive.Service.Mappings;
using TaskHive.Service.Services.PaymentService;
using TaskHive.Service.Settings;

var builder = WebApplication.CreateBuilder(args);

// --- 1) Bind & register PayOS (unchanged) ---
builder.Services.Configure<PayOsSettings>(builder.Configuration.GetSection("PayOs"));
builder.Services.AddSingleton<PayOS>(sp =>
{
    var s = sp.GetRequiredService<IOptions<PayOsSettings>>().Value;
    return new PayOS(s.ClientId, s.ApiKey, s.ChecksumKey);
});
builder.Services.AddScoped<IPaymentService, PaymentService>();

// --- 2) Bind & register Cloudinary (unchanged) ---
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));
builder.Services.AddSingleton(sp =>
{
    var c = sp.GetRequiredService<IOptions<CloudinarySettings>>().Value;
    return new Cloudinary(new Account(c.CloudName, c.ApiKey, c.ApiSecret));
});

// --- 3) CORS: Update for production ---
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.WithOrigins("http://localhost:5173")  // FE origin cho dev
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
        else
        {
            // ✅ Production CORS - có thể add FE domain sau
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
            // Note: Không dùng AllowCredentials() với AllowAnyOrigin()
        }
    });
});

// --- 4) SignalR & Controllers & EF & DI & Swagger ---
builder.Services.AddSignalR();
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddApplicationServices();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TaskHive API",
        Version = "v1",
        Description = builder.Environment.IsProduction()
            ? "TaskHive Production API - Authorized Access Only"
            : "TaskHive Development API"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

// --- 5) Authentication / Google ---
builder.Services.AddAuthentication(opts =>
{
    opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opts =>
{
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
    opts.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var path = context.HttpContext.Request.Path;
            if (path.StartsWithSegments("/hubs/chat") &&
                context.Request.Query.TryGetValue("access_token", out var token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        }
    };
})
.AddGoogle(opts =>
{
    opts.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
    opts.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
});

var app = builder.Build();

// --- 6) Middleware pipeline ---
app.UseCors();

// ✅ Enable Swagger cho cả Development và Production
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskHive API V1");

    if (app.Environment.IsDevelopment())
    {
        c.RoutePrefix = string.Empty; // Root URL cho development
        c.DocumentTitle = "TaskHive API - Development";
    }
    else
    {
        c.RoutePrefix = "api-docs"; // Custom route cho production
        c.DocumentTitle = "TaskHive API - Production (Authorized Access)";

        // ✅ Basic Auth cho Production Swagger
        c.ConfigObject.AdditionalItems.Add("requestInterceptor",
            @"(request) => {
                request.headers['Authorization'] = 'Basic ' + btoa('admin:TaskHive@2024');
                return request;
            }");

        // ✅ Hide some UI elements in production
        c.DefaultModelsExpandDepth(-1);
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    }
});

// ✅ HTTPS Redirect chỉ cho Development
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

// --- 7) Map SignalR hub & API controllers ---
app.MapHub<ChatHub>("/hubs/chat");
app.MapControllers();

// ✅ Health check endpoint
app.MapGet("/health", () => new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    environment = app.Environment.EnvironmentName,
    swagger = app.Environment.IsProduction() ? "enabled with basic auth" : "enabled"
});

app.Run();
