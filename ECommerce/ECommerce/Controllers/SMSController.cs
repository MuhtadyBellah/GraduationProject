//using ECommerce.DTO;
//using ECommerce.Errors;
//using ECommerce.Service.SMS;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Twilio.Rest.Api.V2010.Account;

//namespace ECommerce.Controllers
//{
//    public class SMSController : ApiBaseController
//    {
//        private readonly ISMSServices _sMSServices;

//        public SMSController(ISMSServices SMSServices)
//        {
//            _sMSServices = SMSServices;
//        }

//        [HttpPost("send")]
//        [ProducesResponseType(typeof(MessageResource), 200)]
//        [ProducesResponseType(typeof(ApiResponse), 500)]
//        public IActionResult SendSMS(SMSRequestDTO requestDTO)
//        {

//            if (requestDTO == null || string.IsNullOrWhiteSpace(requestDTO.Phone) || string.IsNullOrWhiteSpace(requestDTO.Body))
//                return BadRequest(new ApiResponse(400, "Invalid request. Phone and Body are required."));

//            try
//            {
//                var twilioResponse = _sMSServices.Send(requestDTO.Phone, requestDTO.Body);
//                return Ok(twilioResponse);
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(new ApiResponse(500, ex.Message));
//            }
//        }
//    }
//}
