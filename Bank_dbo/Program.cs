using Microsoft.EntityFrameworkCore;
using Bank.Infrastructure.Context;
using Bank.Infrastructure.Repositories;
using Bank.Domain.Interfaces;
using Bank.Application.Services.Interfaces;
using Bank.Application.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BankDbContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();