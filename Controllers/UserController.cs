using HotelReservation.DTo;
using HotelReservation.model;
using HotelReservation.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace HotelReservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly PasswordResetService _resetService;
        private readonly IConfiguration _configuration;

        public UserController(
            AppDbContext context,
            IConfiguration configuration,
            PasswordResetService resetService,
            IWebHostEnvironment environment)
        {
            _context = context;
            _configuration = configuration;
            _resetService = resetService;
            _environment = environment;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetUsers()
        {
            var users = await _context.Users.Select(x => new UserDto
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
                phoneNumber = x.phoneNumber,
                Role = x.Role

            }).ToListAsync();
            return Ok(users);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _context.Users.Where(x => x.Id == id).Select(x => new UserDto
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
                phoneNumber = x.phoneNumber,
                Role = x.Role
            }).FirstOrDefaultAsync();
            if (user == null)
                return NotFound();
            return Ok(user);
        }
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto dto)
        {
            var emailExists = await _context.Users.AnyAsync(x => x.Email == dto.Email);
            if (emailExists)
                return BadRequest("Email already exists");

            var user = new User()
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                phoneNumber = dto.phoneNumber,
                Role = "customer"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                phoneNumber = user.phoneNumber,
                Role = user.Role
            });
        }
        [Authorize]
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            return Ok(new
            {
                userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                name = User.Identity?.Name,
                email = User.FindFirst(ClaimTypes.Email)?.Value,
                role = User.FindFirst(ClaimTypes.Role)?.Value
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
         public async Task<ActionResult<UserDto>> UpdateUser(
          int id,
           UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound();

            bool emailExists = await _context.Users.AnyAsync(
                x => x.Email == dto.Email && x.Id != id);

            if (emailExists)
                return BadRequest("Email already exists.");

            user.Name = dto.Name;
            user.Email = dto.Email;
            user.phoneNumber = dto.phoneNumber;

            user.Role = dto.Role;

            await _context.SaveChangesAsync();

            return Ok(new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                phoneNumber = user.phoneNumber,
                Role = user.Role
            });
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (user == null ||
                !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return Unauthorized();
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]!
            );

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()
            ),

            new Claim(
                ClaimTypes.Name,
                user.Name
            ),

            new Claim(
                ClaimTypes.Email,
                user.Email
            ),

            new Claim(
                ClaimTypes.Role,
                user.Role
            )
                }),

                Expires = DateTime.UtcNow.AddMinutes(
                    double.Parse(
                        _configuration["Jwt:DurationInMinutes"]!
                    )
                ),

                Issuer = _configuration["Jwt:Issuer"],

                Audience = _configuration["Jwt:Audience"],

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                Token = tokenString
            });
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("admin-test")]
        public IActionResult AdminTest()
        {
            return Ok("Welcome Admin");
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(
            ForgotPasswordDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
            {
                return Ok(new
                {
                    message = "If the account exists, reset instructions will be sent."
                });
            }

            string token = _resetService.GenerateToken();

            user.PasswordResetTokenHash =
                Convert.ToHexString(
                    SHA256.HashData(Encoding.UTF8.GetBytes(token)));

            user.PasswordResetTokenExpiry =
                DateTime.UtcNow.AddMinutes(15);

            await _context.SaveChangesAsync();

            if (_environment.IsDevelopment())
            {
                return Ok(new
                {
                    message = "Reset token generated for development.",
                    token = token
                });
            }

            return Ok(new
            {
                message = "If the account exists, reset instructions will be sent."
            });
        }
        [AllowAnonymous]
        [HttpPost("reset-password")]

       
        public async Task<IActionResult> ResetPassword(
    ResetPasswordDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null ||
                string.IsNullOrEmpty(user.PasswordResetTokenHash) ||
                user.PasswordResetTokenExpiry == null ||
                user.PasswordResetTokenExpiry <= DateTime.UtcNow)
            {
                return BadRequest(new
                {
                    message = "Invalid or expired reset request."
                });
            }

            string tokenHash = Convert.ToHexString(
                SHA256.HashData(Encoding.UTF8.GetBytes(dto.Token)));

            if (!CryptographicOperations.FixedTimeEquals(
                Convert.FromHexString(user.PasswordResetTokenHash),
                Convert.FromHexString(tokenHash)))
            {
                return BadRequest(new
                {
                    message = "Invalid or expired reset request."
                });
            }

            user.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            user.PasswordResetTokenHash = null;
            user.PasswordResetTokenExpiry = null;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Password reset successfully."
            });
        }


    }

}