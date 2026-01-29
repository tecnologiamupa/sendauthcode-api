using MicroserviceWhatsapp.Application.Template;
using MicroserviceWhatsapp.Data.Request;
using MicroserviceWhatsapp.Data.Response;

namespace MicroserviceWhatsapp.Application.Interface
{
    public interface ISendMessage
    {
        public  Task<ResponseMSG<string>>  SendCodeUser(RequestSendMessage requestSend); 
        public Task<ResponseMSG<ResponseToken>>  VerifiedIdentityId(RequestValidIdentityId _RequestValidIdentityId);

        public Task<ResponseMSG<ResponseToken>> ValidCodSendWS(RequestCode requestSend,string token);

    }
}
