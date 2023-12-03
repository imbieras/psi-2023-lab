using Microsoft.EntityFrameworkCore;
using StudyBuddy.API.Data;
using StudyBuddy.API.Data.Repositories;
using StudyBuddy.API.Models;
using StudyBuddy.API.Services;
using StudyBuddy.API.Services.UserService;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

// Register repositories
builder.Services.AddRepositories();

// Register services
builder.Services.AddServices();

// Registering implementations for DI
builder.Services.AddDbContext<StudyBuddyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication? app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using IServiceScope scope = app.Services.CreateScope();
IUserService userService = scope.ServiceProvider.GetRequiredService<IUserService>();
await UserCounter.InitializeAsync(userService);

await app.RunAsync();
