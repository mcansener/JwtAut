using MBM.Common.Helpers;
using MBM.Common.Middleware;
using MBM.Common.Models.Options;
using MBM.DAL.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<AppDbContext>(c => c.UseSqlServer(connectionString));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<UserManager<User>>();

// Configure services
builder.Services.Configure<DomainRestrictionOptions>(builder.Configuration.GetSection("DomainRestriction"));

LoggingHelper.InitializeLogger(args);
builder.Host.UseSerilog();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    // Retrieve the DomainRestrictionOptions from the DI container
    var domainRestrictionOptions = app.Services.GetRequiredService<IOptions<DomainRestrictionOptions>>().Value;

    var middleware = new DomainRestrictionMiddleware(next, domainRestrictionOptions);
    await middleware.InvokeAsync(context);
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();