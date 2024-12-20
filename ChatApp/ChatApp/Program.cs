using ChatApp.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ChatApp.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// �������� ������������ �� ����� appsettings.json
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// ��������� ������� � ���������
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ��������� ���� ������� (��������, TCP � UDP)
builder.Services.AddSingleton<TcpService>();
builder.Services.AddSingleton<UdpService>();

var app = builder.Build();

// ������������ ��� Swagger � �������������
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// �������� ���������� ��������
var tcpService = app.Services.GetRequiredService<TcpService>();
var udpService = app.Services.GetRequiredService<UdpService>();

// ������� CancellationTokenSource ��� ����������� ���������� ������
var cancellationTokenSource = new CancellationTokenSource();
app.Lifetime.ApplicationStopping.Register(() => cancellationTokenSource.Cancel());

// ��������� ������� TCP � UDP � ������� ������
_ = tcpService.StartServerAsync(cancellationTokenSource.Token); // ������ ����������� ����������
_ = udpService.StartServerAsync(cancellationTokenSource.Token); // ������ ����������� ����������

app.Run();





