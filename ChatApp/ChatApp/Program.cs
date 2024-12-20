using ChatApp.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ChatApp.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Загрузка конфигурации из файла appsettings.json
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// Добавляем сервисы в контейнер
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Добавляем ваши сервисы (например, TCP и UDP)
builder.Services.AddSingleton<TcpService>();
builder.Services.AddSingleton<UdpService>();

var app = builder.Build();

// Конфигурация для Swagger и маршрутизации
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Получаем экземпляры сервисов
var tcpService = app.Services.GetRequiredService<TcpService>();
var udpService = app.Services.GetRequiredService<UdpService>();

// Создаем CancellationTokenSource для корректного завершения работы
var cancellationTokenSource = new CancellationTokenSource();
app.Lifetime.ApplicationStopping.Register(() => cancellationTokenSource.Cancel());

// Запускаем серверы TCP и UDP в фоновом режиме
_ = tcpService.StartServerAsync(cancellationTokenSource.Token); // Задача запускается асинхронно
_ = udpService.StartServerAsync(cancellationTokenSource.Token); // Задача запускается асинхронно

app.Run();





