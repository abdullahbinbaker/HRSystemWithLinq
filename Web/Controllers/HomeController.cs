using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using Core.Entity;
using Infrastructure;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Web.ViewsModels;

namespace Web.Controllers
{

    public class HomeController : Controller
    {
        private readonly UnitOfWork uow;
        private readonly IConfiguration _config;
        public HomeController(UnitOfWork unitOfWork, IConfiguration config)
        {
            uow = unitOfWork;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult EmployeeDetails()
        {
            try
            {
                int userId = (int)HttpContext.Session.GetInt32("UserId");
                ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
                EmployeeDetailsViewModel employeeInfo = new EmployeeDetailsViewModel
                {
                    employee = uow.Employees.GetById(userId),
                    password = uow.Logins.GetById(userId).Password
                };
                Log("Employee Details Method: Employee Details returned Succesfully");
                return View(employeeInfo);
            }
            catch (Exception exe)
            {
                return View(ViewBag.Error = exe.Message.ToString());
            }
        }

        public IActionResult ShowManagerDetails()
        {
            try
            {
                int userId = (int)HttpContext.Session.GetInt32("UserId");
                ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
                int managerId = (int)HttpContext.Session.GetInt32("ManagerId");
                ManagerDetailsViewModel managerInfo = new ManagerDetailsViewModel
                {
                    employee = uow.Employees.GetById(managerId),
                };
                Log("ShowManagerDetails Method: Manager's Details returned Succesfully");
                return View(managerInfo);
            }
            catch (Exception exe)
            {
                Log(exe.Message);
                return View(ViewBag.Error = exe.Message.ToString());                
            }
        }

        public IActionResult LoginToSystem(string emailAddress, string password)
        {
            LoginViewModel loginViewModel = new LoginViewModel
            {
                employee = uow.Employees.CheckLogin(emailAddress, password)
            };

            if (loginViewModel.employee != null)
            {
                if (loginViewModel.employee.Role.Equals("HR"))
                {
                    HttpContext.Session.SetInt32("UserId", Convert.ToInt32(loginViewModel.employee.Id));
                    HttpContext.Session.SetString("UserRole", loginViewModel.employee.Role.ToString());
                    HttpContext.Session.SetInt32("ManagerId", Convert.ToInt32(loginViewModel.employee.ManagerId));
                    ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
                    ViewBag.ActionMethodName = "ListEmployeesVacationsRequests";
                    HolidayViewModel employeesRequests = new HolidayViewModel
                    {
                        MyHolidaysList = uow.Holidays.DisplayEmployeesHolidayRequests("UnderProcessing").ToList()
                    };
                    return View("ListVacationsPage", employeesRequests);
                }
                else
                {
                    HttpContext.Session.SetInt32("UserId", Convert.ToInt32(loginViewModel.employee.Id));
                    HttpContext.Session.SetString("UserRole", loginViewModel.employee.Role.ToString());
                    HttpContext.Session.SetInt32("ManagerId", Convert.ToInt32(loginViewModel.employee.ManagerId));
                    ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
                    int userId = (int)HttpContext.Session.GetInt32("UserId");
                    HolidayViewModel MyHolidyas = new HolidayViewModel
                    {
                        MyHolidaysList = uow.Holidays.GetHolidaysByEmployeeId(userId).ToList()
                    };
                    return View("ListVacationsPage", MyHolidyas);
                }
            }
            else
            {
                Log("Login Failed");
                ViewBag.WarningMessage = "Your Email or Password Wrong Please Try Again";
                return View("index", ViewBag.WarningMessage);
            }
        }

        public IActionResult UpdateAccount(int employeeId, string email, string password)
        {
            try
            {
                string userId = HttpContext.Session.GetString("UserId");
                ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
                var item = uow.Employees.GetById(employeeId);
                if (item != null)
                {
                    Login login = new Login(employeeId, email, password);
                    uow.Logins.Update(login);
                    uow.Save();
                    Log("Information Updated successfully");
                }
                else
                {
                    Log("You dont have account please register first");
                }

                return View("RequestVacation");
            }
            catch (Exception exe)
            {
                Log("Updating information Failed due to: "+exe.Message);
                return View("RequestVacation");               
            }
        }

        [HttpGet]
        public IActionResult CreateAccount()
        {
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");

            return View();
        }

        [HttpPost]
        public IActionResult CreateAccount(string employeeName, string email, string role, DateTime employeeBirthDate, string mobileNumber, string gender, string password, int managerId)
        {
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
            int managerIdNumber = (int)HttpContext.Session.GetInt32("UserId");

            try
            {
                Employee employee = new Employee(employeeName, email, mobileNumber, gender, role, employeeBirthDate, managerIdNumber, 30);
                //MailMessage mail = new MailMessage();
                //SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                //mail.From = new MailAddress(_config.GetValue<string>("UserKeys:senderEmail"));
                //mail.To.Add(email);
                //mail.Subject = "Test Mail";
                //mail.Body = "welcome mr/mrs: " + employeeName + " your password is : " + password;
                //SmtpServer.Port = 587;
                //SmtpServer.Credentials = new System.Net.NetworkCredential(_config.GetValue<string>("UserKeys:senderEmail"), _config.GetValue<string>("UserKeys:senderMailPassword"));
                //SmtpServer.EnableSsl = true;
                //SmtpServer.Send(mail);
                //Log("mail Sent succefully");
                uow.Employees.Insert(employee);
                uow.Save();
                Log("Employee created successfully");
                Employee lastInsertedEmployee = uow.Employees.SearchForEmployeeByEmail(email);
                Login login = new Login(lastInsertedEmployee.Id, email, password);
                uow.Logins.Insert(login);
                uow.Save();
                Log("Login Information Created Successfully");
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
            return View();
        }

        [HttpGet]
        public IActionResult RequestVacation()
        {
            try
            {
                int userId = (int)HttpContext.Session.GetInt32("UserId");
                ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
                return View();
            }
            catch (Exception exe)
            {
                Log("while sending vacation request this error Occured: "+exe.Message);
                return View(ViewBag.Error = exe.Message.ToString());
            }
        }

        [HttpPost]
        public IActionResult RequestVacation(DateTime StartDate, DateTime EndDate, String Reason)
        {
            try
            {
                int userId = (int)HttpContext.Session.GetInt32("UserId");
                ViewBag.UserRole = HttpContext.Session.GetString("UserRole");

                Holiday holiday = new Holiday(userId, StartDate, EndDate, "UnderProcessing", Reason);
                uow.Holidays.Insert(holiday);
                uow.Save();
                Log("Vacation Request sent Succefully");
                return View();
            }
            catch (Exception exe)
            {
                Log("while sending vacation request this error has Occured: " + exe.Message);
                return View(ViewBag.Error = exe.Message.ToString());
            }
        }

        public IActionResult ListEmployeeVacations()
        {
            try
            {
                int userId = (int)HttpContext.Session.GetInt32("UserId");
                ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
                ViewBag.ActionMethodName = "ListEmployeeVacations";
                HolidayViewModel MyHolidyas = new HolidayViewModel
                {
                    MyHolidaysList = uow.Holidays.GetHolidaysByEmployeeId(userId).ToList()
                };

                return View("ListVacationsPage", MyHolidyas);
            }
            catch (Exception exe)
            {
                Log("OOPS while Requesting vacations' information this error Occured: " + exe.Message);
                return View("ListVacationsPage", ViewBag.Error = exe.Message.ToString());
            }
        }

        public IActionResult ListEmployeeVacationsByStatus(string status)
        {
            try
            {
                int userId = (int)HttpContext.Session.GetInt32("UserId");
                ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
                ViewBag.ActionMethodName = "ListEmployeeVacationsByStatus";
                HolidayViewModel MyHolidyas = new HolidayViewModel
                {
                    MyHolidaysList = uow.Holidays.ListVacationsByEmployeeIdAndStatus(userId, status).ToList()
                };

                return View("ListVacationsPage", MyHolidyas);
            }
            catch (Exception exe)
            {
                Log("OOPS while sending vacation request this error Occured: " + exe.Message);
                return View("ListVacationsPage", ViewBag.Error = exe.Message.ToString());
            }
        }

        public IActionResult ListEmployeesVacationsRequests()
        {
            try
            {
                ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
                ViewBag.ActionMethodName = "ListEmployeesVacationsRequests";
                HolidayViewModel employeesRequests = new HolidayViewModel
                {
                    MyHolidaysList = uow.Holidays.DisplayEmployeesHolidayRequests("UnderProcessing").ToList()
                };
                return View("ListVacationsPage", employeesRequests);
            }
            catch (Exception exe)
            {
                Log("OOPS we couldnt List your Employees' vacations Requests due to : " + exe.Message);
                return View("ListVacationsPage", ViewBag.Error = exe.Message.ToString());
            }
        }

        public IActionResult UpdateHolidayStatus(int holidayId, string holidayStatus)
        {
            try
            {
                int userId = (int)HttpContext.Session.GetInt32("UserId");
                ViewBag.UserRole = HttpContext.Session.GetString("UserRole");


                uow.Holidays.UpdateHolidayStatus(holidayId, holidayStatus);
                uow.Save();
                HolidayViewModel employeesRequests = new HolidayViewModel
                {
                    MyHolidaysList = uow.Holidays.DisplayEmployeesHolidayRequests(holidayStatus).ToList()
                };
                if (employeesRequests != null)
                {
                    return RedirectToAction("ListEmployeesVacationsRequests");
                }
                else
                    return View("ListEmployeesVacationsRequests");
            }
            catch (Exception exe)
            {
                return View("ListEmployeesVacationsRequests", ViewBag.Error = exe.Message.ToString());
            }
        }

        public IActionResult Logout()
        {

            HttpContext.Session.SetString("UserId", "null");
            HttpContext.Session.SetString("UserRole", "null");
            HttpContext.Session.Clear();
            Log("Session cleared succefully ");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult RetrievePasswordByEmail(string email)
        {
            try
            {
                string password = uow.Logins.RetrievePasswordByEmail(email);
                Log("Password retreived from database succefully");
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress(_config.GetValue<string>("UserKeys:senderEmail"));
                mail.To.Add(email);
                mail.Subject = "Test Mail";
                mail.Body = "welcome sir your password is : " + password;
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential(_config.GetValue<string>("UserKeys:senderEmail"), _config.GetValue<string>("UserKeys:senderMailPassword"));
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);
                Log("mail Sent");

            }
            catch (Exception exe)
            {
                Log(exe.Message.ToString());
                ViewBag.errorMessage = exe.Message.ToString();
                return View("Index");
            }
            return View("Index");
        }

        private void Log(string text)
        {
            try
            {
                string fileDirectory = _config.GetValue<string>("UserKeys:LogFileDirctroy");
                string fileTitle = "EmailNotificationService";
                if (!Directory.Exists(fileDirectory))
                    Directory.CreateDirectory(fileDirectory);
                using (StreamWriter sw = new StreamWriter(fileDirectory + '\\' + fileTitle + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt", true))
                {
                    sw.WriteLine(DateTime.Now + "  " + text);
                    sw.Flush();
                }
            }
            catch (Exception exe)
            {
                Console.Write(exe.Message);
            }
        }

        public IActionResult RetrieveManagersEmployees()
        {
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
            int userId = (int)HttpContext.Session.GetInt32("UserId");
            Manager_sEmployeesViewModel MyEmployeesList = new Manager_sEmployeesViewModel
            {
                MyEmployeesList = uow.Employees.RetrieveManagersEmployees(userId).ToList()
            };
            return View(MyEmployeesList);
        }
    }
}

