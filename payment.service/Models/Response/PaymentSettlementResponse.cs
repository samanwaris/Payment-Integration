namespace payment_service.Models.Response
{
    public class PaymentSettlementResponse
    {
        public string ChannelId { get; set; }
        public string Currency { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMessage { get; set; }
        public string FlagType { get; set; }
        public string PaymentReffId { get; set; }
    }
}
