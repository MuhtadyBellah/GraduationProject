//using ECommerce.Core.Services;
//using ECommerce.DTO;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Stripe;

//namespace ECommerce.Controllers
//{
//    public class PaymentController : ApiBaseController
//    {
//        private readonly IPayment _payment;
//        const string endpointSecret = "whsec_czhCCsLvmUIlUHWHR92omt0zq0dQgqOO";

//        public PaymentController(IPayment payment)
//        {
//            this._payment = payment;
//        }

//        [Authorize]
//        [HttpPost("{basketId}")]
//        public async Task<ActionResult<CustomerBasketDTO>> CreateOrUpdate(string basketId)
//        {
//            var basket = await _payment.CreatePaymentIntent(basketId);
//            return (basket is null) ? BadRequest("Problem with creating payment intent") : Ok(basket);
//        }

//        [HttpPost("webhook")]
//        public async Task<IActionResult> Index()
//        {
//            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
//            try
//            {
//                var stripeEvent = EventUtility.ParseEvent(json);
//                var signatureHeader = Request.Headers["Stripe-Signature"];

//                stripeEvent = EventUtility.ConstructEvent(json,
//                        signatureHeader, endpointSecret);
                
//                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
//                if (paymentIntent == null) return BadRequest("Invalid PaymentIntent object");
//                // If on SDK version < 46, use class Events instead of EventTypes
//                if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
//                {
//                    await _payment.PaymentSucceedOrFailed(paymentIntent.Id, true);
//                }
//                else if (stripeEvent.Type == EventTypes.PaymentIntentPaymentFailed)
//                {
//                    await _payment.PaymentSucceedOrFailed(paymentIntent.Id, false);
//                }
//                else
//                {
//                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
//                }
//                return Ok();
//            }
//            catch (StripeException e)
//            {
//                Console.WriteLine("Error: {0}", e.Message);
//                return BadRequest();
//            }
//            catch (Exception e)
//            {
//                return StatusCode(500);
//            }
//        }
//    }
//}

