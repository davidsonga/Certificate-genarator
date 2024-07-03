using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Xml.Linq;

namespace wiseguy.Pages
{
    public class AdminUserModel : PageModel
    {
        private string link = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "link", "link.json");
        public List<UserGetSet> list = new List<UserGetSet>();
        public bool isAdd = false;
       
        public bool DisplayDetails { get; private set; }
        
        public async Task OnGetAsync()
        {
            DisplayDetails = await isDisplayDetails();
            list.Sort((x, y) => x.date.CompareTo(y.date));
        }

        public async Task<bool> isDisplayDetails()
        {
            string projectId = "wiseguy-50b00";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", link);
            FirestoreDb db = FirestoreDb.Create(projectId);


            CollectionReference machinesCollection = db.Collection("Details")
                                    .Document("Wiseguy")
                                    .Collection("Login");
             

            

            QuerySnapshot snapshot = await machinesCollection.GetSnapshotAsync();

            if (snapshot.Count > 0)
            {
                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    if (document.Exists)
                    {
                        Dictionary<string, object> data = document.ToDictionary();

                        // Check if the keys exist before accessing them



                        string? role = data.ContainsKey("role") ? data["role"]?.ToString() : "Not available";
                        string? email = data.ContainsKey("email") ? data["email"]?.ToString() : "0";
                        string? ID = document.Id;
                        string? name = data.ContainsKey("name") ? data["name"]?.ToString() : "Not assigned";
                        string? surname = data.ContainsKey("surname") ? data["surname"]?.ToString() : "Not assigned";
                        string? password = data.ContainsKey("password") ? data["password"]?.ToString() : "Not assigned";
                        string? date = data.ContainsKey("Date") ? data["Date"]?.ToString() : "Not assigned";
                         
                       



                        UserGetSet addUser = new UserGetSet(role, email, name, surname, password, date, ID);
                        list.Add(addUser);
                    }

                }
                return true;
            }

            return false;
        }
    }
}
public class UserGetSet
{
    public UserGetSet(string role, string email, string name, string surname, string password, string date, string id)
    {
        this.role = role;
        this.email = email;
        this.name = name;
        this.surname = surname;
        this.password = password;
        this.date = date;
        this.id = id;
    }

    public string role { get; set; }
    public string email { get; set; }
    public string name { get; set; }
    public string surname { get; set; }
    public string password { get; set; }
    public string date { get; set; }
    public string id { get; set; }
 

}
