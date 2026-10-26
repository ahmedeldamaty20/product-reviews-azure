using Azure.Identity;
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

// Blob Storage without connection string, using DefaultAzureCredential for authentication
// on local development, it will use your Visual Studio account.
// on Azure, it will automatically use the Managed Identity of the App Service.
var blobEndpoint = builder.Configuration["Storage:BlobEndpoint"];
if (string.IsNullOrWhiteSpace(blobEndpoint))
{
    throw new InvalidOperationException(
        "Missing configuration 'Storage:BlobEndpoint' " +
        "(e.g. https://<account>.blob.core.windows.net).");
}

builder.Services.AddAzureClients(clients =>
{
    clients.AddBlobServiceClient(new Uri(blobEndpoint));
    clients.UseCredential(new DefaultAzureCredential());
});

//// Add Azure Blob Storage client to the DI container
//// The connection string is retrieved from the configuration (appsettings.json or environment variables)
//builder.Services.AddAzureClients(clients =>
//{
//    clients.AddBlobServiceClient(builder.Configuration.GetConnectionString("BlobStorage"));
//});

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
