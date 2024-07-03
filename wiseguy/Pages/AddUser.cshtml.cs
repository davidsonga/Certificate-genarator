using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System;
using System.Diagnostics;
using Firebase.Auth;
using System.Security.Cryptography;

namespace wiseguy.Pages
{
    public class AddUserModel : PageModel
    {
        private string link = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "link", "link.json");
        private static Random random = new Random();
        private bool success = false;
        private bool isException = false;
        private string error = "";
        private string rnd = "";
        [BindProperty]
        public string? name { get; set; }
     

        [BindProperty]
        public string? email { get; set; }
        [BindProperty]
        public string role { get; set; } // Change from string? to string
        [BindProperty]
        public string? surname { get; set; }
        [BindProperty]
        public string? password { get; set; }
        private string exception;

        public async Task OnPostAsync()
        {
            rnd = GenerateRandomString(17, 15);
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(surname))
            {
                //await stored(rnd);
                await CreateAccount(email, password);
            }

            if (success)
            {
                TempData["SuccessMessage"] = "Operation completed successfully!";
            }
            else
            {
                if (isException)
                {
                    TempData["SuccessMessage"] = "Operation completed successfully! ";
                }
                else
                {
                    if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(name) && string.IsNullOrEmpty(password) && string.IsNullOrEmpty(surname))
                    {
                        TempData["ErrorMessage"] = "All field should be completed";
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "Operation completed successfully!";
                    }
                }
            }
        }
        public static string GenerateRandomString(int lettersCount, int numbersCount)
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

            const string numbers = "0123456789";

            StringBuilder result = new StringBuilder(lettersCount + numbersCount);

            // Add random letters
            result.Append(new string(Enumerable.Repeat(letters, lettersCount)
                                .Select(s => s[random.Next(s.Length)]).ToArray()));




            // Add random numbers
            result.Append(new string(Enumerable.Repeat(numbers, numbersCount)
                                .Select(s => s[random.Next(s.Length)]).ToArray()));

            // Shuffle the result to mix letters, symbols, and numbers
            return new string(result.ToString().ToCharArray().OrderBy(s => (random.Next(2) % 2) == 0).ToArray());
        }

        public async Task stored(string id)
        {
            string projectId = "wiseguy-50b00"; // Replace with your project ID
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", link);

            FirestoreDb db = FirestoreDb.Create(projectId);




            DocumentReference docRef = db.Collection("Details")
                                      .Document("Wiseguy")
                                      .Collection("Login")
                                      .Document(id);

            try
            {

                Dictionary<string, object> user = new Dictionary<string, object>
        {
            { "role", role  },
            { "email", email },
            { "name", name },
            { "surname", surname },
            { "password", password },

            { "Date", GetCurrentDate() },


        };

                await docRef.SetAsync(user);
                success = true;



            }
            catch (Exception ex)
            {
                error = ex.ToString();
                isException = true;

            }
        }

        public static string GetCurrentDate()
        {
            DateTime now = DateTime.Now;
            string formattedDateTime = now.ToString("dd/MM/yyyy HH:mm");

            return formattedDateTime;
        }


        public async Task<bool> CreateAccount(string email, string password)
        {
            bool success = false;

            // Initialize FirebaseApp
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyBGLvCCpMYq242o0JVrkDiUUIFvHZRqKeI"));

            try
            {
                // Create user with email and password
                var authResult = await authProvider.CreateUserWithEmailAndPasswordAsync(email, password);

                if (authResult != null && !string.IsNullOrEmpty(authResult.User.LocalId))
                {
                    // Save user's role to Firestore
                    await SaveUserRole(authResult.User.LocalId);

                    success = true;
                }
            }
            catch (FirebaseAuthException e)
            {
                exception = $"Error: {e.Reason}";
                success = false;
            }

            return success;
        }

        private async Task SaveUserRole(string userId)
        {

          

        
            // Assuming you have Firestore initialized elsewhere
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", link);
            FirestoreDb db = FirestoreDb.Create("wiseguy-50b00");

            try
            {
                // Document reference for the user's role
                DocumentReference docRef = db.Collection("dd").Document(userId);

       

                Dictionary<string, string> user = new Dictionary<string, string>
        {
                  
         
            { "Role", role??"" },



        };

                await docRef.SetAsync(user);
                
            }
            catch (Exception e)
            {
                // Handle exceptions appropriately
                exception = $"Error saving user role: {e.Message}";
            }
        }

    }
}
