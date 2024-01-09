
using MBM.Common.Middleware;
using MBM.Common.Models.Options;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Configuration.AddJsonFile("appsettings.json");

var configuration = builder.Configuration;

var dalBaseUrl = configuration["ApiSettings:DalBaseUrl"];
var tokenBaseUrl = configuration["ApiSettings:TokenBaseUrl"];

builder.Services.AddHttpClient("DalApiClient", client =>
{
    client.BaseAddress = new Uri(dalBaseUrl!);
});

builder.Services.AddHttpClient("TokenApiClient", client =>
{
    client.BaseAddress = new Uri(tokenBaseUrl!);
});

var app = builder.Build();

//var domainRestrictionOptions = builder.Configuration.GetSection("DomainRestriction").Get<DomainRestrictionOptions>();

//app.UseMiddleware<DomainRestrictionMiddleware>(Options.Create(domainRestrictionOptions!));

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
