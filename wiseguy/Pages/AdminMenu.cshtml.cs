using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace wiseguy.Pages
{
    public class AdminMenuModel : PageModel
    {
        private string link = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "link", "link.json");
        public List<SubjectGetSets> list = new List<SubjectGetSets>();
        public bool isAdd = false;
        public bool DisplayDetails { get; private set; }

        
       
        public async Task OnGetAsync()
        {
            DisplayDetails = await isDisplayDetails();
            list.Sort((y, x) => y.Date.CompareTo(x.Date));
        }

        public async Task OnPost()
        {
            
          //  string fullname = Request.Form["buttonPosition"];
            string Course = Request.Form["Course"];
            string StudentID = Request.Form["StudentID"];
            string CompletedDate = Request.Form["CompletedDate"];
            string CertificateNo = Request.Form["CertificateNo"];
            string Date = Request.Form["Date"];
            string sysID = Request.Form["sysID"];

            await stored(  sysID);
 

        }

        public async Task<bool> isDisplayDetails()
        {
            string projectId = "wiseguy-50b00";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", link);
            FirestoreDb db = FirestoreDb.Create(projectId);


            CollectionReference machinesCollection = db.Collection("Details")
                                    .Document("Wiseguy")
                                    .Collection("StudentCertificate");



            QuerySnapshot snapshot = await machinesCollection.GetSnapshotAsync();

            if (snapshot.Count > 0)
            {
                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    if (document.Exists)
                    {
                        Dictionary<string, object> data = document.ToDictionary();

                        // Check if the keys exist before accessing them



                        string? FullName = data.ContainsKey("FullName") ? data["FullName"]?.ToString() : "Not available";
                        string? StudentID = data.ContainsKey("StudentID") ? data["StudentID"]?.ToString() : "0";
                        string? ID = document.Id;
                        string? Course = data.ContainsKey("Course") ? data["Course"]?.ToString() : "Not assigned";
                        string? CompletedDate = data.ContainsKey("CompletedDate") ? data["CompletedDate"]?.ToString() : "Not assigned";
                        string? CertificateNo = data.ContainsKey("CertificateNo") ? data["CertificateNo"]?.ToString() : "Not assigned";
                        string? date = data.ContainsKey("Date") ? data["Date"]?.ToString() : "Not assigned";
                        string? Status = data.ContainsKey("Status") ? data["Status"]?.ToString() : "0";



                        SubjectGetSets addGrade = new SubjectGetSets(ID,FullName, StudentID, Course, CompletedDate, CertificateNo, date, ID, Status);
                        list.Add(addGrade);
                    }

                }
                return true;
            }

            return false;
        }
       

        public async Task stored( string sysID)
        {
            bool success = false;
            string projectId = "wiseguy-50b00"; // Replace with your project ID
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", link);

            FirestoreDb db = FirestoreDb.Create(projectId);




            DocumentReference docRef = db.Collection("Details")
                                      .Document("Wiseguy")
                                      .Collection("StudentCertificate")
                                      .Document(sysID);

            try
            {

                Dictionary<string, object> user = new Dictionary<string, object>
        {
           // { "FullName", fullname  },
           // { "StudentID", StudentID },
           // { "Course", Course },
            //{ "CompletedDate", CompletedDate },
            //{ "CertificateNo", CertificateNo },

           // { "Date", Date },
            { "Status", "1" },



        };

                await docRef.UpdateAsync(user);

                success = true;



            }
            catch (Exception ex)
            {
                success = false;

            }
        }
    }
}
public class SubjectGetSets
{
    public SubjectGetSets(string verifyID, string fullName,string studentID, string course, string completedDate, string certificateNo, string date, string id, string status)
    {

        VerifyID = verifyID;
        FullName = fullName;
        StudentID = studentID;
        Course = course;
        CompletedDate = completedDate;
        CertificateNo = certificateNo;
        Date = date;
        this.id = id;
        Status = status;
    }

    public string VerifyID { get; set; }
    public string FullName { get; set; }
    public string StudentID { get; set; }
    public string Course { get; set; }
    public string CompletedDate { get; set; }
    public string CertificateNo { get; set; }
    public string Date { get; set; }
    public string id { get; set; }
    public string Status { get; set; }








}