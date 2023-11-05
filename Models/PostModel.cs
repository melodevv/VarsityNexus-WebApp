using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VarsityNexusApp.Models
{
    [FirestoreData]
    public class PostModel
    {
        [FirestoreProperty]
        public string PostId { get; set; }

        [FirestoreProperty]
        public string OwnerId { get; set; }

        [FirestoreProperty]
        public string Description { get; set; }

        [FirestoreProperty]
        public string Username { get; set; }

        [FirestoreProperty]
        public Timestamp Timestamp { get; set; }

    }
}