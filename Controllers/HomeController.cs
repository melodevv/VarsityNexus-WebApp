using Firebase.Auth;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using VarsityNexusApp.Models;

namespace VarsityNexusApp.Controllers
{
    public class HomeController : Controller
    {
        private string directory = "varsity-nexus-843009bba24e.json";
        private string projectId;
        private FirestoreDb _firestoreDb;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", directory);
            projectId = "varsity-nexus";
            _firestoreDb = FirestoreDb.Create(projectId);
        }

        public IActionResult Index()
        {
            //var token = HttpContext.Session.GetString("_UserToken");
            //if (token != null)
            //{
            return View();
            //}
            //else
            //{
            //    return RedirectToAction("Login", "Account");
            //}
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<List<UserModel>> GetSearchAsync(string query)
        {
            Query usersQuery = _firestoreDb.Collection("users");
            QuerySnapshot userQuerySnapshot = await usersQuery.WhereEqualTo("username", query).GetSnapshotAsync();
            List<UserModel> listUsers = new List<UserModel>();

            foreach (DocumentSnapshot snapshot in userQuerySnapshot.Documents)
            {
                if (snapshot.Exists)
                {
                    Dictionary<string, object> user = snapshot.ToDictionary();
                    string json = JsonConvert.SerializeObject(user);
                    UserModel newUser = JsonConvert.DeserializeObject<UserModel>(json);
                    newUser.Id = snapshot.Id;
                    listUsers.Add(newUser);
                }
            }
            return listUsers;
        }

        [HttpPost]
        public async Task<PartialViewResult> Search(string query)
        {
            if (query != null)
            {
                try
                {
                    List<UserModel> searchList = await GetSearchAsync(query);

                    return PartialView("_SearchResultsPartial", searchList);
                }
                catch (Exception e)
                {
                    ErrorViewModel error = new ErrorViewModel();
                    error.RequestId = e.Message;
                    PartialView("Error", error);
                }
            }
            return PartialView("Error");
        }

        public async Task<List<UserModel>> GetFollowSuggestAsync()
        {
            Query usersQuery = _firestoreDb.Collection("users");
            QuerySnapshot userQuerySnapshot = await usersQuery.GetSnapshotAsync();
            List<UserModel> listUsers = new List<UserModel>();

            foreach (DocumentSnapshot snapshot in userQuerySnapshot.Documents)
            {
                if (snapshot.Exists)
                {
                    Dictionary<string, object> user = snapshot.ToDictionary();
                    string json = JsonConvert.SerializeObject(user);
                    UserModel newUser = JsonConvert.DeserializeObject<UserModel>(json);
                    newUser.Id = snapshot.Id;
                    listUsers.Add(newUser);
                }
            }
            return listUsers;
        }

        [HttpGet]
        public async Task<PartialViewResult> FollowSuggest()
        {

            try
            {
                List<UserModel> followSuggestList = await GetFollowSuggestAsync();

                return PartialView("_FollowSuggestPartial", followSuggestList);
            }
            catch (Exception e)
            {
                ErrorViewModel error = new ErrorViewModel();
                error.RequestId = e.Message;
                PartialView("Error", error);
            }
            return PartialView("Error");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}