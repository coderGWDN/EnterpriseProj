using Comp1640.Data;
using Comp1640.Models;
using Comp1640.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Comp1640.Areas.QA_Coordinator.Controllers
{
    [Area(SD.Area_QA_COORDINATOR)]
    [Authorize(Roles =SD.Role_ADMIN)]
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public DepartmentsController(ApplicationDbContext db)
        {
            _db = db;
        }
        [HttpGet]
        // GET: DepartmentsController
        public IActionResult List()
        {
            var data = _db.Departments.ToList();
            if (data == null)
            {
                ViewBag.message = "Error: Department is null";
                return RedirectToAction(nameof(List));
            }
            return View(data);
        }

        // GET: DepartmentsController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DepartmentsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Department department)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.message = "Error: Insert failed!";
                return View(department);
            }
            var name = department.Name;
            var isDepartmentNameExisted = await _db.Categories
                .AnyAsync(c => c.Name.ToLower().Trim() == department.Name.ToLower().Trim());

            if (isDepartmentNameExisted)
            {
                ViewBag.message = "Error: Name Department already exists";
                return View();

            }

            _db.Add(department);
            _db.SaveChanges();

            ViewBag.Message = "Add Department successfully";
            return RedirectToAction(nameof(List));



        }
        [HttpGet]
        // GET: DepartmentsController/Edit/5
        public IActionResult Update(int id)
        {
            if (id < 0)
            {
                ViewBag.meesage = "Id Department not found";
                return RedirectToAction(nameof(List));

            }
            else
            {
                var data = _db.Departments.Where(c => c.Id == id).SingleOrDefault();
                return View(data);
            }
        }

        // POST: DepartmentsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Department department)
        {

                var data = await _db.Departments.FirstOrDefaultAsync(c => c.Id == id);
                if (data != null)
                {
                    data.Name = department.Name;
                    _db.SaveChanges();
                    return RedirectToAction(nameof(List));
                }
                return View();
        }


        // POST: DepartmentsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var department = await _db.Departments.FindAsync(id);
            if (department != null)
            {
                _db.Departments.Remove(department);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(List));

            }

            return RedirectToAction(nameof(List));

        }
    }
}
