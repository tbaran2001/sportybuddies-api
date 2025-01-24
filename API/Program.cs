using API.Common.Behaviors;
using API.Common.Exceptions.Handler;
using API.Data;
using API.Extensions;
using API.Hubs;
using API.Services.Implementations;
using API.Services.Interfaces;
using Carter;
using FluentValidation;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCarter();

builder.Services.AddMediatR(serviceConfiguration =>
{
    serviceConfiguration.RegisterServicesFromAssembly(typeof(Program).Assembly);
    serviceConfiguration.AddOpenBehavior(typeof(ValidationBehavior<,>));
    serviceConfiguration.AddOpenBehavior(typeof(LoggingBehavior<,>));
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

builder.Services.AddMassTransit(configuration =>
{
    configuration.SetKebabCaseEndpointNameFormatter();
    configuration.AddConsumers(typeof(Program).Assembly);
    configuration.UsingInMemory((context, config) => config.ConfigureEndpoints(context));
});

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("React",
        corsPolicyBuilder => corsPolicyBuilder.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

var app = builder.Build();

app.UseCors("React");

app.MapCarter();

app.UseExceptionHandler(_ => { });

app.InitializeDatabaseAsync();


app.UseSwagger();
app.UseSwaggerUI();

app.MapIdentityApi();
app.MapHub<ChatHub>("chatHub");

app.Run();