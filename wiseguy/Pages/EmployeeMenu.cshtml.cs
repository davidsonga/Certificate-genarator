using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.IO;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing.Imaging;

namespace wiseguy.Pages
{
    public class EmployeeMenuModel : PageModel
    {
        private string link = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "link", "link.json");
        public string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "link", "image.jpg");
        public List<SubjectGetSet> list = new List<SubjectGetSet>();
        public bool isAdd = false;
        public bool DisplayDetails { get; private set; }

        public async Task OnGetAsync()
        {
            DisplayDetails = await isDisplayDetails();
            list.Sort((x, y) => x.Date.CompareTo(y.Date));
          
        }
        [HttpPost]
        public async Task<IActionResult> OnPost()
        {
         
            string ID = Request.Form["sysID"];
            if (await isDisplayDetails())
            {
                foreach (var element in list)
                {
                    if (element.id == ID)
                    {
                        await pic(element.FullName, element.StudentID, element.Course, element.CompletedDate, element.CertificateNo);

                    }
                    
                }
            }


            return Redirect("~/image.jpeg");
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



                        SubjectGetSet addGrade = new SubjectGetSet(FullName, StudentID, Course, CompletedDate, CertificateNo, date, ID, Status);
                        list.Add(addGrade);
                    }

                }
                return true;
            }

            return false;
        }

        private async Task  pic(string name, string id, string course, string completed ,string certNumber)
        {
            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "link", "pic.jpg"); // Change path as needed
            ///string imagePath = "path/to/your/image.jpg"; // Replace with path to your image
           
            string textToWrite = "";
            if (name.Length <= 25)
            {
                textToWrite = "\n\n\n\n\n\n\n\n\n\n\n\n                                          " + name +
                              "\n                                           " + id +
                              "\n\n\n\n\n\n\n\n\n                                               " + course +
                              "\n\n\n\n                                              " + completed +
                               "\n\n\n                                  " + certNumber; ;
            }
            else
            {
                textToWrite = "\n\n\n\n\n\n\n\n\n\n\n\n                          " + name +
                               "\n                                           " + id +
                               "\n\n\n\n\n\n\n\n\n                                             " + course +
                               "\n\n\n\n                                              " + completed +
                               "\n\n\n                                  " + certNumber; ;
            }

            string outputPath = Path.Combine("wwwroot", "image.jpeg");

            int x = 100; // Adjust X coordinate as needed 
            int y = 100; // Adjust Y coordinate as needed

            WriteTextToImage(imagePath, textToWrite, outputPath, x, y);
        }

        public static void WriteTextToImage(string imagepath, string text, string outputpath, int x, int y)
        {
            try
            {
                using (var image = System.Drawing.Image.FromFile(imagepath))
                {
                    using (var graphics = Graphics.FromImage(image))
                    {
                        var font = new Font("Arial", 15, FontStyle.Bold); // Set your desired font and size here
                        var brush = new SolidBrush(Color.Black); // Set your desired color here

                        graphics.DrawString(text, font, brush, x, y);

                        image.Save(outputpath, ImageFormat.Jpeg);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        [HttpPost("DownloadImage")]
        public IActionResult DownloadImage()
        {
            // Assuming the image file is located in wwwroot/images directory
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "image.jpeg");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(); // Or return any appropriate error response
            }

            // Provide the file for download
            return PhysicalFile(filePath, "image/jpeg", "image.jpeg");
        }









    }
}
public class SubjectGetSet
{
    public SubjectGetSet(string fullName, string studentID, string course, string completedDate, string certificateNo, string date, string id, string status)
    {
        
        FullName = fullName;
        StudentID = studentID;
        Course = course;
        CompletedDate = completedDate;
        CertificateNo = certificateNo;
        Date = date;
        this.id = id;
        Status = status;
    }

    public string FullName { get; set; }
    public string StudentID { get; set; }
    public string Course { get; set; }
    public string CompletedDate { get; set; }
    public string CertificateNo { get; set; }
    public string Date { get; set; }
    public string id { get; set; }
    public string Status { get; set; }








}