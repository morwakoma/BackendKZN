using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using Microsoft.AspNet.Identity;

namespace LTSM.Controllers
{
    public class ReportController : Controller
    {
        //private readonly UserProfileRepositoryBLL _userProfileRepositoryBLL;
        //public ReportController()
        //{
        //    this._userProfileRepositoryBLL = new UserProfileRepositoryBLL(new UserProfileRepository());
        //}

        // GET: Report
        [HttpGet]
        public ActionResult Index(string reportname)
        {
            //ViewBag.userRole = userInfo;
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Remote;
            IReportServerCredentials irsc = new CustomReportCredentials(System.Configuration.ConfigurationManager.AppSettings["sReportUser"]
                                               , System.Configuration.ConfigurationManager.AppSettings["sReportPWD"]
                                               , System.Configuration.ConfigurationManager.AppSettings["sDomain"]);
            
            //reportViewer.SizeToReportContent = true;
            //reportViewer.Width = Unit.Percentage(100);
            //reportViewer.Height = Unit.Percentage(100);
            //reportViewer.Width = Unit.Percentage(50);
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Pixel(750);

            //reportViewer.ZoomMode = ZoomMode.Percent;
            reportViewer.ZoomMode = ZoomMode.Percent;
            //reportViewer.SizeToReportContent = true;
            reportViewer.AsyncRendering = false;

            reportViewer.ServerReport.ReportServerCredentials = irsc;
            reportViewer.ServerReport.ReportServerUrl = new Uri(System.Configuration.ConfigurationManager.AppSettings["sReportServerURL"]);
            reportViewer.ServerReport.ReportPath = System.Configuration.ConfigurationManager.AppSettings["sReportPath"] + reportname;
           
            if (reportname != null || reportname != "")
            {
                switch (reportname)
                {
                    case "RequisitionDetails":
                        //ReportParameter[] RptParameters = new Microsoft.Reporting.WebForms.ReportParameter[1];//declare the number of parameters 
                        //RptParameters[0] = new Microsoft.Reporting.WebForms.ReportParameter("ReqID", ReqID);// first parameter 
                        //reportViewer.ServerReport.SetParameters(RptParameters);
                        break;
                    case "WithProvinceID":
                        //ReportParameter[] RptProvinceID = new Microsoft.Reporting.WebForms.ReportParameter[1];//declare the number of parameters 
                        //RptProvinceID[0] = new Microsoft.Reporting.WebForms.ReportParameter("ProvinceID", ProvinceVal);// first parameter 
                        //reportViewer.ServerReport.SetParameters(RptProvinceID);
                        break;
                    default:
                        Console.WriteLine("Default case");
                        break;
                }
            }
   
            reportViewer.ServerReport.Refresh();

            ViewBag.ReportViewer = reportViewer;

            return View();
        }
     
        //[HttpGet]
        //public ActionResult Index()
        //{
        //    ReportViewer reportViewer = new ReportViewer();
        //    ViewBag.ReportViewer = reportViewer;
        //    return View();
        //}
        public class CustomReportCredentials : IReportServerCredentials
        {
            private string _UserName;
            private string _PassWord;
            private string _DomainName;

            public CustomReportCredentials(string UserName, string PassWord, string DomainName)
            {
                _UserName = UserName;
                _PassWord = PassWord;
                _DomainName = DomainName;
            }

            public System.Security.Principal.WindowsIdentity ImpersonationUser
            {
                get { return null; }
            }

            public ICredentials NetworkCredentials
            {
                get { return new NetworkCredential(_UserName, _PassWord, _DomainName); }
            }

            public bool GetFormsCredentials(out Cookie authCookie, out string user,
             out string password, out string authority)
            {
                authCookie = null;
                user = password = authority = null;
                return false;
            }
        }

    }
}