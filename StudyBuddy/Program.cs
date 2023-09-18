using StudyBuddy.Abstractions;
using StudyBuddy.Managers;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IMvcBuilder mvcBuilder = builder.Services.AddRazorPages();

if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}

// Registering <IUserManager> with its implementation for DI
builder.Services.AddScoped<IUserManager, UserManager>();


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

app.Run();
