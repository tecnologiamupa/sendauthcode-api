using MicroserviceWhatsapp.Data.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroserviceWhatsapp.Data.Response
{
    public class ResponseToken
    {
        public string AccessToken { get; set; }
        public RequestUserLogin UserInfo { get; set; }
    }
}
