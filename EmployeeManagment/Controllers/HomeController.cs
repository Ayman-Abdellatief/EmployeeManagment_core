using EmployeeManagment.Models;
using EmployeeManagment.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagment.Controllers
{
    [Authorize]

    public class HomeController :Controller
    {
        private readonly IEmployeeeRepository _employeeeRepository;
        private readonly ILogger logger;

        public IHostingEnvironment HostingEnvironment { get; }

        public HomeController(IEmployeeeRepository employeeeRepository,
            IHostingEnvironment hostingEnvironment,
            ILogger<HomeController> logger)
        {
            _employeeeRepository =  employeeeRepository;
            HostingEnvironment = hostingEnvironment;
            this.logger = logger;
        }

        [AllowAnonymous]
        public ViewResult Index()
        {
        var model = _employeeeRepository.GetAllEmployees();

            return View(model);
        }

        [AllowAnonymous]
        public ViewResult Details(int? id)
        {

            //    throw new Exception("Error in Details View");
            logger.LogTrace("Trace Log");
            logger.LogDebug("Debug Log");
            logger.LogInformation("Information Log");
            logger.LogWarning("Warning Log");
            logger.LogError("Error Log");
            logger.LogCritical("Critical Log");
            Employee employee = _employeeeRepository.GetEmployee(id.Value);
            if(employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound",id.Value);
            }
            HomeDetaisViewModel model = new HomeDetaisViewModel()
            {
                Employee = employee,
                 PageTitle = "Employee Details"
        };
            return View(model);
        }


        [HttpGet]
        public ViewResult Edit(int id)
        {
            Employee employee = _employeeeRepository.GetEmployee(id);
            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel()
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath
            };
            return View(employeeEditViewModel);

        }

        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = _employeeeRepository.GetEmployee(model.Id);
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;
                if (model.Photo != null)
                {
                

                    if (model.ExistingPhotoPath != null)
                    {
                        string filePath = Path.Combine(HostingEnvironment.WebRootPath,
                            "images", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    employee.PhotoPath = ProcessUpliedFile(model);
                }

                _employeeeRepository.Upadte(employee);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        private string ProcessUpliedFile(EmployeeCreateViewModel model)
        {
            string UniqueFileName = null;
            if (model.Photo != null)
            {
                string UploadFolder = Path.Combine(HostingEnvironment.WebRootPath, "images");
                UniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string FilePath = Path.Combine(UploadFolder, UniqueFileName);
                using (var fileStream = new FileStream(FilePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }
            }

            return UniqueFileName;
        }

        [HttpGet]
        public ViewResult Create()
        {
            return View();
            
        }

        [HttpPost]
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string UniqueFileName = ProcessUpliedFile(model);
              
                var NewEmployee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = UniqueFileName

                };

                _employeeeRepository.Add(NewEmployee);
                return RedirectToAction("details", new { id = NewEmployee.Id });
            }
            return View();
        }
    }
}
