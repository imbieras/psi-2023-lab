using StudyBuddy.Attributes;
using StudyBuddy.Middlewares;
using StudyBuddy.Hubs;
using StudyBuddy.Services.UserSessionService;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IMvcBuilder mvcBuilder = builder.Services.AddRazorPages();

if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}

builder.Services.AddHttpClient("StudyBuddy.API", client =>
{
    // For production
    client.BaseAddress = new Uri("http://studybuddy.api:80/");
    // For local development
    // client.BaseAddress = new Uri("http://localhost:5100");
});

builder.Services.AddMvc();
builder.Services.AddSignalR();

builder.Services.AddScoped<CustomAuthorizeAttribute>();

builder.Services.AddScoped<IUserSessionService, UserSessionService>();

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

app.MapHub<ChatHub>("/chat");

app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");

await app.RunAsync();
