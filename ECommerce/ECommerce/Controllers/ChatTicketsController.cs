using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ECommerce.Core;
using ECommerce.Core.Models;
using ECommerce.Errors;
using ECommerce.DTO.Request;

namespace ECommerce.Controllers
{
    [Authorize("Admin")]
    public class ChatTicketsController : ApiBaseController
    {
        private readonly IUnitWork _unitWork;

        public ChatTicketsController(IUnitWork unitWork)
        {
            _unitWork = unitWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChatTicket>>> GetChatTickets()
            => Ok(await _unitWork.Repo<ChatTicket>().GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<ChatTicket>> GetChatTicket(int id)
        {
            if (id <= 0) return BadRequest(new ApiResponse(400));

            var chatTicket = await _unitWork.Repo<ChatTicket>().GetByIdAsync(id);
            if (chatTicket == null)
                return NotFound(new ApiResponse(404));

            return Ok(chatTicket);
        }

        [HttpPost]
        public async Task<ActionResult> PostChatTicket([FromBody] TicketRequest request)
        {
            var chatTicket = new ChatTicket
            {
                TicketNumber = request.TicketNumber,
                Topic = request.Topic,
                Description = request.Description,
                ChatId = request.ChatId,
                CreatedAt = DateTimeOffset.Now
            };

            try
            {
                await _unitWork.Repo<ChatTicket>().AddAsync(chatTicket);
                await _unitWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                await _unitWork.DisposeAsync();
                return BadRequest(new ApiResponse(500));
            }
            return Ok(new ApiResponse(200, "Succeed \t :)"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChatTicket(int id)
        {
            var chatTicket = await _unitWork.Repo<ChatTicket>().GetByIdAsync(id);
            if (chatTicket == null)
                return NotFound(new ApiResponse(404));

            try
            {
                _unitWork.Repo<ChatTicket>().Delete(chatTicket);
                await _unitWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                await _unitWork.DisposeAsync();
                return BadRequest(new ApiResponse(500));
            }
            return NoContent();
        }
    }
}
