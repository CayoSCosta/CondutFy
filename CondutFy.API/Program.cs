using CondutFy.Application.Common.Interfaces;
using CondutFy.Infrastructure.Context;
using CondutFy.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Pega a connection string padrão do appsettings correspondente ao ambiente
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// REGISTRO HÍBRIDO DO BANCO DE DADOS
builder.Services.AddDbContext<CondutFyDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        // 🛠️ Se for ambiente de desenvolvimento, usa SQLite
        options.UseSqlite(connectionString);
    }
    else
    {
        // 🚀 Se for Homologação ou Produção, usa PostgreSQL
        options.UseNpgsql(connectionString);
    }
});

builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<CondutFyDbContext>());
builder.Services.AddScoped<IMessagingService, ExternalMessagingService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();