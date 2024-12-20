using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChatApp.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace ChatApp.Services
{
    public class UdpService
    {
        private readonly string _ipAddress;
        private readonly int _port;
        private UdpClient _udpClient;
        private readonly AppSettings _settings;
        private readonly ILogger<UdpService> _logger;

        public UdpService(IOptions<AppSettings> options, ILogger<UdpService> logger)
        {
            _settings = options.Value;
            _ipAddress = _settings.IpAddress;
            _port = _settings.UdpPort;
            _logger = logger;
        }

        public async Task StartServerAsync(CancellationToken cancellationToken)
        {
            try
            {
                _udpClient = new UdpClient(_port);
                _logger.LogInformation($"UDP Server started on {_ipAddress}:{_port}");

                while (!cancellationToken.IsCancellationRequested)
                {
                    var receivedResult = await _udpClient.ReceiveAsync();
                    var receivedMessage = Encoding.UTF8.GetString(receivedResult.Buffer);
                    _logger.LogInformation($"Received UDP message: {receivedMessage}");

                    // Отправляем подтверждение обратно
                    var responseMessage = Encoding.UTF8.GetBytes("Message received via UDP.");
                    await _udpClient.SendAsync(responseMessage, responseMessage.Length, receivedResult.RemoteEndPoint);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in UDP server: {ex.Message}");
            }
            finally
            {
                _udpClient.Close();
                _logger.LogInformation("UDP Server stopped.");
            }
        }

        public async Task SendMessageAsync(string message)
        {
            var endpoint = new IPEndPoint(IPAddress.Parse(_ipAddress), _port);
            var messageBytes = Encoding.UTF8.GetBytes(message);
            await _udpClient.SendAsync(messageBytes, messageBytes.Length, endpoint);
        }
    }
}






