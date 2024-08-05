using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LTAssignment.Models.Account;
using LTAssignment.Models.DBConnection;
using Microsoft.Win32;
using System.Data;
using LTAssignment.Models.Product;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LTAssignment.Controllers
{
    public class AccountController : Controller
    {
        private readonly Common _common;
        public AccountController(Common common)
        {
            _common = common;
        }
        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated)
                return RedirectToAction("GetProductList", "Products");


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Accounts modelLogin, Register register)
        {

            var (dataSet, success, message) = _common.ExecuteStoreProcedure(register, "sp_Registeration", 41);
            if (!success)
            {
                ViewBag.ErrorMessage = message;
                return View("Error");
            }

            Register dataSetRegister = new Register();
            if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                DataRow row = dataSet.Tables[0].Rows[0];
                dataSetRegister.Email = row["Email"].ToString();
                dataSetRegister.Password = row["Password"].ToString();
                dataSetRegister.ID = (int)row["ID"];
                dataSetRegister.UserName = row["UserName"].ToString();
                dataSetRegister.Type = row["Type"].ToString();
                 dataSetRegister.ProfileImage = row["ProfileImage"].ToString();
           
            if(modelLogin.Email !=null || modelLogin.Password != null)
            {
                if (modelLogin.Email == dataSetRegister.Email &&
                                modelLogin.Password == dataSetRegister.Password)
                {
                    List<Claim> claims = new List<Claim>() {
            new Claim(ClaimTypes.NameIdentifier, modelLogin.Email),
            new Claim(ClaimTypes.Name, dataSetRegister.UserName),
             new Claim(ClaimTypes.PrimarySid, dataSetRegister.ID.ToString()),
            new Claim(ClaimTypes.Role, dataSetRegister.Type),
            new Claim(ClaimTypes.Actor, dataSetRegister.ProfileImage),
            new Claim("OtherProperties","Example Role")

        };

                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                        CookieAuthenticationDefaults.AuthenticationScheme);

                    AuthenticationProperties properties = new AuthenticationProperties()
                    {

                        AllowRefresh = true,
                        IsPersistent = modelLogin.KeepLoggedIn
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), properties);


                    return RedirectToAction("GetProductList", "Products");
                }
            }
            }



            ViewData["ValidateMessage"] = "Email or Password Worng";
            return View();
        }
        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");

        }
        public IActionResult Register()
        {
            ViewData["Roles"] = new List<SelectListItem>
        {
            new SelectListItem { Value = "seller", Text = "Seller" },
            new SelectListItem { Value = "admin", Text = "Admin" },
            new SelectListItem { Value = "buyer", Text = "Buyer" }
        };
            return View();
        }

        [HttpPost]
        public IActionResult Register(Register register)
        { 

            if (register.Image != null && register.Image.Length > 0)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserProfiles");
                string setPath = "";
                string DTFile = "";

                // Create folder if it doesn't exist
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                // Handle the single image file
                var file = register.Image;
                if (file.Length > 0)
                {
                    // Get file extension
                    FileInfo fileInfo = new FileInfo(file.FileName);
                    string fileName = Guid.NewGuid().ToString() + fileInfo.Extension;
                    DTFile = "UserProfiles/" + fileName;
                    string fileNameWithPath = Path.Combine(path, fileName);
                    //DTFile = setPath;

                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }

                register.FinalDTFile = DTFile;
                register.Image = null;
                // Execute store procedure and handle success/failure
                var (dataSet, success, message) = _common.ExecuteStoreProcedure(register, "sp_Registeration", 11);
                if (!success)
                {
                    ViewBag.ErrorMessage = message;
                    return View("Error");
                }

                ViewBag.SuccessMessage = "File uploaded successfully";
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var (dataSet, success, message) = _common.ExecuteStoreProcedure(register, "sp_Registeration", 11);
                if (!success)
                {
                    ViewBag.ErrorMessage = message;
                    return View("Error");
                }

                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult EditProfile(int ID)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.PrimarySid)?.Value;
            var userType = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            var prm = new { ID = userId };
            var (dataSet, success, message) = _common.ExecuteStoreProcedure(prm, "sp_Registeration", 42);
            if (!success)
            {
                ViewBag.ErrorMessage = message;
                return View("Error");
            }
            var _register = dataSet.Tables[0].AsEnumerable().Select(row => new Register
            {
                ID = Convert.ToInt32(row["ID"]),
                Email = row["Email"].ToString(),
                Mobile = row["Mobile"].ToString(),
                Password = row["Password"].ToString(),
                ProfileImage = row["ProfileImage"].ToString().Replace("UserProfiles/", ""),
                FinalDTFile = row["ProfileImage"].ToString()
            }).FirstOrDefault();

            if (_register == null)
            {
                return NotFound();
            }

            return View(_register);
        }

        public IActionResult UpdateProfile(Register _register)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.PrimarySid)?.Value;
            var userType = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out int Id))
            {
                _register.ID = Id;
            }

            if (_register.Image != null && _register.Image.Length > 0)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserProfiles");
                string setPath = "";
                string DTFile = "";

                // Create folder if it doesn't exist
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                // Handle the single image file
                var file = _register.Image;
                if (file.Length > 0)
                {
                    // Get file extension
                    FileInfo fileInfo = new FileInfo(file.FileName);
                    string fileName = Guid.NewGuid().ToString() + fileInfo.Extension;
                    DTFile = "UserProfiles/" + fileName;
                    string fileNameWithPath = Path.Combine(path, fileName);
                    //DTFile = setPath;

                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }

                _register.FinalDTFile = DTFile;
                _register.Image = null;
                // Execute store procedure and handle success/failure
                var (dataSet, success, message) = _common.ExecuteStoreProcedure(_register, "sp_Registeration", 21);
                if (!success)
                {
                    ViewBag.ErrorMessage = message;
                    return View("Error");
                }

                ViewBag.SuccessMessage = "File uploaded successfully";
                return RedirectToAction("Login", "Account");
            }
            else {
                var (dataSet, success, message) = _common.ExecuteStoreProcedure(_register, "sp_Registeration", 21);
                if (!success)
                {
                    ViewBag.ErrorMessage = message;
                    return View("Error");
                }
            }
            
            
            return RedirectToAction("SuccessfullyUpdate", "Account");
        }
        public IActionResult SuccessfullyUpdate()
        {
            return View();
        }


    }
}
