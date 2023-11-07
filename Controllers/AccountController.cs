using Firebase.Auth;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NuGet.Common;
using VarsityNexusApp.Model;
using VarsityNexusApp.Models;

namespace VarsityNexusApp.Controllers
{
    public class AccountController : Controller
    {
        FirebaseAuthProvider auth;
        private string directory = "varsity-nexus-843009bba24e.json";
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

                    // get the user token
                    string token = fbAuthLink.FirebaseToken;

                    // get the current logged in user
                    User currentUser = await auth.GetUserAsync(token);

                    // Set the current logged in user
                    GlobalVariables.CurrentUser = await auth.GetUserAsync(token);

                    //saving the token in a session variable
                    if (token != null)
                    {
                        HttpContext.Session.SetString("_UserToken", token);

                        //create the user collection on users collection
                        UserModel user = new UserModel
                        {
                            DisplayName = registerModel.Name,
                            Username = "",
                            Email = registerModel.Email,
                            Bio = "",
                            PhotoUrl = "https://saiuniversity.edu.in/wp-content/uploads/2021/02/default-img.jpg",
                            Location = "",
                            DateOfBirth = "",
                            StudyLevel = "",
                            StudyYear = "",
                            Institution = "",
                            Gender = "",
                            Id = currentUser.LocalId,
                            IsOnline = false,
                            LastSeen = Timestamp.GetCurrentTimestamp(),
                            SignedUpAt = Timestamp.GetCurrentTimestamp()
                        };

                        // create the user document using the User's ID
                        DocumentReference docRef = _firestoreDb.Collection("users").Document(currentUser.LocalId);
                        await docRef.SetAsync(user);

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
            // Check if the form entry is valid
            if (ModelState.IsValid)
            {
                try
                {
                    //log in the user
                    var fbAuthLink = await auth
                                    .SignInWithEmailAndPasswordAsync(loginModel.Email, loginModel.Password);

                    string token = fbAuthLink.FirebaseToken;

                    // set the current logged in user
                    GlobalVariables.CurrentUser = await auth.GetUserAsync(token);


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
