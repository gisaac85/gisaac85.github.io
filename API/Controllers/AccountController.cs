using API.Errors;
using API.Extensions;
using AutoMapper;
using Core.Dtos;
using Core.Entities.UserModels;
using Core.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper)
        {
            _mapper = mapper;          
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
        }

       
        [HttpGet("getuser")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userManager.FindByEmailFromClaimsPrinciple(HttpContext.User);
            var role = await _userManager.GetRolesAsync(user);
            return new UserDto
            {
                Email = user.Email,
                Token = _tokenService.CreateToken(user),
                DisplayName = user.DisplayName,
                Role = role.FirstOrDefault()                
            };
        }

        
        [HttpGet("address")]
        public async Task<ActionResult<AddressUserDto>> GetUserAddress()
        {
            var user = await _userManager.FindByUserByClaimsPrincipleWithAddressAsync(HttpContext.User);

            var address = _mapper.Map<AddressUser,AddressUserDto>(user.AddressUser);
            return address;
        }

       
        [HttpPost("updateaddress")]
        public async Task<ActionResult<AddressUserDto>> UpdateUserAddress(AddressUserDto address)
        {
            var user = await _userManager.FindByUserByClaimsPrincipleWithAddressAsync(HttpContext.User);

            user.AddressUser = _mapper.Map<AddressUserDto, AddressUser>(address);

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded) return Ok(_mapper.Map<AddressUser, AddressUserDto>(user.AddressUser));

            return BadRequest("Problem updating the user");
        }

        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmailExistsAsync([FromQuery] string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }      

       
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
            {
                return Unauthorized(new ApiResponse(401));
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            var address = await _userManager.FindAddressAsync(user.Id);
            var addressDto = _mapper.Map<AddressUser, AddressUserDto>(address);

            if (!result.Succeeded)
            {
                return Unauthorized(new ApiResponse(401));
            }
            var role = await _userManager.GetRolesAsync(user);

            return new UserDto
            {
                Email = loginDto.Email,
                Token = _tokenService.CreateToken(user),
                DisplayName = user.DisplayName,
                Role = role.FirstOrDefault(),
                Address = addressDto
            };

        }

      
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {           
            var user = new AppUser
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.Email                
            };
            
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            var address = new AddressUser
            {
                AppUser = user,
                AppUserId = user.Id,
                City = registerDto.City,
                State = registerDto.State,
                Street = registerDto.Street,
                Zipcode = registerDto.Zipcode,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName
            };

            var addressDto = _mapper.Map<AddressUser, AddressUserDto>(address);
            user.AddressUser = address;

            await _userManager.AddToRoleAsync(user, "Member");

            if (!result.Succeeded) return BadRequest(new ApiResponse(400));

            return new UserDto
            {
                DisplayName = user.DisplayName,
                Token = _tokenService.CreateToken(user),
                Email = user.Email ,
                Role = "Member",
                Address = addressDto
            };
        }

    }
}
