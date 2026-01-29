using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroserviceWhatsapp.Data.Response
{
    public class ResponseMSG<T>
    {
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public string Code { get; set; }
        public T data { get; set; }
    }
}
