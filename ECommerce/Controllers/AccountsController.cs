using AutoMapper;
using ECommerce.Core.Models.Identity;
using ECommerce.Core.Services;
using ECommerce.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ECommerce.Controllers
{
    public class AccountsController : ApiBaseController
    {
        private readonly UserManager<AppUser> _manager;
        private readonly SignInManager<AppUser> _signManager;
        private readonly IToken _token;
        private readonly IMapper _mapper;

        public AccountsController(UserManager<AppUser> manager, SignInManager<AppUser> signManager, IToken token, IMapper mapper)
        {
            _manager = manager;
            _signManager = signManager;
            _token = token;
            _mapper = mapper;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO model)
        {
            var emailExists = await ValidateEmail(model.Email);
            if (emailExists.Value)
                return BadRequest("Email is Exist");

            var user = new AppUser()
            {
                Name = model.Name,
                Email = model.Email,
                UserName = model.Email.Split('@')[0],
                PhoneNumber = model.Phone,
            };

            var res = await _manager.CreateAsync(user, model.Password);
            if (!res.Succeeded) return BadRequest(res);

            var Returned = new UserDTO()
            {
                Name = user.Name,
                Email = user.Email ?? string.Empty,
                Token = await _token.CreateTokenAsync(user, _manager)
            };
            return Ok(Returned);
        }

        [HttpGet("Register/ValidateEmail")]
        public async Task<ActionResult<bool>> ValidateEmail(string email)
            => await _manager.FindByEmailAsync(email) is not null;

        [HttpPost("Login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO model)
        {
            var user = await _manager.FindByEmailAsync(model.Email);
            if (user is null) return Unauthorized(user);

            var res = await _signManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!res.Succeeded) return Unauthorized(res);

            var Returned = new UserDTO()
            {
                Name = user.Name,
                Email = user.Email ?? string.Empty,
                Token = await _token.CreateTokenAsync(user, _manager)
            };
            return Ok(Returned);
        }

        [Authorize]
        [HttpGet("User")]
        public async Task<ActionResult<UserDTO>> GetUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                return BadRequest("Email claim not found");
            }

            var user = await _manager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var Returned = new UserDTO()
            {
                Name = user.Name,
                Email = user.Email ?? string.Empty,
                Token = await _token.CreateTokenAsync(user, _manager)
            };
            return Ok(Returned);
        }

        [Authorize]
        [HttpGet("User/Address")]
        public async Task<ActionResult<AddressDTO>> GetAddress()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                return BadRequest("Email not found");
            }

            var user = await _manager.Users.Where(e => e.Email == email).Include(a => a.Address).FirstOrDefaultAsync();
            if (user == null || user.Address == null)
            {
                return NotFound("User or address not found");
            }

            var MappedAddress = _mapper.Map<Address, AddressDTO>(user.Address);
            return Ok(MappedAddress);
        }

        [Authorize]
        [HttpPut("User/Address")]
        public async Task<ActionResult<AddressDTO>> PutAddress(AddressDTO updated)
        {
            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                if (email == null)
                {
                    return BadRequest("Email claim not found");
                }

                var user = await _manager.Users.Where(e => e.Email == email).Include(a => a.Address).FirstOrDefaultAsync();
                if (user == null || user.Address == null)
                {
                    return NotFound("User or address not found");
                }

                var MappedAddress = _mapper.Map<AddressDTO, Address>(updated);
                MappedAddress.Id = user.Address.Id;
                user.Address = MappedAddress;

                var res = await _manager.UpdateAsync(user);
                if (!res.Succeeded) return BadRequest(res);

                return Ok(res);
            }
            catch (Exception)
            {
                return NoContent();
            }
        }
    }
}
