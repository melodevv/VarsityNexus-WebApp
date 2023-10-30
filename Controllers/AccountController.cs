using Firebase.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
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

        // Register User
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
            // TODO: Find a way to display the error messages in a popup style
            catch (Exception e)
            {
                // ViewBag.Exception = ExceptionErrors(e);
                //Create a view for displaying errors and pass the exception to it
                return View();
            }
        }

        // Login The User
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
            }
            // TODO: Find a way to display the error messages in a popup style
            catch (Exception e)
            {

                // ViewBag.Exception = ExceptionErrors(e);
                foreach (var i in e.Message.Split("{}"))
                {
                    Console.WriteLine(i);
                }
                //Create a view for displaying errors and pass the exception to it
                return View();
            }
        }

        // Reset Password
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPasword(UserModel userModel)
        {
            try
            {
                //Send Reset Password Email to the user
                await auth.SendPasswordResetEmailAsync(email: userModel.Email);

                //Take user to login page
                return RedirectToAction("Login");

            }
            // TODO: Find a way to display the error messages in a popup style
            catch (Exception e)
            {
                // ViewBag.Exception = ExceptionErrors(e.Data);
                //Create a view for displaying errors and pass the exception to it
                return View();
            }
        }

        // Logout the user
        public IActionResult LogOut()
        {
            HttpContext.Session.Remove("_UserToken");
            return RedirectToAction("Login");
        }

        // String ExceptionErrors(dynamic e)
        // {
        //     if (e.Contains("INVALID_LOGIN_CREDENTIALS"))
        //     {
        //         return "Invalid login credentials";
        //     }
        //     return "DIdint work";
        // }

    } // End class
}
