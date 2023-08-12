using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Zefir.API.Middlewares;
using Zefir.BL.Abstractions;
using Zefir.BL.Services;
using Zefir.BL.Services.ProductServices;
using Zefir.DAL;

var builder = WebApplication.CreateBuilder();

// Add services to the container.

builder.Services.AddTransient<HandleGlobalErrorsMiddleware>();
builder.Services.AddControllers();

builder.Services.AddSingleton<ITokenService>(new TokenService(builder.Configuration));

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

builder.Services.AddTransient<ISortProductsService, SortProductsService>();
builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<SortProductsService>();
builder.Services.AddTransient<ICategoryService, CategoryService>();
builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddTransient<IBasketService, BasketService>();
builder.Services.AddScoped<IAccountService, AccountService>();

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

app.UseMiddleware<HandleGlobalErrorsMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

/// <summary>
///     s
/// </summary>
public partial class Program
{
}
