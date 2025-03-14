using ECommerce.DTO;
using ECommerce.Service.Emails;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    public class MailController : ApiBaseController
    {
        private readonly IMailServices _mailServices;

        public MailController(IMailServices mailServices)
        {
            _mailServices = mailServices;
        }

        [HttpPost("send")]
        public async Task<ActionResult> SendEmail([FromBody] MailRequestDTO mailRequest)
        {
            try
            {
                await _mailServices.SendEmailAsync(mailRequest.ToEmail, mailRequest.Subject, mailRequest.Body, mailRequest.Files);
                return Ok();
            }
            catch (Exception ex)
            { 
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
