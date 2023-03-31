using Comp1640.Data;
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using Microsoft.AspNetCore.Hosting;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Crypto;

namespace Comp1640.Areas.QA_Coordinator.Controllers
{
    [Area(SD.Area_QA_COORDINATOR)]
    [Authorize(Roles = SD.Role_QA_MANAGER + "," + SD.Role_QA_COORDINATOR + "," + SD.Role_STAFF)]
    public class IdeasController : BaseController
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ISendMailService _emailSender;
        private readonly IHostingEnvironment _hosting;

        public IdeasController(ApplicationDbContext db, UserManager<IdentityUser> userManager, ISendMailService emailSender, IHostingEnvironment hosting)
        {
            _db = db;
            _userManager = userManager;
            _emailSender = emailSender;
            _hosting = hosting;
        }

        public async Task<IActionResult> PageSubmit(string sortOrder, int id)
        {
            ViewData["ViewSortParm"] = sortOrder == "View" ? "" : "View";
            ViewData["LikeSortParm"] = sortOrder == "Like" ? "" : "Like";
            ViewData["DislikeSortParm"] = sortOrder == "Dislike" ? "" : "Dislike";
            var ideas = _db.Ideas
                .Include(i => i.Category)
                .Include(i => i.Topic)
                .Include(i => i.User)
                .AsNoTracking();
            var ideaLists = new List<ListIdeaVM>();
            foreach (var idea in ideas)
            {
                var ideaList = new ListIdeaVM()
                {
                    Idea = idea,
                    Comment = new CommentViewModel()
                    {
                        IdealID = idea.Id
                    },
                    ListComment = await _db.Comments.Where(c=>c.IdealID==idea.Id).ToListAsync(),
                    View = new View()
                    {
                        IdealID = idea.Id
                    },
                    ListView = await _db.Views.Where(c => c.IdealID == idea.Id).ToListAsync(),
                    React = await _db.Reacts.Where(r => r.IdealID == idea.Id && r.UserID == GetUserId()).FirstOrDefaultAsync(),
                    ListReactLike = await _db.Reacts.Where(r => r.IdealID == idea.Id && r.Like == true).ToListAsync(),
                    ListReactDislike = await _db.Reacts.Where(r => r.IdealID == idea.Id && r.Dislike == true).ToListAsync(),
                };
                PopulateCategoriesDropDownList();
                PopulateTopicsDropDownList();
                ideaLists.Add(ideaList);
            }

            switch (sortOrder)
            {
                case "View":
                    ideaLists = ideaLists.OrderBy(i => i.View.Count).ToList();
                    break;
                case "Like":
                    ideaLists = ideaLists.OrderByDescending(i => i.ListReactLike.Count).ToList();
                    break;
                case "Dislike":
                    ideaLists = ideaLists.OrderByDescending(i => i.ListReactDislike.Count).ToList();
                    break;
                default:
                    ideaLists = ideaLists.OrderByDescending(i => i.Idea.CreatedDate).ToList();
                    break;
            }
            int numberOfRecords = ideaLists.Count();     //Count SQL
            int numberOfPages = (int)Math.Ceiling((double)numberOfRecords / 5);
            ViewBag.numberOfPages = numberOfPages;
            ViewBag.currentPage = id;
            List<ListIdeaVM> list = ideaLists.Skip(id * 5)  //Offset SQL
                .Take(5).ToList();
            return View(list);
        }
        // GET: IdealsController/Create
        public IActionResult Create()
        {
            PopulateCategoriesDropDownList();
            PopulateTopicsDropDownList();
            return View();
        }


        public async Task<IActionResult> MyIdeas(string sortOrder)
        {
            ViewData["ViewSortParm"] = sortOrder == "View" ? "" : "View";
            ViewData["LikeSortParm"] = sortOrder == "Like" ? "" : "Like";
            ViewData["DislikeSortParm"] = sortOrder == "Dislike" ? "" : "Dislike";
            var ideas = _db.Ideas
                .Include(i => i.Category)
                .Include(i => i.Topic)
                .Include(i => i.User)
                .AsNoTracking();
            var ideaLists = new List<ListIdeaVM>();
            foreach (var idea in ideas)
            {
                var ideaList = new ListIdeaVM()
                {
                    Idea = idea,
                    Comment = new CommentViewModel()
                    {
                        IdealID = idea.Id
                    },
                    ListComment = await _db.Comments.Where(c => c.IdealID == idea.Id).ToListAsync(),
                    View = new View()
                    {
                        IdealID = idea.Id
                    },
                    ListView = await _db.Views.Where(c => c.IdealID == idea.Id).ToListAsync(),
                    React = await _db.Reacts.Where(r => r.IdealID == idea.Id && r.UserID == GetUserId()).FirstOrDefaultAsync(),
                    ListReactLike = await _db.Reacts.Where(r => r.IdealID == idea.Id && r.Like == true).ToListAsync(),
                    ListReactDislike = await _db.Reacts.Where(r => r.IdealID == idea.Id && r.Dislike == true).ToListAsync(),
                };
                PopulateCategoriesDropDownList();
                PopulateTopicsDropDownList();
                ideaLists.Add(ideaList);
            }

            switch (sortOrder)
            {
                case "View":
                    ideaLists = ideaLists.OrderBy(i => i.View.Count).ToList();
                    break;
                case "Like":
                    ideaLists = ideaLists.OrderByDescending(i => i.ListReactLike.Count).ToList();
                    break;
                case "Dislike":
                    ideaLists = ideaLists.OrderByDescending(i => i.ListReactDislike.Count).ToList();
                    break;
                default:
                    ideaLists = ideaLists.OrderByDescending(i => i.Idea.CreatedDate).ToList();
                    break;
            }
            ideaLists = ideaLists.Where(i => i.Idea.UserID == GetUserId()).ToList();
            return View(ideaLists);
        }
        // POST: IdealsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile file, Idea idea)
        {
            if (file != null)
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
            await SendNotificationtoQA();
            return RedirectToAction(nameof(PageSubmit));

        }

