using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using ProductReviews.Web.Data;
using ProductReviews.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// EnableRetryOnFailure is important for Azure SQL Database, especially if the database is paused.
// It helps to automatically retry the connection in case of transient failures.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.EnableRetryOnFailure(maxRetryCount: 5)
    );
});

// Add Azure Blob Storage client to the DI container
// The connection string is retrieved from the configuration (appsettings.json or environment variables)
builder.Services.AddAzureClients(clients =>
{
    clients.AddBlobServiceClient(builder.Configuration.GetConnectionString("BlobStorage"));
});

builder.Services.AddScoped<IImageStorageService, BlobImageStorageService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
