using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BlogAuthentication.Models;
using Microsoft.AspNet.Identity;

namespace BlogAuthentication.Controllers
{
    public class BlogPostController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private IQueryable<BlogPost> GetBlogPosts()
        {

           
            string userId = User.Identity.GetUserId();

            if (!User.Identity.IsAuthenticated)
            {
                return db.BlogPosts.Where(b => b.Public == true);
            }
            else
            {
                return db.BlogPosts.Include(b => b.Owner).Where(b => b.OwnerId == userId || b.Public == true);
            }
            
        }

        // GET: BlogPost
        public ActionResult Index()
        {
            var blogposts = GetBlogPosts();
            return View(blogposts.ToList().OrderByDescending(b => b.Created));
        }

        public ActionResult MyPosts()
        {
            string userId = User.Identity.GetUserId();
            var blogposts = db.BlogPosts.Include(b => b.Owner).Where(b => b.OwnerId == userId);
            return View(blogposts.ToList().OrderByDescending(b => b.Created));
        }

        // GET: BlogPost/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // GET: BlogPost/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BlogPost/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Teaser,Body,Public,OwnerId")] BlogPost blogPost)
        {
            if (ModelState.IsValid)
            {
                blogPost.OwnerId = User.Identity.GetUserId();
                blogPost.Created = DateTime.Now;
                blogPost.Author = User.Identity.GetUserName();
                db.BlogPosts.Add(blogPost);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(blogPost);
        }

        // GET: BlogPost/Edit/5
        public ActionResult Edit(int? id)
        {
            BlogPost blogPost = db.BlogPosts.Find(id);
            var userId = User.Identity.GetUserId();
            var post = db.BlogPosts.SingleOrDefault(m => m.Id == blogPost.Id  && m.OwnerId == userId);
            if (post == null)
            {
                return new HttpNotFoundResult();
            }



            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPost/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Teaser,Body,Public,Author,Created")] BlogPost blogPost)
        {
            if (ModelState.IsValid)
            {
                blogPost.Author = User.Identity.GetUserName();
                blogPost.OwnerId = User.Identity.GetUserId();
                db.Entry(blogPost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blogPost);
        }

        // GET: BlogPost/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPost/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BlogPost blogPost = db.BlogPosts.Find(id);
            db.BlogPosts.Remove(blogPost);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
