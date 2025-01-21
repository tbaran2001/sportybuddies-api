using API.Common.Exceptions.Handler;
using API.Data;
using API.Extensions;
using API.Hubs;
using API.Services.Implementations;
using API.Services.Interfaces;
using Carter;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCarter();

builder.Services.AddMediatR(serviceConfiguration =>
{
    serviceConfiguration.RegisterServicesFromAssembly(typeof(Program).Assembly);
});
builder.Services.AddValidatorsFromAssemblyContaining(typeof(Program));
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder
    .AddIdentity()
    .AddDatabase();

builder.Services.Configure<BlobStorageSettings>(builder.Configuration.GetSection("BlobStorage"));
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();

builder.Services.AddScoped<IBuddyService, BuddyService>();
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<IProfilePhotoService, ProfilePhotoService>();


builder.Services.AddSignalR();

var app = builder.Build();

app.MapCarter();

app.InitializeDatabaseAsync();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.MapIdentityApi();
app.MapHub<ChatHub>("chatHub");

app.Run();