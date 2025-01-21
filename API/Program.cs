using API.Common.Exceptions.Handler;
using API.Common.Models;
using API.Data;
using API.Data.Repositories.Implementations;
using API.Data.Repositories.Interfaces;
using API.Extensions;
using Carter;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCarter();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database"));
});
builder.Services.AddScoped<IProfilesRepository, ProfilesRepository>();
builder.Services.AddScoped<IUnitOfWork>(serviceProvider =>
    serviceProvider.GetRequiredService<ApplicationDbContext>());

builder.Services.AddMediatR(serviceConfiguration =>
{
    serviceConfiguration.RegisterServicesFromAssembly(typeof(Program).Assembly);
});
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddIdentity(builder.Configuration);
builder.Services.AddScoped<ISportsRepository, SportsRepository>();

var app = builder.Build();

app.MapCarter();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.MapIdentityApi();

app.Run();