using Google.Authenticator;
using ManagePassword.Database;
using ManagePassword.Database.Entity;
using ManagePassword.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;

namespace ManagePassword.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private DataBaseContext _db;
        private const string Key = "dfg756!@@)(*";
        List<General> lists = new List<General>();
        public HomeController(ILogger<HomeController> logger , DataBaseContext db)
        {
            _logger = logger;
            _db = db;
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

                HttpContext.Session.Set("auth", userToByte);


                //2FA Setup

                TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();

                string uniqueKeyforUser = (login.Username + Key);

                //  Session["UserUniqueKey"] = UserUniqueKey;

                var setupInfo = tfa.GenerateSetupCode("AGENT-47 Awesome",

                login.Username, uniqueKeyforUser, false, 3);

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
        public IActionResult MyProfile()
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
            ViewBag.Alert = "Code is not valid";
            return RedirectToAction("Login", "Home");
        }

        [HttpPost]
        public IActionResult SetPassword(PasswordModel model)
        {

            General general = new General()
            {
                Password = model.Password,
                Username = model.Username,
                Website = model.WebSite
            };
            _db.Generals.Add(general);
            _db.SaveChanges();
            //var exitsUrl = _db.Generals.Where(x => x.Website == model.WebSite).FirstOrDefault();
            //if (exitsUrl != null)
            //{
            //    exitsUrl.Password = model.Password;
            //    exitsUrl.Username = model.Username;
            //    _db.SaveChanges();
            //}
            //else
            //{
               
            //}
            return RedirectToAction(nameof(MyProfile));
        }
        
        public IActionResult GetPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult GetPassword( PasswordModel model)
        {
            
            if (model.Username != null && model.WebSite != null)
            {

                lists = _db.Generals.Where(x => x.Username == model.Username && x.Website == model.WebSite).ToList();


            }
            else if (model.Username != null)
            {
                lists = _db.Generals.Where(x => x.Username == model.Username).ToList();
            }
            else
            {
                lists = _db.Generals.Where(x => x.Website == model.WebSite).ToList();
            }
            return View(lists);
        }

        public IActionResult GetResult(List<General> generals)
        {
            generals = lists;
            return View(generals);
        }
    }
}