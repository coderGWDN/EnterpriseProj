using Comp1640.Data;
using Comp1640.Models;
using Comp1640.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comp1640.Areas.QA_Coordinator.Controllers
{
    [Area(SD.Area_QA_COORDINATOR)]
    public class TopicsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public TopicsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: TopicsController
        public ActionResult List()
        {
            var data = _db.Topics.ToList();
            if (data == null)
            {
                ViewBag.message = "Topic is null";
                return RedirectToAction(nameof(List));
            }
            return View(data);
        }

        // GET: TopicsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TopicsController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TopicsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Topic topic)
        {
            if (ModelState.IsValid)
            { 
                DateTime currentDate = DateTime.Now;
                DateTime clousureDate = topic.ClosureDate;
                DateTime finalClosureDate = topic.FinalClosureDate;
                var name = topic.Name;
                if (name != null)
                {
                    var count = _db.Topics.Where(c => c.Name.Contains(name)).Count();
                    int result = DateTime.Compare(currentDate, clousureDate);
                    int finalResult = DateTime.Compare(clousureDate, finalClosureDate);
                    if (count > 0)
                    {
                        ViewBag.message = "Name Topic already exists";
                        return View();

                    }
                    else if(result > 0)
                    {
                        ViewBag.message = "Date is not valid";
                        return View();
					}
                    else if(finalResult > 0)
                    {
                        ViewBag.message = "Final Clousure Date must be bigger than Closeure Date";
                        return View();
                    }    
                    _db.Add(topic);
                    _db.SaveChanges();
                    ViewBag.Message = "Add Topic successfully";
                    return RedirectToAction(nameof(List));
                }
            }
            ViewBag.message = "Insert failed!";
            return View(topic);
        }

        [HttpGet]
        // GET: Topic/Edit/5
        public async Task<ActionResult> Update(int id)
        {
            if (id < 0)
            {
                ViewBag.meesage = "Id Topic not found";
                return RedirectToAction(nameof(List));

            }
            else
            {
                var data = _db.Topics.Where(c => c.Id == id).SingleOrDefault();
                return View(data);
            }
        }

        // POST: TopicsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Topic topic)
        {
            if (id < 0)
            {
                return NotFound();
            }
            else
            {
                var data = _db.Topics.FirstOrDefault(c => c.Id == id);
                if (data != null)
                {
                    data.Name = topic.Name;
                    _db.SaveChanges();
                    return RedirectToAction(nameof(List));
                }
                return View();
            }
        }

        // GET: TopicsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TopicsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult>  DeleteConfirmed(int id)
        {
			var topic = await _db.Topics.FindAsync(id);
			_db.Topics.Remove(topic);
			await _db.SaveChangesAsync();
			return RedirectToAction(nameof(List));
		}
    }
}
