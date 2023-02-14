using Comp1640.Data;
using Comp1640.Models;
using Comp1640.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System.Linq;
using System.Threading.Tasks;

namespace Comp1640.Areas.QA_Coordinator.Controllers
{
    [Area(SD.Area_QA_COORDINATOR)]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoriesController(ApplicationDbContext db)
        {
            _db = db;
        }


        [HttpGet]
        // GET: CategoriesController
        public IActionResult List()
        {
            var data = _db.Categories.ToList();
            if(data == null)
            {
                ViewBag.message = "Category is null";
                return RedirectToAction(nameof(List));
            }
            return View(data);
        }

        // GET: CategoriesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CategoriesController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CategoriesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                var name = category.Name;
                if(name != null)
                {
                    var count = _db.Categories.Where(c => c.Name.Contains(name)).Count();
                    if (count > 0)
                    {
                        ViewBag.message = "Name Category already exists";
                        return View();

                    }
                    _db.Add(category);
                    _db.SaveChanges();
                    ViewBag.Message = "Add Category successfully";
                    return RedirectToAction(nameof(List));
                }
            }
            ViewBag.message = "Insert failed!";
            return View(category);
        }

        [HttpGet]
        // GET: CategoriesController/Edit/5
        public IActionResult Update(int id)
        {
            if(id < 0)
            {
                ViewBag.meesage = "Id Category not found";
                return RedirectToAction(nameof(List));

            }
            else
            {
                var data = _db.Categories.Where(c => c.Id == id).SingleOrDefault();
                return View(data);
            }
        }

        // POST: CategoriesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Category category)
        {
            if(id < 0)
            {
                return NotFound();
            }
            else
            {
                var data = _db.Categories.FirstOrDefault(c => c.Id == id);
                if(data != null)
                {
                    data.Name = category.Name;
                    _db.SaveChanges();
                    return RedirectToAction(nameof(List));
                }
                return View();
            }
        }


        // POST: CategoriesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var category = await _db.Categories.FindAsync(id);
            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(List));
            
        }
    }
}
