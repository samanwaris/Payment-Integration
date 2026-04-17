namespace payment_service.Models.Request
{
    public class SettlementRequest
    {
        public class NonSnapSettlement
        {
            public string ChannelId { get; set; }
            public string Currency { get; set; }
            public string TransactionNo { get; set; }
            public string TransactionAmount { get; set; }
            public string TransactionFee { get; set; }
            public string TransactionDate { get; set; }
            public string TransactionStatus { get; set; }
            public string TransactionMessage { get; set; }
            public string ChannelType { get; set; }
            public string FlagType { get; set; }
            public string InsertId { get; set; }
            public string CustomerAccount { get; set; }
            public string PaymentReffId { get; set; }
            public string AdditionalData { get; set; }
            public string AuthCode { get; set; }
        }

        public class SnapSettlement
        {
            public string PartnerServiceId { get; set; }
            public string CustomerNo { get; set; }
            public string VirtualAccountNo { get; set; }
            public string VirtualAccountName { get; set; }
            public string ChannelCode { get; set; }
            public string PaymentRequestId { get; set; }
            public string TrxDateTime { get; set; }
            public string TrxId { get; set; }
            public PaidAmount PaidAmount { get; set; }
            public string ReferenceNo { get; set; }
            public string FlagAdvise { get; set; }
            public AdditionalInfo AdditionalInfo { get; set; }
        }

        public class PaidAmount
        {
            public string Value { get; set; }
            public string Currency { get; set; }
        }

        public class AdditionalInfo
        {
            public string InsertId { get; set; }
            public string TagId { get; set; }
            public string FlagType { get; set; }
        }
    }
}
