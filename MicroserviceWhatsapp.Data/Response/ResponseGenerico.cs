using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroserviceWhatsapp.Data.Response
{
    public class ResponseGenerico<T>
    {
        public List<T>? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public int CodError { get; set; }
        public int Cantidad { get; set; } = 0;
        public int TotalRecord { get; set; } = 0;
        public int TotalPage { get; set; } = 0;
    }
}
