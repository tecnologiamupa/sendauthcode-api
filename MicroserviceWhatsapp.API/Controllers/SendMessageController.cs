using MicroserviceWhatsapp.Application.Interface;
using MicroserviceWhatsapp.Application.Middleware;
using MicroserviceWhatsapp.Data.Request;
using MicroserviceWhatsapp.Data.Response;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace MicroserviceWhatsapp.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SendMessageController : ControllerBase
    {
        private readonly ISendMessage _sendMessage;

        public SendMessageController(ISendMessage sendMessage)
        {
            _sendMessage = sendMessage;
        }

        [HttpPost("ValidIdentityId")]
        public async Task<ActionResult<ResponseMSG<ResponseToken>>> ValidIdentityId(RequestSendMessage sendMessage)
        {
            var requestValidIdentityId = new RequestValidIdentityId
            {
                IdentityId = sendMessage.IdentityId
            };

            var resumen = await _sendMessage.VerifiedIdentityId(requestValidIdentityId);
            return StatusCode(resumen.StatusCode, resumen);
        }
        [HttpPost("ValidCodSendWS")]
        public async Task<ActionResult<ResponseMSG<ResponseToken>>> ValidCodSendWS([FromBody] RequestCode requestCode, [FromHeader(Name = "Authorization")] string token)
        {
            var resumen = await _sendMessage.ValidCodSendWS(requestCode, token);
            return StatusCode(resumen.StatusCode, resumen);
        }



    }
}
