using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using payment_service.Helper;
using payment_service.Interfaces;
using payment_service.Models;
using payment_service.Models.Request;
using payment_service.Models.Response;
using System.Text;
using System.Text.RegularExpressions;
using static payment_service.Models.Response.CreatePaymentResponse;

namespace payment_service.Services
{
    public class CreatePaymentService : ICreatePaymentService
    {
        private readonly PaymentSettings _paymentSettings;

        public CreatePaymentService(IOptions<PaymentSettings> paymentSettings)
        {
            _paymentSettings = paymentSettings.Value;
        }

        public async Task<ApiResponse<CreatePaymentResponse.NonSnap>> CreatePaymentNonSnapAsync(CreatePaymentRequest.NonSnap reqBody)
        {
            try
            {
                string _secretKey = _paymentSettings.SecretKey;
                string url = _paymentSettings.BaseUrl.NoSnapUrl + _paymentSettings.Endpoints.Register;
                CreatePaymentResponse.NonSnap nonSnapResponse = new CreatePaymentResponse.NonSnap();

                var authCode = SignatureHelper.HashSha256(reqBody.TransactionNo + reqBody.TransactionAmount.ToString("0") + reqBody.ChanelId + _secretKey);
                using (HttpClient client = new HttpClient())
                {
                    var formData = new Dictionary<string, string>
                {
                    { "channelId", reqBody.ChanelId },
                    { "serviceCode", reqBody.ServiceCode },
                    { "currency", reqBody.Currency },
                    { "callbackURL", reqBody.CallbackUrl },
                    { "transactionNo", reqBody.TransactionNo },
                    { "transactionAmount", reqBody.TransactionAmount.ToString("0") },
                    { "transactionDate", reqBody.TransactionDate.ToString("yyyy-MM-dd HH:mm:ss") },
                    { "transactionExpire", reqBody.TransactionExpire.ToString("yyyy-MM-dd HH:mm:ss") },
                    { "description", reqBody.Description },
                    { "customerAccount", reqBody.CustomerAccount },
                    { "customerName", reqBody.CustomerName },
                    { "customerEmail", reqBody.CustomerEmail },
                    { "customerPhone", reqBody.CustomerPhone },
                    { "authCode", authCode }
                };
                    var content = new FormUrlEncodedContent(formData);
                    var response = await client.PostAsync(url, content);
                    if (!response.IsSuccessStatusCode)
                        return ApiResponse<CreatePaymentResponse.NonSnap>.Result(false, "No response from payment gateway 3rd party", nonSnapResponse);

                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    var match = Regex.Match(jsonString, @"\{.*\}", RegexOptions.Singleline);
                    nonSnapResponse = JsonConvert.DeserializeObject<CreatePaymentResponse.NonSnap>(match.Value);
                    new LoggingHelper().AppendTextFunction("RegisterPaymentNonSnapAsync", match.Value);
                    if (nonSnapResponse == null)
                        return ApiResponse<CreatePaymentResponse.NonSnap>.Result(false, "Response data is empty", nonSnapResponse);
                    if (nonSnapResponse.InsertStatus != "00") // 00 is success status from payment gateway
                        return ApiResponse<CreatePaymentResponse.NonSnap>.Result(false, "Invalid insert status", nonSnapResponse);
                }

                return ApiResponse<CreatePaymentResponse.NonSnap>.Result(true, "Success", nonSnapResponse);
            }
            catch (Exception ex)
            {
                new LoggingHelper().AppendTextFunction("CreatePaymentNonSnapAsync", ex.Message);
                return ApiResponse<CreatePaymentResponse.NonSnap>.Fail(ex.Message);
            }
        }

