using StudyBuddy.Abstractions;
using StudyBuddy.Managers;

var builder = WebApplication.CreateBuilder(args);

var mvcBuilder = builder.Services.AddRazorPages();

if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}

// Register your IUserManager and its implementation here
builder.Services.AddScoped<IUserManager, UserManager>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
name: "default",
pattern: "{controller=Home}/{action=Index}/{id?}");

// Check if the "avatars" folder exists, and create it if it doesn't
string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
string avatarsFolder = Path.Combine(webRootPath, "avatars");
if (!Directory.Exists(avatarsFolder))
{
    Directory.CreateDirectory(avatarsFolder);
}

app.Run();
