﻿using Elearn_temp.Models;
using Elearn_temp.Services.Interfaces;
using Elearn_temp.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Elearn_temp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager; // user crate etmek ucundur

        private readonly SignInManager<AppUser> _signInManager; //  sayita giris etmek logout olmaq ucun istifade olunur

        private readonly IEmailService _emailService;
        public AccountController(UserManager<AppUser> userManager,
                               SignInManager<AppUser> signInManager, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            AppUser newUser = new()
            {
                UserName = model.Username,
                Email = model.Email,
                FullName = model.FullName,
            };

            IdentityResult result = await _userManager.CreateAsync(newUser, model.Password); // passwordu colden ediriki onu haslaya bilsin yeni kodlari baxmaq olmur 


            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }

                return View(model);
            }



            string token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

            string link = Url.Action(nameof(ConfirmEmail), "Account", new { userId = newUser.Id, token }, Request.Scheme, Request.Host.ToString());

            string subject = "Register confirmation";

            string html = string.Empty;

            using (StreamReader reader = new StreamReader("wwwroot/templates/verify.html"))
            {
                html = reader.ReadToEnd();
            }

            html = html.Replace("{{link}}", link);
            html = html.Replace("{{headerText}}", "Hello P135");

            _emailService.Send(newUser.Email, subject, html);

            return RedirectToAction(nameof(VerifyEmail));

            
           

        }


        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null) return BadRequest();

            AppUser user = await _userManager.FindByIdAsync(userId);

            if (user == null) return NotFound();

            await _userManager.ConfirmEmailAsync(user, token);

            await _signInManager.SignInAsync(user, false);

            return RedirectToAction("Index", "Home");
        }



        public IActionResult VerifyEmail()
        {
            return View();
        }



        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            AppUser user = await _userManager.FindByEmailAsync(model.EmailOrUsername);  // burda yoxlayirki email ila daxil olubdu

            if (user is null)  // eger tapa bilmedise nuldusa girir serti yoxlayir
            {
                user = await _userManager.FindByNameAsync(model.EmailOrUsername);  // burda yoxlayirki username ile daxil olubdu

            }

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Email or password is wrong");  // eger bele bir email tapilmasa bu eroro cixart
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Email or password is wrong");  // eger bele succesd deyilse bu eroro cixart
                return View(model);

            }







            return RedirectToAction("Index", "Home");
        }




        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();  // bu deyirki gedib logouta clicik edende bu methodu islet yeni cixsin logout etsin
            return RedirectToAction("Index", "Home");
        }




    }
}
