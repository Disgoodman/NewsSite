using aspnet_config.Models;
using aspnet_config.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace aspnet_config.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationContext db;
        public AccountController(ApplicationContext context)
        {
            db = context;
        }

        #region Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Email_User == model.Email && u.Password_User == model.Password);
                if (user != null)
                {
                    await Authenticate(model.Email); // аутентификация
                    TestClass.TestInt = user.Id_User;
                    return RedirectToAction("Posts", "Home");
                    if(user.Id_User == 1) return RedirectToAction("Posts", "Home");
                    else return RedirectToAction("Posts", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }
        #endregion

        #region Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Email_User == model.Email);
                if (user == null)
                {
                    foreach (var item in db.Users)
                    {
                        if (item.Email_User == model.Email )
                        {
                            ModelState.AddModelError("Email_User", "This user already exists");
                        }
                    }
                    // добавляем пользователя в бд
                    db.Users.Add(new User 
                    { 
                        Email_User = model.Email, 
                        Name_User = model.name,
                        Surname_User = model.surname,
                        Password_User = model.Password });
                    await db.SaveChangesAsync();

                    await Authenticate(model.Email);

                    return RedirectToAction("Login", "Account");
                }
                else
                    ModelState.AddModelError("", "Wrong password and(or) Email");
            }
            return View(model);
        }
        #endregion

        #region Authenticate
        private async Task Authenticate(string userName)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
        #endregion

        #region Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
        #endregion
    }
}