using Comp1640.Data;
using Comp1640.Models;
using Comp1640.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Comp1640.Areas.QA_Coordinator.Controllers
{
    [Area(SD.Area_QA_COORDINATOR)]
    public class IdealsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public IdealsController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        // GET: IdealsController
        public async Task<IActionResult> List()
        {
            var ideas = _db.Ideas.Include(i => i.Category).Include(i => i.Topic).Include(i => i.User).AsNoTracking();
            return View(await ideas.ToListAsync());
        }

        // GET: IdealsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: IdealsController/Create
        public IActionResult Create()
        {
            PopulateCategoriesDropDownList();
            PopulateTopicsDropDownList();
            return View();
        }

        // POST: IdealsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Content,FilePath,CreatedDate, CategoryID, TopicID")] Idea idea)
        {
            if (idea != null)
            {
                var content = idea.Content;
                var count = _db.Ideas.Where(i => i.Content.Contains(content)).Count();
                idea.CreatedDate = System.DateTime.Now;
                string thisUser = _userManager.GetUserId(HttpContext.User);
                idea.UserID = thisUser;
                if (content == null)
                {
                    ViewBag.message = "Content is not null";
                    return RedirectToAction(nameof(Create));
                }
                else if (count < 0)
                {
                    ViewBag.message = "Content is exist";
                    return RedirectToAction(nameof(Create));
                }

                _db.Add(idea);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }
            PopulateCategoriesDropDownList(idea.CategoryID);
            PopulateTopicsDropDownList(idea.TopicID);
            return View(idea);
        }

        // GET: IdealsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: IdealsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(List));
            }
            catch
            {
                return View();
            }
        }

        // GET: IdealsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: IdealsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(List));
            }
            catch
            {
                return View();
            }
        }
        private void PopulateCategoriesDropDownList(object selectedCategory = null)
        {
            var cartegoriesQuery = from d in _db.Categories
                                   orderby d.Name
                                   select d;
            ViewBag.CategoryID = new SelectList(cartegoriesQuery.AsNoTracking(), "Id", "Name", selectedCategory);
        }
        private void PopulateTopicsDropDownList(object selectedTopic = null)
        {
            var topicsQuery = from t in _db.Topics
                              orderby t.Name
                              select t;
            ViewBag.TopicID = new SelectList(topicsQuery.AsNoTracking(), "Id", "Name", selectedTopic);
        }
    }
}
