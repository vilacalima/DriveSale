using Application.Common.Interfaces;
using Application.Common.Abstractions;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("Default");
    options.UseNpgsql(cs, m => m.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
});

// Application handlers via MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IApplicationMarker).Assembly));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Em desenvolvimento, evitamos redirecionar para HTTPS
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

// Aplica migrações automaticamente ao subir a aplicação (dev/prod)
try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}
catch (Exception ex)
{
    var raw = builder.Configuration.GetConnectionString("Default") ?? string.Empty;
    var masked = System.Text.RegularExpressions.Regex.Replace(raw, "(?i)(Password|Pwd)=[^;]*", "$1=****");
    app.Logger.LogError(ex, "Erro ao aplicar migrations. Verifique a connection string e o Postgres. ConnStr: {ConnStr}", masked);
    throw;
}

app.Run();
