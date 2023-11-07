using StudyBuddy.Data;
using Microsoft.EntityFrameworkCore;
using StudyBuddy.Data.Repositories;
using StudyBuddy.Data.Repositories.MatchRepository;
using StudyBuddy.Data.Repositories.UserRepository;
using StudyBuddy.Managers.FileManager;
using StudyBuddy.Middlewares;
using StudyBuddy.Services.MatchingService;
using StudyBuddy.Services.UserService;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using StudyBuddy.Controllers.ChatController;
using StudyBuddy.Hubs;
using StudyBuddy.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IMvcBuilder mvcBuilder = builder.Services.AddRazorPages();

if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<IMatchRequestRepository, MatchRequestRepository>();

// Register services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<MessageService>();

builder.Services.AddMvc();
builder.Services.AddSignalR();

// Registering <AuthenticationMiddleware> with its implementation for DI
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddControllersWithViews();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Add middleware to the HTTP request pipeline.
app.UseMiddleware<AuthenticationMiddleware>();

app.UseWebSockets(); // for SignalR

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chat");
});



app.MapControllerRoute(
"default",
"{controller=Home}/{action=Index}/{id?}");


// Retrieve the FileManager singleton and execute LoadUsersFromCsv
// FileManager fileManager = app.Services.GetRequiredService<FileManager>();
// fileManager.LoadUsersFromCsv("test.csv");

app.Run();
