namespace payment_service.Models
{
    public class PaymentSettings
    {
        public string SecretKey { get; set; }
        public BaseUrlConfig BaseUrl { get; set; }
        public PaymnentEndpoints Endpoints { get; set; }
    }

    public class BaseUrlConfig
    {
        public string SnapUrl { get; set; }
        public string NoSnapUrl { get; set; }
    }

    public class PaymnentEndpoints
    {
        public string Register { get; set; }
        public string CreateVA { get; set; }
        public string Payment { get; set; }
    }
}
