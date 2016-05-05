using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hypster_admin.Areas.WebsiteManagement.Controllers
{
    [Authorize]
    public class manageManualsController : Controller
    {
        //
        // GET: /WebsiteManagement/manageManuals/

        public ActionResult Index()
        {
            hypster_tv_DAL.manualManagement manualManager = new hypster_tv_DAL.manualManagement();

            List<hypster_tv_DAL.Manual> manuals_list = new List<hypster_tv_DAL.Manual>();
            manuals_list = manualManager.GetAllManuals();


            return View(manuals_list);
        }





        [HttpPost]
        public ActionResult AddNewManual(string ManualHeader, int ManualActive)
        {
            hypster_tv_DAL.manualManagement manualManager = new hypster_tv_DAL.manualManagement();

            hypster_tv_DAL.Manual manual = new hypster_tv_DAL.Manual();
            manual.Manual_Active = ManualActive;
            manual.Manual_Date = DateTime.Now;
            manual.Manual_Header = ManualHeader;
            manual.Manual_Guid = ManualHeader.Replace("/", "").Replace("\\", "").Replace("&", "").Replace("+", "").Replace(" ", "-").Replace("?", "").Replace("!", "").Replace("*", "").Replace("$", "").Replace("\"", "").Replace("'", "").Replace("{", "").Replace("}", "").Replace(")", "").Replace("(", "").Replace("[", "").Replace("]", "").Replace("|", "").Replace(".", "").Replace(",", "").Replace(":", "").Replace(";", "");


            if (manualManager.GetManualByGuid(manual.Manual_Guid).Manual_ID != 0)
            {
                Random r = new Random();
                manual.Manual_Guid = manual.Manual_Guid + "-" + r.Next(1,100000);
            }


            System.IO.DirectoryInfo dirInf = new System.IO.DirectoryInfo(System.Configuration.ConfigurationManager.AppSettings["ManualsStorage_Path"] + "\\" + manual.Manual_Guid);
            dirInf.Create();


            manualManager.AddNewManual(manual);


            return RedirectPermanent("/WebsiteManagement/manageManuals");
        }


        [HttpPost]
        public ActionResult AddManualImage(int Img_Manual_ID, int Manual_Active, HttpPostedFileBase Img_Manual_Image)
        {
            hypster_tv_DAL.manualManagement manualManager = new hypster_tv_DAL.manualManagement();
            hypster_tv_DAL.Manual currManual = new hypster_tv_DAL.Manual();
            currManual = manualManager.GetManualByID(Img_Manual_ID);


            currManual.Manual_Active = Manual_Active;
            manualManager.UpdateManual(currManual);



            if (Img_Manual_Image != null && Img_Manual_Image.FileName != null && Img_Manual_Image.FileName != "")
            {

                System.IO.DirectoryInfo dirInf = new System.IO.DirectoryInfo(System.Configuration.ConfigurationManager.AppSettings["ManualsStorage_Path"] + "\\" + currManual.Manual_Guid);
                if (dirInf.Exists == false)
                {
                    dirInf.Create();
                }

                var extension = ".jpg"; //System.IO.Path.GetExtension(Img_Manual_Image.FileName);
                string tmp_image_path = System.Configuration.ConfigurationManager.AppSettings["ManualsStorage_Path"] + "\\" + currManual.Manual_Guid + "\\TMP_" + Img_Manual_ID + extension;
                string perm_image_path = System.Configuration.ConfigurationManager.AppSettings["ManualsStorage_Path"] + "\\" + currManual.Manual_Guid + "\\" + Img_Manual_ID + extension;
                Img_Manual_Image.SaveAs(tmp_image_path);


                hypster_tv_DAL.Image_Resize_Manager imageResizer = new hypster_tv_DAL.Image_Resize_Manager();
                imageResizer.Resize_Image(tmp_image_path, 700, -1, System.Drawing.Imaging.ImageFormat.Jpeg, perm_image_path, 70L);

                System.IO.FileInfo file_del = new System.IO.FileInfo(tmp_image_path);
                file_del.Delete();

            }



            return RedirectPermanent("/WebsiteManagement/manageManuals");
        }




        public ActionResult EditManual(string id)
        {
            ViewBag.GUID = id;
            hypster_admin.Areas.WebsiteManagement.ViewModels.ManualViewModel model = new ViewModels.ManualViewModel();


            hypster_tv_DAL.manualManagement manualManager = new hypster_tv_DAL.manualManagement();

            model.manual = manualManager.GetManualByGuid(id);
            model.manual_slides = manualManager.GetManualSlides(model.manual.Manual_ID);



            return View(model);
        }









        
        
        [HttpPost, ValidateInput(false)] 
        public ActionResult AddNewSlide(int Manual_ID, string Slide_Header, HttpPostedFileBase Slide_Image, int Slide_SortOrder)
        {
            hypster_tv_DAL.manualManagement manualManager = new hypster_tv_DAL.manualManagement();
            hypster_tv_DAL.Manual currManual = new hypster_tv_DAL.Manual();
            currManual = manualManager.GetManualByID(Manual_ID);


            hypster_tv_DAL.Manual_Slide slide = new hypster_tv_DAL.Manual_Slide();
            slide.Manual_Slide_Manual_ID = Manual_ID;
            slide.Manual_Slide_Header = Slide_Header;

            string img_name_guid = Guid.NewGuid().ToString();
            
            

            if (Slide_Image.FileName != null && Slide_Image.FileName != "")
            {
                System.IO.DirectoryInfo dirInf = new System.IO.DirectoryInfo(System.Configuration.ConfigurationManager.AppSettings["ManualsStorage_Path"] + "\\" + currManual.Manual_Guid);
                if (dirInf.Exists == false)
                {
                    dirInf.Create();
                }

                var extension = System.IO.Path.GetExtension(Slide_Image.FileName);
                string tmp_image_path = System.Configuration.ConfigurationManager.AppSettings["ManualsStorage_Path"] + "\\" + currManual.Manual_Guid + "\\TMP_" + img_name_guid + extension;
                string perm_image_path = System.Configuration.ConfigurationManager.AppSettings["ManualsStorage_Path"] + "\\" + currManual.Manual_Guid + "\\" + img_name_guid + extension;
                Slide_Image.SaveAs(tmp_image_path);


                hypster_tv_DAL.Image_Resize_Manager imageResizer = new hypster_tv_DAL.Image_Resize_Manager();
                imageResizer.Resize_Image(tmp_image_path, 1024, -1, System.Drawing.Imaging.ImageFormat.Jpeg, perm_image_path, 70L);

                System.IO.FileInfo file_del = new System.IO.FileInfo(tmp_image_path);
                file_del.Delete();



                //IMAGE
                slide.Manual_Slide_Image = img_name_guid + extension;
            }


            slide.Manual_Slide_Date = DateTime.Now;
            slide.Manual_Slide_SortOrder = Slide_SortOrder;
            slide.Manual_Slide_Active = 1;

            manualManager.AddNewManualSlide(slide);


            return RedirectPermanent("/WebsiteManagement/manageManuals");
        }


        [HttpPost, ValidateInput(false)] 
        public ActionResult UpdateSlide(int Manual_Slide_ID, string Manual_Slide_Header, int Manual_Slide_SortOrder, int Manual_Slide_Active)
        {

            hypster_tv_DAL.manualManagement manualManager = new hypster_tv_DAL.manualManagement();
            hypster_tv_DAL.Manual_Slide slide = new hypster_tv_DAL.Manual_Slide();
            slide.Manual_Slide_ID = Manual_Slide_ID;
            slide.Manual_Slide_Header = Manual_Slide_Header;
            slide.Manual_Slide_SortOrder = Manual_Slide_SortOrder;
            slide.Manual_Slide_Active = Manual_Slide_Active;
            manualManager.UpdateSlide(slide);


            return RedirectPermanent("/WebsiteManagement/manageManuals");
        }



        public ActionResult DeleteSlide(int id)
        {
            hypster_tv_DAL.manualManagement manualManager = new hypster_tv_DAL.manualManagement();
            hypster_tv_DAL.Manual currManual = new hypster_tv_DAL.Manual();
            currManual = manualManager.GetManualByID(id);



            manualManager.DeleteSlide(id);


            return RedirectPermanent("/WebsiteManagement/manageManuals");
        }



    }
}
