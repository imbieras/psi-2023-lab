using StudyBuddy.Abstractions;
using StudyBuddy.Managers;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IMvcBuilder mvcBuilder = builder.Services.AddRazorPages();

if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}

// Registering implementations for DI
builder.Services.AddSingleton<IUserManager, UserManager>();
builder.Services.AddSingleton<FileManager>();

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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
"default",
"{controller=Home}/{action=Index}/{id?}");

// Retrieve the FileManager singleton and execute LoadUsersFromCsv
var fileManager = app.Services.GetRequiredService<FileManager>();
fileManager.LoadUsersFromCsv("test.csv");

app.Run();
