using Comp1640.Data;
using Comp1640.Models;
using Comp1640.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Comp1640.Areas.QA_Coordinator.Controllers
{
    [Area(SD.Area_QA_COORDINATOR)]
    public class CommentsController : BaseController
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
       

        public CommentsController(ApplicationDbContext db, UserManager<IdentityUser> userManageranager)
        {
            _db = db;
            _userManager = userManageranager;
        }

        // GET: CommentController
        public async Task<IActionResult> List()
        {
            var comments = _db.Comments.AsNoTracking();
            return View(await comments.ToListAsync());
        }
       

        // GET: CommentController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CommentController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Comment comment)
        {
            if (comment != null)
            {   

                var content = comment.Content;
                var count = _db.Comments.Where(c => c.Content.Contains(content)).Count();
                comment.DateTime = System.DateTime.Now;
                string thisUser = _userManager.GetUserId(HttpContext.User);
                comment.UserID = thisUser;
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

                _db.Add(comment);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }

            return View(comment);
        }

       
      
        // GET: CommentController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CommentController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var comment = await _db.Comments.FindAsync(id);
            _db.Comments.Remove(comment);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(List));

        }

        

    }
}
