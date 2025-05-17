using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ECommerce.Core;
using ECommerce.Core.Models.Laravel;
using ECommerce.Core.Models;
using ECommerce.Errors;
using ECommerce.Core.Specifications;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using ECommerce.DTO.Request;

namespace ECommerce.Controllers
{
    [Authorize("Sanctum")]
    public class ChatsController : ApiBaseController
    {
        private readonly IUnitWork _unitWork;

        public ChatsController(IUnitWork context)
        {
            _unitWork = context;
        }


        /*
        // GET: api/Chats
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Chat>>> GetChats()
        {
            return await _unitWork.Chats.ToListAsync();
        }

        // GET: api/Chats/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Chat>> GetChat(int id)
        {
            var chat = await _unitWork.Chats.FindAsync(id);

            if (chat == null)
            {
                return NotFound();
            }

            return chat;
        }
        */

        public record ChatResponse(int chatId, string category);
        [HttpPost("{category}")]
        public async Task<ActionResult<ChatResponse>> PostChat(string category)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            if (userRole == null) return Unauthorized(new ApiResponse(401));

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Chat? chat;
            if (userRole == "user")
            {
                chat = new Chat
                {
                    Category = category,
                    CustomerId = int.Parse(userId),
                    Status = StatusOptions.Pending,
                    StartDate = DateTimeOffset.UtcNow
                };

                try
                {
                    await _unitWork.Repo<Chat>().AddAsync(chat);
                    await _unitWork.CompleteAsync();
                }
                catch (Exception ex)
                {
                    await _unitWork.DisposeAsync();
                    return BadRequest(new ApiResponse(400, ex.Message));
                }
            }
            else
            {
                var spec = new ChatSpec(StatusOptions.Pending);
                chat = await _unitWork.Repo<Chat>().GetByIdAsync(spec);
                if (chat == null) return NotFound(new ApiResponse(404, "There is no Pending Customer Chats"));

                chat.Status = StatusOptions.Active;
                chat.AgentId = int.Parse(userId);

                _unitWork.Repo<Chat>().Update(chat);
                await _unitWork.CompleteAsync();

                category = chat.Category;
            }
            return Ok(new ChatResponse(chat.Id, category));
        }

        [HttpPut("{chatId}")]
        public async Task<ActionResult> PutChat(int chatId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized(new ApiResponse(401));

            var spec = new ChatSpec(chatId);
            var chat = await _unitWork.Repo<Chat>().GetByIdAsync(spec);
            if (chat == null) return BadRequest(new ApiResponse(400, "Chat not found"));

            chat.Status = StatusOptions.Closed;
            chat.EndDate = DateTimeOffset.UtcNow;

            _unitWork.Repo<Chat>().Update(chat);
            await _unitWork.CompleteAsync();
            return Ok(new ApiResponse(200, "Customer Leaved Chat"));
        }

        /*
        // DELETE: api/Chats/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChat(int id)
        {
            var chat = await _unitWork.Chats.FindAsync(id);
            if (chat == null)
            {
                return NotFound();
            }

            _unitWork.Chats.Remove(chat);
            await _unitWork.SaveChangesAsync();

            return NoContent();
        }

        private bool ChatExists(int id)
        {
            return _unitWork.Chats.Any(e => e.Id == id);
        }
        */
    }
}
