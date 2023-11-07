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

        public async Task<UserModel> GetPostOwnerModelAsync(string ownerId)
        {
            UserModel user = new UserModel();
            DocumentReference documentReference = _firestoreDb.Collection("users").Document(ownerId);
            DocumentSnapshot documentSnapshot = await documentReference.GetSnapshotAsync();

            if (documentSnapshot.Exists)
            {
                Dictionary<string, object> snapshotDic = documentSnapshot.ToDictionary();
                string json = JsonConvert.SerializeObject(snapshotDic);
                user = JsonConvert.DeserializeObject<UserModel>(json);
            }
            return user;
        }

        public async Task<List<PostModel>> GetPostModelsAsync()
        {
            Query postQuery = _firestoreDb.Collection("posts");
            QuerySnapshot postQuerySnapshot = await postQuery.OrderByDescending("timestamp").GetSnapshotAsync();
            List<PostModel> listPosts = new List<PostModel>();

            foreach (DocumentSnapshot snapshot in postQuerySnapshot.Documents)
            {
                if (snapshot.Exists)
                {
                    Dictionary<string, object> user = snapshot.ToDictionary();
                    string json = JsonConvert.SerializeObject(user);
                    PostModel posts = JsonConvert.DeserializeObject<PostModel>(json);
                    posts.PostOwner = await GetPostOwnerModelAsync(posts.OwnerId);
                    listPosts.Add(posts);
                }
            }
            return listPosts;
        }

        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("_UserToken");
            // Check if the user is authenticated
            if (token != null)
            {
                try
                {
                    // Get all the posts on the database and display on page
                    List<PostModel> posts = await GetPostModelsAsync();
                    return View(posts);
                }
                catch (Exception e)
                {
                    return View("Error");
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult Explore()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ExploreAsync()
        {
            try
            {
                List<PostModel> posts = await GetPostModelsAsync();
                posts.Reverse();
                return View(posts);
            }
            catch (Exception e)
            {
                return View("Error");
            }
        }

        public IActionResult Notifications()
        {
            return View();
        }

        public IActionResult Message()
        {
            return View();
        }

        public IActionResult Profile()
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