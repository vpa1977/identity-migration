using AutoMapper;
using Sample.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Sample.Hydra;
using Microsoft.Extensions.Options;

namespace Sample.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly HydraClient _hydraClient;
        private readonly HydraConfig _config;

        public AccountController(UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, 
            HydraClient hydraClient, 
            IOptions<HydraConfig> config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _hydraClient = hydraClient;
            _config = config.Value;
        }

        public IActionResult Consent(string consent_challenge)
        {
            var redirectTo = _hydraClient.AcceptConsent(consent_challenge, new()
            {
                { "CustomKey", "CustomValue"}
            });

            return new RedirectResult(redirectTo);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegistrationModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userModel);
            }

            var user = new ApplicationUser()
            {
                UserName = userModel.Email,
                Email = userModel.Email
            };

            var result = await _userManager.CreateAsync(user, userModel.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }

                return View(userModel);
            }

            await _userManager.AddToRoleAsync(user, "Visitor");

            return View();
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null, string? login_challenge = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["LoginChallenge"] = login_challenge;
            return View();
        }   

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginModel userModel, string challenge, string returnUrl = "/")
        {
            var result = await _signInManager.PasswordSignInAsync(userModel.Email, userModel.Password, userModel.RememberMe, false);
            if (result.Succeeded)
            {
                returnUrl = _hydraClient.AcceptLoginRequest(challenge, userModel.Email,  req =>
                {
                    req.Subject = userModel.Email;
                    req.Remember = userModel.RememberMe;
                    req.RememberFor = 0;
                });

                return new RedirectResult(returnUrl);
            }
            else
            {
                ModelState.AddModelError("", "Invalid UserName or Password");
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return View();
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return View();
        }
    }
}