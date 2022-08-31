using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cholo_Sikhi.Models;

namespace Cholo_Sikhi.Controllers
{
    public class RegistrationController : Controller
    {

        usersEntities4 db = new usersEntities4();


        public ActionResult Login()
        {
            if (Session["Useremail"]==null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("DashBoard");
            }
        }

      
        public ActionResult Course()
        {
            List<Cours> course = db.Courses.ToList();
            return View(course);
        }

        [HttpPost]
         public ActionResult Course(String sortOrder)
         {
             var students = from s in db.Courses
                            select s;



            switch (sortOrder)
            {
                case "Programming":
                    students = students.Where(s => s.catagory.Equals("Programming"));
                    break;
                case "Design":
                    students = students.Where(s => s.catagory.Equals("Design"));
                    break;
                case "price_desc":
                    students = students.OrderByDescending(s => s.price);
                    break;
                case "price":
                    students = students.OrderBy(s => s.price);
                    break;
                case "All":
                    students = students.OrderBy(s => s.c_id);
                    break;
            }

            return View(students.ToList());

         }
      

         public ActionResult CourseDetails(int id)
        {
           
            List<Cours>course = db.Courses.ToList();
            List<Section> section = db.Sections.ToList();
            List<SectionContent> subsection = db.SectionContents.ToList();

            var info = from c in course
                       join sec in section on c.c_id equals sec.c_id into table1
                       from sec in table1.DefaultIfEmpty()
                       join sub in subsection on sec.c_id equals sub.c_id into table2
                       from sub in table2.DefaultIfEmpty()
                       where c.c_id == id
                       select new Multiple
                       {
                           coursedetails = c,
                           sectiondetails = sec,
                           content = sub
                       };
            
            //var courseinfos = db.Courses.Where(x => x.c_id == id).FirstOrDefault();
            return View(info);
        }
        [HttpPost]
        public ActionResult CourseDetails(String mail,int prc,int courseid)
        {
            Cart cart = new Cart();
            cart.price = prc;
            cart.coursename = "CourseName";
            cart.c_id = courseid;
            cart.username = Session["Username"].ToString();
            cart.user_email = mail;

            db.Carts.Add(cart);
            db.SaveChanges();
            return RedirectToAction("Cart");
        }

        public ActionResult Cart()
        {
            List<Cart> cartinfo = db.Carts.ToList();
            List<Cart>cart = db.Carts.Where(x => x.user_email.Equals("rahmangronthona@gmail.com")).ToList();

            return View(cart);
        }
        [HttpPost]
        public ActionResult Cart(String mail)
        {
            
            try
            {
                List<Cart> cart = db.Carts.Where(x => x.user_email == "rahmangronthona@gmail.com").ToList();
              
                Purchase p = new Purchase();
                foreach(var pur in cart)
                {
                    p.price = pur.price;
                    p.coursename = pur.coursename;
                    p.c_id = pur.c_id;
                    p.purchasedate = "08-30-2022";
                    p.useremail = pur.user_email;
                    db.Purchases.Add(p);
                    db.SaveChanges();
                }
                // Try to remove it
              
                db.Carts.RemoveRange(cart);

                // Save the changes
                db.SaveChanges();
            }
            catch
            {
                //Log the error add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return Content("lala");
        }

        public ActionResult CoursebyCatagorys()
        {
           return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Login(userinfo s)
        {

            if (ModelState.IsValid)
            {

                    var obj = db.userinfoes.Where(a =>a.email.Equals(s.email) && a.pass.Equals(s.pass)).FirstOrDefault();
                    if (obj != null)
                    {
                       Session["Useremail"] = obj.email.ToString();
                       Session["Username"] = obj.name.ToString();
                    return RedirectToAction("DashBoard");
                    }
                    else
                    {
                       TempData["Message"] = "Login Unsuccesful.";
                       return View();
                    }
            }
           
                return View();
        }
       

        public ActionResult DashBoard()
        {
            if (Session["Useremail"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
       

        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp([Bind(Include ="email,name,pass")]userinfo s)
        {
            if(ModelState.IsValid)
            {
                var obj = db.userinfoes.Where(a => a.email.Equals(s.email)).FirstOrDefault();
                if (obj == null)
                {
                    db.userinfoes.Add(s);
                    db.SaveChanges();
                    TempData["Message"] = "User registed Succesfully.";
                    return View();
                }
                else
                {
                    TempData["Message"] = "This login failed. Please enter the correct email or password.";
                    return View();
                }
            }
            return View();
        }
    }
}