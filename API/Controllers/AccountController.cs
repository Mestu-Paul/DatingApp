using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DataTransferObjects;
using API.Entities;
using API.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private DataContext _context ;
        private ITokenService _tokentService ;
        public AccountController(DataContext context, ITokenService tokenService){
            _context = context;
            _tokentService = tokenService;
        }

        [HttpPost("register")] // api/account/register
        public async Task<ActionResult<UserDTO>>Register(RegisterDTO registerDTO){
            
            if(await UserExist(registerDTO.UserName))return BadRequest("Username is taken already.");
            using var hmac = new HMACSHA512();
            var user = new AppUser{
                UserName = registerDTO.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new UserDTO{
                Username = user.UserName,
                Token = _tokentService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO){
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == loginDTO.UserName);
            if(user==null)return Unauthorized("Invalid username");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
            for(int i=0; i<computeHash.Length; i++){
                if(computeHash[i]!=user.PasswordHash[i])
                    return Unauthorized("Invalid password");
            }
            return new UserDTO{
                Username = user.UserName,
                Token = _tokentService.CreateToken(user)
            };
        }
        private async Task<bool> UserExist(string username){
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower() );
        }
    }
}