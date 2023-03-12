using Comp1640.Data;
using Comp1640.Models;
using Comp1640.Utility;
using Comp1640.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.Mime.MediaTypeNames;

namespace Comp1640.Areas.QA_Coordinator.Controllers
{
    [Area(SD.Area_QA_COORDINATOR)]
    [Authorize(Roles =SD.Role_QA_MANAGER + "," + SD.Role_QA_COORDINATOR + "," + SD.Role_STAFF)]
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

        [HttpGet]
        public async Task<IActionResult> DownloadFile(int id)
        {
            var idea = _db.Ideas.Find(id);
            var wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            
            var fileFolderPath = Path.Combine(wwwRootPath, "file");
            var filePath = Path.Combine(fileFolderPath, idea.FilePath.Replace("/file/",""));
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            // Create a memory stream to write the zip file to
            var memoryStream = new MemoryStream();

            // Create a new zip archive in the memory stream
            using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                // Create a new zip entry for the file
                var zipEntry = zipArchive.CreateEntry(Path.GetFileName(filePath));

                // Open the file and copy its contents to the zip entry stream
                using (var zipEntryStream = zipEntry.Open())
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Open))
                    {
                        fileStream.CopyTo(zipEntryStream);
                    }
                }
            }

            // Set the position of the memory stream to the beginning
            memoryStream.Position = 0;

            // Return the memory stream as a FileResult with the MIME type set to application/zip
            return File(memoryStream, "application/zip", Path.GetFileNameWithoutExtension(filePath) + ".zip");
        }
        
        [HttpPost]
        public FileResult DownloadFileCsv()
        {
            List<object> ideas = (from idea in _db.Ideas.Take(10)
                  select new[] {
                    idea.Content.ToString(),
                    idea.FilePath.ToString(),
                    idea.CreatedDate.ToShortDateString(),
                    idea.Category.Name.ToString(),
                    idea.Topic.Name.ToString(),
                    idea.User.FullName.ToString()
                }).ToList<object>();
 
            //Insert the Column Names.
            ideas.Insert(0, new string[6] { "Content", "FilePath", "Created Date", "Category Name", "Topic Name", "User Name" });
 
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < ideas.Count; i++)
            {
                string[] idea = (string[])ideas[i];
                for (int j = 0; j < idea.Length; j++)
                {
                    //Append data with separator.
                    sb.Append(idea[j] + ',');
                }
 
                //Append new line character.
                sb.Append("\r\n");
            }
 
            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "DownloadFileCsv.csv");
        }
    }
}
