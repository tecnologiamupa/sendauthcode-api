using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroserviceWhatsapp.Data.Response
{
    public class ResponseAPIWhatsApp
    {
        public string MessagingProduct { get; set; }
        public List<Contact> Contacts { get; set; }
        public List<Message> Messages { get; set; }
    }

    public class Contact
    {
        public string Input { get; set; }
        public string WaId { get; set; }
    }

    public class Message
    {
        public string Id { get; set; }
        public string MessageStatus { get; set; }
    }
}
