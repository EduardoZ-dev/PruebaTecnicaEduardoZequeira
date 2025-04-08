using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RouletteTechTest.API.Data.Common;
using RouletteTechTest.API.Data.Context;
using RouletteTechTest.API.Data.Repositories;
using RouletteTechTest.API.Models.DTOs.User;
using RouletteTechTest.API.Models.Entities;
using RouletteTechTest.API.Models.Validations;
using RouletteTechTest.API.Services.Implementations;
using RouletteTechTest.API.Services.Interfaces;


var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy
              .WithOrigins("http://localhost:5173") // URL de tu frontend
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
        });
});


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuraci�n de DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Validadores
builder.Services.AddScoped<IValidator<Session>, SessionValidator>();
builder.Services.AddScoped<IValidator<UserCreateDTO>, UserValidator>();
builder.Services.AddScoped<SpinValidator>();

// Repositorios
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<IRoundRepository, RoundRepository>();
builder.Services.AddScoped<IBetRepository, BetRepository>();

// Servicios
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IBetService, BetService>();
builder.Services.AddScoped<IRoundService, RoundService>();

builder.Services.AddScoped<IBetCalculator, BetCalculator>();


builder.Services.AddScoped<IUnitOfWork>(provider =>
{
    var context = provider.GetRequiredService<ApplicationDbContext>();
    return new UnitOfWork(
        context,
        new SessionRepository(context),
        new UserRepository(context),
        new RoundRepository(context),
        new BetRepository(context)
    );
});

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
