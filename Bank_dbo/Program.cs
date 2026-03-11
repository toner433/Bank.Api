using Bank.API.Handlers;
using Bank.Application.Services.Implementations;
using Bank.Application.Services.Interfaces;
using Bank.Domain.Interfaces;
using Bank.Infrastructure.Context;
using Bank.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BankDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IDataBaseRepository, DataBaseRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ICardRepository, CardRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IOperationTypeRepository, OperationTypeRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<IOperationService, OperationService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IVerificationCodeService, VerificationCodeService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Session";
    options.DefaultChallengeScheme = "Session";
})
.AddScheme<AuthenticationSchemeOptions, SessionAuthenticationHandler>("Session", null);

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseMiddleware<Bank.API.Middleware.SessionAuthMiddleware>();
app.UseAuthentication();

app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BankDbContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();