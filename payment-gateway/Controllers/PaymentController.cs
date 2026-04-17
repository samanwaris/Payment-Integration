using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using payment_gateway.Attributes;
using payment_service.Interfaces;
using payment_service.Models;
using payment_service.Models.Request;
using System.Text;

namespace payment_gateway.Controllers
{
    [Route("api")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ICreatePaymentService _paymentRegisterService;

        public PaymentController(ICreatePaymentService paymentRegisterService)
        {
            _paymentRegisterService = paymentRegisterService;
        }

        [HttpPost("payment/create-nonsnap")]
        [AllowAnonymous]
        public async Task<IActionResult> CreatePaymentNonSnap(CreatePaymentRequest.NonSnap request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _paymentRegisterService.CreatePaymentNonSnapAsync(request);

            return result.Success ?
                Ok(result) :
                BadRequest(result);
        }

        [HttpPost("payment/settlement-nonsnap")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentSettlementNonSnap(SettlementRequest.NonSnapSettlement request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _paymentRegisterService.PaymentSettlementAsync(request);

            return result.Success ?
                Ok(result) :
                BadRequest(result);
        }

        [HttpPost("payment/create-snap")]
        [AllowAnonymous]
        public async Task<IActionResult> CreatePaymentSnap(CreatePaymentRequest.Snap request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var headers = Request.Headers;
            var snapHeader = new SnapRequestHeader
            {
                Signature = headers["X-SIGNATURE"].FirstOrDefault(),
                Timestamp = headers["X-TIMESTAMP"].FirstOrDefault(),
                PartnerId = headers["X-PARTNER-ID"].FirstOrDefault(),
                ExternalId = headers["X-EXTERNAL-ID"].FirstOrDefault(),
                ChanelId = headers["CHANNEL-ID"].FirstOrDefault(),
            };

            var result = await _paymentRegisterService.CreatePaymentSnapAsync(request, snapHeader);

            return result.Success ?
                Ok(result) :
                BadRequest(result);
        }

        [SnapHeader]
        [HttpPost("payment/settlement-snap")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentSettlementSnap([FromBody] SettlementRequest.SnapSettlement request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            HttpContext.Request.EnableBuffering();

            var headers = Request.Headers;
            var snapHeader = new SnapRequestHeader
            {
                Signature = headers["X-SIGNATURE"].FirstOrDefault(),
                Timestamp = headers["X-TIMESTAMP"].FirstOrDefault(),
                PartnerId = headers["X-PARTNER-ID"].FirstOrDefault(),
                ExternalId = headers["X-EXTERNAL-ID"].FirstOrDefault(),
                ChanelId = headers["CHANNEL-ID"].FirstOrDefault(),
            };

            //This is for generate stringToSign and generate signature
            string body;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true))
            {
                body = await reader.ReadToEndAsync();
                Request.Body.Position = 0;
            }

            string httpMethod = Request.Method;

            var result = await _paymentRegisterService.PaymentSettlementSnapAsync(snapHeader, body, httpMethod);
            Response.Headers["X-TIMESTAMP"] = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");

            return result.Success
            ? Ok(result)
            : BadRequest(result);
        }
    }
}
