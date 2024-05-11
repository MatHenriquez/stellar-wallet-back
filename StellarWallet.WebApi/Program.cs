using StellarWallet.Application.Interfaces;
using StellarWallet.Application.Mappers;
using StellarWallet.Application.Services;
using StellarWallet.Domain.Repositories;
using StellarWallet.Infrastructure.DatabaseConnection;
using StellarWallet.Infrastructure.Repositories;
using StellarWallet.Infrastructure.Stellar;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using StellarWallet.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

byte[]? keyBytes;

if (builder.Environment.IsEnvironment("dev"))
{
    builder.Configuration.AddUserSecrets<Program>();
    builder.Configuration.AddJsonFile("appsettings.json");
    var secretKey = builder.Configuration.GetSection("settings").GetSection("secretKey").Value ?? throw new Exception("Secret key not found");

    keyBytes = Encoding.UTF8.GetBytes(secretKey);
}
else
{
    builder.Configuration.AddUserSecrets<Program>();
    builder.Configuration.AddJsonFile("appsettings.test.json");
    var secretKey = builder.Configuration.GetSection("settings").GetSection("secretKey").Value ?? throw new Exception("Secret key not found");

    keyBytes = Encoding.UTF8.GetBytes(secretKey);
}

// Add services to the container.

// Add authentication services
builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddControllers();
builder.Services.AddDbContext<DatabaseContext>(); // Add DatabaseContext
builder.Services.AddScoped<IUserRepository, UserRepository>(); // Add UserRepository
builder.Services.AddScoped<IUserService, UserService>(); // Add UserService
builder.Services.AddScoped<IBlockchainService, StellarService>(); // Add BlockchainService
builder.Services.AddScoped<ITransactionService, TransactionService>(); // Add TransactionService
builder.Services.AddScoped<IAuthService, AuthService>(); // Add AuthService
builder.Services.AddScoped<IJwtService, JwtService>(); // Add JwtService
builder.Services.AddScoped<IEncryptionService, EncryptionService>(); // Add EncryptionService
builder.Services.AddScoped<IUserContactRepository, UserContactRepository>(); // Add UserContactRepository
builder.Services.AddScoped<IUserContactService, UserContactService>(); // Add UserContactService
builder.Services.AddScoped<IBlockchainAccountRepository, BlockchainAccountRepository>(); // Add BlockchainAccountRepository
builder.Services.AddAutoMapper(typeof(AutoMapperProfile)); // Add AutoMapper

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowBlazor");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
