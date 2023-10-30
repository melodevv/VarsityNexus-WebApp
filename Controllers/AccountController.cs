using Firebase.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VarsityNexusApp.Models;

namespace VarsityNexusApp.Controllers
{
    public class AccountController : Controller
    {
        FirebaseAuthProvider auth;
        public AccountController()
        {
            auth = new FirebaseAuthProvider(
                            new FirebaseConfig("AIzaSyDd3q08TleR7jciLnMl23-pgXBPeeK2rRc"));
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserModel registerModel)
        {
            try
            {
                //create the user
                await auth.CreateUserWithEmailAndPasswordAsync(email: registerModel.Email, password: registerModel.Password, displayName: registerModel.Name);

                //log in the new user
                var fbAuthLink = await auth
                                .SignInWithEmailAndPasswordAsync(registerModel.Email, registerModel.Password);

                string token = fbAuthLink.FirebaseToken;

                //saving the token in a session variable
                if (token != null)
                {
                    HttpContext.Session.SetString("_UserToken", token);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View();
                }
            }
            catch (Exception e)
            {
                //Create a view for displaying errors and pass the exception to it
                return View();
            }
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(UserModel userModel)
        {
           try
            {
                //log in the user
                var fbAuthLink = await auth
                                .SignInWithEmailAndPasswordAsync(userModel.Email, userModel.Password);
                string token = fbAuthLink.FirebaseToken;
                //saving the token in a session variable
                if (token != null)
                {
                    HttpContext.Session.SetString("_UserToken", token);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View();
                }
            }catch (Exception e)
            {
                //Create a view for displaying errors and pass the exception to it
                return View();
            }
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Remove("_UserToken");
            return RedirectToAction("Login");
        }

    } // End class
}
