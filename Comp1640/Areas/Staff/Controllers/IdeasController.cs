using Comp1640.Data;
using Comp1640.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Comp1640.Areas.Staff.Controllers
{
    [Area(SD.Area_STAFF)]
    public class IdeasController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public IdeasController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        // GET: IdeasController
        public async Task<IActionResult> List()
        {
            var ideas = _db.Ideas
                .Include(i => i.Category)
                .Include(i => i.Topic)
                .Include(i => i.User)
                .AsNoTracking();

            return View(await ideas.ToListAsync());
        }
        public async Task<IActionResult> Index()
        {
            var ideas = _db.Ideas
                .Include(i => i.Category)
                .Include(i => i.Topic)
                .Include(i => i.User)
                .AsNoTracking();

            return View(await ideas.ToListAsync());
        }

        // GET: IdeasController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: IdeasController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: IdeasController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        // GET: IdeasController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: IdeasController/Edit/5
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

        // GET: IdeasController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: IdeasController/Delete/5
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
    }
}
