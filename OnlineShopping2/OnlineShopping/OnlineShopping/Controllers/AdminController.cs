using OnlineShopping.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;

namespace OnlineShopping.Controllers
{
    public class AdminController : Controller
    {

        onlineshoppingEntities db = new onlineshoppingEntities();

        // GET: Admin
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(tbl_admin ad)
        {

            tbl_admin a = db.tbl_admin.Where(x => x.ad_name == ad.ad_name && x.ad_password == ad.ad_password).SingleOrDefault();

            if (a != null)
            {
                Session["admin"] = a.ad_id;
                Session["Ad_Name"] = ad.ad_name;
                return RedirectToAction("Category");
            }
            else
            {
                ViewBag.error = "Invalid Name & Password!!";
            }

            return View();
        }

        
        public ActionResult Category()
        {
            if( Session["admin"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }


        [HttpPost]
        public ActionResult Category(tbl_category c, HttpPostedFileBase imgfile)
        {
            //tbl_admin a = new tbl_admin();

            string path = uploadimage(imgfile);

            if (path.Equals("-1"))
            {
                ViewBag.error = "Image could not be uploaded";
            }
            else
            {
                tbl_category ca = new tbl_category();

                ca.cat_name = c.cat_name;
                ca.cat_image = path;
                ca.cat_status = 1;
                ca.ad_id_fk = Convert.ToInt32(Session["admin"].ToString());
                db.tbl_category.Add(ca);
                var result = db.SaveChanges();
                if(result == 1)
                {
                    return RedirectToAction("ViewCategory");
                }
                else
                {
                    ViewBag.error = "Upload Problem";

                }
            }
            return View();
        }

        public ActionResult ViewCategory(int ? page)
        {
            int pagesize = 2, pageindex = 1;

            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_category.Where(x => x.cat_status == 1).OrderByDescending(x => x.cat_id).ToList();
            IPagedList<tbl_category> cate = list.ToPagedList(pageindex, pagesize);
            return View(cate);
        }

        public string uploadimage(HttpPostedFileBase file)

        {

            Random r = new Random();

            string path = "-1";

            int random = r.Next();

            if (file != null && file.ContentLength > 0)

            {

                string extension = Path.GetExtension(file.FileName);

                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))

                {

                    try

                    {



                        path = Path.Combine(Server.MapPath("~/Content/upload"), random + Path.GetFileName(file.FileName));

                        file.SaveAs(path);

                        path = "~/Content/upload/" + random + Path.GetFileName(file.FileName);



                        //    ViewBag.Message = "File uploaded successfully";

                    }

                    catch (Exception ex)

                    {

                        path = "-1";

                    }

                }

                else

                {

                    Response.Write("<script>alert('Only jpg ,jpeg or png formats are acceptable....'); </script>");

                }

            }



            else

            {

                Response.Write("<script>alert('Please select a file'); </script>");

                path = "-1";

            }







            return path;


        }
    }
}