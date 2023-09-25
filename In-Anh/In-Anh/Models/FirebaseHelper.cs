using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;

namespace In_Anh.Models
{
    public static class FirebaseHelper
    {
        static string filePath = "D:\\config.json";
        public static FirestoreDb? Database {  get; private set; }
        public static void SetEnviromentVar()
        {
            //filePath = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetRandomFileName())) + ".json";
            //File.WriteAllText(filePath, filecf);    
            File.SetAttributes(filePath, FileAttributes.Hidden);
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",filePath);
            Database = FirestoreDb.Create("jin-nie");
            if (FirebaseApp.GetInstance("jin-nie") == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(filePath),
                },"jin-nie");
            }
        }


    }
}
