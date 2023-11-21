using StudyBuddy.Data;
using Microsoft.EntityFrameworkCore;
using StudyBuddy.Attributes;
using StudyBuddy.Data.Repositories;
using StudyBuddy.Managers.FileManager;
using StudyBuddy.Middlewares;
using StudyBuddy.Hubs;
using StudyBuddy.Models;
using StudyBuddy.Services;
using StudyBuddy.Services.UserService;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IMvcBuilder mvcBuilder = builder.Services.AddRazorPages();

if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}

// Register repositories
builder.Services.AddRepositories();

// Register services
builder.Services.AddServices();

// Registering implementations for DI
builder.Services.AddScoped<FileManager>();
builder.Services.AddScoped<CustomAuthorizeAttribute>();
builder.Services.AddDbContext<StudyBuddyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddMvc();
builder.Services.AddSignalR();

// Registering <AuthenticationMiddleware> with its implementation for DI
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddControllersWithViews();

WebApplication app = builder.Build();

using IServiceScope scope = app.Services.CreateScope();
IUserService userService = scope.ServiceProvider.GetRequiredService<IUserService>();
await UserCounter.InitializeAsync(userService);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Add middleware to the HTTP request pipeline.
app.UseMiddleware<AuthenticationMiddleware>();

app.UseWebSockets();// for SignalR

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapHub<ChatHub>("/chat");

app.MapControllerRoute(
"default",
"{controller=Home}/{action=Index}/{id?}");

// Retrieve the FileManager singleton and execute LoadUsersFromCsv
// FileManager fileManager = app.Services.GetRequiredService<FileManager>();
// fileManager.LoadUsersFromCsv("test.csv");

await app.RunAsync();
