namespace payment_service.Models.Request
{
    public class CreatePaymentRequest
    {
        public class Snap
        {
            public string XChannelId { get; set; }
            public string XPartnerId { get; set; }
            public string PartnerServiceId { get; set; }
            public string CustomerNo { get; set; }
            public string VirtualAccountNo { get; set; }
            public string VirtualAccountName { get; set; }
            public string VirtualAccountEmail { get; set; }
            public string TrxId { get; set; }
            public TotalAmount TotalAmount { get; set; }
            public List<BillDetail> BillDetails { get; set; }
            public string ExpiredDate { get; set; }
            public Dictionary<string, object> AdditionalInfo { get; set; }
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

        public class NonSnap
        {
            public string ChanelId { get; set; }
            public string ServiceCode { get; set; }
            public string PartnerServiceId { get; set; }
            public string SubBankId { get; set; }
            public string CustomerNo { get; set; }
            public string Type { get; set; }
            public string Currency { get; set; }
            public string CallbackUrl { get; set; }
            public string TransactionNo { get; set; }
            public decimal TransactionAmount { get; set; }
            public DateTime TransactionDate { get; set; }
            public DateTime TransactionExpire { get; set; }
            public string Description { get; set; }
            public string CustomerAccount { get; set; }
            public string CustomerName { get; set; }
            public string CustomerEmail { get; set; }
            public string CustomerPhone { get; set; }
            public DateTime VirtualAccountExpiry { get; set; }
        }
    }
}
