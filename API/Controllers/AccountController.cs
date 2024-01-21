using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DataTransferObjects;
using API.Entities;
using API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private DataContext _context ;
        private ITokenService _tokentService ;
        private readonly IMapper _mapper;

        public AccountController(DataContext context, ITokenService tokenService, IMapper mapper){
            _context = context;
            _tokentService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("register")] // api/account/register
        public async Task<ActionResult<UserDTO>>Register(RegisterDTO registerDTO){
            
            if(await UserExist(registerDTO.UserName))return BadRequest("Username is taken already.");
            var user = _mapper.Map<AppUser>(registerDTO);

            using var hmac = new HMACSHA512();
            
            user.UserName = registerDTO.UserName.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
            user.PasswordSalt = hmac.Key;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new UserDTO{
                Username = user.UserName,
                Token = _tokentService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO){
            var user = await _context.Users
                    .Include(p => p.Photos)
                    .SingleOrDefaultAsync(x => x.UserName == loginDTO.UserName);
            if(user==null)return Unauthorized("Invalid username");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
            for(int i=0; i<computeHash.Length; i++){
                if(computeHash[i]!=user.PasswordHash[i])
                    return Unauthorized("Invalid password");
            }
            return new UserDTO{
                Username = user.UserName,
                Token = _tokentService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs
            };
        }
        private async Task<bool> UserExist(string username){
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower() );
        }
    }
}