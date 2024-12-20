using ChatApp.Models;
using ChatApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{
    [Route("api/chat")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly TcpService _tcpService;
        private readonly UdpService _udpService;

        public ChatController(TcpService tcpService, UdpService udpService)
        {
            _tcpService = tcpService;
            _udpService = udpService;
        }

        [HttpPost("sendTcpMessage")]
        public async Task<IActionResult> SendTcpMessage([FromBody] string message)
        {
            await _tcpService.SendMessageAsync(message);
            return Ok($"TCP Message sent: {message}");
        }

        [HttpPost("sendUdpMessage")]
        public async Task<IActionResult> SendUdpMessage([FromBody] string message)
        {
            await _udpService.SendMessageAsync(message);
            return Ok($"UDP Message sent: {message}");
        }
    }
}