        public async Task<ApiResponse<CreatePaymentResponse.Snap>> CreatePaymentSnapAsync(CreatePaymentRequest.Snap reqBody, SnapRequestHeader SnapHeader)
        {
            try
            {
                var url = _paymentSettings.BaseUrl.SnapUrl + _paymentSettings.Endpoints.CreateVA;
                string relativeUrl = _paymentSettings.Endpoints.CreateVA;
                CreatePaymentResponse.Snap snapResponse = new CreatePaymentResponse.Snap();

                var rsa = SignatureHelper.LoadPrivateKey(@"your private key"); // SHA256 private key

                string xTimeStamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
                string externalId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                string minifyRequest = JsonConvert.SerializeObject(reqBody, Formatting.None);
                string stringToSign = SignatureHelper.StringToSign("POST", relativeUrl, minifyRequest, xTimeStamp);
                string signature = SignatureHelper.GenerateSignature(stringToSign, rsa);
                string jsonBody = JsonConvert.SerializeObject(reqBody, Formatting.None); //minify body

                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("X-TIMESTAMP", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                client.DefaultRequestHeaders.Add("X-SIGNATURE", signature);
                client.DefaultRequestHeaders.Add("X-PARTNER-ID", reqBody.XPartnerId);
                client.DefaultRequestHeaders.Add("X-EXTERNAL-ID", externalId);
                client.DefaultRequestHeaders.Add("CHANNEL-ID", reqBody.XChannelId);

                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                    return ApiResponse<CreatePaymentResponse.Snap>.Result(false, "No response from payment gateway 3rd party", snapResponse);

                var jsonString = response.Content.ReadAsStringAsync().Result;
                var match = Regex.Match(jsonString, @"\{.*\}", RegexOptions.Singleline);
                new LoggingHelper().AppendTextFunction("RegisterPaymentSnapAsync-Response", match.Value);
                snapResponse = JsonConvert.DeserializeObject<CreatePaymentResponse.Snap>(match.Value);
                if (snapResponse == null)
                    return ApiResponse<CreatePaymentResponse.Snap>.Result(false, "No response data from payment gateway", snapResponse);

                if (snapResponse.ResponseCode != "2002700")
                    return ApiResponse<CreatePaymentResponse.Snap>.Result(false, "Invalid status response code", snapResponse);

                return ApiResponse<CreatePaymentResponse.Snap>.Result(true, "Success", snapResponse);
            }
            catch (Exception ex)
            {
                new LoggingHelper().AppendTextFunction("CreatePaymentSnapAsync", ex.Message);
                return ApiResponse<CreatePaymentResponse.Snap>.Fail(ex.Message);
            }
        }

        public async Task<ApiResponse<PaymentSettlementResponse>> PaymentSettlementAsync(SettlementRequest.NonSnapSettlement reqBody)
        {
            string _secretKey = _paymentSettings.SecretKey;
            string externalId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

            //standard response base on doc from payment gateway service
            var response = new PaymentSettlementResponse
            {
                ChannelId = reqBody.ChannelId,
                Currency = reqBody.Currency,
                FlagType = reqBody.FlagType,
                PaymentReffId = reqBody.PaymentReffId,
            };

            try
            {
                string data = reqBody.TransactionNo + reqBody.TransactionAmount + reqBody.ChannelId + reqBody.TransactionStatus + reqBody.InsertId + _secretKey;
                var generatedCode = SignatureHelper.HashSha256(data);

                if (generatedCode != reqBody.AuthCode)
                {
                    response.PaymentMessage = "Invalid AuthCode";
                    response.PaymentStatus = "01";
                    return ApiResponse<PaymentSettlementResponse>.Result(false, "Failed Payment", response);
                }

                if (reqBody.Currency != "IDR")
                {
                    response.PaymentMessage = "Invalid Currency";
                    response.PaymentStatus = "01";
                    return ApiResponse<PaymentSettlementResponse>.Result(false, "Failed Payment", response);
                }

                if (reqBody.TransactionStatus != "00") //00 is success status from payment gateway
                {
                    response.PaymentMessage = "Invalid Transaction Status";
                    response.PaymentStatus = "01";
                    return ApiResponse<PaymentSettlementResponse>.Result(false, "Failed Payment", response);
                }

                if (SignatureHelper.IsValidAmount(reqBody.TransactionAmount))
                {
                    response.PaymentMessage = "Invalid Field Format {TransactionAmount}";
                    response.PaymentStatus = "01";
                    return ApiResponse<PaymentSettlementResponse>.Result(false, "Invalid Field Format {paidAmount.Amount}", response);
                }


                response.PaymentStatus = "00";
                return ApiResponse<PaymentSettlementResponse>.Result(true, "Success", response);
            }
            catch (Exception ex)
            {
                new LoggingHelper().AppendTextFunction("CreatePaymentSnapAsync", ex.Message);
                return ApiResponse<PaymentSettlementResponse>.Fail(ex.Message);
            }

        }

        public async Task<ApiResponse<VirtualAccountData>> PaymentSettlementSnapAsync(SnapRequestHeader header, string rawBody, string method)
        {
            string endpoint = _paymentSettings.Endpoints.Payment;
            try
            {
                if (string.IsNullOrEmpty(header.Signature))
                    return ApiResponse<VirtualAccountData>.Result(false, "Invalid Mandatory Field X-SIGNATURE", null);
                if (string.IsNullOrEmpty(header.PartnerId))
                    return ApiResponse<VirtualAccountData>.Result(false, "Invalid Mandatory Field X-PARTNER-ID", null);
                if (string.IsNullOrEmpty(header.ExternalId))
                    return ApiResponse<VirtualAccountData>.Result(false, "Invalid Mandatory Field X-EXTERNAL-ID", null);
                if (string.IsNullOrEmpty(header.ChanelId))
                    return ApiResponse<VirtualAccountData>.Result(false, "Invalid Mandatory Field CHANNEL-ID", null);

                string requestHeader = JsonConvert.SerializeObject(header);
                var minifyRequest = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(rawBody), Formatting.None);
                new LoggingHelper().AppendTextFunction("PaymentSettlementSnapAsync", $"minifyRequest {minifyRequest}");

                string bodyHash = SignatureHelper.HashSha256(minifyRequest).ToLower();
                var rsaPublic = SignatureHelper.LoadPublicKey(@"your public key"); //SHA256 public Key
                string stringToSign = SignatureHelper.StringToSign(method, endpoint, minifyRequest, header.Timestamp);
                new LoggingHelper().AppendTextFunction("PaymentSettlementSnapAsync", $"stringToSign {stringToSign}");

                var isValid = SignatureHelper.VerifySignature(stringToSign, header.Signature, rsaPublic);
                if (!isValid)
                    return ApiResponse<VirtualAccountData>.Result(false, "Unauthorized. [verify sign failed]", null);

                var data = JsonConvert.DeserializeObject<SettlementRequest.SnapSettlement>(minifyRequest);
                if (data == null)
                    return ApiResponse<VirtualAccountData>.Result(false, "Bad Request", null);

                if (data.PartnerServiceId == "")
                    return ApiResponse<VirtualAccountData>.Result(false, "Missing Mandatory Field partnerServiceId", null);

                if (SignatureHelper.IsValidAmount(data.PaidAmount.Value))
                    return ApiResponse<VirtualAccountData>.Result(false, "Invalid Field Format {paidAmount.Amount}", null);

                var responseBody = new VirtualAccountData
                {
                    PartnerServiceId = header.PartnerId,
                    CustomerNo = data.CustomerNo,
                    VirtualAccountNo = data.VirtualAccountNo,
                    VirtualAccountName = data.VirtualAccountName,
                    PaymentRequestId = data.PaymentRequestId,
                    PaidAmount = new PaidAmount
                    {
                        Value = data.PaidAmount.Value.ToString(),
                        Currency = "IDR"
                    },
                    PaymentFlagReason = new PaymentFlagReason
                    {
                        English = "Success",
                        Indonesia = "Sukses"
                    },
                    PaymentFlagStatus = "00" //suksess status
                };

                return ApiResponse<VirtualAccountData>.Result(true, "Success", responseBody);
            }
            catch (Exception ex)
            {
                new LoggingHelper().CreateFullLog(ex, "PaymentSettlementSnapAsync");
                return ApiResponse<VirtualAccountData>.Fail(ex.Message);
            }
        }
    }
}
