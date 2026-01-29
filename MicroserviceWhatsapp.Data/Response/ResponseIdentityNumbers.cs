using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroserviceWhatsapp.Data.Response
{
    public class ResponseIdentityNumbers
    {
        public int StatusCode { get; set; }
        public ResponseIdentityNumbersData Data { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }

    public class ResponseIdentityNumbersData
    {
        public Guid Id { get; set; }
        public string? ProfilePhotoUrl { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? IdentityNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public string? Title { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }

        public string? phoneCountryCode { get; set; }
    }
}
