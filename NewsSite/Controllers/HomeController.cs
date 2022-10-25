using aspnet_config.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using X.PagedList;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace aspnet_config.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        IWebHostEnvironment _appEnvironment;

        private ApplicationContext db;
        ApplicationContext _context;

        public HomeController(ApplicationContext context, IWebHostEnvironment appEnvironment)
        {
            db = context;
            _context = context;
            _appEnvironment = appEnvironment;
        }

        #region Protocol God Eye
        public async Task<IActionResult> Index()
        {
            return View(await db.Users.ToListAsync());
        }
        #endregion

        #region Create profile
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        #region Details profile
        public async Task<IActionResult> Details(int? id)
        {
            User user = await db.Users.FirstOrDefaultAsync(p => p.Id_User == id);
            return View(user);
        }
        #endregion

        #region Find profile

        //[HttpPost]
        //public string FindProfile(string searchString, bool notUsed)
        //{
        //    TestClass.TestStr = searchString;
        //    return "From [HttpPost]FindProfile: filter on " + searchString;
        //}

        public async Task<IActionResult> FindProfile(string SearchString)
        {
            var origins = from m in db.Users select m;
            if (!String.IsNullOrEmpty(SearchString))
            {
                TestClass.TestStr = SearchString;
                origins = origins.Where(s => s.Email_User.Contains(SearchString));
            }
            return View(await origins.ToListAsync());
        }
        #endregion

        #region Delete profile
        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                User user = await db.Users.FirstOrDefaultAsync(p => p.Id_User == id);
                if (user != null)
                    return View(user);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                User user = new User { Id_User = id.Value };
                db.Entry(user).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("ProfileShow");
            }
            return NotFound();
        }
        #endregion

        #region Edit profile
        [HttpGet]
        public async Task<IActionResult> EditProfile(int? id)
        {
                User user = await db.Users.FirstOrDefaultAsync(p => p.Id_User == id);
                    return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(User user)
        {
            db.Users.Update(user);
            await db.SaveChangesAsync();
            return RedirectToAction("ProfileShow");
        }

        [HttpGet]
        public async Task<IActionResult> BanUser(int? id)
        {
            User user = await db.Users.FirstOrDefaultAsync(p => p.Id_User == id);
                return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> BanUser(User user)
        {
            db.Users.Update(user);
            await db.SaveChangesAsync();
            return RedirectToAction("ProfileShow");
        }
        #endregion

        #region Showing profile
        public IActionResult ProfileShow()
        {
            var model = new UserPost
            {
                Posts = db.Posts.ToList(),
                Users = db.Users.ToList()
            };
            return View(model);
        }
        #endregion

        #region Create post
        public IActionResult CreatePost()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreatePost(Post post, IFormFile uploadedFile)
        {
            string path = "";
            try
            {
                if (uploadedFile != null) path = "/Files/" + uploadedFile.FileName;
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                path = "/Files/placeholder.jpg";
            }
            post.WasCreated = DateTime.Now;
            post.Photo_post = path;
            post.Owner_Post = TestClass.TestInt;
            post.Is_Approved = false;
            db.Posts.Add(post);
            await db.SaveChangesAsync();
            return RedirectToAction("Posts");
        }
        #endregion

        #region Show profile posts
        public async Task<IActionResult> DetailsProfilePost(int? id)
        {
            TestClass.TestOwner = (int)id;
            return View(await db.Posts.ToListAsync());
        }
        #endregion

        #region Delete post
        [HttpGet]
        [ActionName("DeletePost")]
        public async Task<IActionResult> ConfirmDeletePost(int? id)
        {
            if (id != null)
            {
                Post post = await db.Posts.FirstOrDefaultAsync(p => p.Id_Post == id);
                if (post != null)
                    return View(post);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(int? id)
        {
            if (id != null)
            {
                Post post = new Post { Id_Post = id.Value };
                db.Entry(post).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("ProfileShow");
            }
            return NotFound();
        }
        #endregion

        #region Edit post
        [HttpGet]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id != null)
            {
                Post post = await db.Posts.FirstOrDefaultAsync(p => p.Id_Post == id);
                if (post != null)
                {
                    TestClass.TestPhoto = post.Photo_post;
                    TestClass.TestDate = post.WasCreated;
                }
                    return View(post);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> EditPost(Post post)
        {
            post.Photo_post = TestClass.TestPhoto;
            post.WasCreated = TestClass.TestDate;
            db.Posts.Update(post);
            await db.SaveChangesAsync();
            return RedirectToAction("ProfileShow");
        }
        #endregion

        #region Showing posts
        public IActionResult Posts(int? page)
        {
            page ??= 1;

            List<Post> ApprovedPosts = db.Posts
                .OrderByDescending(p => p.WasCreated)
                .Where(p => p.Is_Approved).ToList();

            var model = new PostList
            {
                Posts = ApprovedPosts.ToPagedList(pageNumber: page.Value, pageSize: 1),
                //Posts = db.Posts.ToList(),
                Users = db.Users.ToList()
            };
            return View(model);
        }
        #endregion

        #region Comment

        public IActionResult CreateComment()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateComment(Comment comment, IFormFile uploadedFile)
        {
            comment.WasCreated = DateTime.Now;
            comment.WhoCreated = TestClass.TestInt;
            db.Comment.Add(comment);
            await db.SaveChangesAsync();
            return RedirectToAction("Post");
        }

        #endregion

        #region Show comment 
        public async Task<IActionResult> DetailsProfileComment(int? id)
        {
            TestClass.TestOwner = (int)id;
            return View(await db.Comment.ToListAsync());
        }
        #endregion

        #region Delete comment
        [HttpGet]
        [ActionName("DeleteComment")]
        public async Task<IActionResult> ConfirmDeleteComment(int? id)
        {
            if (id != null)
            {
                Comment comment = await db.Comment.FirstOrDefaultAsync(p => p.Id_Comment == id);
                if (comment != null)
                    return View(comment);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(int? id)
        {
            if (id != null)
            {
                Comment comment = new Comment { Id_Comment = id.Value };
                db.Entry(comment).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("ProfileShow");
            }
            return NotFound();
        }
        #endregion

        #region Edit comment
        [HttpGet]
        public async Task<IActionResult> EditComment(int? id)
        {
            if (id != null)
            {
                Comment comment = await db.Comment.FirstOrDefaultAsync(p => p.Id_Comment == id);
                if (comment != null)
                {
                    TestClass.TestDate = comment.WasCreated;
                }
                return View(comment);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> EditComment(Comment comment)
        {
            comment.WasCreated = TestClass.TestDate;
            db.Comment.Update(comment);
            await db.SaveChangesAsync();
            return RedirectToAction("ProfileShow");
        }
        #endregion
    }
}