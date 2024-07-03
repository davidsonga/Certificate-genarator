using System;
using System.IO;
using System.Threading.Tasks;
using Firebase.Auth;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
 

namespace wiseguy.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private string link = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "link", "link.json");
        private string exception = "";
        public bool Admin = false;
        [BindProperty]
        public string? email { get; set; }
        [BindProperty]
        public string? password { get; set; }
        [BindProperty]
        public string? role { get; set; }
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async Task OnPostAsync()
        {
           // await Login(email,password);

            if (await Login(email, password))
            {
                TempData["SuccessMessage"] = email+" Login completed successfully!";
               // RedirectToPage("/AddUser");

            }
            else
            {
                if(exception == "Error: Undefined")
                {
                    TempData["ErrorMessage"] = "Email or password might be wrong";
                }
                else
                {
                    TempData["ErrorMessage"] = exception;
                }
               
            }
          
        }

           async Task<bool> Login( string email, string password)
        {
            bool success = false;
            // Initialize FirebaseApp
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyBGLvCCpMYq242o0JVrkDiUUIFvHZRqKeI"));





            try
            {
                // Login with email and password
                var authResult = await authProvider.SignInWithEmailAndPasswordAsync(email, password);
                // success = true;
                if (authResult != null && !string.IsNullOrEmpty(authResult.User.LocalId))
                {
                    // Get user's role from Firestore
                    string role = await GetUserRole(authResult.User.LocalId);

                    // If user's role is Admin, set success to true
                    if (role == "Admin")
                    {
                        success = true;
                        Admin = true;
                    }

                    if (role == "User")
                    {
                        success = true;
                        Admin = false;
                    }
                }
            }
            catch (FirebaseAuthException e)
            {

                exception = $"Error: {e.Reason}";

                success = false;
            }
            return success;
        }
        private async Task<string> GetUserRole(string uid)
        {
            // Firestore database instance
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", link);
            FirestoreDb db = FirestoreDb.Create("wiseguy-50b00");

            // Document reference for the user's role
            DocumentReference docRef = db.Collection("dd").Document(uid);

            // Get the user document
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            // If document exists, retrieve the user's role
            if (snapshot.Exists)
            {
                Dictionary<string, object> data = snapshot.ToDictionary();
                if (data.ContainsKey("Role"))
                {
                    return data["Role"].ToString();
                }
            }

            // Default to empty role if user document or role not found
            return "";
        }

    }
}
