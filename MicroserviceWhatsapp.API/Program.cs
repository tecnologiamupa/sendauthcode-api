using MicroserviceWhatsapp.Application.Interface;
using MicroserviceWhatsapp.Application.Service;
using MicroserviceWhatsapp.Data.Request;
using MicroserviceWhatsapp.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configurar servicios
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("MessageConnectionString");
 
// Registrar el DbContext usando UseMySql en el IServiceCollection
builder.Services.AddDbContext<ServicioMensajeriaDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 32))));

var awsSetting = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConfiguracionFB");
var JwtSettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("JwtSettings");
builder.Services.Configure<WSConfig>(awsSetting);
builder.Services.Configure<JwtSettings>(JwtSettings);
builder.Services.AddScoped<ISendMessage, ServiceSendMessage>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());
}
);

var app = builder.Build();

app.UseCors("CorsPolicy");
app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.


// Configure the HTTP request pipelin
app.UseAuthorization();

app.MapControllers();
app.Run();
