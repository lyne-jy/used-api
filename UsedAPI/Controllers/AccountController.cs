using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsedAPI.Data;
using UsedAPI.Dto;
using UsedAPI.Interfaces;
using UsedAPI.Models;
using UsedAPI.Services;

namespace UsedAPI.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> GetEmail(int id)
        {
            var seller = await _context.Sellers.FindAsync(id);

            if (seller == null)
            {
                return NotFound();
            }

            return seller.Email ;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            var hmac = new HMACSHA512();
            
            var existSeller = await _context.Sellers
                .SingleOrDefaultAsync(x => x.Email == registerDto.Email);

            if (existSeller != null) return BadRequest("Email has been used");

            var seller = new Seller
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Sellers.Add(seller);
            await _context.SaveChangesAsync();
            
            return new UserDto
            {
                Name = registerDto.Name,
                Token = _tokenService.CreateToken(seller)
            };
        }
        
        [HttpPost("login")]
        public async Task <ActionResult<UserDto>>Login(LoginDto loginDto)
        {
            var seller = await _context.Sellers
                .SingleOrDefaultAsync(x => x.Email == loginDto.Email);

            if (seller == null) return Unauthorized("Invalid Email");

            var hmac = new HMACSHA512(seller.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for(int i=0;i<computedHash.Length;i++)
            {
                if (computedHash[i] != seller.PasswordHash[i]) return Unauthorized("Invalid Password");
            }

            return new UserDto
            {
                Name = seller.Name,
                Token = _tokenService.CreateToken(seller)
            };
        }
        
        private async Task <bool>UserExists(string email)
        {
            return await _context.Sellers.AnyAsync(x => x.Email == email);
        }
    }
}