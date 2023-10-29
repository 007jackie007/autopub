using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using WebApplication5;

var builder = WebApplication.CreateBuilder(args);
//string connectionString = builder.Configuration.GetConnectionString("AppConfig");
//builder.Configuration.AddAzureAppConfiguration(connectionString);
builder.Configuration.AddAzureAppConfiguration(options =>
    options.Connect(
        new Uri("https://appconfigjackietestdev.azconfig.io"),
        new MyClientAssertionCredential()));

//Connect(new Uri(connectionString),
//new DefaultAzureCredential(new DefaultAzureCredentialOptions() { TenantId = "c92c5826-4969-414a-a289-1409683ebac0" }))


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.Configure<Settings>(builder.Configuration.GetSection("TestApp:Settings"));

var appConfigSeting = builder.Configuration["TestApp:Settings:BackgroundColor"];
throw new ArgumentException($"value of azure app configuration : {appConfigSeting}");

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
