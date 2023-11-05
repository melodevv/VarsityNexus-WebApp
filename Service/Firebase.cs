using Google.Cloud.Firestore;

namespace VarsityNexusApp.Service
{
    class FirebaseService
    {
        private string directory = "varsity-nexus-843009bba24e.json";
        private string projectId;
        private FirestoreDb _firestoreDb;

        public void FirestroeInit()
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", directory);
            projectId = "varsity-nexus";
            _firestoreDb = FirestoreDb.Create(projectId);
        }

        public FirestoreDb FirestoreDb
        {
            get { return _firestoreDb; }
        }
    }
}
