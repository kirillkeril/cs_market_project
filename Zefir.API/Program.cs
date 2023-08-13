using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Zefir.API.Infrastructure;
using Zefir.API.Middlewares;
using Zefir.DAL;

var builder = WebApplication.CreateBuilder();

// Add services to the container.
builder.Services.AddTransient<HandleGlobalErrorsMiddleware>();

builder.UseBlServices();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Zefir.API API", Version = "v1" });
    var filePath = Path.Combine(AppContext.BaseDirectory, "Zefir.xml");
    c.IncludeXmlComments(filePath);
});
// Add db context

builder.Services.AddDbContext<AppDbContext>(optionsBuilder =>
{
    optionsBuilder.UseNpgsql(builder.Configuration.GetValue<string>("DbConnection"));
});

// Authentication and authorization
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    var key = builder.Configuration["Jwt:Key"];
    if (key is null) throw new ArgumentException("key can't be null");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key))
    };
});


var app = builder.Build();


// Configure the HTTP request pipeline.
app.UseMiddleware<HandleGlobalErrorsMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.MapControllers();

app.Run();

/// <summary>
///
/// </summary>
public partial class Program
{
}