        [NonAction]
        private async Task SendNotificationtoQA()
        {
            var getDepartmentByUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == GetUserId());
           
            var userList = await _db.ApplicationUsers
                .Where(u => u.DepartmentId == getDepartmentByUser.DepartmentId)
                .ToListAsync();

            foreach (var user in userList)
            { 
                var roleTemp = await _userManager.GetRolesAsync(user);
                user.Role = roleTemp.FirstOrDefault();
            }

            var getQA = userList.FirstOrDefault(u => u.Role == SD.Role_QA_COORDINATOR);

            if (getQA == null)
                return;

            MailContent content = new MailContent
            {
                To = getQA.Email,
                Subject = "New Idea",
                Body = "Have a new idea"
            };
            await _emailSender.SendMail(content);
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
        public async Task<IActionResult> Update(int id, IFormFile file, Idea idea)
        {
            PopulateCategoriesDropDownList();
            PopulateTopicsDropDownList();
            if (id != idea.Id)
            {
                return NotFound();
            }
            if (file != null)
            {
                string fileName = idea.Id.ToString() + Path.GetFileName(file.FileName);
                string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/file", fileName);
                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                idea.FilePath = "/file/" + fileName;
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


                return RedirectToAction(nameof(PageSubmit));
            }
            PopulateCategoriesDropDownList();
            PopulateTopicsDropDownList();
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
                return RedirectToAction(nameof(PageSubmit));
            }
            try
            {
                _db.Ideas.Remove(idea);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(PageSubmit));
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
                              where DateTime.Compare(t.ClosureDate, DateTime.Now) > 0
                              orderby t.Name
                              select t;
            ViewBag.TopicID = new SelectList(topicsQuery.AsNoTracking(), "Id", "Name", selectedTopic);
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment(CommentViewModel commentView)
        {
            var userComment = _db.Comments.Where(c => c.IdealID == commentView.IdealID && c.UserID == GetUserId()).FirstOrDefault();
            if (userComment != null)
            {
                ViewBag.Message = "Error: User only one comment just one idea";
            }
            else
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
                await SendNotificationtoAuthorIdea(comment.IdealID);
            }

