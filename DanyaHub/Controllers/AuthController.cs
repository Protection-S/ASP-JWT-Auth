using DanyaHub.Data;
using DanyaHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DanyaHub.Properties;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace DanyaHub.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly Context _context;
        private readonly JwtSettings _jwtSettings;

        public AuthController(Context context, JwtSettings jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        public IActionResult Login(User loginUser)
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == loginUser.Username && u.Password == loginUser.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(loginUser);
            }

            var token = GenerateJwtToken(user);

            HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            var redirectUrl = user.IsAdmin ? "/Admin/Users" : "/Files/Index";
            return Redirect(redirectUrl);
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(User model)
        {
            if (ModelState.IsValid)
            {
                model.IsAdmin = false;

                _context.Users.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(model);
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            if (user.IsAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, "User"));
            }


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<IActionResult> Logout()
        {
            if (HttpContext.Request.Cookies.ContainsKey("jwt"))
            {
                HttpContext.Response.Cookies.Delete("jwt");
            }
            return RedirectToAction("Index", "Home");
        }




    }
}
