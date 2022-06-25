﻿using BrowserChat.Client.Core.Services;
using BrowserChat.Client.Core.Session;
using BrowserChat.Entity.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BrowserChat.Client.Controllers
{
    public class AccountController : Controller
    {
        private readonly ISecurityService _securityService;
        private readonly SessionManagement _sessionMgr;

        public AccountController(ISecurityService securityService, SessionManagement sessionMgr)
        {
            _securityService = securityService;
            _sessionMgr = sessionMgr;
        }

        public IActionResult Login()
        {
            if (_sessionMgr.IsLoggedIn())
            {
                var user = _securityService.ValidateSession();
                if (!string.IsNullOrEmpty(user?.Token))
                {
                    _sessionMgr.SetUserSession(user);
                    return RedirectToAction("Index", "Home");
                    
                }

                _sessionMgr.RemoveUserSession();
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(UserLoginDTO login)
        {
            var user = _securityService.Login(login);

            if (string.IsNullOrEmpty(user?.Token))
            {
                return RedirectToAction("Login", "Account");
            }
             else
            {
                _sessionMgr.SetUserSession(user);
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Logout()
        {
            _sessionMgr.RemoveUserSession();
            return RedirectToAction("Login", "Account");
        }

        public IActionResult Register()
        {
            return View();
        }
    }
}
