using Firebase.Auth;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using VarsityNexusApp.Model;
using VarsityNexusApp.Models;

namespace VarsityNexusApp.Controllers
{
    public class PartialsController : Controller
    {
        private string directory = "varsity-nexus-843009bba24e.json";
        private string projectId;
        private FirestoreDb _firestoreDb;

        public PartialsController()
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", directory);
            projectId = "varsity-nexus";
            _firestoreDb = FirestoreDb.Create(projectId);
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
            QuerySnapshot userQuerySnapshot = await usersQuery.WhereNotEqualTo("id", GlobalVariables.CurrentUser.LocalId).GetSnapshotAsync();
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

        [HttpGet]
        public async Task<PartialViewResult> UserProfile()
        {

            try
            {
                DocumentReference documentReference = _firestoreDb.Collection("users").Document(GlobalVariables.CurrentUser.LocalId);
                DocumentSnapshot documentSnapshot = await documentReference.GetSnapshotAsync();

                if (documentSnapshot.Exists)
                {
                    Dictionary<string, object> snapshotDic = documentSnapshot.ToDictionary();
                    string json = JsonConvert.SerializeObject(snapshotDic);
                    UserModel user = JsonConvert.DeserializeObject<UserModel>(json);
                    return PartialView("_UserProfilePartial", user);
                }
                return PartialView("Error");
            }
            catch (Exception e)
            {
                ErrorViewModel error = new ErrorViewModel();
                error.RequestId = e.Message;
                PartialView("Error", error);
            }
            return PartialView("Error");
        }


    }
}