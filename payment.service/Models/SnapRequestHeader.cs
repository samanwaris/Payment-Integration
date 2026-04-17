using Microsoft.AspNetCore.Mvc;

namespace payment_service.Models
{
    public class SnapRequestHeader
    {
        [FromHeader(Name = "X-SIGNATURE")]
        public string Signature { get; set; }

        [FromHeader(Name = "X-TIMESTAMP")]
        //[Required]
        public string Timestamp { get; set; }

        [FromHeader(Name = "X-PARTNER-ID")]
        //[Required]
        public string PartnerId { get; set; }

        [FromHeader(Name = "X-EXTERNAL-ID")]
        //[Required]
        public string ExternalId { get; set; }

        [FromHeader(Name = "CHANNEL-ID")]
        //[Required]
        public string ChanelId { get; set; }
    }
}
