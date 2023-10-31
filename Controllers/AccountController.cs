using Firebase.Auth;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NuGet.Common;
using VarsityNexusApp.Models;

namespace VarsityNexusApp.Controllers
{
    public class AccountController : Controller
    {
        FirebaseAuthProvider auth;
        private string directory = "C:\\Users\\l224\\Desktop\\VarsityNexus-WebApp\\varsity-nexus-843009bba24e.json";
        private string projectId;
        private FirestoreDb _firestoreDb;

        public AccountController()
        {
            auth = new FirebaseAuthProvider(
                            new FirebaseConfig("AIzaSyDd3q08TleR7jciLnMl23-pgXBPeeK2rRc"));
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", directory);
            projectId = "varsity-nexus";
            _firestoreDb = FirestoreDb.Create(projectId);
        }

        // Register User
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //create the user
                    await auth.CreateUserWithEmailAndPasswordAsync(email: registerModel.Email, password: registerModel.Password, displayName: registerModel.Name);

                    //log in the new user
                    var fbAuthLink = await auth
                                    .SignInWithEmailAndPasswordAsync(registerModel.Email, registerModel.Password);

                    string token = fbAuthLink.FirebaseToken;
                    User currentUser = await auth.GetUserAsync(token);

                    //saving the token in a session variable
                    if (token != null)
                    {
                        HttpContext.Session.SetString("_UserToken", token);

                        //create the user collection on users collection
                        UserModel user = new UserModel();
                        user.DisplayName = registerModel.Name;
                        user.Username = "";
                        user.Email = registerModel.Email;
                        user.Bio = "";
                        user.PhotoUrl = "";
                        user.Location = "";
                        user.UserId = currentUser.LocalId;
                        user.IsOnline = false;
                        user.LastSeen = Timestamp.GetCurrentTimestamp();
                        user.SignedUpAt = Timestamp.GetCurrentTimestamp();

                        CollectionReference collectionReference = _firestoreDb.Collection("users");
                        await collectionReference.AddAsync(user);

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
                    ViewBag.Exception = ExceptionErrors(e.Message);
                    //Create a view for displaying errors and pass the exception to it
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        // Login The User
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //log in the user
                    var fbAuthLink = await auth
                                    .SignInWithEmailAndPasswordAsync(loginModel.Email, loginModel.Password);

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

                    ViewBag.exception = ExceptionErrors(e.Message);

                    //Create a view for displaying errors and pass the exception to it
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        // Reset Password
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Send Reset Password Email to the user
                    await auth.SendPasswordResetEmailAsync(email: forgotPasswordModel.Email);

                    ViewBag.Success = "Please check your Inbox for further instructions";
                    return View();

                }
                // TODO: Find a way to display the error messages in a popup style
                catch (Exception e)
                {
                    ViewBag.Exception = ExceptionErrors(e.Message);
                    //Create a view for displaying errors and pass the exception to it
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        // Logout the user
        public IActionResult LogOut()
        {
            HttpContext.Session.Remove("_UserToken");
            return RedirectToAction("Login");
        }

        String ExceptionErrors(String e)
        {
            String errorMessage = "";
            if (e.Contains("INVALID_LOGIN_CREDENTIALS"))
            {
                errorMessage = "Invalid login credentials";
            }
            else if (e.Contains("TOO_MANY_ATTEMPTS_TRY_LATER"))
            {
                errorMessage = "Access to this account has been temporarily disabled due to many failed login attempts. Click Forgot Password to reset.";
            }
            else if (e.Contains("EMAIL_EXISTS"))
            {
                errorMessage = "Email already exists.";
            }
            else if (e.Contains("Response status code does not indicate success"))
            {
                errorMessage = "Please make sure your entered a correct email";
            }
            return errorMessage;
        }

    } // End class
}
