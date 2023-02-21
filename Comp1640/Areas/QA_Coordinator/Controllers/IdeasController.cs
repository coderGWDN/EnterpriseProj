﻿using Comp1640.Data;
using Comp1640.Models;
using Comp1640.Utility;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles =SD.Role_QA_MANAGER + "," + SD.Role_QA_COORDINATOR)]
    public class IdeasController : BaseController
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public IdeasController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
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
        public async Task<IActionResult> Create(Idea idea)
        {
            if (!ModelState.IsValid)
            {
                PopulateCategoriesDropDownList(idea.CategoryID);
                PopulateTopicsDropDownList(idea.TopicID);
                return View(idea);
            }

            var content = idea.Content;
            var count = _db.Ideas.Where(i => i.Content.Contains(content)).Count();
            idea.CreatedDate = System.DateTime.Now;
            string thisUser = GetUserId();
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

            _db.Ideas.Add(idea);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(List));

        }

        // GET: IdealsController/Edit/5
        public async Task<IActionResult> Update(int id)
        {
            var idea = await _db.Ideas.FindAsync(id);
            if (idea == null)
            {
                return NotFound();
            }
            PopulateCategoriesDropDownList(idea.CategoryID);
            PopulateTopicsDropDownList(idea.TopicID);
            return View(idea);
        }

        // POST: IdealsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, [Bind("Id,Content,FilePath,, CategoryID, TopicID")] Idea idea)
        {
            if (id != idea.Id)
            {
                return NotFound();
            }
            if (idea != null)
            {
                var ideaToUpdate = await _db.Ideas.FirstOrDefaultAsync(i => i.Id == id);
                if (ideaToUpdate == null)
                {
                    return NotFound();
                }
                ideaToUpdate.Content = idea.Content;
                ideaToUpdate.FilePath = idea.FilePath;
                ideaToUpdate.CreatedDate = ideaToUpdate.CreatedDate;
                ideaToUpdate.CategoryID = idea.CategoryID;
                ideaToUpdate.TopicID = idea.TopicID;
                ideaToUpdate.UserID = ideaToUpdate.UserID;

                _db.Update(ideaToUpdate);
                await _db.SaveChangesAsync();


                return RedirectToAction(nameof(List));
            }
            PopulateCategoriesDropDownList(idea.CategoryID);
            PopulateTopicsDropDownList(idea.TopicID);
            return View(idea);
        }

        // GET: IdealsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: IdealsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var idea = await _db.Ideas.FindAsync(id);
            if (idea == null)
            {
                return RedirectToAction(nameof(List));
            }
            try
            {
                _db.Ideas.Remove(idea);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Unable to delete idea " + id + ". Error is: " + ex.Message);
                return NotFound();

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