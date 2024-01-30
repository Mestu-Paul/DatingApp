using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DataTransferObjects;
using API.Entities;
using API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private ITokenService _tokentService ;
        private readonly IMapper _mapper;

        public AccountController(UserManager<AppUser>userManager, ITokenService tokenService, IMapper mapper){
            _userManager = userManager;
            _tokentService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("register")] // api/account/register
        public async Task<ActionResult<UserDTO>>Register(RegisterDTO registerDTO){
            
            if(await UserExist(registerDTO.UserName))return BadRequest("Username is taken already.");
            var user = _mapper.Map<AppUser>(registerDTO);
            
            user.UserName = registerDTO.UserName.ToLower();

            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            if(!result.Succeeded) return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user,"Member");

            if(!roleResult.Succeeded)return BadRequest(result.Errors);
            
            return new UserDTO{
                Username = user.UserName,
                Token = await _tokentService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO){
            var user = await _userManager.Users
                    .Include(p => p.Photos)
                    .SingleOrDefaultAsync(x => x.UserName == loginDTO.UserName);
            if(user==null)return Unauthorized("Invalid username");

            var result = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
            if(!result) return Unauthorized("Invalid password");
            
            return new UserDTO{
                Username = user.UserName,
                Token = await _tokentService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }
        private async Task<bool> UserExist(string username){
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower() );
        }
    }
}