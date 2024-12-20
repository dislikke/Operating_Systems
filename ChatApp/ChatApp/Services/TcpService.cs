using ChatApp.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatApp.Services
{
    public class TcpService
    {
        private readonly ILogger<TcpService> _logger;
        private readonly string _ipAddress;
        private readonly int _port;
        private TcpListener _tcpListener;
        private readonly AppSettings _settings;

        public TcpService(IOptions<AppSettings> options, ILogger<TcpService> logger)
        {
            _logger = logger;
            _settings = options.Value;
            _ipAddress = _settings.IpAddress;
            _port = _settings.TcpPort;
        }

        public async Task StartServerAsync(CancellationToken cancellationToken)
        {
            try
            {
                _tcpListener = new TcpListener(IPAddress.Parse(_ipAddress), _port);
                _tcpListener.Start();
                _logger.LogInformation($"TCP Server started on {_ipAddress}:{_port}");

                while (!cancellationToken.IsCancellationRequested)
                {
                    // Ожидаем подключения клиентов
                    if (_tcpListener.Pending())
                    {
                        var tcpClient = await _tcpListener.AcceptTcpClientAsync();
                        _ = HandleClientAsync(tcpClient, cancellationToken); // Обрабатываем клиента асинхронно
                    }
                    await Task.Delay(100); // Это необходимо для освобождения потока
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in TCP server: {ex.Message}");
            }
            finally
            {
                _tcpListener.Stop();
                _logger.LogInformation("TCP Server stopped.");
            }
        }

        private async Task HandleClientAsync(TcpClient tcpClient, CancellationToken cancellationToken)
        {
            try
            {
                using (var networkStream = tcpClient.GetStream())
                {
                    var buffer = new byte[1024];
                    int bytesRead;
                    while ((bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) != 0)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        _logger.LogInformation($"Received TCP message: {message}");

                        var response = Encoding.UTF8.GetBytes("Message received.");
                        await networkStream.WriteAsync(response, 0, response.Length, cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error handling client: {ex.Message}");
            }
        }

        public async Task SendMessageAsync(string message)
        {
            using (var tcpClient = new TcpClient(_ipAddress, _port))
            {
                var networkStream = tcpClient.GetStream();
                var messageBytes = Encoding.UTF8.GetBytes(message);
                await networkStream.WriteAsync(messageBytes, 0, messageBytes.Length);
            }
        }
    }
}









