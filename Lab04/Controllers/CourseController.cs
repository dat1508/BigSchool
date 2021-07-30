using Lab04.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Lab04.Controllers
{
    [Authorize]
    public class CourseController : Controller
    {
        // GET: Course
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            BigSchoolContext db = new BigSchoolContext();
            Course objCourse = new Course();
            objCourse.ListCategory = db.Categories.ToList();
            return View(objCourse);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Course obj)
        {
            BigSchoolContext db = new BigSchoolContext();
            ModelState.Remove("LecturerId");
            if (!ModelState.IsValid)
            {
                obj.ListCategory = db.Categories.ToList();
                return View("Create", obj);
            }
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            obj.LecturerId = user.Id;

            db.Courses.Add(obj);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Attending()
        {
            BigSchoolContext context = new BigSchoolContext();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var listAttendances = context.Attendances.Where(p => p.Attendee == currentUser.Id ).ToList();
            var courses = new List<Course>();
            foreach (Attendance temp in listAttendances)
            {
                Course objCourse = temp.Course;
                if (objCourse.DateTime > DateTime.Now)
                {
                    objCourse.Name = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(objCourse.LecturerId).Name;
                    courses.Add(objCourse);
                }
            }
            return View(courses);
        }

        public ActionResult Mine()
        {
            BigSchoolContext context = new BigSchoolContext();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var courses = context.Courses.Where(c => c.LecturerId == currentUser.Id && c.DateTime > DateTime.Now).ToList();
            foreach (var i in courses)
            {
                i.LecturerId = currentUser.Name;
            }
            if (TempData["Error"] != null) ViewBag.Error = TempData["Error"].ToString();
            return View(courses);
        }
        public ActionResult Edit(int? id)
        {
            BigSchoolContext context = new BigSchoolContext();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = context.Courses.Where(c => c.Id == id && c.LecturerId == currentUser.Id).FirstOrDefault();
            if (course == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(context.Categories, "Id", "Name", course.CategoryId);
            return View(course);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,LecturerId,Place,DateTime,CategoryId")] Course course)
        {
            BigSchoolContext context = new BigSchoolContext();
            if (ModelState.IsValid)
            {
                context.Entry(course).State = EntityState.Modified;
                context.SaveChanges();
                return RedirectToAction("Mine");
            }
            ViewBag.CategoryId = new SelectList(context.Categories, "Id", "Name", course.CategoryId);
            return View(course);
        }

        public ActionResult Delete(int? id)
        {
            BigSchoolContext context = new BigSchoolContext();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = context.Courses.Where(c => c.Id == id && c.LecturerId == currentUser.Id).FirstOrDefault();
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BigSchoolContext context = new BigSchoolContext();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            Course course = context.Courses.Where(c => c.Id == id && c.LecturerId == currentUser.Id).FirstOrDefault();
            Attendance attendance = context.Attendances.Where(a => a.IdCourse == id).FirstOrDefault();
            context.Courses.Remove(course);
            if (attendance != null)
            {
                TempData["Error"] = "Khóa học đang có người tham gia!";
                return RedirectToAction("Mine");

            }
            context.SaveChanges();
            return RedirectToAction("Mine");
        }
        public ActionResult LectureIamGoing()
        {
            ApplicationUser currentUser =
            System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
            .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            BigSchoolContext context = new BigSchoolContext();
            //danh sách giảng viên được theo dõi bởi người dùng (đăng nhập) hiện tại
            var listFollwee = context.Followings.Where(p => p.FollowerId ==

            currentUser.Id).ToList();

            var upcommingCourse = context.Courses.Where(p => p.DateTime >
            DateTime.Now).OrderBy(p => p.DateTime).ToList();

            var courses = new List<Course>();
            foreach (var course in upcommingCourse)
            {
                foreach (var item in listFollwee)
                {
                    if (item.FolloweeId == course.LecturerId)
                    {
                        Course objCourse = course;
                        objCourse.LectureName = 
                        System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                        .FindById(objCourse.LecturerId).Name;
                        courses.Add(objCourse);
                    }
                }
            }
               
            return View(courses);
        }
    }
}