            return RedirectToAction(nameof(PageSubmit));
        }

        [NonAction]
        private async Task SendNotificationtoAuthorIdea(int ideaId)
        {
            var findAuthorIdeabyIdeaId = _db.Ideas.FirstOrDefault(u => u.Id == ideaId);

            var authorIdea = _db.ApplicationUsers.FirstOrDefault(u => u.Id == findAuthorIdeabyIdeaId.UserID);

            if (authorIdea == null)
                return;

            MailContent content = new MailContent
            {
                To = authorIdea.Email,
                Subject = "New Comment",
                Body = "Have a new Comment"
            };
            await _emailSender.SendMail(content);
        }


        [HttpGet("QA_Coordinator/Ideas/viewIdea/{id}")]
        public async Task<ActionResult> ViewIdea([FromRoute] int id)
        {
            var viewDb = _db.Views.FirstOrDefault(_ => _.IdealID == id && _.UserID == GetUserId());
            if (viewDb == null)
            {
                var view = new View() 
                {
                    VisitDate = DateTime.Now,
                    UserID = GetUserId(),
                    IdealID = id,
                    Count = 1
                };

                _db.Views.Add(view);
                _db.SaveChanges();
                return Ok();
            }

            viewDb.Count++;
            _db.Views.Update(viewDb);
            _db.SaveChanges();

            return Ok();  
        }


        [HttpGet("QA_Coordinator/Ideas/likeIdea/{id}")]
        public async Task<ActionResult> LikeIdea([FromRoute] int id)
        {
            var reactDb = await _db.Reacts.FirstOrDefaultAsync(_ => _.IdealID == id && _.UserID == GetUserId());
            if (reactDb == null)
            {
                var react = new React()
                {
                    Like = true,
                    UserID = GetUserId(),
                    IdealID = id,
                };
                _db.Reacts.Add(react);
                await _db.SaveChangesAsync();
                return Ok();
            }
            if (!reactDb.Like)
            {
                reactDb.Like = true;
                _db.Reacts.Update(reactDb);
                await _db.SaveChangesAsync();
                return Ok();
            }
            else
            {
                reactDb.Like = false;
                _db.Reacts.Update(reactDb);
                await _db.SaveChangesAsync();
                return Ok();
            }     
        }


        [HttpGet("QA_Coordinator/Ideas/DislikeIdea/{id}")]
        public async Task<ActionResult> DislikeIdea([FromRoute] int id)
        {
            var reactDb = await _db.Reacts.FirstOrDefaultAsync(_ => _.IdealID == id && _.UserID == GetUserId());
            if (reactDb == null)
            {
                var react = new React()
                {
                    Dislike = true,
                    UserID = GetUserId(),
                    IdealID = id,
                };
                _db.Reacts.Add(react);
                await _db.SaveChangesAsync();
                return Ok();
            }
            if (!reactDb.Dislike)
            {
                reactDb.Dislike = true;
                _db.Reacts.Update(reactDb);
                await _db.SaveChangesAsync();
                return Ok();
            }
            else
            {
                reactDb.Dislike = false;
                _db.Reacts.Update(reactDb);
                await _db.SaveChangesAsync();
                return Ok();
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> DownloadFileZip()
        //{
        //    // Create a memory stream to write the zip file to
        //    var memoryStream = new MemoryStream();

        //    var ideas = _db.Ideas.Where(i => i.FilePath != null).ToList();

        //    var wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        //    var fileFolderPath = Path.Combine(wwwRootPath, "file");
        //    foreach(var idea in ideas)
        //    {
        //        var filePath = Path.Combine(fileFolderPath, idea.FilePath.Replace("/file/", ""));

        //        // Create a new zip archive in the memory stream
        //        using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        //        {
        //            // Create a new zip entry for the file
        //            var zipEntry = zipArchive.CreateEntry(Path.GetFileName(filePath));

        //            // Open the file and copy its contents to the zip entry stream
        //            using (var zipEntryStream = zipEntry.Open())
        //            {
        //                using (var fileStream = new FileStream(filePath, FileMode.Open))
        //                {
        //                    fileStream.CopyTo(zipEntryStream);
        //                }
        //            }

        //        }
        //    }
        //    // Set the position of the memory stream to the beginning
        //    memoryStream.Position = 0;
        //    // Return the memory stream as a FileResult with the MIME type set to application/zip
        //    return File(memoryStream, "application/zip", "AllFile.zip");
        //}


        [HttpPost]
        public async Task<IActionResult> DownloadFileZip()
        {
            var ideas = _db.Ideas.Where(i => i.FilePath != null).ToList();
            var tempFile = Path.GetTempFileName();
            var wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var fileFolderPath = Path.Combine(wwwRootPath, "file");
            using (var zipFile = System.IO.File.Create(tempFile))
            using (var zipArchive = new ZipArchive(zipFile, ZipArchiveMode.Create))
            {
                foreach (var idea in ideas)
                {
                    var filePath = Path.Combine(fileFolderPath, idea.FilePath.Replace("/file/", ""));
                    if (!System.IO.File.Exists(filePath))
                    {
                        return NotFound();
                    }
                    zipArchive.CreateEntryFromFile(filePath, Path.GetFileName(filePath));
                }
            }

            var stream = new FileStream(tempFile, FileMode.Open);
            return File(stream, "application/zip", "my.zip");
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
            var ideas = _db.Ideas
                .Include(i => i.Category)
                .Include(i => i.Topic)
                .Include(i => i.User)
                .AsNoTracking();
            var ideaLists = new List<string[]>();
            foreach (var idea in ideas)
            {
                var ideaList = new string[] 
                {
                    idea.Id.ToString(),
                    idea.FilePath,
                    idea.Content.ToString(),
                    idea.CreatedDate.ToShortDateString(),
                    idea.Category.Name.ToString(),
                    idea.Topic.Name.ToString(),
                    idea.User.FullName.ToString(),
                    _db.Comments.Where(c => c.IdealID == idea.Id).Count().ToString(),
                    _db.Reacts.Where(l => l.IdealID == idea.Id && l.Like == true).Count().ToString(),
                    _db.Reacts.Where(d => d.IdealID == idea.Id && d.Dislike == true).Count().ToString(),
                    _db.Views.Where(c => c.IdealID == idea.Id).ToList().Count().ToString()
                };
                ideaLists.Add(ideaList);
            }

            //Insert the Column Names.
            ideaLists.Insert(0, new string[11] {"ID Idea", "FilePath", "Content", "Created Date", "Category Name", "Topic Name", "User Name", "Commnet", "Like", "Dislike", "View" });
 
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < ideaLists.Count; i++)
            {
                string[] idea = ideaLists[i];
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
