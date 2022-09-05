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
          
            if (Session["Useremail"] ==null)
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
            List<Review> reviews = db.Reviews.ToList();
            List<Purchase> pur = db.Purchases.ToList();

            Session["Enrollment"] = null;

            if (Session["Useremail"] != null)
            {
                foreach (var v in pur)
                {
                    int k = Convert.ToInt32(v.c_id);

                    if (k == id && v.useremail.Equals(Session["Useremail"].ToString()))
                    {
                        Session["Enrollment"] = Session["Useremail"];
                    }
                }
            }

            var obj = reviews.Find(x =>x.c_id.HasValue && x.c_id.Value == id);
            if (obj != null)
            {
                var info = from c in course
                           join rev in reviews on c.c_id equals rev.c_id
                           join sec in section on c.c_id equals sec.c_id
                           join sub in subsection on sec.c_id equals sub.c_id
                           where c.c_id == id
                           select new Multiple
                           {
                               coursedetails = c,
                               reviewdetails = rev,
                               sectiondetails = sec,
                               content = sub
                           };
                Session["ReviewActive"] = "Yes";
                return View(info);
            }
            else
            {
                var info = from c in course
                           join sec in section on c.c_id equals sec.c_id
                           join sub in subsection on sec.c_id equals sub.c_id
                           where c.c_id == id
                           select new Multiple
                           {
                               coursedetails = c,
                               sectiondetails = sec,
                               content = sub
                           };

                Session["ReviewActive"] = "No";
                return View(info);
            }
        }
        [HttpPost]
        public ActionResult CourseDetails(int prc,int courseid,String rating,String revieww, String addcart,String coursename)
        {

           
            if(addcart.Equals("YES"))
            {
                
                if (Session["Useremail"]==null)
                {
                    Session["CartCid"] = courseid;
                    Session["CartRequest"] = "Yes";
                    return RedirectToAction("Login");
                }
                else
                {
                    Cart cart = new Cart();
                    cart.price = prc;
                    cart.coursename = coursename;
                    cart.c_id = courseid;
                    cart.username = Session["Username"].ToString();
                    cart.user_email = Session["Useremail"].ToString();
                    db.Carts.Add(cart);
                    db.SaveChanges();
                    Session["AddtoCart"] = Session["Useremail"];
                    return RedirectToAction("Cart");
                }
            }
            else
            {
                 int total_rating = 0, t = 0;
                 double result = 0.0;
                 DateTime today = DateTime.Now;
                 Review rev = new Review();
                 rev.c_id = courseid;
                 rev.reviewdate = today.ToString();
                 rev.review1 = revieww;
                 rev.rating = Convert.ToInt32(rating);
                 rev.email = Session["Useremail"].ToString();
                 rev.username = Session["Username"].ToString();

                 db.Reviews.Add(rev);
                 db.SaveChanges();

                 /*Update Review*/

                  List<Review> r = db.Reviews.ToList();
                  foreach (var d in r)
                  {
                      if (d.c_id == courseid)
                      {
                          total_rating += Convert.ToInt32(d.rating);
                          t++;
                      }
                  }
                  var courseupdate = db.Courses.Where(x => x.c_id == courseid).FirstOrDefault();

                  result = Convert.ToDouble(total_rating) / Convert.ToDouble(t);

                  courseupdate.rating = result.ToString();
                  db.SaveChanges();

                  /*Update Review*/

                 List<Cours> course = db.Courses.ToList();
                 List<Section> section = db.Sections.ToList();
                 List<SectionContent> subsection = db.SectionContents.ToList();
                 List<Review> reviews = db.Reviews.ToList();



                var info = from c in course
                           join revv in reviews on c.c_id equals rev.c_id
                           join sec in section on revv.c_id equals sec.c_id
                           join sub in subsection on sec.c_id equals sub.c_id
                           where c.c_id == courseid
                           select new Multiple
                           {
                               coursedetails = c,
                               reviewdetails = revv,
                               sectiondetails = sec,
                               content = sub
                           };
                return View(info);
            }
        }

        public ActionResult DeleteCart(int id)
        {
            //List<Cart> cartinfo = db.Carts.ToList();
            String user = Session["Useremail"].ToString();
            var c = db.Carts.Where(x => x.c_id == id && x.user_email==user).FirstOrDefault();
            db.Carts.Remove(c);
            db.SaveChanges();
           
            List<Cart> cart = db.Carts.Where(x => x.user_email == user).ToList();
            return View(cart);
        }

        [HttpPost]
        public ActionResult DeleteCart(String mail)
        {
        
                try
                {
                    List<Cart> cartt = db.Carts.Where(x => x.user_email == mail).ToList();

                    DateTime today = DateTime.Now;
                    Purchase p = new Purchase();
                    foreach (var pur in cartt)
                    {
                        p.price = pur.price;
                        p.coursename = pur.coursename;
                        p.c_id = pur.c_id;
                        p.purchasedate = today.ToString();
                        p.useremail = pur.user_email;
                        db.Purchases.Add(p);
                        db.SaveChanges();

                    }
                    // Try to remove it
                    Session["CartRequest"] = null;
                    Session["CartCid"] = null;
                    Session["AddtoCart"] = null;
                    db.Carts.RemoveRange(cartt);

                    // Save the changes
                    db.SaveChanges();
                }
                catch
                {
                    //Log the error add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }

                List<Cart> cartinfo = db.Carts.ToList();
                String user = Session["Useremail"].ToString();
                List<Cart> cart = db.Carts.Where(x => x.user_email == user).ToList();
                return View(cart);
            }
            public ActionResult Cart()
        {
            List<Cart> cartinfo = db.Carts.ToList();
            String user = Session["Useremail"].ToString();
            List<Cart>cart = db.Carts.Where(x => x.user_email== user).ToList();
            return View(cart);
        }
        [HttpPost]
        public ActionResult Cart(String mail)
        { 
            try
            {
                List<Cart> cartt = db.Carts.Where(x => x.user_email == mail).ToList();

                DateTime today = DateTime.Now;
                Purchase p = new Purchase();
                foreach(var pur in cartt)
                {
                    p.price = pur.price;
                    p.coursename = pur.coursename;
                    p.c_id = pur.c_id;
                    p.purchasedate = today.ToString();
                    p.useremail = pur.user_email;
                    db.Purchases.Add(p);
                    db.SaveChanges();

                }
                // Try to remove it
                Session["CartRequest"] = null;
                Session["CartCid"] = null;
                Session["AddtoCart"] = null;
                db.Carts.RemoveRange(cartt);

                // Save the changes
                db.SaveChanges();
            }
            catch
            {
                //Log the error add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            List<Cart> cartinfo = db.Carts.ToList();
            String user = Session["Useremail"].ToString();
            List<Cart> cart = db.Carts.Where(x => x.user_email == user).ToList();
            return View(cart);
        }

        public ActionResult CoursebyCatagory(String catagoryname)
        {
            
                List<Cours> course = db.Courses.Where(x => x.catagory == catagoryname).ToList();
                return View(course);
            
        }

        [HttpPost]
        public ActionResult CoursebyCatagory(String sortOrder,String sss)
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

        public ActionResult Quizport(int id , String secname)
        {

            if (Session["Useremail"] != null)
            {
                List<Quiz> quizinfo = db.Quizs.Where(x => x.c_id == id && x.section_name == secname).ToList();
                string s = Session["Useremail"].ToString();
                List<Result> quizcheck = db.Results.ToList();

                int quizmark = 0;

                Session["Quizmark"] = "painai";
                foreach (var c in quizcheck)
                {
                    int cid = Convert.ToInt32(c.c_id);
                    String sectionname = c.section_tittle.ToString();
                    String quizuser = c.useremail.ToString();
                    quizmark = Convert.ToInt32(c.marks);
                    if (cid == id)
                    {
                        if (sectionname.Equals(secname))
                        {
                            if (s.Equals(quizuser))
                            {
                                Session["Quizmark"] = Session["Useremail"];
                                break;
                            }
                        }
                        //return Content("paisi");
                    }
                }
                //return Content(Session["Quizmark"].ToString());
                if (Session["Quizmark"] == Session["Useremail"])
                {
                    Session["Quizmark"] = quizmark;
                    return View(quizinfo);
                }
                else
                {
                    return View(quizinfo);
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
            
        }

        [HttpPost]
        public ActionResult Quizport(String user, int result,int courseid,String sectionname)
        {
            Result result1 = new Result();
            result1.useremail = user;
            result1.section_tittle = sectionname;
            result1.marks = result;
            result1.c_id = courseid;
            db.Results.Add(result1);
            db.SaveChanges();
            return RedirectToAction("Quizport", new { id = courseid , secname =sectionname});
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
                      TempData["LoginSuccess"] = "Login Successful";
                    if (Session["CartRequest"]!=null)
                       {
                          return RedirectToAction("CourseDetails", new {id = Convert.ToInt32(Session["CartCid"]) });
                       }
                       else
                       {
                          return RedirectToAction("DashBoard");
                       }
                       
                    }
                    else
                    {
                    TempData["Loginerror"] = "Login Unsuccessful";
                    return View();
                    }
            }
           
                return View();
        }
       
        public ActionResult endsession()
        {
            Session["Useremail"] = null;
            return View();
        }

        [HttpPost]
        public ActionResult endsession(userinfo s)
        {

            if (ModelState.IsValid)
            {

                var obj = db.userinfoes.Where(a => a.email.Equals(s.email) && a.pass.Equals(s.pass)).FirstOrDefault();
                if (obj != null)
                {
                    Session["Useremail"] = obj.email.ToString();
                    Session["Username"] = obj.name.ToString();
                    TempData["LoginSuccess"] = "Login Successful";
                    if (Session["CartRequest"] != null)
                    {
                        return RedirectToAction("CourseDetails", new { id = Convert.ToInt32(Session["CartCid"]) });
                    }
                    else
                    {
                        return RedirectToAction("DashBoard");
                    }

                }
                else
                {
                    TempData["Loginerror"] = "Login Unsuccessful";
                    return View();
                }
            }

            return View();
        }

        public ActionResult DashBoard()
        {
            /* if (Session["Useremail"] != null)
             {

             }
             else
             {
                 return RedirectToAction("Login");
             }*/
            return View();
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
                    TempData["Success"] = "User registed Succesfully.";
                    return View();
                }
                else
                {
                    TempData["Message"] = "This SignUp failed.";
                    return View();
                }
            }
            return View();
        }
    }
}