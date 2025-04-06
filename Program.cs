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


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración de DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")),
    ServiceLifetime.Scoped
);

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
