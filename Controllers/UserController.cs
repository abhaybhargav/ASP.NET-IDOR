using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;

namespace IDORDemoApp.Controllers
{
    public class UserController : Controller
    {
        private readonly DataStore _dataStore;
        private readonly ILogger<UserController> _logger;

        public UserController(DataStore dataStore, ILogger<UserController> logger)
        {
            _dataStore = dataStore;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SecureDashboard()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login");
            }
            
            var sensitiveInfoList = _dataStore.GetSensitiveInfoForUser(userId.Value, true);
            ViewBag.IsSecureMode = true;
            
            return View("Dashboard", sensitiveInfoList);
        }

        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Signup(string username, string password)
        {
            _dataStore.AddUser(username, password);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _dataStore.AuthenticateUser(username, password);
            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.Id);
                return RedirectToAction("Dashboard");
            }
            ViewBag.Error = "Invalid username or password";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Dashboard(bool secure = false)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login");
            }
            
            HttpContext.Session.SetString("SecureMode", secure.ToString());
            var sensitiveInfoList = _dataStore.GetSensitiveInfoForUser(userId.Value, secure);
            ViewBag.IsSecureMode = secure;
            
            return View(sensitiveInfoList);
        }

        [HttpPost]
        public IActionResult AddSensitiveInfo(string info)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login");
            }
            int infoId = _dataStore.AddSensitiveInfo(userId.Value, info);
            return RedirectToAction("ViewSensitiveInfo", new { id = infoId });
        }

        public IActionResult ViewSensitiveInfo(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login");
            }

            var isSecure = HttpContext.Session.GetString("SecureMode") == "true";
            SensitiveInfo info = _dataStore.GetSensitiveInfo(id, isSecure ? userId : null);

            if (info == null)
            {
                return NotFound();
            }

            return View(info);
        }


    }
}