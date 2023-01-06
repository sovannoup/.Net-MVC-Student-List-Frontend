using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class StudentController : Controller
    {
        static readonly HttpClient client = new HttpClient();
        private string baseUrl = "https://localhost:7243/api/student/";
        public async Task<IActionResult> Index()
        {
            var StudentList = new List<Student>();
            try
            {
                using HttpResponseMessage response = await client.GetAsync(baseUrl + "getstudents");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                StudentList = JsonSerializer.Deserialize<List<Student>>(responseBody);
                return View(StudentList);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }

            return View(StudentList);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            if(student.name is null && student.age == 0)
            {
                return View();
            }
            var StudentList = new List<Student>();
            try
            {
                var stringContent = new StringContent(JsonSerializer.Serialize(student), Encoding.UTF8, "application/json");
                using HttpResponseMessage response = await client.PostAsync(baseUrl + "createstudent", stringContent);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                StudentList = JsonSerializer.Deserialize<List<Student>>(responseBody);

                return RedirectToAction("Index", StudentList);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            return View();
        }
        public IActionResult Edit()
        { 
            string id = Request.Query["id"];
            string name = Request.Query["name"];
            string age = Request.Query["age"];
            Student student = new Student();
            student.id = Convert.ToInt32(id);
            student.name = name;
            student.age = Convert.ToInt32(age);
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Student student)
        {
            if (student.name is null && student.age == 0)
            {
                return View();
            }
            var StudentList = new List<Student>();
            try
            {
                var stringContent = new StringContent(JsonSerializer.Serialize(student), Encoding.UTF8, "application/json");
                using HttpResponseMessage response = await client.PutAsync(baseUrl + "editstudentbyid/" + student.id, stringContent);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                StudentList = JsonSerializer.Deserialize<List<Student>>(responseBody);
                Console.WriteLine(baseUrl + "student/" + student.id);
                return RedirectToAction("Index", StudentList);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            return View();
        }


        public IActionResult Delete()
        {
            ViewBag.id = Request.Query["id"];
            Console.WriteLine("First ");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id) 
        {
            Console.WriteLine("Second One " + id);
            var StudentList = new List<Student>();
            try
            {
                using HttpResponseMessage response = await client.DeleteAsync(baseUrl + "deletestudentbyid/" + id);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                StudentList = JsonSerializer.Deserialize<List<Student>>(responseBody);
                return RedirectToAction("Index", StudentList);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            return View();
        }

        public IActionResult Details(Student student)
        {
            return View(student);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
