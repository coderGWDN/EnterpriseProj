﻿using Comp1640.Data;
using Comp1640.EmailService;
using Comp1640.Models;
using Comp1640.Utility;
using Comp1640.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.Mime.MediaTypeNames;

namespace Comp1640.Areas.QA_Coordinator.Controllers
{
    [Area(SD.Area_QA_COORDINATOR)]
    [Authorize(Roles =SD.Role_QA_MANAGER + "," + SD.Role_QA_COORDINATOR)]
    public class IdeasController : BaseController
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ISendMailService _emailSender;

        public IdeasController(ApplicationDbContext db, UserManager<IdentityUser> userManager, ISendMailService emailSender)
        {
            _db = db;
            _userManager = userManager;
            _emailSender = emailSender;
        }
        // GET: IdealsController
        public async Task<IActionResult> List()
        {
            var ideas = _db.Ideas
                .Include(i => i.Category)
                .Include(i => i.Topic)
                .Include(i => i.User)
                .AsNoTracking();

            return View(await ideas.ToListAsync());
        }

        // GET: IdealsController/Details/5
        public async Task<IActionResult> Index()
        {
            var ideas = _db.Ideas
                .Include(i => i.Category)
                .Include(i => i.Topic)
                .Include(i => i.User)
                .AsNoTracking();

            return View(await ideas.ToListAsync());
        }
        public async Task<IActionResult> PageSubmit()
        {
            var ideas = _db.Ideas
                .Include(i => i.Category)
                .Include(i => i.Topic)
                .Include(i => i.User)
                .AsNoTracking();
            var ideaLists = new List<ListIdeaVM>();
            foreach(var idea in ideas)
            {
                var ideaList = new ListIdeaVM()
                {
                    Idea = idea,
                    Comment = new CommentViewModel()
                    {
                        IdealID = idea.Id
                    },
                    ListComment = await _db.Comments.Where(c=>c.IdealID==idea.Id).ToListAsync(),


                };
                PopulateCategoriesDropDownList(idea.CategoryID);
                PopulateTopicsDropDownList(idea.TopicID);
                ideaLists.Add(ideaList);
            }

            return View(ideaLists);
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
        public async Task<IActionResult> Create(IFormFile file, Idea idea)
        {
            if(file != null)
            {
                string fileName = idea.Id.ToString() + Path.GetFileName(file.FileName);
                string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/file", fileName);
                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                idea.FilePath = "/file/" + fileName;
            }
            idea.CreatedDate = System.DateTime.Now;
            idea.UserID = GetUserId();
            if (idea.Content == null)
            {
                ViewBag.message = "Error: Content is not null";
                return RedirectToAction(nameof(Create));
            }

            //
            
            MailContent content = new MailContent
            {
                To = "minhdacamst@gmail.com",
                Subject = "New Idea",
                Body = "Have a new idea"
            };
            await _emailSender.SendMail(content);

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
        public async Task<IActionResult> Update(int id, Idea idea)
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

        [HttpPost]
        public async Task<IActionResult> CreateComment(CommentViewModel commentView)
        {
            var comment = new Comment()
            {
                Content = commentView.Content,
                DateTime = DateTime.Now,
                UserID = GetUserId(),
                IdealID = commentView.IdealID
            };
            _db.Add(comment);
            await _db.SaveChangesAsync();   
            return RedirectToAction(nameof(PageSubmit));
        }
    }
}
