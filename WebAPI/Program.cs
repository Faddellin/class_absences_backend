using BusinessLogic;
using BusinessLogic.ServiceInterfaces;
using BusinessLogic.Services;
using class_absences_backend.Middlewares;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddScoped<IRequestService, RequestService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(
    options =>
    {
        options.UseNpgsql(configuration.GetConnectionString(nameof(AppDbContext)));
    });
builder.Services.AddBusinessLogic();

var app = builder.Build();

app.UseCustomExtensionsHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();