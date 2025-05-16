using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using ECommerce.Core;
using ECommerce.Core.Models.Laravel;
using ECommerce.Errors;
using ECommerce.Core.Models;
using ECommerce.Core.Specifications;
using ECommerce.ChatServices;
using ECommerce.DTO.Request;
using ECommerce.DTO.Response;

namespace ECommerce.Controllers
{
    [Authorize("Sanctum")]
    public class MeassagesController : ApiBaseController
    {
        private readonly IUnitWork _unitWork;
        private readonly IMapper _mapper;
        private readonly IHubContext<ChatHub, IChatClient> _contextHttp;
        private readonly GeminiService _gemini;

        public MeassagesController(IUnitWork unitWork, IMapper mapper, IHubContext<ChatHub, IChatClient> contextHttp, GeminiService gemini)
        {
            _unitWork = unitWork;
            _mapper = mapper;
            _contextHttp = contextHttp;
            _gemini = gemini;
        }

        [HttpGet("getUserMessages")]
        public async Task<ActionResult<IEnumerable<MessageResponse>>> GetUserMessages()
        {
            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized(new ApiResponse(401));

            var userName = User?.FindFirstValue(ClaimTypes.Name);
            var messages = await _unitWork.Repo<Message>().GetAllAsync(new MessageSpec(int.Parse(userId), null));
            if (messages == null) return NotFound(new ApiResponse(404));

            var mapped = _mapper.Map<IEnumerable<MessageResponse>>(messages);
            await _contextHttp.Clients.All.ReceiveUserMessages(userName, mapped);
            return Ok(mapped);
        }

        [HttpPost]
        public async Task<ActionResult> SendMessage([FromBody] MessageRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized(new ApiResponse(401));

            var message = new Message
            {
                ChatId = request.ChatId,
                Content = request.Content,
                IsRead = false,
                SentAt = DateTimeOffset.UtcNow,
                SenderId = int.Parse(userId),
                connectionId = $"Conn-{userId}"
            };

            await _unitWork.Repo<Message>().AddAsync(message);
            await _unitWork.CompleteAsync();

            if (request.Content.StartsWith($"@bot"))
            {
                string botResponse =
                    await _gemini.AskGemini($"You are a helpful assistant in Customer Service, Billing, Call Center, etc. Question: {request.Content.Replace("@bot", "")}");

                await _contextHttp.Clients.Group(request.ChatId.ToString()).AskBot("Bot", botResponse);
                return Ok(new ApiResponse(200));
            }

            await _contextHttp.Clients.Group(request.ChatId.ToString())
                .ReceiveMessage(request.UserDisplay, request.Content);

            return Ok(new ApiResponse(200));
        }
        /*
        [HttpPost("File")]
        public async Task<IActionResult> SendFile([FromForm] MessageRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized(new ApiResponse(401));

            if (request.File == null || request.File.Length == 0)
                return BadRequest(new ApiResponse(400, "Invalid file"));

            var fileName = Path.GetFileName(request.File.FileName);
            //var filePath = Path.Combine("wwwroot/uploads/file", fileName);
            //Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            //using (var stream = new FileStream(filePath, FileMode.Create))
            //    await file.CopyToAsync(stream);

            var connectionId = connMapping.TryGetValue(userId);
            if (connectionId == null) return NotFound(new ApiResponse(404));

            var message = new Meassage
            {
                ChatId = request.ChatId,
                Content = fileName,
                IsRead = false,
                SentAt = DateTimeOffset.UtcNow,
                SenderId = userId,
                connectionId = connectionId
            };

            await _unitWork.Repo<Meassage>().AddAsync(message);
            await _unitWork.CompleteAsync();

            await _contextHttp.Clients.Group(request.ChatId.ToString())
                .ReceiveMessage(request.UserDisplay, fileName);

            return Ok(new ApiResponse(200));
        }

        [HttpPost("Audio")]
        public async Task<IActionResult> SendAudio([FromForm] MessageRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized(new ApiResponse(401));

            if (request.Audio == null || request.Audio.Length == 0)
                return BadRequest(new ApiResponse(400, "Invalid file"));

            var fileName = Path.GetFileName(request.Audio.FileName);
            //var audioPath = Path.Combine("wwwroot/uploads/audio", audioFileName);
            //Directory.CreateDirectory(Path.GetDirectoryName(audioPath));
            //using (var stream = new FileStream(audioPath, FileMode.Create))
            //    await audio.CopyToAsync(stream);

            var connectionId = connMapping.TryGetValue(userId);
            if (connectionId == null) return NotFound(new ApiResponse(404));

            var message = new Meassage
            {
                ChatId = request.ChatId,
                Content = fileName,
                IsRead = false,
                SentAt = DateTimeOffset.UtcNow,
                SenderId = userId,
                connectionId = connectionId
            };

            await _unitWork.Repo<Meassage>().AddAsync(message);
            await _unitWork.CompleteAsync();

            await _contextHttp.Clients.Group(request.ChatId.ToString())
                .ReceiveMessage(request.UserDisplay, fileName);

            return Ok(new ApiResponse(200));
        }


        // GET: api/Meassages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Meassage>> GetMeassage(int id)
            => (await _unitWork.Repo<Meassage>().GetByIdAsync(id) is null) ? NotFound(new ApiResponse(404)) : Ok(await _unitWork.Repo<Meassage>().GetByIdAsync(id)); 
      
        
        //// PUT: api/Meassages/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMeassage(int id, Meassage meassage)
        {
            if (id != meassage.Id)
            {
                return BadRequest();
            }

            _unitWork.Entry(meassage).State = EntityState.Modified;

            try
            {
                await _unitWork.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MeassageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Meassages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Meassage>> PostMeassage(Meassage meassage)
        {
            _unitWork.Messages.Add(meassage);
            await _unitWork.SaveChangesAsync();

            return CreatedAtAction("GetMeassage", new { id = meassage.Id }, meassage);
        }

        // DELETE: api/Meassages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeassage(int id)
        {
            var meassage = await _unitWork.Messages.FindAsync(id);
            if (meassage == null)
            {
                return NotFound();
            }

            _unitWork.Messages.Remove(meassage);
            await _unitWork.SaveChangesAsync();

            return NoContent();
        }

        private bool MeassageExists(int id)
        {
            return _unitWork.Messages.Any(e => e.Id == id);
        }
        */
    }
}
