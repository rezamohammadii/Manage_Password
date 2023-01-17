using Google.Authenticator;
using ManagePassword.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;

namespace ManagePassword.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private const string Key = "dfg756!@@)(*";
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(UserLoginModel login)
        {
            string? message = "";
            bool status = false;
            if (login.Username == "Administrator" && login.Password == "Password1")

            {

                status = true; //It indicates 2FA form

                message = "2FA Verification";
                
                byte[] userToByte = Encoding.ASCII.GetBytes(login.Username);
                    
                HttpContext.Session.Set("auth" , userToByte);


                //2FA Setup

                TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();

                string uniqueKeyforUser = (login.Username + Key);

              //  Session["UserUniqueKey"] = UserUniqueKey;

                var setupInfo = tfa.GenerateSetupCode("AGENT-47 Awesome",

                login.Username, uniqueKeyforUser,false ,3);

                ViewBag.BarcodeImageUrl = setupInfo.QrCodeSetupImageUrl;

                ViewBag.SetupCode = setupInfo.ManualEntryKey;

            }

            else

            {

                message = "Invalid credential";

            }

            ViewBag.Message = message;



            ViewBag.Status = status;

            return View();
        }
        public ActionResult MyProfile()
        {
            string? user = HttpContext.Session.GetString("auth");

            if (user == null)
            {

                return RedirectToAction("Login");

            }

         //  user = Encoding.ASCII.GetString()

            ViewBag.Message = "Welcome " + user;

            return View();

        }
        public IActionResult Verify2FA(UserLoginModel input)
        {

            var token = input.passcode;
            string? user = HttpContext.Session.GetString("auth");
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();

            string UserUniqueKey = user + Key;

            bool isValid = tfa.ValidateTwoFactorPIN(UserUniqueKey, token);

            if (isValid)
            {
                //       Session["IsValidAuthentication"] = true;
                return RedirectToAction("MyProfile", "Home");
            }
            return RedirectToAction("Login", "Home");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}