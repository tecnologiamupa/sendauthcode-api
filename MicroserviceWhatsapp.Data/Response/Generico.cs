using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroserviceWhatsapp.Data.Response
{
    public class Generico
    {
        public int CodError { get; set; } = 0;
        public string Message { get; set; } = string.Empty;
    }
}
