using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace payment_service.Models.Response
{
    public class CreatePaymentResponse
    {
        public class NonSnap
        {
            public string ChannelId { get; set; }
            public string Currency { get; set; }
            public string InsertStatus { get; set; }

            [JsonConverter(typeof(StringOrArrayConverter))]
            public List<string> InsertMessage { get; set; }
            public string InsertId { get; set; }
            public string AdditionalData { get; set; }
            public string UrlQris { get; set; }
            public string QrisText { get; set; }
            public string RedirectURL { get; set; }
            public string CustomerAccount { get; set; }
        }

        public class Snap
        {
            public string ResponseCode { get; set; }
            public string ResponseMessage { get; set; }
            public VirtualAccountData VirtualAccountData { get; set; }
        }

        public class VirtualAccountData
        {
            public string PartnerServiceId { get; set; }
            public string CustomerNo { get; set; }
            public string VirtualAccountNo { get; set; }
            public string VirtualAccountName { get; set; }
            public string VirtualAccountEmail { get; set; }
            public string PaymentRequestId { get; set; }
            public PaidAmount PaidAmount { get; set; }
            public PaymentFlagReason PaymentFlagReason { get; set; }
            public string PaymentFlagStatus { get; set; }

            public string TrxId { get; set; }
            public TotalAmount TotalAmount { get; set; }


            public List<BillDetail> BillDetails { get; set; }

            public DateTime ExpiredDate { get; set; }
        }

        public class PaidAmount
        {
            public string Value { get; set; }
            public string Currency { get; set; }
        }

        public class PaymentFlagReason
        {
            public string English { get; set; }
            public string Indonesia { get; set; }
        }

        public class TotalAmount
        {
            public string Value { get; set; }
            public string Currency { get; set; }
        }

        public class BillDetail
        {
            public BillDescription BillDescription { get; set; }
        }
        public class BillDescription
        {
            public string English { get; set; }
            public string Indonesia { get; set; }
        }
    }

    public class StringOrArrayConverter : JsonConverter<List<string>>
    {
        public override List<string> ReadJson(JsonReader reader, Type objectType, List<string> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);

            if (token.Type == JTokenType.Array)
                return token.ToObject<List<string>>();

            return new List<string> { token.ToString() };
        }

        public override void WriteJson(JsonWriter writer, List<string> value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
