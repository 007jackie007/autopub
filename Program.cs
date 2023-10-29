using Azure.Identity;
using Microsoft.Extensions.Configuration;
using WebApplication5;

var builder = WebApplication.CreateBuilder(args);
//string connectionString = builder.Configuration.GetConnectionString("AppConfig");
//builder.Configuration.AddAzureAppConfiguration(connectionString);
builder.Configuration.AddAzureAppConfiguration(options =>
    options.Connect(
        new Uri("https://appconfigjackietestdev.azconfig.io"),
        new ManagedIdentityCredential("4665a88d-987e-4633-9eaf-2d672f22c04d")));
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.Configure<Settings>(builder.Configuration.GetSection("TestApp:Settings"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
