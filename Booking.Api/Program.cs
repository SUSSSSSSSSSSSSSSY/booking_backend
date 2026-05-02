//using Booking.Infrastructure;
//using System.Text;
//using Booking.Infrastructure.Auth;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;




//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Configuration.GetConnectionString("Default");

//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();




//builder.Services.AddInfrastructure();



//var app = builder.Build();



//// Configure the HTTP request pipeline.
////if (app.Environment.IsDevelopment())
////{
//    app.UseSwagger();
//    app.UseSwaggerUI();
////}



//app.UseHttpsRedirection();



//app.UseAuthorization();



//app.MapControllers();




//app.MapGet("/ping", () => Results.Ok("pong"));




//app.Run();


using System.Text;
using Booking.Infrastructure;
using Booking.Infrastructure.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Booking.Api.Auth;
using Booking.Application.Abstractions;
using Booking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Booking API",
        Version = "v1",
        Description = "Backend API for Booking-like hotel reservation application"
    });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Enter JWT token only. Example: eyJhbGciOiJIUzI1NiIs...",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, jwtSecurityScheme);

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            jwtSecurityScheme,
            []
        }
    });
});

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("Jwt"));

builder.Services.Configure<GoogleAuthOptions>(
    builder.Configuration.GetSection("Authentication:Google"));

var jwtOptions = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtOptions>();

if (jwtOptions is null)
{
    throw new InvalidOperationException("JWT options are not configured.");
}

if (string.IsNullOrWhiteSpace(jwtOptions.Key))
{
    throw new InvalidOperationException("JWT key is not configured.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions.Key)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173"
            //  URL Azure Static Web Apps frontend
            // "https://your-frontend.azurestaticapps.net"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

var usePostgres = bool.TryParse(
    builder.Configuration["Persistence:UsePostgres"],
    out var parsedUsePostgres
) && parsedUsePostgres;

if (usePostgres)
{
    using var scope = app.Services.CreateScope();

    var dbContext = scope.ServiceProvider.GetRequiredService<BookingDbContext>();

    await DatabaseSeeder.SeedAsync(dbContext);
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/ping", () => Results.Ok("pong"));

app.Run();