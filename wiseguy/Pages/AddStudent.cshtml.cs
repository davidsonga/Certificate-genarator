using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;

namespace wiseguy.Pages
{
    public class AddStudentModel : PageModel
    {
        private string link = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "link", "link.json");
        private static Random random = new Random();
        private bool success = false;
        private bool isException = false;
        private string error = "";
        private string rnd = "";
        [BindProperty]
        public string? FullName { get; set; }
        [BindProperty]
        public string? StudentID { get; set; }
        [BindProperty]
        public string? Course { get; set; }
        [BindProperty]
        public string? CompletedDate { get; set; }
        [BindProperty]
        public string? CertificateNo { get; set; }


        public async Task OnPostAsync()
        {
            rnd = GenerateRandomString(17, 15);
            if (!string.IsNullOrEmpty(FullName) && !string.IsNullOrEmpty(StudentID) && !string.IsNullOrEmpty(Course) && !string.IsNullOrEmpty(CompletedDate))
            {
                await stored(rnd);
            }

            if (success)
            {
                TempData["SuccessMessage"] = "Operation completed successfully!";
            }
            else
            {
                if (isException)
                {
                    TempData["ErrorMessage"] = "An error occurred during the operation. " + error;
                }
                else
                {
                    if (string.IsNullOrEmpty(FullName) && string.IsNullOrEmpty(StudentID) && string.IsNullOrEmpty(Course) && string.IsNullOrEmpty(CompletedDate))
                    {
                        TempData["ErrorMessage"] = "All field should be completed";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "An error occurred during the operation.";
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
                                      .Collection("StudentCertificate")
                                      .Document(id);

            try
            {

                Dictionary<string, object> user = new Dictionary<string, object>
        {
            { "FullName", FullName  },
            { "StudentID", StudentID },
            { "Course", Course },
            { "CompletedDate", CompletedDate },
            { "CertificateNo", CertificateNo },

            { "Date", GetCurrentDate() },
            { "Status", "0" },



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

    }
}