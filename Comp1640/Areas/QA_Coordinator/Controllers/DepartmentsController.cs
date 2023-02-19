using Comp1640.Data;
using Comp1640.Models;
using Comp1640.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Comp1640.Areas.QA_Coordinator.Controllers
{
    [Area(SD.Area_QA_COORDINATOR)]
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
                ViewBag.message = "Department is null";
                return RedirectToAction(nameof(List));
            }
            return View(data);
        }

        // GET: DepartmentsController/Details/5
        public IActionResult Details(int id)
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
            if (ModelState.IsValid)
            {
                var name = department.Name;
                if (name != null)
                {
                    var count = _db.Departments.Where(c => c.Name.Contains(name)).Count();
                    if (count > 0)
                    {
                        ViewBag.message = "Name Department already exists";
                        return View();

                    }
                    _db.Add(department);
                    _db.SaveChanges();
                    ViewBag.Message = "Add Category successfully";
                    return RedirectToAction(nameof(List));
                }
            }
            ViewBag.message = "Insert failed!";
            return View(department);
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
            if (id < 0)
            {
                return NotFound();
            }
            else
            {
                var data = _db.Departments.FirstOrDefault(c => c.Id == id);
                if (data != null)
                {
                    data.Name = department.Name;
                    _db.SaveChanges();
                    return RedirectToAction(nameof(List));
                }
                return View();
            }
        }


        // POST: DepartmentsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var department = await _db.Departments.FindAsync(id);
            if(department != null)
            {
                _db.Departments.Remove(department);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(List));

            }

            return RedirectToAction(nameof(List));

        }
    }
}
