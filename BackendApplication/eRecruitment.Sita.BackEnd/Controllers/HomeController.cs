using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.IO;
using Newtonsoft.Json;
using System.Web.Helpers;
using System.Threading.Tasks;
using SITA.Notifications;
using SITA.Cryptography;
using System.Text.RegularExpressions;
using System.Text;

namespace eRecruitment.Sita.BackEnd.Controllers
{
    public class HomeController : Controller
    {
        NotificationBL notify = new NotificationBL(new Notification());
        CryptographyBL cryptography = new CryptographyBL(new Cryptograph());

        //_notify.SendEmail("nbadama@gmail.com", "Test Subject", "Body Message");

        //readonly BusinessDomain.Cryptography.Cryptography cryptography = new BusinessDomain.Cryptography.Cryptography();
       readonly BusinessDomain.DAL.DataAccess dataAccess = new BusinessDomain.DAL.DataAccess();

        public ActionResult Index()
        {
            string bada = string.Empty;
            string err = string.Empty;
            string newString = string.Empty;
            string str1 = string.Empty;

            StringBuilder sms = new StringBuilder();
            sms.Append($"Dear: Theodore Willemse{Environment.NewLine}");
            sms.Append($"Please be informed that the Interview invitation for a Network Engineer has been sent to the following email address: louismelissa9@gmail.com.{Environment.NewLine} Kindly accept the invitation.");
            sms.Append($"{Environment.NewLine}Kind Regards. e - Recruitment Team)");
            //if (notify.SendSMS("0766549152", sms.ToString()))
            //{
            //    string badX = string.Empty;
            //}

            //if (notify.SendEmail("temobrilly@gmail.com", "Test Subject", "Body Message"))
            //{
            //    string bad = string.Empty;
            //}



            bada = "Basic R425 qualification (i.e. diploma/degree in nursing) or equivalent qualification that allows registration with the SANC as a Professional Nurse@.  A minimum of 14 years appropriate/recognizable nursing experience after registration as Professional Nurse with the SANC in General Nursing  At least 10 years of the period referred to above must be appropriate/ recognizable experience in Nursing Education after obtaining the 1 - year post - basic qualification in Nursing Education";
            try
            {
                //newString = Regex.Replace("Hello*Hello'Hello&Hello@Hello Hello", @"[^0-9A-Za-z ,]", " ").Replace(" ", " ");
                newString = Regex.Replace(bada, @"[^0-9A-Za-z ,]", " ").Replace(" ", " ");
            }
            catch (Exception e)
            {
                err = e.Message;
            }

            //string encText = cryptography.EncryptText(bada);
            //string decText = cryptography.DecryptText(encText);
            
            ViewBag.NewString = newString;
            return View();
        }

        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                if ((str[i] >= '0' && str[i] <= '9')
                    || (str[i] >= 'A' && str[i] <= 'z'
                        || (str[i] == '.' || str[i] == '_')))
                {
                    sb.Append(str[i]);
                }
            }

            return sb.ToString();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            //notify.SendEmail("nbadama@gmail.com", "Test Subject", "Body Message");
            notify.SendEmail("temobrilly@gmail.com", "Test Subject", "Body Message");
            return View();
        }

        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            var url = System.Configuration.ConfigurationManager.AppSettings["LogOutURL"];
            return Redirect(url);
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        [HttpGet]
        public ActionResult Blank()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [HttpPost]
        public ActionResult Blank(HttpPostedFileBase postedBusinessCase,IEnumerable<HttpPostedFileBase> files)
        {
            ViewBag.Message = "Your application description page.";
            foreach (var file in files)
            {
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    //var path = Path.Combine(Server.MapPath("~/Images/Activity_Image"), fileName);
                    //var sqlpath = "~/Images/Activity_Image" + fileName;
                    //file.SaveAs(path);
                    /* write here your sql code to save image path into database */
                }
            }
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}