using Entities;
using Entities.Enums;
using Infrastructure.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using UseCases;
using UseCases.Repositories;
using UseCases.TaskResults;

namespace Infrastructure.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly AuthenticationManager _authenticationManager;

        private readonly UserManager _userManager;

        public AuthenticationController(AuthenticationManager authenticationManager, UserManager userManager)
        {
            _authenticationManager = authenticationManager;
            _userManager = userManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var loginResult = await _authenticationManager.LoginAsync(loginRequest.Username, loginRequest.Password);

            if (loginResult.ResultCode == UseCases.TaskResults.LoginResultCodes.Success)
            {
                var user = loginResult.User ?? throw new Exception("An error occurred.");

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                if (user.Role == Entities.Enums.UserRole.Admin)
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("Login failed", loginResult.Message);
                return View();
            }
        }

        public async Task LoginByGoogle()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleResponse", "Authentication")
                });
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);


            if (!result.Succeeded || result.Principal == null)
            {
                TempData["error"] = "Google authentication failed";
                return RedirectToAction("Login");
            }


            var emailClaim = result.Principal.FindFirst(ClaimTypes.Email);
            if (emailClaim == null)
            {
                TempData["error"] = "Unable to retrieve email from Google account.";
                return RedirectToAction("Login");
            }

            var email = emailClaim.Value;
            var username = email.Split('@')[0];

            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser == null)
            {
                var passwordHasher = new PasswordHasher<User>();
                var hashedPassword = passwordHasher.HashPassword(null, Guid.NewGuid().ToString());
                var newUser = new User
                {
                    Email = email,
                    Username = username,
                    Password = hashedPassword,
                    Phone = "xxx-xxx-xxxx",
                    Role = Entities.Enums.UserRole.Customer 
                };

                await _userManager.AddAsync(newUser);
                existingUser = newUser; 
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, existingUser.Id.ToString()),
                new Claim(ClaimTypes.Name, existingUser.Username),
                new Claim(ClaimTypes.Email, existingUser.Email),
                new Claim(ClaimTypes.Role, existingUser.Role.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            TempData["success"] = "Login successful";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Signup(User user)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var signupResult = await _authenticationManager.SignupAsync(user);
            if (signupResult.ResultCode == UseCases.TaskResults.SignupResultCodes.Success)
            {
                return RedirectToAction("Login");
            }
            else
            {
                ModelState.AddModelError("Signup failed", signupResult.Message);
                return View();
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
