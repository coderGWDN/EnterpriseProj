using Comp1640.Data;
using Comp1640.Models;
using Comp1640.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comp1640.Areas.QA_Coordinator.Controllers
{
    [Area(SD.Area_QA_COORDINATOR)]
    [Authorize(Roles =SD.Role_QA_MANAGER)]
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
            if (!ModelState.IsValid)
            {
                ViewBag.message = "Error: Insert failed!";
                return View(topic);
            }
            DateTime currentDate = DateTime.Now;
            DateTime clousureDate = topic.ClosureDate;
            DateTime finalClosureDate = topic.FinalClosureDate;
            if (topic.Name == null)
            {
                ViewBag.message = "Error: Insert failed!";
                return View(topic);
            }
            int result = DateTime.Compare(currentDate, clousureDate);
            int finalResult = DateTime.Compare(clousureDate, finalClosureDate);
            var isTopicNameExisted = await _db.Topics
                .AnyAsync(c => c.Name.ToLower().Trim() == topic.Name.ToLower().Trim());
            if (isTopicNameExisted)
            {
                ViewBag.message = "Error: Name Category already exists";
                return View();

            }
            if (result > 0)
            {
                ViewBag.message = "Error: Date is not valid";
                return View();
            }
            if (finalResult > 0)
            {
                ViewBag.message = "Error: Final Clousure Date must be bigger than Closeure Date";
                return View();
            }

            _db.Add(topic);
            await _db.SaveChangesAsync();

            ViewBag.Message = "Add Topic successfully";
            return RedirectToAction(nameof(List));

        }

        [HttpGet]
        // GET: Topic/Edit/5
        public async Task<ActionResult> Update(int id)
        {
                var data = await _db.Topics.Where(c => c.Id == id).SingleOrDefaultAsync();
                return View(data);
        }

        // POST: TopicsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Topic topic)
        {
                var data = await _db.Topics.FirstOrDefaultAsync(c => c.Id == id);
                if (data != null)
                {
                    data.Name = topic.Name;
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(List));
                }
                return View();
        }

        // GET: TopicsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TopicsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var topic = await _db.Topics.FindAsync(id);
            _db.Topics.Remove(topic);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(List));
        }
    }
}
