using JobTracker.Data;
using JobTracker.Data.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add MVC to the app (this enables Controllers and Views)
builder.Services.AddControllersWithViews();

// Register the database context.
// This tells EF Core: "use SQL Server, and get the connection string from appsettings.json"
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register our service classes so they can be injected anywhere that needs them.
// AddScoped means: create one fresh instance per web request.
builder.Services.AddScoped<JobNumberService>();
builder.Services.AddScoped<JobService>();
builder.Services.AddScoped<TimeCardService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

// This sets the default page to Jobs/Index instead of Home/Index
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Jobs}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
