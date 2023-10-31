using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VarsityNexusApp.Models
{
    [FirestoreData]
    public class UserModel
    {
        public string UserId { get; set; }

        [FirestoreProperty]
        public string DisplayName { get; set; }

        [FirestoreProperty]
        public string Email { get; set; }

        [FirestoreProperty]
        public string Username { get; set; }

        [FirestoreProperty]
        public string PhotoUrl { get; set; }

        [FirestoreProperty]
        public string Location { get; set; }

        [FirestoreProperty]
        public string Bio { get; set; }

        [FirestoreProperty]
        public Timestamp SignedUpAt { get; set; }

        [FirestoreProperty]
        public Timestamp LastSeen { get; set; }

        [FirestoreProperty]
        public bool IsOnline { get; set; }

    }
}