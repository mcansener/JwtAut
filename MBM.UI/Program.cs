
using MBM.Common.Helpers;
using MBM.Common.Middleware;
using MBM.Common.Models.Options;
using Microsoft.Extensions.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Configuration.AddJsonFile("appsettings.json");

var configuration = builder.Configuration;

var dalBaseUrl = configuration["DalBaseUrl"];
var tokenBaseUrl = configuration["TokenBaseUrl"];

builder.Services.AddHttpClient("DalApiClient", client =>
{
    client.BaseAddress = new Uri(dalBaseUrl!);
});

builder.Services.AddHttpClient("TokenApiClient", client =>
{
    client.BaseAddress = new Uri(tokenBaseUrl!);
});

// Configure services
builder.Services.Configure<DomainRestrictionOptions>(builder.Configuration.GetSection("DomainRestriction"));

LoggingHelper.InitializeLogger(args);
builder.Host.UseSerilog();


var app = builder.Build();

app.Use(async (context, next) =>
{
    // Retrieve the DomainRestrictionOptions from the DI container
    var domainRestrictionOptions = app.Services.GetRequiredService<IOptions<DomainRestrictionOptions>>().Value;

    var middleware = new DomainRestrictionMiddleware(next, domainRestrictionOptions);
    await middleware.InvokeAsync(context);
});

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

app.Run();
