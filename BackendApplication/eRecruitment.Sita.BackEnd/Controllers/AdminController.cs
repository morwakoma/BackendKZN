using eRecruitment.BusinessDomain.DAL.Entities.AppModels;
using eRecruitment.Sita.BackEnd.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using eRecruitment.Sita.BackEnd.App_Data.Entities.DAL;
using System.Data.Entity;
using SITA.Notifications;
using System.Text.RegularExpressions;

namespace eRecruitment.Sita.BackEnd.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationSignInManager _signInManager;
        readonly eRecruitment.BusinessDomain.DAL.DataAccess _dal = new BusinessDomain.DAL.DataAccess();
        eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext();
        private ApplicationUserManager _userManager;
        NotificationBL notify = new NotificationBL(new Notification());
        //CryptographyBL cryptography = new CryptographyBL(new Cryptograph());

        // GET: Admin

        //[Authorize]
        //[HttpGet]
        public ActionResult Index(user UserModel)
        {
            string UserID = User.Identity.GetUserId();
            string emailAddress = _db.AspNetUsers.Where(x => x.Id == UserID).Select(x => x.Email).FirstOrDefault();
            var UserOrganizationID = _dal.GetOrganisationID(UserID);
            ViewBag.UserList = _dal.GetAllUserList(UserOrganizationID, emailAddress);
            //ViewBag.UserList = _dal.GetUserList();
            return View();
        }


        public JsonResult GetMenuItemsPerUserId(Guid userid)
        {
            var data = _db.sp_RoleMenuItem_Get_PerUserId(userid);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SysAdminDashboard()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            string userid = User.Identity.GetUserId();
            int cid = _db.AspNetUserRoles.Where(x => x.UserId == userid).Select(x => x.OrganisationID).FirstOrDefault();
            int totalVacancy = 0;
            int totalApproved = 0;
            int totalRejected = 0;
            int totalWithdrawn = 0;

            if (User.IsInRole("Recruiter"))
            {
                //totalVacancy = _db.tblVacancies.Where(x => x.UserID == userid).Count();
                totalVacancy = (from a in _db.tblVacancies
                                from b in _db.tblFinYears
                                where a.OrganisationID == cid && a.RecruiterUserId == userid
                                && a.CreatedDate >= b.StartDate && a.CreatedDate <= b.EndDate && a.RecruiterUserId == userid
                                select a.ID).Count();

                //totalApproved = _db.tblVacancies.Where(x => x.UserID == userid && x.StatusID == 3).Count();
                totalApproved = (from a in _db.tblVacancies
                                 from b in _db.tblFinYears
                                 where a.OrganisationID == cid && a.RecruiterUserId == userid
                                 && a.StatusID == 3
                                 && a.CreatedDate >= b.StartDate && a.CreatedDate <= b.EndDate && a.RecruiterUserId == userid
                                 select a.ID).Count();

                //totalRejected = _db.tblVacancies.Where(x => x.UserID == userid && x.StatusID == 4).Count();
                totalRejected = (from a in _db.tblVacancies
                                 from b in _db.tblFinYears
                                 where a.OrganisationID == cid && a.RecruiterUserId == userid
                                 && a.StatusID == 4
                                 && a.CreatedDate >= b.StartDate && a.CreatedDate <= b.EndDate && a.RecruiterUserId == userid
                                 select a.ID).Count();

                //totalWithdrawn = _db.tblVacancies.Where(x => x.UserID == userid && x.StatusID == 5).Count();
                totalWithdrawn = (from a in _db.tblVacancies
                                  from b in _db.tblFinYears
                                  where a.OrganisationID == cid && a.RecruiterUserId == userid
                                  && a.StatusID == 5
                                  && a.CreatedDate >= b.StartDate && a.CreatedDate <= b.EndDate && a.RecruiterUserId == userid
                                  select a.ID).Count();
            }

            else if (User.IsInRole("Recruiter Admin"))
            {
                //totalVacancy = _db.tblVacancies.Where(x => x.UserID == userid).Count();
                totalVacancy = (from a in _db.tblVacancies
                                from b in _db.tblFinYears
                                where a.OrganisationID == cid && a.UserID == userid
                                && a.CreatedDate >= b.StartDate && a.CreatedDate <= b.EndDate
                                select a.ID).Count();

                //totalApproved = _db.tblVacancies.Where(x => x.UserID == userid && x.StatusID == 3).Count();
                totalApproved = (from a in _db.tblVacancies
                                 from b in _db.tblFinYears
                                 where a.OrganisationID == cid && a.UserID == userid
                                 && a.StatusID == 3
                                 && a.CreatedDate >= b.StartDate && a.CreatedDate <= b.EndDate
                                 select a.ID).Count();

                //totalRejected = _db.tblVacancies.Where(x => x.UserID == userid && x.StatusID == 4).Count();
                totalRejected = (from a in _db.tblVacancies
                                 from b in _db.tblFinYears
                                 where a.OrganisationID == cid && a.UserID == userid
                                 && a.StatusID == 4
                                 && a.CreatedDate >= b.StartDate && a.CreatedDate <= b.EndDate
                                 select a.ID).Count();

                //totalWithdrawn = _db.tblVacancies.Where(x => x.UserID == userid && x.StatusID == 5).Count();
                totalWithdrawn = (from a in _db.tblVacancies
                                  from b in _db.tblFinYears
                                  where a.OrganisationID == cid && a.UserID == userid
                                  && a.StatusID == 5
                                  && a.CreatedDate >= b.StartDate && a.CreatedDate <= b.EndDate
                                  select a.ID).Count();
            }

            else if (User.IsInRole("Approver"))
            {
                //totalVacancy = _db.tblVacancies.Where(x => x.OrganisationID == cid).Count();
                totalVacancy = (from a in _db.tblVacancies
                                from b in _db.tblFinYears
                                where a.OrganisationID == cid
                                && a.CreatedDate >= b.StartDate && a.CreatedDate <= b.EndDate && a.Manager == userid
                                select a.ID).Count();

                //totalApproved = _db.tblVacancies.Where(x => x.OrganisationID == cid && x.StatusID == 3).Count();
                totalApproved = (from a in _db.tblVacancies
                                 from b in _db.tblFinYears
                                 where a.OrganisationID == cid && a.StatusID == 3
                                 && a.CreatedDate >= b.StartDate && a.CreatedDate <= b.EndDate && a.Manager == userid
                                 select a.ID).Count();

                //totalRejected = _db.tblVacancies.Where(x => x.OrganisationID == cid && x.StatusID == 4).Count();
                totalRejected = (from a in _db.tblVacancies
                                 from b in _db.tblFinYears
                                 where a.OrganisationID == cid && a.StatusID == 4
                                 && a.CreatedDate >= b.StartDate && a.CreatedDate <= b.EndDate && a.Manager == userid
                                 select a.ID).Count();

                //totalWithdrawn = _db.tblVacancies.Where(x => x.OrganisationID == cid && x.StatusID == 5).Count();
                totalWithdrawn = (from a in _db.tblVacancies
                                  from b in _db.tblFinYears
                                  where a.OrganisationID == cid && a.StatusID == 5
                                  && a.CreatedDate >= b.StartDate && a.CreatedDate <= b.EndDate && a.Manager == userid
                                  select a.ID).Count();
            }
            ViewBag.TotalVacancy = totalVacancy;
            ViewBag.TotalApproved = totalApproved;
            ViewBag.TotalRejected = totalRejected;
            ViewBag.TotalWithdrawn = totalWithdrawn;

            return View();
        }


        public ActionResult ManageRoles()
        {
            string UserID = User.Identity.GetUserId();
            var UserOrganizationID = _dal.GetOrganisationID(UserID);
            string emailAddress = _db.AspNetUsers.Where(x => x.Id == UserID).Select(x => x.Email).FirstOrDefault();
            ViewBag.UserList = _dal.GetAllUserList(UserOrganizationID,emailAddress);
            return View();
        }
        [HttpPost]
        public JsonResult GetDepartmentListByDivisionID(string stateID)
        {
            int stateiD = Convert.ToInt32(stateID);
            ViewBag.Municipality = _dal.GetDepartmentListUsingDivisionID(stateiD);

            var DList = ViewBag.Municipality;

            return Json(DList, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public JsonResult GetDepartmentPerIDs(string ids)
        {
            return Json(_dal.GetDepartmentPerIDs(ids), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ManageAssignedRole(string id)
        {
            List<UserListModel> data = _dal.GetUserForAssignUseraRole(id);
            int OrganisationID = _db.AspNetUserRoles.Where(x => x.UserId == id).Select(x => x.OrganisationID).FirstOrDefault();
            ViewBag.Organisation = _dal.GetOrganisationList();
            ViewBag.Division = _dal.GetDivisionListUsingOrganisationID(OrganisationID);
            if (data.Count() > 0 && data[0].SelectedDivisions != null)
            {
                if (data[0].SelectedDivisions.Count() > 0)
                {
                    ViewBag.Department = _dal.GetDepartmentPerIDs(String.Join(";", data[0].SelectedDivisions));
                }
            }
            else
            {
                ViewBag.Department = _dal.GetDepartmentPerIDs("0");
            }

            ViewBag.Roles = _dal.GetOfficialRoleList();
            ViewBag.UserRoleDetailsGeneral = _dal.GetUserRoleDetailsGeneral(id);

            return View(data);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageAssignedRole(UserListModel model, string id, FormCollection fc)
        {
            ViewBag.Users = _dal.GetUserList();
            ViewBag.Roles = _dal.GetOfficialRoleList();
            int OrganisationID = _db.AspNetUserRoles.Where(x => x.UserId == id).Select(x => x.OrganisationID).FirstOrDefault();
            string roleID = _db.AspNetUserRoles.Where(x => x.UserId == id).Select(x => x.RoleId).FirstOrDefault();
            //string roleID = (fc["item.RoleID"]);
            string DivisionID = (fc["item.SelectedDivisions"]);
            string DepartmentID = (fc["item.SelectedDepartments"]);

            try
            {
                if (DivisionID == "" || DepartmentID == "")
                {
                    //ModelState.AddModelError("LastName", "The last name cannot be the same as the first name.");
                    TempData["Warning"] = "Please select Division/Department";
                }
                else
                {   //Validate Duplicate
                    //int Duplicate = _db.AssignedDivisionDepartments.Where(x => x.UserId == id && x.DivisionID == Convert.ToInt32(DivisionID) && x.DepartmentID == Convert.ToInt32(DepartmentID)).Count();
                    //if (Duplicate > 0)
                    //{
                    //    TempData["Duplicate"] = "The value was already entered,All item must be unique,Please try another Division/Department";
                    //}
                    //else
                    //{
                        _dal.AssignRole(id, roleID, OrganisationID);
                        _dal.AssignDivisionDepartment(id, OrganisationID, DivisionID, DepartmentID);

                        string UserDetails = _db.tblProfiles.Where(s => s.UserID == id).Select(f => f.FirstName + " " + f.Surname + ", ID Number :" + f.IDNumber).FirstOrDefault();

                        TempData["Message"] = "You Have Successfully Assigned User Division/Department to the following User: " + UserDetails + " ";
                    //}
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }


            return RedirectToAction("ManageAssignedRole", "Admin", new { id = id });
        }

        public ActionResult DeleteAssignedDivisionDepartment(int id)
        {
            string UserID = _db.AssignedDivisionDepartments.Where(s => s.AssignedDivisionDepartmentID == id).Select(s => s.UserId).FirstOrDefault();
            _dal.DeleteAssignedDivisionDepartment(id);
            return RedirectToAction("ManageAssignedRole", "Admin", new { id = UserID });
        }
        [HttpGet]
        public ActionResult EditAssignedRole(string id)
        {
            var data = _dal.GetUserForEditUseraRoleOrganisation(id);
            string uid = User.Identity.GetUserId();
            ViewBag.Organisation = _dal.GetOrganisationList(uid);
            ViewBag.Roles = _dal.GetOfficialRoleList();
            return View(data);

        }
        [HttpPost]
        public ActionResult EditAssignedRole(UserListModel item, string id)
        {

            ViewBag.Organisation = _dal.GetOrganisationList();
            ViewBag.Roles = _dal.GetOfficialRoleList();
            if (item.OrganisationID == null || item.OrganisationID == "")
            {
                ModelState.AddModelError("Error", "Please select Organisation");
                return RedirectToAction("EditAssignedRole", "Admin", new { id = id });
            }
            if (ModelState.IsValid)
            {
                var t = new UserListModel
                {
                    UserID = id,
                    RoleID = item.RoleID,
                    OrganisationID = item.OrganisationID
                };

                _dal.EditUserData(t);
                string UserDetails = _db.tblProfiles.Where(s => s.UserID == id).Select(f => f.FirstName + " " + f.Surname + ", ID Number :" + f.IDNumber).FirstOrDefault();

                TempData["Message"] = "Successfully Edited the following User's Role: " + UserDetails + ".";
            }

            //var data = _dal.GetUserForEdit(id);
            return RedirectToAction("ManageRoles", "Admin");
        }

        public ActionResult ActivateDeActivateUser(string id)
        {

            string status = Convert.ToString(Request.QueryString["status"]);
            _dal.ActivateDeActivateUser(id, Convert.ToBoolean(Convert.ToInt32(status)));


            //string UserDetails = _db.tblProfiles.Where(s => s.UserID == id).Select(f => f.FirstName + " " + f.Surname + ", ID Number :" + f.IDNumber).FirstOrDefault();
            //string PhoneNo = _db.tblProfiles.Where(s => s.UserID == id).Select(f => f.CellNo).FirstOrDefault();

            var data = _db.tblProfiles.Where(x => x.UserID == id).FirstOrDefault();
            string UserDetails = data.FirstName + " " + data.Surname + ", ID Number : " + data.IDNumber;
            string PhoneNo = data.CellNo;

            string message = string.Empty;
            if (status == "0")
            {
                message = "Successfully De-Activated the following User: ";
            }
            else if (status == "1")
            {
                message = "Successfully Activated the following User: ";
            }

            notify.SendSMS(PhoneNo, message + UserDetails + " ");
            TempData["Message"] = message + UserDetails + " ";
            return RedirectToAction("ManageRoles", "Admin");

        }

        [HttpGet]
        public ActionResult Error403()
        {
            return View();
        }

        public ActionResult DeActivateUserRole(string id)
        {
            int OrganisationID = _db.AspNetUserRoles.Where(x => x.UserId == id).Select(x => x.OrganisationID).FirstOrDefault();
            _dal.DeActivateUserRole(id, "0", OrganisationID, 0);

            string UserDetails = _db.tblProfiles.Where(s => s.UserID == id).Select(f => f.FirstName + " " + f.Surname + ", ID Number :" + f.IDNumber).FirstOrDefault();
            string PhoneNo = _db.tblProfiles.Where(s => s.UserID == id).Select(f => f.CellNo).FirstOrDefault();
            notify.SendSMS(PhoneNo, "Successfully De-Activated the following User: " + UserDetails + " ");
            TempData["Message"] = "Successfully De-Activated the following User: " + UserDetails + " ";
            return RedirectToAction("ManageRoles", "Admin");
        }

        public ActionResult AddOrganisation()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOrganisation(OrganisationModel model, HttpPostedFileBase postedFile)
        {
            byte[] bytes = null;
            string filePath = null;
            string ContentType = null;
            string fileName = null;
            string userid = User.Identity.GetUserId();
            ViewBag.Organisation = _dal.GetOrganisationList(userid);

            var data = _dal.CheckIfOrganisationExists(model.OrganisationName , model.OrganisationCode , model.OrganisationID);

            if (data > 0)
            {
                ModelState.AddModelError(" ", "Record already Exist");
            }
            if (postedFile != null && postedFile.ContentLength > 00)
            {
                var fileExt = System.IO.Path.GetExtension(postedFile.FileName).Substring(1);

                var filesize = 5;
                if (fileExt != "pdf" && fileExt != "tiff" && fileExt != "png" && fileExt != "gif" && fileExt != "jpeg" && fileExt != "jpg")
                {
                    ModelState.AddModelError("", "File Extension Is InValid - Only Upload Pdf/Images File");
                }
                else if (postedFile.ContentLength > (filesize * 1024))
                {
                    //ModelState.AddModelError("", "File size Should Be UpTo " + filesize + "KB");
                    //ModelState.AddModelError("", "File size Should Be UpTo 5MB");
                }
            }
            else
            {
                if (postedFile == null && postedFile.ContentLength <= 0)
                {
                    ModelState.AddModelError("", "Organisation Logo cannot be empty");

                }
            }
            if (ModelState.IsValid)
            {
                if (postedFile != null && postedFile.ContentLength > 00)
                {
                    using (BinaryReader br = new BinaryReader(postedFile.InputStream))
                    {
                        bytes = br.ReadBytes(postedFile.ContentLength);
                    }
                    filePath = Path.GetFileName(postedFile.FileName);
                    fileName = filePath.Remove(filePath.LastIndexOf("."));
                    ContentType = postedFile.ContentType;
                }

                _dal.InsertIntoOrganisation(model.OrganisationCode, model.OrganisationName, fileName, bytes, ContentType);

                TempData["message"] = "Organisation has been successfully added";

                return RedirectToAction("Organisation");
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditOrganisation(int id)
        {
            var p = _dal.GetOrganisationListById(id);
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOrganisation(OrganisationModel item, HttpPostedFileBase postedFile, int id)
        {
            byte[] bytes = null;
            string filePath = null;
            string ContentType = null;
            string fileName = null;

            if (ModelState.IsValid)
            {
                if (postedFile != null && postedFile.ContentLength > 00)
                {
                    using (BinaryReader br = new BinaryReader(postedFile.InputStream))
                    {
                        bytes = br.ReadBytes(postedFile.ContentLength);
                    }
                    filePath = Path.GetFileName(postedFile.FileName);
                    fileName = filePath.Remove(filePath.LastIndexOf("."));
                    ContentType = postedFile.ContentType;
                }

                _dal.UpdateOrganisation(item.OrganisationCode, item.OrganisationName, fileName, bytes, ContentType, id);
                TempData["message"] = "Organisation has been successfully edited";
                return RedirectToAction("Organisation");
            }
            return View(item);
        }

        public ActionResult DeleteOrganisation(int id)
        {
            int data = _dal.CheckIfOrganisationExistsInUser(id);

            if (data > 0)
            {
                TempData["danger"] = "Organisation cannot be deactivated, users are already link to the Organisation";
            }
            else
            {
                _dal.DeleteOrganisation(id);
            }
            return RedirectToAction("Organisation");
        }

        [Authorize]
        public ActionResult Organisation()
        {
            string userid = User.Identity.GetUserId();
            string role = GetUserRole();

            //string role = null;
            //if (User.IsInRole("Admin")) { role = "Admin"; }
            //else if (User.IsInRole("SysAdmin")) { role = "SysAdmin"; }
            OrganisationModel organisationName = new OrganisationModel();
            organisationName = _dal.GetOrganisationName(userid);
            string UserOrganizationName = organisationName.OrganisationName;
            int UserOrganizationID = organisationName.OrganisationID;

            ViewBag.Roles = _dal.GetOfficialRoleList();

            if (UserOrganizationID == 1 || UserOrganizationName.TrimEnd() == "State Information Technology Agency")
            {
                //ViewBag.Organisation = _dal.GetOrganisationList();
                ViewBag.Organisation = _dal.GetAllOrganisationList(userid, role);

            }
            else
            {
                ViewBag.Organisation = _dal.GetAllOrganisationListForDep(userid, role);

               // ViewBag.Organisation = _dal.GetOrganisationListById(UserOrganizationID);
            }

            //ViewBag.UserList = _dal.GetUserList();
            return View();
        }


        public ActionResult DepartmentList()
        {
            string userid = User.Identity.GetUserId();
           ViewBag.Department = _dal.GetDepartmentList(userid);
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddDepartment()
        {
            string userid = User.Identity.GetUserId();
            //int orgid = _db.AspNetUserRoles.Where(x => x.UserId == userid).Select(x => x.OrganisationID).FirstOrDefault();
            int orgid = _dal.GetOrgId(userid);
            ViewBag.ManagerList = _dal.GetDepartmentalManagerList(orgid);
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            ViewBag.Division = _dal.GetDivisionList(userid);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDepartment(DepartmentModel model)
        {

            string userid = User.Identity.GetUserId();
            //int orgid = _db.AspNetUserRoles.Where(x => x.UserId == userid).Select(x => x.OrganisationID).FirstOrDefault();
            int orgid = _dal.GetOrgId(userid);
            ViewBag.ManagerList = _dal.GetDepartmentalManagerList(orgid);
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            ViewBag.Division = _dal.GetDivisionList(userid);
            int data = _dal.CheckIfDepartmentExists(model.DepartmentName, model.DivisionID);

            if (data > 0)
            {
                ModelState.AddModelError(" ", "Record/Department already exist");
            }

            if (ModelState.IsValid)
            {
                _dal.InsertIntoDepartment(model.DepartmentName, model.DivisionID);

                TempData["message"] = "Department has been successfully added";

                return RedirectToAction("DepartmentList", "Admin");
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditDepartment(int id)
        {
            string userid = User.Identity.GetUserId();
            int orgid = _dal.GetOrgId(userid);
            ViewBag.ManagerList = _dal.GetDepartmentalManagerList(orgid);
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            ViewBag.Division = _dal.GetDivisionList(userid);
            var Dep = _dal.GetDepartmentForEdit(id);
            ViewBag.Vacancy = Dep;
            return View(Dep);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDepartment(DepartmentModel item, int id)
        {
            string userid = User.Identity.GetUserId();
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            int orgid = _dal.GetOrgId(userid);
            ViewBag.ManagerList = _dal.GetDepartmentalManagerList(orgid);
            ViewBag.Division = _dal.GetDivisionList(userid);
            if (ModelState.IsValid)
            {
                _dal.UpdateIntoDepartment(id, item.DepartmentName, item.DivisionID);

                TempData["message"] = "Department has been successfully edited";

                return RedirectToAction("DepartmentList", "Admin");
            }
            return View();
        }

        public ActionResult DeleteDepartment(int id)
        {
            var data = _dal.CheckIfDepartmentExistsInVacancy(id);

            if (data > 0)
            {
                TempData["danger"] = "Department Cannot Be Deleted Because it Belongs to an Advert";
            }

            if (data <= 0)
            {
                _dal.DeleteIntoDepartment(id);
                return RedirectToAction("DepartmentList", "Admin");
            }
            return RedirectToAction("DepartmentList", "Admin");

        }

        public ActionResult DivisionList()
        {
            string userid = User.Identity.GetUserId();
            ViewBag.Division = _dal.GetDivisionList(userid);
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddDivision()
        {
            string userid = User.Identity.GetUserId();
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDivision(DivisionModel model, FormCollection fc)
        {
            string userid = User.Identity.GetUserId();
            ViewBag.Organisation = _dal.GetOrganisationList(userid);

            string Organisation = fc["Organisation"];

            var data = _dal.CheckIfDepartmentExists(model.DivisionDiscription, Convert.ToInt32(Organisation));

            if (data > 0)
            {
                ModelState.AddModelError(" ", "Record already Exist");
            }
            if (ModelState.IsValid)
            {
                _dal.InsertIntoDivision(model.DivisionDiscription, Convert.ToInt32(Organisation));

                TempData["message"] = "Division has been successfully added";

                return RedirectToAction("DivisionList", "Admin");
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditDivision(int id)
        {

            string userid = User.Identity.GetUserId();
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            var Div = _dal.GetDivisionForEdit(id);

            return View(Div);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDivision(DivisionModel item, int id, FormCollection fc)
        {
            string userid = User.Identity.GetUserId();
            ViewBag.Organisation = _dal.GetOrganisationList(userid);

            string Organisation = fc["Organisation"];

            if (ModelState.IsValid)
            {
                _dal.UpdateIntoDivision(id, item.DivisionDiscription, Convert.ToInt32(Organisation));

                TempData["message"] = "Division has been successfully edited";

                return RedirectToAction("DivisionList", "Admin");
            }
            return View();
        }

        public ActionResult DeleteDivision(int id)
        {
            var data = _dal.CheckIfDivisionExistsInDepartment(id);

            if (data > 0)
            {
                TempData["danger"] = "Division Cannot Be Deactivated Because it Belongs to a Department";
            }

            if (data <= 0)
            {
                _dal.DeleteIntoDivision(id);
                return RedirectToAction("DivisionList", "Admin");
            }
            return RedirectToAction("DivisionList", "Admin");

        }

        private string GetUserRole()
        {
            string role = null;
            if (User.IsInRole("Admin"))
            {
                role = "Admin";
            }
            else if (User.IsInRole("SysAdmin"))
            {
                role = "SysAdmin";
            }
            return role;
        }

        public ActionResult CheckUser(user UserModel)
        {
            ViewBag.UserList = _dal.GetUserList();
            return View("Index");
        }
        //[Authorize]
        //[HttpGet]
        public ActionResult Add(UserModel userCreate)
        {
            ViewBag.UserList = _dal.GetUserList();
            ViewBag.Organisation = _dal.GetOrganisationList();
            ViewBag.Roles = _dal.GetOfficialRoleList();
            ViewBag.user = null;
            return View();
        }
        //[Authorize]
        //[HttpGet]
        [HttpPost]
        public ActionResult SearchUsers( FormCollection fc, UserModel userCreate)
        {
            // ViewBag.UserList = _dal.GetUserList();
            //string iDNumber = fc["UserID"];
            OrganisationModel organisationName = new OrganisationModel();
            string UserID = User.Identity.GetUserId();
            organisationName = _dal.GetOrganisationName(UserID);
            string UserOrganizationName = organisationName.OrganisationName;
            int UserOrganizationID = organisationName.OrganisationID;

            ViewBag.Roles = _dal.GetOfficialRoleList();

            if(UserOrganizationID == 1 || UserOrganizationName.TrimEnd() == "State Information Technology Agency")
            {
                ViewBag.Organisation = _dal.GetOrganisationList();

            }
            else
            {
                ViewBag.Organisation = _dal.GetOrganisationListById(UserOrganizationID); 
            }

            IDNumberValidity validateID = new IDNumberValidity(userCreate.IDNumber);
            if (!validateID.isValid())
            {
                ModelState.AddModelError("", "ID Number entered is invalid");
                return View("Add");
            }
            IDNumberValidity dob = new IDNumberValidity(userCreate.IDNumber);
            {
                dob.GetDateOfBirth();
                ViewBag.test = validateID.GetDateOfBirth();
            }


            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["PortalUsers"] + userCreate.IDNumber);
            myRequest.Timeout = 5000;
            HttpWebResponse response = (HttpWebResponse)myRequest.GetResponse();
            Console.Write("Status Code: " + response.StatusCode);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                Console.Write("Reading Data Returned from WEB API");
                string responseFromServer = reader.ReadToEnd();

                dynamic data = JsonConvert.DeserializeObject(responseFromServer);

                var userdata = new UserModel();
                if (data["user"] != null)
                {

                    //add the portal user to the the system Convert.ToString(data["user"]["email"]);
                    userCreate.IDNumber = Convert.ToString(data["user"]["idNumber"]);
                    userCreate.FirstName = Convert.ToString(data["user"]["name"]);
                    userCreate.Surname = Convert.ToString(data["user"]["surname"]);
                    userCreate.CellNo = Convert.ToString(data["contact"]["cellHome"]);
                    userCreate.Email = Convert.ToString(data["user"]["email"]);
                    userCreate.area = Convert.ToString(data["address"]["area"]);
                    userCreate.municipality = Convert.ToString(data["address"]["municipality"]);
                    userCreate.province = Convert.ToString(data["address"]["province"]);
                    
                    ViewBag.user = userCreate;
                   
                    return View("Add", userCreate);
                }
                else
                {
                    //user not on portal add user on portal first
                   // ViewBag.Record = "User not Founnd.";
                    ModelState.AddModelError("Error", "The user does not exist on the Portal");
                }
            }

            return View("Add");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        private void SendNotification(string userID)
        {
            string Senderid = User.Identity.GetUserId();


            StringBuilder sbEmail = new StringBuilder();

            var UserDetails = _db.tblProfiles.Where(x => x.UserID == userID).FirstOrDefault();
            string UserRoleID = _db.AspNetUserRoles.Where(x => x.UserId == userID).Select(a => a.RoleId).FirstOrDefault();
            string SenderRoleID = _db.AspNetUserRoles.Where(x => x.UserId == Senderid).Select(a => a.RoleId).FirstOrDefault();


            string UserRoleName = _db.AspNetRoles.Where(x => x.Id == UserRoleID).Select(r => r.Name).FirstOrDefault();
            string SenderRoleName = _db.AspNetRoles.Where(x => x.Id == SenderRoleID).Select(r => r.Name).FirstOrDefault();
            string userid = UserDetails.UserID.ToString();
            string To = UserDetails.EmailAddress;
            string PhoneNo = UserDetails.CellNo;
            //int Status = 1;
            sbEmail.Append("Dear : <b>" + UserDetails.FirstName + " " + UserDetails.Surname + "</b>" + "<br/>");
            sbEmail.AppendLine();
            sbEmail.Append("<br/>");
            sbEmail.Append("You Have been Successfully Assigned as (<b>" + UserRoleName + "</b>) .<br/> ");
            sbEmail.Append("<br/>");
            sbEmail.Append("Please log on to https://www.eservices.gov.za/");
            sbEmail.Append("<br/>");
            sbEmail.AppendLine();
            sbEmail.Append("<br/>");
            sbEmail.Append("Kind Regards<br/>e-Recruitment System Admin");

            string UserDetailsEmail = sbEmail.ToString();

            notify.SendEmail(To, "e-Recruitment Notification", UserDetailsEmail);
            notify.SendSMS(PhoneNo, UserDetailsEmail);
        }

        [HttpPost]
        public async Task<ActionResult> SaveUser(UserModel userCreate, FormCollection fc, RegisterViewModel model, UserListModel users)
        {
            var userdata = new UserModel();
            model.Password = "P@$$w0rd1";
            model.ConfirmPassword = "P@$$w0rd1";
            string OrganisationID = users.OrganisationID;

            if (users.RoleID == null )
            {
                TempData["Message"] = ("Role  Cannot Be Null");
                return RedirectToAction("Add");
            }

            if (users.EmployeeNO == null)
            {
                TempData["Message"] = ("Employee Number Cannot Be Null");
                return RedirectToAction("Add");
            }

            var user1 = new ApplicationUser { UserName = userCreate.Email, Email = model.Email };
               // var result = await UserManager.CreateAsync(user1, model.Password);
                //if (result.Succeeded)
                //{
                //    await SignInManager.SignInAsync(user1, isPersistent: false, rememberBrowser: false);

                //}
               // AddErrors(result);
            
           
            if (_dal.CheckIfUserExists(userCreate.IDNumber) == true)
            {
               
                string UserID = _db.tblProfiles.Where(s => s.IDNumber == userCreate.IDNumber).Select(s => s.UserID).FirstOrDefault();
                var t = new UserListModel
                {

                    UserID = UserID,
                    RoleID = userCreate.RoleID,
                    OrganisationID = Convert.ToString(OrganisationID),
                    EmployeeNO = userCreate.EmployeeNO,
                    CellNo = userCreate.CellNo
                };
           
                _dal.EditUserData(t);
            }
            else
            {
                var result = await UserManager.CreateAsync(user1, model.Password);
                //if (result.Succeeded)
                //{
                //    await SignInManager.SignInAsync(user1, isPersistent: false, rememberBrowser: false);

                //}
                AddErrors(result);
                userdata = new UserModel
                {
                    Email = user1.Email,
                    Password = user1.PasswordHash,
                    Id = user1.Id,
                    SecurityStamp = user1.SecurityStamp,
                    UserName = user1.UserName,
                    FirstName = userCreate.FirstName,
                    Surname = userCreate.Surname,
                    CellNo = userCreate.CellNo,
                    IDNumber = userCreate.IDNumber,
                    EmployeeNO = userCreate.EmployeeNO,
                    Passport = string.Empty


                };
                var t = new UserListModel
                {

                    UserID = user1.Id,
                    RoleID = userCreate.RoleID,
                    OrganisationID = Convert.ToString(OrganisationID),
                    EmployeeNO = userCreate.EmployeeNO,
                    CellNo = userCreate.CellNo
                };

                _dal.EditUserData(t);
                //User does not exists, therefore add user to the aspnetuser table and create user profile
                //_db.proc_eRecruitmentCreateUserProfile(user1.Id, userCreate.IDNumber, userCreate.Passport, userCreate.Surname, userCreate.FirstName, userCreate.CellNo, userCreate.Email);

                _dal.SaveUserInfo(userdata);
            }
            string UserId = _db.tblProfiles.Where(s => s.IDNumber == userCreate.IDNumber).Select(s => s.UserID).FirstOrDefault();
            string UserDetails = _db.tblProfiles.Where(s => s.UserID == UserId).Select(f => f.FirstName + " " + f.Surname + ", ID Number :" + f.IDNumber).FirstOrDefault();

            TempData["Message"] = "Successfully Assigned the following User: " + UserDetails + " a Role ";
            SendNotification(UserId);
           

            return RedirectToAction("Index");
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //[Authorize]
        //[HttpGet]
        public ActionResult Edit(string id)
        {
            //ViewBag.EditUser = _dal.GetUserForEdit(id);
            /// var data = _dal.GetUserForEdit(id);
            //var data = _dal.GetUserForAssignUseraRole(id);
            string uid = User.Identity.GetUserId();

            var data = _dal.GetUserForAssignUseraRoleOrganisation(id);
            //ViewBag.Organisation = _dal.GetOrganisationList();
            ViewBag.Organisation = _dal.GetOrganisationList(uid);
            ViewBag.Roles = _dal.GetOfficialRoleList();
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserListModel item, string id, FormCollection fc)
        {
            //yuo can pass this one after form collection as well,, , string Organisation
            string uid = User.Identity.GetUserId();
            //ViewBag.Organisation = _dal.GetOrganisationList();
            string orgId = Convert.ToString( fc["Organisation"]);
            //string val = Organisation;
            item.OrganisationID = Convert.ToString( orgId);
            ViewBag.Organisation = _dal.GetOrganisationList(uid);
            ViewBag.Roles = _dal.GetOfficialRoleList();

            int ID = Convert.ToInt32(Request.QueryString["OrganisationID"]);
        
            if (item.OrganisationID == null || item.OrganisationID == "")
            {
                ModelState.AddModelError("Error", "Please select Organisation");

                return RedirectToAction("Edit", "Admin", new { id = id });

            }
            if (ModelState.IsValid)
            {
                var t = new UserListModel
                {
                    UserID = id,
                    RoleID = item.RoleID,
                    OrganisationID = item.OrganisationID,
                    EmployeeNO = item.EmployeeNO
                };

                _dal.EditUserData(t);
                string UserDetails = _db.tblProfiles.Where(s => s.UserID == id).Select(f => f.FirstName + " " + f.Surname + ", ID Number :" + f.IDNumber).FirstOrDefault();

                TempData["Message"] = "Successfully Assigned the following User: " + UserDetails + " a Role ";
                SendNotification(id);
            }

            //var data = _dal.GetUserForEdit(id);
            return RedirectToAction("Index", "Admin");
            //return View(data);
        }

        public ActionResult DisabilityList()
        {
            string userid = User.Identity.GetUserId();
            ViewBag.Disability = _dal.GetDisabilityList();
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddDisability()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddDisability(DisabilityModel model)
        {
            var data = _dal.CheckIfDisabilityExists(model.Disability);

            if (data > 0)
            {
                ModelState.AddModelError(" ", "Record already Exist");
            }
            if (ModelState.IsValid)
            {
                _dal.InsertIntoDisability(model.Disability);

                TempData["message"] = "Disability has been successfully added";

                return RedirectToAction("DisabilityList", "Admin");
            }
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditDisability(int id)
        {
            var Div = _dal.GetDisabilityForEdit(id);

            return View(Div);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDisability(DisabilityModel item, int id)
        {

            if (ModelState.IsValid)
            {
                _dal.UpdateIntoDisability(id, item.Disability);

                TempData["message"] = "Disability has been successfully edited";

                return RedirectToAction("DisabilityList", "Admin");
            }
            return View();
        }

        public ActionResult DeleteDisability(int id)
        {
            _dal.DeleteIntoDisability(id);
            TempData["message"] = "Disability Successfully Deleted";
            return RedirectToAction("DisabilityList", "Admin");
        }

        public ActionResult DisclamerList()
        {
            string userid = User.Identity.GetUserId();
            var UserOrganizationID = _dal.GetOrganisationID(userid);

            List<DisclaimerModel> disclaimers = new List<DisclaimerModel>();
            ViewBag.Disclamer = _dal.GetDisclamerList(UserOrganizationID);
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddDisclamer()
        {
            string userid = User.Identity.GetUserId();
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            return View();
        }

        [HttpPost, ValidateInput(false)]
        //[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDisclamer(DisclamerModel model)
        {

            string userid = User.Identity.GetUserId();
            //&#61623;
            //int orgid = _db.AspNetUserRoles.Where(x => x.UserId == userid).Select(x => x.OrganisationID).FirstOrDefault();
            int orgid = _dal.GetOrgId(userid);
            model.OrginazationID = orgid;
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            var data = _dal.CheckIfDisclamerExists(model.OrginazationID);

            if (data > 0)
            {
                ModelState.AddModelError(" ", "Disclamer already Exist for this Organisation");
            }

            if (ModelState.IsValid)
            {
                string disclaimer = Regex.Replace(model.Disclamer, @"[^0-9A-Za-z ,]", " ").Replace("&#61623", ".").Replace("  61623", ".").Replace("&#61553",".").Replace("  61553", ".").Replace(" ", " ").Trim();
                //_dal.InsertIntoDisclaimer(model.OrginazationID, model.Disclamer);
                _dal.InsertIntoDisclaimer(model.OrginazationID, disclaimer.Trim());

                TempData["message"] = "Disclaimer has been successfully added";

                return RedirectToAction("DisclamerList", "Admin");
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditDisclaimer(int id)
        {
            string userid = User.Identity.GetUserId();
            ViewBag.Organisation = _dal.GetOrganisationList(userid);

            var Disclaimer = _dal.GetDisclaimerForEdit(id);

            return View(Disclaimer);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditDisclaimer(DisclamerModel item, int id)
        {
            string userid = User.Identity.GetUserId();
            ViewBag.Organisation = _dal.GetOrganisationList(userid);

            int orgid = _dal.GetOrgId(userid);
            item.OrginazationID = orgid;

            if (ModelState.IsValid)
            {
                //string disclaimer = Regex.Replace(item.Disclamer, @"[^0-9A-Za-z ,]", " ").Replace("&#61623", ".").Replace("  61623", ".").Replace("&#61553", ".").Replace("  61553", ".").Replace(" ", " ").Trim();
                string disclaimer = item.Disclamer.Trim();
                //_dal.UpdateIntoDisclamer(id, item.OrginazationID, item.Disclamer);
                _dal.UpdateIntoDisclamer(id, item.OrginazationID, disclaimer.Trim());

                TempData["message"] = "Disclamer has been successfully edited";

                return RedirectToAction("DisclamerList", "Admin");
            }
            return View();
        }

        public ActionResult DeleteDisclaimer(int id)
        {
            _dal.DeleteIntoDisclaimer(id);
            TempData["message"] = "Disclaimer Successfully Deleted";
            return RedirectToAction("DisclamerList", "Admin");
        }

        public ActionResult EmploymentTypeList()
        {
            string userid = User.Identity.GetUserId();
            ViewBag.EmploymentTypeList = _dal.GetEmploymentTypeList();
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddEmploymentType()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddEmploymentType(EmploymentTypeModel model)
        {
            var data = _dal.CheckIfEmploymentTypeExists(model.EmploymentType);

            if (data > 0)
            {
                ModelState.AddModelError(" ", "Record already Exist");
            }
            if (ModelState.IsValid)
            {
                _dal.InsertIntoEmploymentType(model.EmploymentType);

                TempData["message"] = "Empoyment Type has been successfully added";

                return RedirectToAction("EmploymentTypeList", "Admin");
            }
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditEmploymentType(int id)
        {

            var Div = _dal.GetEmploymentTypeForEdit(id);

            return View(Div);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditEmploymentType(EmploymentTypeModel item, int id)
        {

            if (ModelState.IsValid)
            {
                _dal.UpdateIntoEmploymentType(id, item.EmploymentType);

                TempData["message"] = "Employment Type has been successfully edited";

                return RedirectToAction("EmploymentTypeList", "Admin");
            }
            return View();
        }
        public ActionResult DeleteEmploymentType(int id)
        {
            _dal.DeleteIntoEmploymentType(id);
            TempData["message"] = "Employment Type Successfully Deleted";
            return RedirectToAction("EmploymentTypeList", "Admin");
        }

        public ActionResult GeneralQuestionList()
        {
            string userid = User.Identity.GetUserId();
            ViewBag.GeneralQuestionList = _dal.GetQuestionBanksList(userid);
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddGeneralQuestion()
        {
            string userid = User.Identity.GetUserId();
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            ViewBag.QuestionCategory = _dal.GetQuestionCategoryList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddGeneralQuestion(GeneralQuestionModel model, FormCollection fc)
        {
            string userid = User.Identity.GetUserId();
            string Organisation = fc["Organisation"];

            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            ViewBag.QuestionCategory = _dal.GetQuestionCategoryList();

            var data = _dal.CheckIfGeneralQuestionExists(model.GeneralQuestionDesc, Convert.ToInt32(Organisation));

            if (data > 0)
            {
                ModelState.AddModelError(" ", "Record already Exist");
            }
            if (ModelState.IsValid)
            {
                _dal.InsertIntoGeneralQuestion(model.GeneralQuestionDesc, Convert.ToInt32(Organisation), model.QCategoryID);

                TempData["message"] = "General Question has been successfully added";

                return RedirectToAction("GeneralQuestionList", "Admin");
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditGeneralQuestion(int id)
        {

            string userid = User.Identity.GetUserId();
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            ViewBag.QuestionCategory = _dal.GetQuestionCategoryList();
            var Div = _dal.GetGeneralQuestionForEdit(id);

            return View(Div);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditGeneralQuestion(GeneralQuestionModel item, int id, FormCollection fc)
        {
            string userid = User.Identity.GetUserId();
            string Organisation = fc["Organisation"];

            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            ViewBag.QuestionCategory = _dal.GetQuestionCategoryList();

            if (ModelState.IsValid)
            {
                _dal.UpdateIntoGeneralQuestion(id, item.GeneralQuestionDesc, Convert.ToInt32(Organisation), item.QCategoryID);

                TempData["message"] = "General Question has been successfully edited";

                return RedirectToAction("GeneralQuestionList", "Admin");
            }
            return View();
        }

        public ActionResult DeleteGeneralQuestion(int generalQuestionId)
        {
            _dal.DeleteIntoGeneralQuestion(generalQuestionId);
            return RedirectToAction("GeneralQuestionList", "Admin");
        }

        public ActionResult InterviewCategoryList()
        {
            string userid = User.Identity.GetUserId();
            ViewBag.InterviewCategory = _dal.GetInterviewCategory();
            return View();
        }


        [Authorize]
        [HttpGet]
        public ActionResult AddInterviewCategory()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddInterviewCategory(InterviewCategoryModel model)
        {
            var data = _dal.CheckIfInterviewCategoryExists(model.InterviewCatDescription);

            if (data > 0)
            {
                ModelState.AddModelError(" ", "Record already Exist");
            }
            if (ModelState.IsValid)
            {
                _dal.InsertIntoInterviewCategory(model.InterviewCatDescription);

                TempData["message"] = "Interview Category has been successfully added";

                return RedirectToAction("InterviewCategoryList", "Admin");
            }
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditInterviewCategory(int id)
        {

           var Div = _dal.GetInterviewCategoryForEdit(id);

            return View(Div);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditInterviewCategory(InterviewCategoryModel item, int id)
        {
            if (ModelState.IsValid)
            {
                _dal.UpdateIntoInterviewCategory(id, item.InterviewCatDescription);

                TempData["message"] = "Interview Category has been successfully edited";

                return RedirectToAction("InterviewCategoryList", "Admin");
            }
            return View();
        }

        public ActionResult DeleteInterviewCategory(int InterviewCategoryid)
        {
            _dal.DeleteIntoInterviewCategory(InterviewCategoryid);
            return RedirectToAction("InterviewCategoryList", "Admin");
        }

        public ActionResult InterviewTypeList()
        {
            string userid = User.Identity.GetUserId();
            ViewBag.InterviewType = _dal.GetInterviewType();
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddInterviewType()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddInterviewType(InterviewTypeModel model)
        {
            var data = _dal.CheckIfInterviewTypeExists(model.InterviewTypeDescription);

            if (data > 0)
            {
                ModelState.AddModelError(" ", "Record already Exist");
            }
            if (ModelState.IsValid)
            {
                _dal.InsertIntoInterviewType(model.InterviewTypeDescription);

                TempData["message"] = "Interview Type has been successfully added";

                return RedirectToAction("InterviewTypeList", "Admin");
            }
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditInterviewType(int id)
        {
            var Div = _dal.GetInterviewTypeForEdit(id);

            return View(Div);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditInterviewType(InterviewTypeModel item, int id)
        {
            if (ModelState.IsValid)
            {
                _dal.UpdateIntoInterviewType(id, item.InterviewTypeDescription);

                TempData["message"] = "Interview Type has been successfully edited";

                return RedirectToAction("InterviewTypeList", "Admin");
            }
            return View();
        }

        public ActionResult DeleteInterviewType(int interviewTypeID)
        {
            _dal.DeleteIntoInterviewType(interviewTypeID);
            return RedirectToAction("InterviewTypeList", "Admin");
        }

        public ActionResult JobLevelList()
        {
            string userid = User.Identity.GetUserId();
            ViewBag.JobLevelList = _dal.GetJobLevelList(userid);
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddJobLevel()
        {
            string userid = User.Identity.GetUserId();

            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            /*ViewBag.SalaryCategory = _dal.GetAllSalaryCategoryList(userid);
            ViewBag.SalarySubCategory = _dal.GetSalarySubCategoryList(userid);*/
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddJobLevel(JobLevelModel model, FormCollection fc)
        {
            string userid = User.Identity.GetUserId();
            string Organisation = fc["Organisation"];

            ViewBag.Organisation = _dal.GetOrganisationList(userid);

            var data = _dal.CheckIfJobLevelExists(Convert.ToInt32(Organisation), model.JobLevelName);

            if (data > 0)
            {
                ModelState.AddModelError(" ", "Record already Exist");
            }
            if (ModelState.IsValid)
            {
                _dal.InsertIntoJobLevel(Convert.ToInt32(Organisation), model.JobLevelName);

                TempData["message"] = "Job Level has been successfully added";

                return RedirectToAction("JobLevelList", "Admin");
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditJobLevel(int id)
        {

            string userid = User.Identity.GetUserId();

            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            var Div = _dal.GetJobLevelForEdit(id);

            return View(Div);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditJobLevel(JobLevelModel item, int id, FormCollection fc)
        {
            string userid = User.Identity.GetUserId();
            string Organisation = fc["Organisation"];

            ViewBag.Organisation = _dal.GetOrganisationList(userid);

            if (ModelState.IsValid)
            {
                _dal.UpdateIntoJobLevel(id, Convert.ToInt32(Organisation), item.JobLevelName);

                TempData["message"] = "Job Level has been successfully edited";

                return RedirectToAction("JobLevelList", "Admin");
            }
            return View();
        }

        public ActionResult DeleteJobLevel(int jobLevelID)
        {
            var data = _dal.CheckIfJobLevelExistsInSalaryStructure(jobLevelID);

            if (data > 0)
            {
                TempData["danger"] = "Job Level Cannot Be Deactivated Because it Belongs to a Salary Structure";
            }

            if (data <= 0)
            {
                _dal.DeleteIntoJobLevel(jobLevelID);
                return RedirectToAction("JobLevelList", "Admin");
            }
            return RedirectToAction("JobLevelList", "Admin");
        }

        public ActionResult VacancyProfileList()
        {
            string userid = User.Identity.GetUserId();

            var OrgID = _dal.GetOrganisationID(userid);

            ViewBag.VacancyProfile = _dal.GetVacancyProfile(OrgID);
            return View();
        }

        public ActionResult JobTitleList()
        {
            string userid = User.Identity.GetUserId();
            ViewBag.JobTitleList = _dal.GetJobTitleList(userid);
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddJobTitle()
        {
            string userid = User.Identity.GetUserId();
            //int orgID = _dal.GetOrganisationList(userid);
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            ViewBag.JobLevel = _dal.GetJobLevelList(userid);
            ViewBag.SalaryCategory = _dal.GetSalaryCategoryList(userid);
           
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddJobTitle(JobTitleModel model, FormCollection fc)
        {
            string userid = User.Identity.GetUserId();
            string Organisation = fc["Organisation"];

            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            ViewBag.JobLevel = _dal.GetJobLevelList(userid);
            ViewBag.SalaryCategory = _dal.GetSalaryCategoryList(userid);

            var data = _dal.CheckIfJobTitleExists(Convert.ToInt32(Organisation), model.JobTitle);

            if (data > 0)
            {
                ModelState.AddModelError(" ", "Record already Exist");
            }
            if (ModelState.IsValid)
            {
                _dal.InsertIntoJobTitle(Convert.ToInt32(Organisation), model.JobTitle);

                TempData["message"] = "Job Title has been successfully added";

                return RedirectToAction("JobTitleList", "Admin");
            }
            return View(model);
        }

        //=================================Peter - 20221207=========================================
        public ActionResult AddJobSpecificQuestion()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddJobSpecificQuestion(JobJobSpecificQuestionModel model)
        {
            var data = _dal.CheckIfDisabilityExists(model.JobSpecificeQuestionDesc); //PETER TRY IF YOU CAN'T CHECK USING ID

            if (data > 0)
            {
                ModelState.AddModelError(" ", "Record already Exist");
            }
            if (ModelState.IsValid)
            {
                _dal.InsertIntoDisability(model.JobSpecificeQuestionDesc);

                TempData["message"] = "Job specific question has been successfully added";

                return RedirectToAction("DisabilityList", "Admin");
            }
            return View();
        }
        //===========================================================================================

        [Authorize]
        [HttpGet]
        public ActionResult EditJobTitle(int id)
        {

            string userid = User.Identity.GetUserId();
            //int orgID = _dal.GetOrganisationList(userid);
            var orgID = _dal.GetOrganisationID(userid);
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            ViewBag.JobLevel = _dal.GetJobLevelList(userid);
            ViewBag.SalaryCategory = _dal.GetSalaryCategoryList(userid);
            ViewBag.Disclamer = _dal.GetDisclamer(Convert.ToInt32(orgID));
            var Div = _dal.GetJobTitleForEdit(id);

            return View(Div);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditJobTitle(JobTitleModel item, int id, FormCollection fc)
        {
            string userid = User.Identity.GetUserId();
            string Organisation = fc["Organisation"];
            var orgID = _dal.GetOrganisationID(userid);

            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            ViewBag.JobLevel = _dal.GetJobLevelList(userid);
            ViewBag.SalaryCategory = _dal.GetSalaryCategoryList(userid);
            ViewBag.Disclamer = _dal.GetDisclamer(Convert.ToInt32(orgID));

            if (ModelState.IsValid)
            {
                _dal.UpdateIntoJobTitle(id, Convert.ToInt32(Organisation), item.JobTitle);

                TempData["message"] = "Job Title has been successfully edited";

                return RedirectToAction("JobTitleList", "Admin");
            }
            return View();
        }

        public ActionResult DeleteJobTitle(int JobTitleID)
        {
            var data = _dal.CheckIfJobTitleExistsInSalaryStructure(JobTitleID);

            if (data > 0)
            {
                TempData["danger"] = "Job Title Cannot Be Deactivated Because it Belongs to a Salary Structure";
            }

            if (data <= 0)
            {
                _dal.DeleteIntoJobTitle(JobTitleID);
                return RedirectToAction("JobTitleList", "Admin");
            }
            return RedirectToAction("JobTitleList", "Admin"); ;

        }

        public ActionResult LocationList()
        {
            string userid = User.Identity.GetUserId();
            var orgID = _dal.GetOrganisationID(userid);
            ViewBag.LocationList = _dal.GetLocationList(Convert.ToInt32(orgID));
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddLocation()
        {
            string userid = User.Identity.GetUserId();
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddLocation(LocationModel model, FormCollection fc)
        {
            string OrganisationID = fc["Organisation"];
            var data = _dal.CheckIfLocationExists(Convert.ToInt32(OrganisationID), model.LocationDiscription);
            string userid = User.Identity.GetUserId();
            ViewBag.Organisation = _dal.GetOrganisationList(userid);

            if (data > 0)
            {
                ModelState.AddModelError(" ", "Record already Exist");
            }
            if (ModelState.IsValid)
            {
                _dal.InsertIntoLocation(Convert.ToInt32(OrganisationID), model.LocationDiscription);

                TempData["message"] = "Location has been successfully added";

                return RedirectToAction("LocationList", "Admin");
            }
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditLocation(int id)
        {
            string userid = User.Identity.GetUserId();
            ViewBag.Organisation = _dal.GetOrganisationList(userid);

            var Div = _dal.GetLocationForEdit(id);

            return View(Div);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditLocation(LocationModel item, int id, FormCollection fc)
        {
            string OrganisationID = fc["Organisation"];
            string userid = User.Identity.GetUserId();
            ViewBag.Organisation = _dal.GetOrganisationList(userid);

            if (ModelState.IsValid)
            {
                _dal.UpdateIntoLocation(id, Convert.ToInt32(OrganisationID), item.LocationDiscription);

                TempData["message"] = "Location has been successfully edited";

                return RedirectToAction("LocationList", "Admin");
            }
            return View();
        }

        public ActionResult DeleteLocation(int locationID)
        {
            _dal.DeleteIntoLocation(locationID);
            return RedirectToAction("LocationList", "Admin");
        }

        public ActionResult QualificationTypeList()
        {
            string userid = User.Identity.GetUserId();
            ViewBag.QualificationTypeList = _dal.GetQualificationTypeList();
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddQualificationType()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddQualificationType(QualificationTypeModel model)
        {
            var data = _dal.CheckIfQualificationTypeExists(model.QualificationTypeName);

            if (data > 0)
            {
                ModelState.AddModelError(" ", "Record already Exist");
            }
            if (ModelState.IsValid)
            {
                _dal.InsertIntoQualificationType(model.QualificationTypeName);

                TempData["message"] = "Qualification Type has been successfully added";

                return RedirectToAction("QualificationTypeList", "Admin");
            }
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditQualificationType(int id)
        {
            var Div = _dal.GetQualificationTypeForEdit(id);

            return View(Div);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditQualificationType(QualificationTypeModel item, int id)
        {
            if (ModelState.IsValid)
            {
                _dal.UpdateIntoQualificationType(id, item.QualificationTypeName);

                TempData["message"] = "Qualification Type has been successfully edited";

                return RedirectToAction("QualificationTypeList", "Admin");
            }
            return View();
        }

        public ActionResult DeleteQualificationType(int qualificationTypeID)
        {
            _dal.DeleteIntoQualificationType(qualificationTypeID);
            return RedirectToAction("QualificationTypeList", "Admin");
        }

        public ActionResult RejectionReasonsList()
        {
            string userid = User.Identity.GetUserId();
            ViewBag.RejectReasonList = _dal.GetRejectReasonList();
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddRejectReason()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddRejectReason(RejectReasonModel model)
        {
            var data = _dal.CheckIfRejectReasonExists(model.RejectReason);

            if (data > 0)
            {
                ModelState.AddModelError(" ", "Record already Exist");
            }
            if (ModelState.IsValid)
            {
                _dal.InsertIntoRejectReason(model.RejectReason);

                TempData["message"] = "Reject Reason has been successfully added";

                return RedirectToAction("RejectionReasonsList", "Admin");
            }
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditRejectReason(int id)
        {
            var Div = _dal.GetRejectReasonForEdit(id);

            return View(Div);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRejectReason(RejectReasonModel item, int id)
        {
            if (ModelState.IsValid)
            {
                _dal.UpdateIntoRejectReason(id, item.RejectReason);

                TempData["message"] = "Reject Reason has been successfully edited";

                return RedirectToAction("RejectionReasonsList", "Admin");
            }
            return View();
        }

        public ActionResult DeleteRejectReason(int rejectReasonID)
        {
            _dal.DeleteIntoRejectReason(rejectReasonID);
            return RedirectToAction("RejectionReasonsList", "Admin");
        }

        public ActionResult RetractReasonList()
        {
            string userid = User.Identity.GetUserId();
            ViewBag.RetractReasonList = _dal.GetRetractReasonList();
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddRetractReason()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddRetractReason(RetractReasonModel model)
        {
            var data = _dal.CheckIfRetractReasonExists(model.RetractReason);

            if (data > 0)
            {
                ModelState.AddModelError(" ", "Record already Exist");
            }
            if (ModelState.IsValid)
            {
                _dal.InsertIntoRetractReason(model.RetractReason);

                TempData["message"] = "Retract Reason has been successfully added";

                return RedirectToAction("RetractReasonList", "Admin");
            }
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditRetractReason(int id)
        {
            var Div = _dal.GetRetractReasonForEdit(id);

            return View(Div);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRetractReason(RetractReasonModel item, int id)
        {
            if (ModelState.IsValid)
            {
                _dal.UpdateIntoRetractReason(id, item.RetractReason);

                TempData["message"] = "Retract Reason has been successfully edited";

                return RedirectToAction("RetractReasonList", "Admin");
            }
            return View();
        }

        public ActionResult DeleteRetractReason(int retractReasonID)
        {
            _dal.DeleteIntoRetractReason(retractReasonID);
            return RedirectToAction("RetractReasonList", "Admin");
        }

        public ActionResult SkillList()
        {
            string userid = User.Identity.GetUserId();
            ViewBag.SkillsList = _dal.GetSkillsList();
            return View();
        }
 

        [Authorize]
        [HttpGet]
        public ActionResult AddSkill()
        {
            ViewBag.SkillsCategoryList = _dal.GetSkillsCategoryList();
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddSkill(SkillModel model)
        {
            var data = _dal.CheckIfSkillExists(model.skillName, model.CategoryID);

            if (data > 0)
            {
                ModelState.AddModelError(" ", "Record already Exist");
            }
            if (ModelState.IsValid)
            {
                _dal.InsertIntoSkill(model.skillName, model.CategoryID);

                TempData["message"] = "Skill has been successfully added";

                return RedirectToAction("SkillList", "Admin");
            }
            ViewBag.SkillsCategoryList = _dal.GetSkillsCategoryList();
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditSkill(int id)
        {
            ViewBag.SkillsCategoryList = _dal.GetSkillsCategoryList();
            var Div = _dal.GetSkillForEdit(id);

            return View(Div);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSkill(SkillModel item, int id)
        {
            if (ModelState.IsValid)
            {
                _dal.UpdateIntoSkill(id, item.skillName, item.CategoryID);

                TempData["message"] = "Skill has been successfully edited";

                return RedirectToAction("SkillList", "Admin");
            }
            return View();
        }

        public ActionResult DeleteSkill(int skillID)
        {
            _dal.DeleteIntoSkill(skillID);
            return RedirectToAction("SkillList", "Admin");
        }

        public ActionResult SkillProficiencyList()
        {
            string userid = User.Identity.GetUserId();
            ViewBag.SkillProficiencyList = _dal.GetSkillProficiencyList();
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddSkillProficiency()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddSkillProficiency(SkillProficiencyModel model)
        {
            var data = _dal.CheckIfSkillProficiencyExists(model.SkillProficiency);

            if (data > 0)
            {
                ModelState.AddModelError(" ", "Record already Exist");
            }
            if (ModelState.IsValid)
            {
                _dal.InsertIntoSkillProficiency(model.SkillProficiency);

                TempData["message"] = "Skill Proficiency has been successfully added";

                return RedirectToAction("SkillProficiencyList", "Admin");
            }
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditSkillProficiency(int id)
        {
            var Div = _dal.GetSkillProficiencyForEdit(id);

            return View(Div);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSkillProficiency(SkillProficiencyModel item, int id)
        {
            if (ModelState.IsValid)
            {
                _dal.UpdateIntoSkillProficiency(id, item.SkillProficiency);

                TempData["message"] = "Skill Proficiency has been successfully edited";

                return RedirectToAction("SkillProficiencyList", "Admin");
            }
            return View();
        }

        public ActionResult DeleteSkillProficiency(int skillProficiencyID)
        {
            _dal.DeleteIntoSkillProficiency(skillProficiencyID);
            return RedirectToAction("SkillProficiencyList", "Admin");
        }


        #region SalaryCategory  
        public ActionResult SalaryCategoryList()
        {
            string userid = User.Identity.GetUserId();
            ViewBag.SalaryCategoryList = _dal.GetSalaryCategoryList(userid);
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddSalaryCategory()
        {
            string userid = User.Identity.GetUserId();
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSalaryCategory(SalaryCategoryModel model, FormCollection fc)
        {
            string userid = User.Identity.GetUserId();
            string Organisation = fc["Organisation"];

            ViewBag.Organisation = _dal.GetOrganisationList(userid);

            var data = _dal.CheckIfSalaryCategoryExists(model.CategoryDescr, Convert.ToInt32(Organisation));

            if (data > 0)
            {
                ModelState.AddModelError(" ", "Record already Exist");
            }
            if (ModelState.IsValid)
            {
                _dal.InsertIntoSalaryCategory(Convert.ToInt32(Organisation), model.CategoryDescr);

                TempData["message"] = "Salary Category has been successfully added";

                return RedirectToAction("SalaryCategoryList", "Admin");
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditSalaryCategory(int id)
        {

            string userid = User.Identity.GetUserId();

            ViewBag.Organisation = _dal.GetOrganisationList(userid);

            var Div = _dal.GetSalaryCategoryForEdit(id);

            return View(Div);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSalaryCategory(SalaryCategoryModel item, int id, FormCollection fc)
        {
            string userid = User.Identity.GetUserId();
            string Organisation = fc["Organisation"];

            ViewBag.Organisation = _dal.GetOrganisationList(userid);


            if (ModelState.IsValid)
            {
                _dal.UpdateIntoSalaryCategory(id, Convert.ToInt32(Organisation), item.CategoryDescr);

                TempData["message"] = "Salary Category has been successfully edited";

                return RedirectToAction("SalaryCategoryList", "Admin");
            }
            return View();
        }

        public ActionResult DeleteSalaryCategory(int SalaryCategoryID)
        {
            var data = _dal.CheckIfSalaryCategoryExistsInSalarySubCategory(SalaryCategoryID);

            if (data > 0)
            {
                TempData["danger"] = "Salary Category Cannot Be Deactivated Because it Belongs to a Salary Sub Category";
            }

            if (data <= 0)
            {
                _dal.DeleteIntoSalaryCategory(SalaryCategoryID);
                return RedirectToAction("SalaryCategoryList", "Admin");
            }
            return RedirectToAction("SalaryCategoryList", "Admin");

        }
        #endregion

        #region SalarySubCategory 
        public ActionResult SalarySubCategoryList()
        {
            string userid = User.Identity.GetUserId();

            ViewBag.SalarySubCategoryList = _dal.GetSalarySubCategoryList(userid);

            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddSalarySubCategory()
        {
            string userid = User.Identity.GetUserId();

            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            ViewBag.SalaryCategory = _dal.GetAllSalaryCategoryList(userid);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSalarySubCategory(SalarySubCategoryModel model)
        {
            string userid = User.Identity.GetUserId();

            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            ViewBag.SalaryCategory = _dal.GetAllSalaryCategoryList(userid);

            var data = _dal.CheckIfSalarySubCategoryExists(model.SalaryCategoryID, model.Descr);

            if (data > 0)
            {
                ModelState.AddModelError(" ", "Record already Exist");
            }
            if (ModelState.IsValid)
            {
                _dal.InsertIntoSalarySubCategory(model.SalaryCategoryID, model.Descr);

                TempData["message"] = "Salary Sub Category has been successfully added";

                return RedirectToAction("SalarySubCategoryList", "Admin");
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditSalarySubCategory(int id)
        {

            string userid = User.Identity.GetUserId();
            //ViewBag.SalarySubCategory = _dal.GetSalarySubCategoryList(userid);

            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            ViewBag.SalaryCategory = _dal.GetAllSalaryCategoryList(userid);

            var Div = _dal.GetSalarySubCategoryForEdit(id);

            return View(Div);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSalarySubCategory(SalarySubCategoryModel item, int id)
        {
            string userid = User.Identity.GetUserId();
            ViewBag.Organisation = _dal.GetOrganisationList(userid);

            if (ModelState.IsValid)
            {
                _dal.UpdateIntoSalarySubCategory(id, item.SalaryCategoryID, item.Descr);

                TempData["message"] = "Salary Sub Category has been successfully edited";

                return RedirectToAction("SalarySubCategoryList", "Admin");
            }
            return View();
        }

        public ActionResult DeleteSalarySubCategory(int SalarySubCategoryID)
        {
            var data = _dal.CheckIfSalarySubCategoryExistsInSalaryStructure(SalarySubCategoryID);

            if (data > 0)
            {
                TempData["danger"] = "Salary Sub Category Cannot Be Deactivated Because it Belongs to a Salary Structure";
            }

            if (data <= 0)
            {
                _dal.DeleteIntoSalarySubCategory(SalarySubCategoryID);
                return RedirectToAction("SalarySubCategoryList", "Admin");
            }
            return RedirectToAction("SalarySubCategoryList", "Admin");

        }
        #endregion 
        #region SalaryStructure   
        //SalarySubCategory
        [Authorize]
        [HttpPost]
        public ActionResult GetSalarySubCategoryList(int id)
        {
            return Json(_dal.GetSubCategoryListforStructure(id));
        }

        //Salary Category
        //[Authorize]
        //[HttpPost]
        //public JsonResult GetSalaryCategoryList(int id)
        //{
        //    return Json(_dal.GetCategoryListforStructure(id), JsonRequestBehavior.AllowGet);
        //}

        //Salary Category For Edit
        [Authorize]
        [HttpPost]
        public JsonResult GetSalarySubCategoryListForEdit(int id)
        {
            return Json(_dal.GetSubCategoryListforStructure(id), JsonRequestBehavior.AllowGet);
        }

        //Joblevel
        [Authorize]
        [HttpPost]
        public ActionResult GetJobLevelIDList(int id)
        {
            return Json(_dal.GetJobLevelList(Convert.ToString(id)));
        }

        #endregion
        //------------------------------------------------------------------------------------


        public ActionResult StructureList()
        {
            //ViewBag.StructureList = _dal.GetStructureList();
            return View();
        }

        public ActionResult SalaryStructureList()
        {
            string userid = User.Identity.GetUserId();
            ViewBag.SalaryStructureList = _dal.GetSalaryStructureList(userid);
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddSalaryStructure()
        {
            string userid = User.Identity.GetUserId();
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            ViewBag.JobTitle = _dal.GetAllJobTitleList(userid);
            ViewBag.SalaryCategory = _dal.GetSalaryCategoryList(userid);
            ViewBag.SalarySubCategory = _dal.GetSalarySubCategoryList(userid);
            ViewBag.JobLevel = _dal.GetJobLevelList(userid);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSalaryStructure(SalaryStructureModel model)
        {
            string userid = User.Identity.GetUserId();
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            ViewBag.JobTitle = _dal.GetAllJobTitleList(userid);
            ViewBag.SalaryCategory = _dal.GetSalaryCategoryList(userid);
            ViewBag.SalarySubCategory = _dal.GetSalarySubCategoryList(userid);
            ViewBag.JobLevel = _dal.GetJobLevelList(userid);

            var data = _dal.CheckIfSalaryStructureExists(model.JobTitleID);

            if (data > 0)
            {
                ModelState.AddModelError(" ", "Record already Exist");
            }

            if (model.MinValue >= model.MaxValue)
            {
                ModelState.AddModelError(" ", "Minimun Value Cannnot Be Greater Than or Equal to Max Value");
            }

            if (ModelState.IsValid)
            {
                _dal.InsertIntoSalaryStructure(model.JobTitleID, model.SalarySubCategoryID, model.JobLevelID, model.MinValue, model.MaxValue);

                TempData["message"] = "Salary Structure has been successfully added";

                return RedirectToAction("SalaryStructureList", "Admin");
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditSalaryStructure(int id)
        {
            var SalarySubCategoryID = _dal.GetSubCategoryID(id);
            string userid = User.Identity.GetUserId();
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            ViewBag.JobTitle = _dal.GetAllJobTitleList(userid);
            //ViewBag.SalaryCategory = _dal.GetCategoryListforStructure(SalarySubCategoryID[0].SalarySubCategoryID);
            ViewBag.SalaryCategory = _dal.GetSalaryCategoryList(userid);
            ViewBag.SalarySubCategory = _dal.GetSalarySubCategoryList(userid);
            ViewBag.JobLevel = _dal.GetJobLevelList(userid);
            var Div = _dal.GetSalaryStructureForEdit(id);

            return View(Div);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSalaryStructure(SalaryStructureModel item, int id, FormCollection fc)
        {          
            string userid = User.Identity.GetUserId();
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            ViewBag.JobTitle = _dal.GetAllJobTitleList(userid);
            ViewBag.SalaryCategory = _dal.GetCategoryListforStructure(item.SalarySubCategoryID);
            ViewBag.SalarySubCategory = _dal.GetSalarySubCategoryList(userid);
            ViewBag.JobLevel = _dal.GetJobLevelList(userid);

            string MinValue = fc["MinValue"];
            string MaxValue = fc["MaxValue"];

            if (item.MinValue >= item.MaxValue)
            {
                ModelState.AddModelError(" ", "Minimun Value Cannnot Be Greater Than or Equal to Max Value");
            }

            if (item.MinValue == 0 || item.MaxValue == 0)
            {
                ModelState.AddModelError(" ", "Invalid Min or Max Value");
            }

            if (ModelState.IsValid)
            {
                _dal.UpdateIntoSalaryStructure(id, item.JobTitleID, item.SalarySubCategoryID, item.JobLevelID, item.MinValue, item.MaxValue);

                TempData["message"] = "Salary Structure has been successfully edited";

                return RedirectToAction("SalaryStructureList", "Admin");
            }
            return View();
        }

        public ActionResult DeleteSalaryStructure(int SalaryStructureID)
        {
            var data = _dal.CheckIfSalaryStructureExistsInJobProfile(SalaryStructureID);

            if (data > 0)
            {
                TempData["danger"] = "Salary Structure Cannot Be Deactivated Because it Belongs to a Job Profile";
            }

            if (data <= 0)
            {
                _dal.DeleteIntoSalaryStructure(SalaryStructureID);
                return RedirectToAction("SalaryStructureList", "Admin");
            }
            return RedirectToAction("SalaryStructureList", "Admin");

        }
        public ActionResult WithdrawalReasonList()
        {
            string userid = User.Identity.GetUserId();
            ViewBag.WithdrawalReasonList = _dal.GetWithdrawalReasonList();
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddWithdrawalReason()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddWithdrawalReason(WithdrawalReasonModel model)
        {
            var data = _dal.CheckIfWithdrawalReasonExists(model.WithdrawalReason);

            if (data > 0)
            {
                ModelState.AddModelError(" ", "Record already Exist");
            }
            if (ModelState.IsValid)
            {
                _dal.InsertIntoWithdrawalReason(model.WithdrawalReason);

                TempData["message"] = "Withdrawal Reason has been successfully added";

                return RedirectToAction("WithdrawalReasonList", "Admin");
            }
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditWithdrawalReason(int id)
        {
            var Div = _dal.GetWithdrawalReasonForEdit(id);

            return View(Div);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditWithdrawalReason(WithdrawalReasonModel item, int id)
        {
            if (ModelState.IsValid)
            {
                _dal.UpdateIntoWithdrawalReason(id, item.WithdrawalReason);

                TempData["message"] = "Withdrawal Reason has been successfully edited";

                return RedirectToAction("WithdrawalReasonList", "Admin");
            }
            return View();
        }
        public ActionResult DeleteWithdrawalReason(int withdrawalReasonID)
        {
            _dal.DeleteIntoWithdrawalReason(withdrawalReasonID);
            return RedirectToAction("WithdrawalReasonList", "Admin");
        }

        //Get Salary Structure for Job Profile
        [Authorize]
        [HttpPost]
        public ActionResult GetSalaryStructureList(int id)
        {
            return Json(_dal.GetSalaryStructureList(id));
        }

        //Get Skills Per Catergory
        [Authorize]
        [HttpPost]
        public ActionResult GetSkillsPerCatergoryList(int id)
        {
            return Json(_dal.GetSkillsPerCatergiryList(id));
        }


        [Authorize]
        [HttpGet]
        public ActionResult AddVacancyProfile()
        {
            string userid = User.Identity.GetUserId();
            var orgID = _dal.GetOrganisationID(userid);

            ViewBag.JobTitle = _dal.GetAllJobTitleListForStructure(userid);
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            ViewBag.JobLevel = _dal.GetJobLevelList(userid);
            ViewBag.SalaryCategory = _dal.GetSalaryCategoryList(userid);
            ViewBag.SkillsCategoryList = _dal.GetSkillsCategoryList();
            ViewBag.SkillsPerCatergory = _dal.GetSkillsList();
            ViewBag.BehaveComp = _dal.GetBehaveCompPerOrgID(orgID);
            ViewBag.LeadComp = _dal.GetLeadCompPerOrgID(orgID);
            ViewBag.TechComp = _dal.GetTechCompPerOrgID(orgID);
            ViewBag.Disclamer = _dal.GetDisclamer(Convert.ToInt32(orgID));

            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult AddVacancyProfile(JobProfileModel model, FormCollection fc)
        {
            string userid = User.Identity.GetUserId();
            string Organisation = fc["Organisation"];
            string Disclaimer = fc["Disclaimer"];
            string techcomp = string.Empty;
            string leadcomp = string.Empty;
            string behavecomp = string.Empty;

            var orgID = _dal.GetOrganisationID(userid);

            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            ViewBag.JobLevel = _dal.GetJobLevelList(userid);
            ViewBag.SalaryCategory = _dal.GetSalaryCategoryList(userid);
            ViewBag.SkillsPerCatergory = _dal.GetSkillsList();
            ViewBag.JobTitle = _dal.GetAllJobTitleListForStructure(userid);
            ViewBag.SkillsCategoryList = _dal.GetSkillsCategoryList();
            ViewBag.BehaveComp = _dal.GetBehaveCompPerOrgID(orgID);
            ViewBag.LeadComp = _dal.GetLeadCompPerOrgID(orgID);
            ViewBag.TechComp = _dal.GetTechCompPerOrgID(orgID);
            ViewBag.Disclamer = _dal.GetDisclamer(Convert.ToInt32(orgID));

            string vacancyPurpose = this.RemoveSpecialCharacters(model.VacancyPurpose);
            string qualExperience = this.RemoveSpecialCharacters(model.QualificationAndExperience);
            string knowledge = this.RemoveSpecialCharacters(model.Knowledge);
            string addRequirements = $@"{model.AdditionalRequirements}";
            string responsibility = this.RemoveSpecialCharacters(model.Responsibility);
            if (model.SelectedTechComps != null)
            {
                techcomp = string.Join(";", model.SelectedTechComps);
            }
            if (model.SelectedLeadComps != null)
            {
                leadcomp = string.Join(";", model.SelectedLeadComps);
            }
            if (model.SelectedBehaveComps != null)
            {
                behavecomp = string.Join(";", model.SelectedBehaveComps);
            }

            var data = _dal.CheckIfJobProfileExists(Convert.ToInt32(Organisation), model.JobTitleID, vacancyPurpose
                       , qualExperience, addRequirements, Disclaimer, responsibility);

            if (data > 0)
            {
                ModelState.AddModelError(" ", "Record already Exist");
            }

            var data1 = _dal.CheckIfVacancyProfileExists(model.JobTitleID, Convert.ToInt32(Organisation));

            if (data1 > 0)
            {
                ModelState.AddModelError(" ", "Record already Exist");
            }

            if (ModelState.IsValid)
            {
                //int? JobProfileID = _dal.InsertIntoJobProfile(Convert.ToInt32(Organisation), model.JobTitleID, model.VacancyPurpose, model.QualificationAndExperience, model.TechnicalCompetenciesDescription, model.AdditonalRequirements, Disclaimer, model.Responsibility);
                int? JobProfileID = _dal.InsertIntoJobProfile(Convert.ToInt32(Organisation), model.JobTitleID, vacancyPurpose, qualExperience, knowledge
                                    , addRequirements, techcomp, leadcomp, behavecomp, Disclaimer, responsibility, Guid.Parse(userid));

                // create a loop that will filter any thing inside fc that starts with sk_
                //List<string> listOfSkills = new List<string>();
                //for (var i = 0; i < fc.Count; i++)
                //{
                //    var name = fc.Keys[i];
                //    if (name.Contains("sk_"))
                //    {
                //        listOfSkills.Add(fc[i]);
                //    }
                //}

                //var newList = listOfSkills;

                //if (model.CategoryID != null && listOfSkills != null)
                //{

                //    string vqid = null;
                //    vqid = string.Join(";", listOfSkills);
                //    _dal.InsertUpdateVacancyProfileSkill((int)JobProfileID, Convert.ToInt32(model.CategoryID), Convert.ToString(vqid));

                //}


                TempData["message"] = "Job Profile has been successfully added";

                return RedirectToAction("VacancyProfileList", "Admin");
            }
            return View(model);
        }

        private string  RemoveSpecialCharacters(string value)
        {
            return Regex.Replace(value, @"[^0-9A-Za-z ,]", " ").Replace("&#61623", ".").Replace("  61623", ".").Replace("&#61553", ".").Replace("  61553", ".").Replace(" ", " ").Trim();
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditVacancyProfile(int id)
        {
            string userid = User.Identity.GetUserId();
            var orgID = _dal.GetOrganisationID(userid);
            string TechComps = string.Empty;
            string LeadComps = string.Empty;
            string BehaveComps = string.Empty;

            ViewBag.JobTitle = _dal.GetAllJobTitleListForStructure(userid);
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            ViewBag.JobLevel = _dal.GetJobLevelList(userid);
            ViewBag.SalaryCategory = _dal.GetSalaryCategoryList(userid);
            ViewBag.VacancyProfileSkillsList = _dal.GetSelectedSkillsPerCatergiryListForJobProfile(id);
            ViewBag.SkillsPerCatergory = _dal.GetSkillsList();
            ViewBag.SkillsCategoryList = _dal.GetSkillsCategoryList();
            ViewBag.BehaveComp = _dal.GetBehaveCompPerOrgID(orgID);
            ViewBag.LeadComp = _dal.GetLeadCompPerOrgID(orgID);
            ViewBag.TechComp = _dal.GetTechCompPerOrgID(orgID);
            ViewBag.Disclamer = _dal.GetDisclamer(Convert.ToInt32(orgID));
            List<DefinitionModel> Definitions = _dal.GetDefinitions(id);
            foreach (var item in Definitions)
            {
                if (item.Type == "Technical")
                {
                    TechComps += item.Name + "\r\n\r\n" + item.Definition + "\r\n\r\n";
                }

                if (item.Type == "Leadership")
                {
                    LeadComps += item.Name + "\r\n\r\n" + item.Definition + "\r\n\r\n";
                }

                if (item.Type == "Behavioural")
                {
                    BehaveComps += item.Name + "\r\n\r\n" + item.Definition + "\r\n\r\n";
                }
            }
            List<JobProfileViewModel> div = _dal.GetVacancyProfileEdit(id);
            div[0].TechComps = TechComps;
            div[0].LeadComps = LeadComps;
            div[0].BehaveComps = BehaveComps;

            return View(div);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditVacancyProfile(JobProfileModel item, int id, FormCollection fc)
        {
            string userid = User.Identity.GetUserId();
            string Organisation = fc["Organisation"];
            string Disclaimer = fc["Disclaimer"];
            var orgID = _dal.GetOrganisationID(userid);
            string TechComps = string.Empty;
            string LeadComps = string.Empty;
            string BehaveComps = string.Empty;
            string techcomp = string.Empty;
            string leadcomp = string.Empty;
            string behavecomp = string.Empty;

            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            ViewBag.JobLevel = _dal.GetJobLevelList(userid);
            ViewBag.SalaryCategory = _dal.GetSalaryCategoryList(userid);
            ViewBag.SkillsPerCatergory = _dal.GetSkillsList();
            ViewBag.JobTitle = _dal.GetAllJobTitleListForStructure(userid);
            ViewBag.VacancyProfileSkillsList = _dal.GetSelectedSkillsPerCatergiryListForJobProfile(item.JobProfileID);
            ViewBag.SkillsPerCatergory = _dal.GetSkillsList();
            ViewBag.SkillsCategoryList = _dal.GetSkillsCategoryList();
            ViewBag.BehaveComp = _dal.GetBehaveCompPerOrgID(orgID);
            ViewBag.LeadComp = _dal.GetLeadCompPerOrgID(orgID);
            ViewBag.TechComp = _dal.GetTechCompPerOrgID(orgID);
            ViewBag.Disclamer = _dal.GetDisclamer(Convert.ToInt32(orgID));

            List<DefinitionModel> Definitions = _dal.GetDefinitions(id);
            foreach (var item1 in Definitions)
            {
                if (item1.Type == "Technical")
                {
                    TechComps += item1.Name + "\r\n\r\n" + item1.Definition + "\r\n\r\n";
                }

                if (item1.Type == "Leadership")
                {
                    LeadComps += item1.Name + "\r\n\r\n" + item1.Definition + "\r\n\r\n";
                }

                if (item1.Type == "Behavioural")
                {
                    BehaveComps += item1.Name + "\r\n\r\n" + item1.Definition + "\r\n\r\n";
                }
            }
            List<JobProfileViewModel> div = _dal.GetVacancyProfileEdit(id);
            div[0].TechComps = TechComps;
            div[0].LeadComps = LeadComps;
            div[0].BehaveComps = BehaveComps;

            if (ModelState.IsValid)
            {
                //string vacancyPurpose = this.RemoveSpecialCharacters(item.VacancyPurpose);
                //string qualExperience = this.RemoveSpecialCharacters(item.QualificationAndExperience);
                //string knowledge = this.RemoveSpecialCharacters(item.Knowledge);
                //string addRequirements = $@"{item.AdditionalRequirements}";
                //string responsibility = this.RemoveSpecialCharacters(item.Responsibility);
                string vacancyPurpose = item.VacancyPurpose;
                string qualExperience = item.QualificationAndExperience;
                string knowledge = item.Knowledge;
                string addRequirements = $@"{item.AdditionalRequirements}";
                string responsibility = item.Responsibility;

                if (item.SelectedTechComps != null)
                {
                    techcomp = string.Join(";", item.SelectedTechComps);
                }
                if (item.SelectedLeadComps != null)
                {
                    leadcomp = string.Join(";", item.SelectedLeadComps);
                }
                if (item.SelectedBehaveComps != null)
                {
                    behavecomp = string.Join(";", item.SelectedBehaveComps);
                }


                //int? JobProfileID = _dal.UpdateIntoJobProfile(id, Convert.ToInt32(Organisation), item.JobTitleID, item.VacancyPurpose, item.QualificationAndExperience, item.TechnicalCompetenciesDescription, item.AdditonalRequirements, Disclaimer, item.Responsibility);
                int? JobProfileID = _dal.UpdateIntoJobProfile(id, Convert.ToInt32(Organisation), item.JobTitleID, vacancyPurpose, qualExperience, knowledge,
                                    addRequirements, techcomp, leadcomp, behavecomp, Disclaimer, responsibility, Guid.Parse(userid));


                // create a loop that will filter any thing inside fc that starts with sk_
                //List<string> listOfSkills = new List<string>();
                //for (var i = 0; i < fc.Count; i++)
                //{
                //    var name = fc.Keys[i];
                //    if (name.Contains("sk_"))
                //    {
                //        listOfSkills.Add(fc[i]);
                //    }
                //}

                //var newList = listOfSkills;

                //if (item.CategoryID != null && listOfSkills != null)
                //{

                //    string vqid = null;
                //    vqid = string.Join(";", listOfSkills);
                //    _dal.InsertUpdateVacancyProfileSkill((int)JobProfileID, Convert.ToInt32(item.CategoryID), Convert.ToString(vqid));

                //}

                TempData["message"] = "Job Profile has been successfully edited";

                return RedirectToAction("VacancyProfileList", "Admin");
            }
            return View();
        }

        public ActionResult DeleteJobProfile(int JobProfileID)
        {
            string userid = User.Identity.GetUserId();
            var orgID = _dal.GetOrganisationID(userid);

            var data1 = _dal.CheckIfVacancyProfileExistsOnAdvert(JobProfileID, Convert.ToInt32(orgID));

            if (data1 > 0)
            {
                TempData["message1"] = "Record Cannot Be De-Activated Due to an Active Advert";
            }
            if (data1 <= 0)
            {
                _dal.DeleteJobProfile(JobProfileID);
                return RedirectToAction("VacancyProfileList", "Admin");
            }
            return RedirectToAction("VacancyProfileList", "Admin");
        }


        //Get Skills Per Catergory Check Box
        [Authorize]
        [HttpPost]
        public ActionResult GetSkillPerCatergory(int id)
        {
            return Json(_dal.GetSkillsPerCatergiryList(id));
        }

        [HttpGet]
        public FileResult DownLoadFile(int OrganisationID)
        {
            var doc = _db.lutOrganisations.Where(x => x.OrganisationID == OrganisationID).FirstOrDefault();
            return File(doc.fileData.ToArray(), doc.contentType, doc.fileName);
        }


        public ActionResult BehaveCompetencyList()
        {
            string userid = User.Identity.GetUserId();
            int orgid = _dal.GetOrgId(userid);
            ViewBag.BehaveCompetency = _dal.GetBehaveCompList(orgid);
            return View();
        }

        [Authorize]
        [HttpPost]
        public JsonResult GetBehaveCompPerIDs(string ids)
        {
            return Json(_dal.GetBehaveCompPerIDs(ids), JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddBehaveCompetency()
        {
            string userid = User.Identity.GetUserId();
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddBehaveCompetency(BehaviouralCompModel model, FormCollection fc)
        {
            string userid = User.Identity.GetUserId();
            string Organisation = fc["Organisation"];

            var data = _dal.CheckIfBehaveCompExists(model.BehaviouralComp, Convert.ToInt32(Organisation));

            if (data > 0)
            {
                ModelState.AddModelError(" ", "Record already Exist");
            }
            if (ModelState.IsValid)
            {
                _dal.InsertIntoBehaveComp(model.BehaviouralComp, model.BehaviouralCompDesc, Convert.ToInt32(Organisation), Guid.Parse(userid));

                TempData["message"] = "Behavioural Competency has been successfully added";

                return RedirectToAction("BehaveCompetencyList", "Admin");
            }
            
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditBehaveCompetency(int id)
        {

            string userid = User.Identity.GetUserId();
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            BehaviouralCompModel behavecomp = _dal.GetBehaveCompForEdit(id);

            return View(behavecomp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBehaveCompetency(BehaviouralCompModel item, int id, FormCollection fc)
        {
            string userid = User.Identity.GetUserId();
            string Organisation = fc["Organisation"];

            if (ModelState.IsValid)
            {
                _dal.UpdateIntoBehaveComp(id, Convert.ToInt32(Organisation), item.BehaviouralComp, item.BehaviouralCompDesc, Guid.Parse(userid));

                TempData["message"] = "Behavioural Competency has been successfully edited";

                return RedirectToAction("BehaveCompetencyList", "Admin");
            }
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            return View();
        }

        public ActionResult DeleteBehaveCompetency(int id)
        {
            string userid = User.Identity.GetUserId();
            var data = _dal.CheckIfBehaveCompExistsInJobProfile(id);

            if (data > 0)
            {
                TempData["danger"] = "Behavioural Competency Cannot Be Deactivated Because it Belongs to a Organisation";
            }

            if (data <= 0)
            {
                _dal.DeleteIntoBehaveComp(id, Guid.Parse(userid));
            }
            return RedirectToAction("BehaveCompetencyList", "Admin");

        }

        public ActionResult LeadCompetencyList()
        {
            string userid = User.Identity.GetUserId();
            int orgid = _dal.GetOrgId(userid);
            ViewBag.LeadCompetency = _dal.GetLeadCompList(orgid);
            return View();
        }

        [Authorize]
        [HttpPost]
        public JsonResult GetLeadCompPerIDs(string ids)
        {
            return Json(_dal.GetLeadCompPerIDs(ids), JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddLeadCompetency()
        {
            string userid = User.Identity.GetUserId();
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddLeadCompetency(LeadershipCompModel model, FormCollection fc)
        {
            string userid = User.Identity.GetUserId();
            string Organisation = fc["Organisation"];

            var data = _dal.CheckIfLeadCompExists(model.LeadershipComp, Convert.ToInt32(Organisation));

            if (data > 0)
            {
                ModelState.AddModelError(" ", "Record already Exist");
            }
            if (ModelState.IsValid)
            {
                _dal.InsertIntoLeadComp(model.LeadershipComp, model.LeadershipCompDesc, Convert.ToInt32(Organisation), Guid.Parse(userid));

                TempData["message"] = "Leadership Competency has been successfully added";

                return RedirectToAction("LeadCompetencyList", "Admin");
            }

            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditLeadCompetency(int id)
        {
            string userid = User.Identity.GetUserId();
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            LeadershipCompModel behavecomp = _dal.GetLeadCompForEdit(id);

            return View(behavecomp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditLeadCompetency(LeadershipCompModel item, int id, FormCollection fc)
        {
            string userid = User.Identity.GetUserId();
            string Organisation = fc["Organisation"];

            if (ModelState.IsValid)
            {
                _dal.UpdateIntoLeadComp(id, Convert.ToInt32(Organisation), item.LeadershipComp, item.LeadershipCompDesc, Guid.Parse(userid));

                TempData["message"] = "Leadership Competency has been successfully edited";

                return RedirectToAction("LeadCompetencyList", "Admin");
            }
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            return View();
        }

        public ActionResult DeleteLeadCompetency(int id)
        {
            string userid = User.Identity.GetUserId();
            var data = _dal.CheckIfLeadCompExistsInJobProfile(id);

            if (data > 0)
            {
                TempData["danger"] = "Leadership Competency Cannot Be Deactivated Because it Belongs to a Organisation";
            }

            if (data <= 0)
            {
                _dal.DeleteIntoLeadComp(id, Guid.Parse(userid));
            }
            return RedirectToAction("LeadCompetencyList", "Admin");

        }

        public ActionResult TechCompetencyList()
        {
            string userid = User.Identity.GetUserId();
            int orgid = _dal.GetOrgId(userid);
            ViewBag.TechCompetency = _dal.GetTechCompList(orgid);
            return View();
        }

        [Authorize]
        [HttpPost]
        public JsonResult GetTechCompPerIDs(string ids)
        {
            return Json(_dal.GetTechCompPerIDs(ids), JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddTechCompetency()
        {
            string userid = User.Identity.GetUserId();
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTechCompetency(TechnicalCompModel model, FormCollection fc)
        {
            string userid = User.Identity.GetUserId();
            string Organisation = fc["Organisation"];

            var data = _dal.CheckIfTechCompExists(model.TechnicalComp, Convert.ToInt32(Organisation));

            if (data > 0)
            {
                ModelState.AddModelError(" ", "Record already Exist");
            }
            if (ModelState.IsValid)
            {
                _dal.InsertIntoTechComp(model.TechnicalComp, model.TechnicalCompDesc, Convert.ToInt32(Organisation), Guid.Parse(userid));

                TempData["message"] = "Technical Competency has been successfully added";

                return RedirectToAction("TechCompetencyList", "Admin");
            }

            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditTechCompetency(int id)
        {
            string userid = User.Identity.GetUserId();
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            TechnicalCompModel techcomp = _dal.GetTechCompForEdit(id);

            return View(techcomp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTechCompetency(TechnicalCompModel item, int id, FormCollection fc)
        {
            string userid = User.Identity.GetUserId();
            string Organisation = fc["Organisation"];

            if (ModelState.IsValid)
            {
                _dal.UpdateIntoTechComp(id, Convert.ToInt32(Organisation), item.TechnicalComp, item.TechnicalCompDesc, Guid.Parse(userid));

                TempData["message"] = "Technical Competency has been successfully edited";

                return RedirectToAction("TechCompetencyList", "Admin");
            }
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            return View();
        }

        public ActionResult DeleteTechCompetency(int id)
        {
            string userid = User.Identity.GetUserId();
            var data = _dal.CheckIfTechCompExistsInJobProfile(id);

            if (data > 0)
            {
                TempData["danger"] = "Technical Competency Cannot Be Deactivated Because it Belongs to a Organisation";
            }

            if (data <= 0)
            {
                _dal.DeleteIntoTechComp(id, Guid.Parse(userid));
            }
            return RedirectToAction("TechCompetencyList", "Admin");

        }

    }
}