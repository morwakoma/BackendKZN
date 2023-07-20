using AutoMapper;
using eRecruitment.BusinessDomain.DAL.Entities.AppModels;
using eRecruitment.Sita.BackEnd.App_Data.Entities.DAL;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace eRecruitment.BusinessDomain.DAL
{
    public class DataAccess
    {
        eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext();

        public List<UserListModel> GetAllUserList(int UserOrganizationID, string emailAddress)
        {
            var p = new List<UserListModel>();
            var data = (dynamic)null;
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                if (emailAddress == "")
                {
                    //        var data = (from a in _db.AspNetUsers
                    //join b in _db.tblProfiles on a.Id equals b.UserID
                    //join c in _db.AspNetUserRoles on a.Id equals c.UserId
                    //join d in _db.AspNetRoles on c.RoleId equals d.Id
                    //into UserRoleGroup
                    //from d in UserRoleGroup.DefaultIfEmpty()
                    //where a.Email != "eRecruit.Admin@sita.co.za" && a.Email != "portal.admin@sita.co.za" && c.OrganisationID != 0
                    ////&& c.OrganisationID == UserOrganizationID
                    //select new { a.Id, Name = (c.RoleId == "0") ? "Not Assigned" : d.Name, a.Email, b.Surname, b.FirstName, b.CellNo, Status = (a.IsActive == 0) ? "Deactive" : "Active" }).ToList();
                }
                else
                {

                }

                data = (from a in _db.AspNetUsers
                        join b in _db.tblProfiles on a.Id equals b.UserID
                        join c in _db.AspNetUserRoles on a.Id equals c.UserId
                        join d in _db.AspNetRoles on c.RoleId equals d.Id
                        into UserRoleGroup
                        from d in UserRoleGroup.DefaultIfEmpty()
                        where a.Email != "eRecruit.Admin@sita.co.za" && a.Email != "portal.admin@sita.co.za" && c.OrganisationID != 0
                        && c.OrganisationID == UserOrganizationID
                        select new { a.Id, Name = (c.RoleId == "0") ? "Not Assigned" : d.Name, a.Email, b.Surname, b.FirstName, b.CellNo, Status = (a.IsActive == 0) ? "Deactive" : "Active" }).ToList();

                foreach (var d in data)
                {

                    UserListModel e = new UserListModel
                    {
                        UserID = Convert.ToString(d.Id),
                        Email = Convert.ToString(d.Email),
                        FullName = Convert.ToString(d.FirstName) + " " + Convert.ToString(d.Surname),
                        CellNo = Convert.ToString(d.CellNo),
                        RoleName = Convert.ToString(d.Name),
                        Status = Convert.ToString(d.Status)
                    };
                    p.Add(e);
                }
                return p;
            }
        }

        public List<DepartmentModel> GetDepartmentListUsingDivisionID(int id)
        {
            var p = new List<DepartmentModel>();
            _db = new eRecruitmentDataClassesDataContext();


            var data = _db.lutDepartments.Where(x => x.DivisionID == id);


            foreach (var d in data)
            {
                DepartmentModel e = new DepartmentModel();
                e.DepartmentID = Convert.ToInt32(d.DepartmentID);
                e.DepartmentName = Convert.ToString(d.DepartmentDiscription);
                //e.OrganisationName = Convert.ToString(d.OrganisationID);
                p.Add(e);
            }
            return p;
        }

        public List<UserListModel> GetUserForAssignUseraRole(string id)
        {
            var p = new List<UserListModel>();
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                var data = (from a in _db.AspNetUsers
                            join b in _db.tblProfiles on a.Id equals b.UserID
                            join c in _db.AspNetUserRoles on a.Id equals c.UserId
                            join h in _db.AspNetRoles on c.RoleId equals h.Id
                            //from d in dep.DefaultIfEmpty()
                            where a.Id == id
                            select new
                            {
                                UserID = a.Id,
                                RoleName = h.Name,
                                RoleID = h.Id,
                                Email = a.Email,
                                Surname = b.Surname,
                                FirstName = b.FirstName,
                                CellNo = b.CellNo,
                                OrganisationID = c.OrganisationID

                            }).FirstOrDefault();

                var selected = _db.sp_AssignedDivisionDepartment_Get_PerUserID(Guid.Parse(id));

                if (data != null)
                {
                    UserListModel e = new UserListModel
                    {
                        UserID = Convert.ToString(data.UserID),
                        Email = Convert.ToString(data.Email),
                        FullName = Convert.ToString(data.FirstName) + " " + Convert.ToString(data.Surname),
                        CellNo = Convert.ToString(data.CellNo),
                        OrganisationID = Convert.ToString(data.OrganisationID),
                        RoleID = Convert.ToString(data.RoleID),
                        //RoleName = Convert.ToString(data.RoleName)
                    };

                    foreach (var s in selected)
                    {
                        if (s.SelectedDivisions != null)
                        {
                            e.SelectedDivisions = Convert.ToString(s.SelectedDivisions).Split(';');
                        }
                        if (s.SelectedDepartments != null)
                        {
                            e.SelectedDepartments = Convert.ToString(s.SelectedDepartments).Split(';');
                        }
                    }
                    p.Add(e);
                }

                return p;
            }
        }

        public List<DivisionModel> GetDivisionListUsingOrganisationID(int id)
        {
            var p = new List<DivisionModel>();
            _db = new eRecruitmentDataClassesDataContext();


            var data = _db.lutDivisions.Where(x => x.OrganisationID == id);


            foreach (var d in data)
            {
                DivisionModel e = new DivisionModel();
                e.DivisionID = Convert.ToInt32(d.DivisionID);
                e.DivisionDiscription = Convert.ToString(d.DivisionDiscription);
                //e.OrganisationName = Convert.ToString(d.OrganisationID);
                p.Add(e);
            }
            return p;
        }

        public List<UserRoleModel> GetRoleList()
        {
            var p = new List<UserRoleModel>();

            var Roles = _db.AspNetRoles.ToList();
            foreach (var d in Roles)
            {
                UserRoleModel e = new UserRoleModel();
                e.RoleID = Convert.ToInt32(d.Id);
                e.RoleName = Convert.ToString(d.Name);
                p.Add(e);
            }
            return p;

        }

        public List<UserRoleModel> GetOfficialRoleList()
        {
            var p = new List<UserRoleModel>();

            var Roles = _db.AspNetRoles.Where(x => x.Name != "Citizen").ToList();
            foreach (var d in Roles)
            {
                UserRoleModel e = new UserRoleModel();
                e.RoleID = Convert.ToInt32(d.Id);
                e.RoleName = Convert.ToString(d.Name);
                p.Add(e);
            }
            return p;

        }

        public void AssignRole(string userID, string roleID, int OrganisationID)
        {
            //_db.proc_eRecruitmentAssignRole(userID, roleID, OrganisationID);
            _db.proc_eRecruitmentReassignUserRole(userID, roleID, OrganisationID, 1);
        }

        public void AssignDivisionDepartment(string userID, int OrganisationID, string DivisionID, string DepartmentID)
        {
            _db.proc_eRecruitmentAssignDivisionDepartment(userID, OrganisationID, DivisionID, DepartmentID);
        }

        //DeleteAssignedDivisionDepartment
        public void DeleteAssignedDivisionDepartment(int id)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentDeleteAssignedDivisionDepartment(id);

        }

        public List<UserListModel> GetUserForEditUseraRoleOrganisation(string id)
        {
            var p = new List<UserListModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = (from a in _db.AspNetUsers
                        join b in _db.tblProfiles on a.Id equals b.UserID
                        join c in _db.AspNetUserRoles on a.Id equals c.UserId
                        join h in _db.AspNetRoles on c.RoleId equals h.Id
                        into UserRoleGroup
                        from h in UserRoleGroup.DefaultIfEmpty()
                        where a.Id == id
                        select new
                        {
                            UserID = a.Id,
                            RoleName = h.Name,
                            RoleID = h.Id,
                            Email = a.Email,
                            Surname = b.Surname,
                            FirstName = b.FirstName,
                            CellNo = b.CellNo,
                            OrganisationID = c.OrganisationID

                        }).FirstOrDefault();

            if (data != null)
            {
                UserListModel e = new UserListModel
                {
                    UserID = Convert.ToString(data.UserID),
                    Email = Convert.ToString(data.Email),
                    FullName = Convert.ToString(data.FirstName) + " " + Convert.ToString(data.Surname),
                    CellNo = Convert.ToString(data.CellNo),
                    OrganisationID = Convert.ToString(data.OrganisationID),
                    RoleID = Convert.ToString(data.RoleID),
                    RoleName = Convert.ToString(data.RoleName)
                };
                p.Add(e);
            }

            return p;
        }

        public List<UserListModel> GetUserForAssignUseraRoleOrganisation(string id)
        {
            var p = new List<UserListModel>();
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                var data = (from a in _db.AspNetUsers
                            join b in _db.tblProfiles on a.Id equals b.UserID
                            //join c in _db.AspNetUserRoles on a.Id equals c.UserId
                            //join h in _db.AspNetRoles on c.RoleId equals h.Id
                            //from d in dep.DefaultIfEmpty()
                            where a.Id == id
                            select new
                            {
                                UserID = a.Id,
                                //RoleName = h.Name,
                                //RoleID = h.Id,
                                Email = a.Email,
                                Surname = b.Surname,
                                FirstName = b.FirstName,
                                CellNo = b.CellNo,
                                //OrganisationID = c.OrganisationID

                            }).FirstOrDefault();

                UserListModel e = new UserListModel
                {
                    UserID = Convert.ToString(data.UserID),
                    Email = Convert.ToString(data.Email),
                    FullName = Convert.ToString(data.FirstName) + " " + Convert.ToString(data.Surname),
                    CellNo = Convert.ToString(data.CellNo)
                    //OrganisationID = Convert.ToString(data.OrganisationID),
                    //RoleID = Convert.ToString(data.RoleID),
                    //RoleName = Convert.ToString(data.RoleName)
                };
                p.Add(e);

                return p;
            }
        }

        //DeActivateUserRole
        public void DeActivateUserRole(string userID, string RoleID, int OrganisationID, int StatusID)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentDeleteAssignedDivisionDepartmentUsingUserID(userID);
            _db.proc_eRecruitmentReassignUserRole(userID, RoleID, OrganisationID, StatusID);

        }

        public void ActivateDeActivateUser(string userId, bool isActive)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.sp_AspNetUsersActivateDeActivateUser_Post(isActive, userId);
        }

        public List<UserRoleDetailsGeneral> GetUserRoleDetailsGeneral(string id)
        {
            var p = new List<UserRoleDetailsGeneral>();

            var data = from a in _db.AspNetUsers
                       join b in _db.AspNetUserRoles on a.Id equals b.UserId
                       join c in _db.AspNetRoles on b.RoleId equals c.Id
                       join h in _db.AssignedDivisionDepartments on b.UserId equals h.UserId
                       join d in _db.lutDepartments on h.DepartmentID equals d.DepartmentID
                       join f in _db.lutDivisions on h.DivisionID equals f.DivisionID
                       join g in _db.lutOrganisations on h.OrganisationID equals g.OrganisationID

                       where b.UserId == id
                       select new
                       {
                           OrganisationName = g.OrganisationName,
                           DivisionName = f.DivisionDiscription,
                           DepartmentName = d.DepartmentDiscription,
                           AssignedDivisionDepartmentID = h.AssignedDivisionDepartmentID

                       };
            foreach (var d in data)
            {
                UserRoleDetailsGeneral e = new UserRoleDetailsGeneral();
                e.OrganisationName = Convert.ToString(d.OrganisationName);
                e.DepartmentName = Convert.ToString(d.DepartmentName);
                e.DivisionName = Convert.ToString(d.DivisionName);
                e.AssignedDivisionDepartmentID = Convert.ToInt32(d.AssignedDivisionDepartmentID);


                p.Add(e);
            }
            return p;

        }


        #region "--Get Province List--"
        /// <summary>
        /// The following function returns list of all provinces
        /// </summary>
        /// <returns></returns>
        /// <remarks><para><b>Created By:</b>Ntshengedzeni Badamarema - 2019/03/05 </para></remarks>
        public List<ProvinceListModel> GetProvinceList()
        {
            var p = new List<ProvinceListModel>();
            _db = new eRecruitmentDataClassesDataContext();

            //============Peter 20221028============
            var data = _db.lutProvinces.Where(x => x.ProvinceID != -1).OrderBy(x => x.ProvinceName).ToList();
            //======================================
            //var data = _db.lutProvinces.Where(x => x.ProvinceID != -1).ToList();

            foreach (var d in data)
            {
                ProvinceListModel e = new ProvinceListModel();
                e.ProvinceID = Convert.ToInt32(d.ProvinceID);
                e.ProvinceName = Convert.ToString(d.ProvinceName);
                p.Add(e);
            }
            return p;

        }
        #endregion

        #region"--Get User Info By Id-"
        /// <summary>
        /// Get User Info By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <remarks><para><b>Created By</b>Ntshengedzeni Badamarema - 2019/05/03</para></remarks>
        public List<UserModel> GetUserInfoById(string id)
        {
            _db = new eRecruitmentDataClassesDataContext();
            var p = new List<UserModel>();

            var data = (from a in _db.AspNetUsers
                        join b in _db.tblProfiles on a.Id equals b.UserID
                        where a.Id == id
                        select new
                        {
                            b.Surname,
                            b.FirstName,
                        }).FirstOrDefault();

            //return data.FirstName;
            UserModel e = new UserModel
            {
                FirstName = Convert.ToString(data.FirstName),
                Surname = Convert.ToString(data.Surname)
            };
            p.Add(e);
            return p;
        }
        #endregion

        #region "--Get Organisation List--"
        /// <summary>
        /// Get Organisation List By UserId
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        /// <remarks><para><b>Created By</b>Ntshengedzeni Badamarema - 2019/05/09</para>
        /// <para><b>Modified By</b></para>
        /// </remarks>
        public List<OrganisationModel> GetOrganisationList(string userid)
        {
            var p = new List<OrganisationModel>();
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                //var data = _db.lutOrganisations.ToList();
                var data = from a in _db.lutOrganisations
                           join b in _db.AspNetUserRoles on a.OrganisationID equals b.OrganisationID
                           where b.UserId == userid
                           orderby a.OrganisationName ascending
                           select a;

                foreach (var d in data)
                {
                    OrganisationModel e = new OrganisationModel();
                    e.OrganisationID = Convert.ToInt32(d.OrganisationID);
                    e.OrganisationCode = Convert.ToString(d.OrganisationCode);
                    e.OrganisationName = Convert.ToString(d.OrganisationName);
                    p.Add(e);
                }
                return p;
            }
        }
        #endregion

        public List<OrganisationModel> GetOrganisationListById(int id)
        {
            var p = new List<OrganisationModel>();
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                var data = _db.lutOrganisations.Where(x => x.OrganisationID == id).ToList();
                foreach (var d in data)
                {
                    OrganisationModel e = new OrganisationModel();
                    e.OrganisationID = Convert.ToInt32(d.OrganisationID);
                    e.OrganisationName = Convert.ToString(d.OrganisationName);
                    e.OrganisationCode = Convert.ToString(d.OrganisationCode);
                    p.Add(e);
                }

            }
            return p;

        }

        public OrganisationModel GetOrganisationName(string UserID)
        {
            var p = new OrganisationModel();
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                var data = from a in _db.lutOrganisations
                           join b in _db.AspNetUserRoles on a.OrganisationID equals b.OrganisationID
                           where b.UserId == UserID


                           select new
                           {
                               OrganisationID = a.OrganisationID,
                               OrganisationName = a.OrganisationName,
                               OrganisationCode = a.OrganisationCode
                           };


                OrganisationModel e = new OrganisationModel();
                foreach (var d in data)
                {
                    e.OrganisationID = Convert.ToInt32(d.OrganisationID);
                    e.OrganisationName = Convert.ToString(d.OrganisationName);
                    e.OrganisationCode = Convert.ToString(d.OrganisationCode);
                }
                return e;
            }

        }

        public int CheckIfOrganisationExists(string OrganisationName, string OrganisationCode, int OrganisationID)
        {
            return _db.lutOrganisations.Where(x => x.OrganisationName == OrganisationName
                            && x.OrganisationCode == OrganisationCode
                            && x.OrganisationID == OrganisationID).Count();
        }

        public int CheckIfOrganisationExistsInUser(int OrganisationID)
        {
            return _db.AspNetUserRoles.Where(x => x.OrganisationID == OrganisationID).Count();
        }

        public void InsertIntoOrganisation(string organisationCode, string organisationName, string fileName, byte[] fileData, string contentType)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentInsertIntoOrganisation(organisationCode, organisationName, fileName, fileData, contentType);

        }

        public void UpdateOrganisation(string organisationCode, string organisationName, string fileName, byte[] fileData, string contentType, int organisationID)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentUpdateOrganisation(organisationCode, organisationName, fileName, fileData, contentType, organisationID);

        }

        public List<OrganisationModel> GetAllOrganisationList(string userid, string role)
        {
            var p = new List<OrganisationModel>();
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                var data = (dynamic)null;

                if (role == "Admin")
                {
                    data = from a in _db.lutOrganisations
                           join b in _db.AspNetUserRoles on a.OrganisationID equals b.OrganisationID
                           where b.UserId == userid
                           select a;
                }
                else if (role == "SysAdmin")
                {
                    data = _db.lutOrganisations.Where(x => x.OrganisationID != 0).ToList();
                }
                else if (role == "Recruiter")
                {
                    data = from a in _db.lutOrganisations
                           join b in _db.AspNetUserRoles on a.OrganisationID equals b.OrganisationID
                           where b.UserId == userid
                           select a;
                }
                else if (role == "Approver")
                {
                    data = from a in _db.lutOrganisations
                           join b in _db.AspNetUserRoles on a.OrganisationID equals b.OrganisationID
                           where b.UserId == userid
                           select a;
                }


                //var data = _db.lutOrganisations.Where(x=>x.OrganisationID != 0).ToList();

                foreach (var d in data)
                {
                    OrganisationModel e = new OrganisationModel
                    {
                        OrganisationID = Convert.ToInt32(d.OrganisationID),
                        OrganisationCode = Convert.ToString(d.OrganisationCode),
                        OrganisationName = Convert.ToString(d.OrganisationName),
                        FileName = Convert.ToString(d.fileName)
                    };
                    p.Add(e);
                }
                return p;
            }
        }

        public List<OrganisationModel> GetAllOrganisationListForDep(string userid, string role)
        {
            var p = new List<OrganisationModel>();
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                var data = (dynamic)null;

                if (role == "Admin")
                {
                    data = from a in _db.lutOrganisations
                           join b in _db.AspNetUserRoles on a.OrganisationID equals b.OrganisationID
                           where b.UserId == userid
                           select a;
                }
                else if (role == "SysAdmin")
                {
                    data = from a in _db.lutOrganisations
                           join b in _db.AspNetUserRoles on a.OrganisationID equals b.OrganisationID
                           where b.UserId == userid
                           select a;
                }
                else if (role == "Recruiter")
                {
                    data = from a in _db.lutOrganisations
                           join b in _db.AspNetUserRoles on a.OrganisationID equals b.OrganisationID
                           where b.UserId == userid
                           select a;
                }
                else if (role == "Approver")
                {
                    data = from a in _db.lutOrganisations
                           join b in _db.AspNetUserRoles on a.OrganisationID equals b.OrganisationID
                           where b.UserId == userid
                           select a;
                }


                //var data = _db.lutOrganisations.Where(x=>x.OrganisationID != 0).ToList();

                foreach (var d in data)
                {
                    OrganisationModel e = new OrganisationModel
                    {
                        OrganisationID = Convert.ToInt32(d.OrganisationID),
                        OrganisationCode = Convert.ToString(d.OrganisationCode),
                        OrganisationName = Convert.ToString(d.OrganisationName),
                        FileName = Convert.ToString(d.fileName)
                    };
                    p.Add(e);
                }
                return p;
            }
        }

        //DeleteOrganisation
        public void DeleteOrganisation(int id)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentDeleteOrganisation(id);

        }

        public List<DepartmentModel> GetDepartmentList(string userid)
        {
            var p = new List<DepartmentModel>();
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {

                var data = from a in _db.lutDepartments
                           join b in _db.lutDivisions on a.DivisionID equals b.DivisionID
                           join c in _db.AspNetUserRoles on b.OrganisationID equals c.OrganisationID
                           join d in _db.lutOrganisations on b.OrganisationID equals d.OrganisationID
                           //join e in _db.tblProfiles on a.ManagerID equals e.UserID
                           where c.UserId == userid

                           select new
                           {
                               DepartmentID = a.DepartmentID,
                               DivisionID = b.DivisionID,
                               DepartmentName = a.DepartmentDiscription,
                               DivisionDiscription = b.DivisionDiscription,
                               OrganisationName = d.OrganisationName
                               ,
                               // ManagerName = e.FirstName + " " + e.Surname
                           };

                foreach (var d in data)
                {
                    DepartmentModel e = new DepartmentModel();
                    e.DepartmentID = Convert.ToInt32(d.DepartmentID);
                    e.DepartmentName = Convert.ToString(d.DepartmentName);
                    e.DivisionDiscription = Convert.ToString(d.DivisionDiscription);
                    e.OrganisationName = Convert.ToString(d.OrganisationName);
                    //e.ManagerName = Convert.ToString(d.ManagerName);
                    ///e.ManagerName = d.ManagerName;
                    p.Add(e);
                }
                return p;
            }
        }

        public List<DepartmentModel> GetDepartment(string userid)
        {
            var p = new List<DepartmentModel>();
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {

                var data = from a in _db.AspNetUserRoles
                           join b in _db.lutOrganisations on a.OrganisationID equals b.OrganisationID
                           join c in _db.lutDivisions on b.OrganisationID equals c.OrganisationID
                           join d in _db.lutDepartments on c.DivisionID equals d.DivisionID
                           //join e in _db.tblProfiles on d.ManagerID equals e.UserID
                           where a.UserId == userid
                           //============Peter 20221028============
                           orderby d.DepartmentDiscription.Trim() ascending
                           //======================================
                           select new
                           {
                               DepartmentID = d.DepartmentID,
                               DepartmentName = d.DepartmentDiscription,

                           };

                foreach (var d in data)
                {
                    DepartmentModel e = new DepartmentModel();
                    e.DepartmentID = Convert.ToInt32(d.DepartmentID);
                    e.DepartmentName = Convert.ToString(d.DepartmentName);
                    p.Add(e);
                }
                return p;
            }
        }

        public List<DepartmentModel> GetDepartmentListByDivision(int id)
        {
            var p = new List<DepartmentModel>();
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {

                var data = from a in _db.lutDepartments
                           join b in _db.lutDivisions on a.DivisionID equals b.DivisionID
                           join c in _db.lutOrganisations on b.OrganisationID equals c.OrganisationID
                           where b.DivisionID == id

                           select new
                           {
                               DepartmentID = a.DepartmentID,
                               DepartmentName = a.DepartmentDiscription,

                           };

                foreach (var d in data)
                {
                    DepartmentModel e = new DepartmentModel();
                    e.DepartmentID = Convert.ToInt32(d.DepartmentID);
                    e.DepartmentName = Convert.ToString(d.DepartmentName);
                    p.Add(e);
                }
                return p;
            }
        }

        public List<DepartmentModel> GetDepartmentPerIDs(string ids)
        {
            var p = new List<DepartmentModel>();
            _db = new eRecruitmentDataClassesDataContext();

            if (ids == "0")
            {
                DepartmentModel e = new DepartmentModel();
                e.DepartmentID = 0;
                e.DepartmentName = "";
                p.Add(e);
            }
            else
            {
                var LeadComps = _db.sp_Departments_Get_PerDivisionIDs(ids);

                foreach (var d in LeadComps)
                {
                    DepartmentModel e = new DepartmentModel();
                    e.DepartmentID = Convert.ToInt32(d.DepartmentID);
                    e.DepartmentName = Convert.ToString(d.DepartmentDiscription);
                    p.Add(e);
                }
            }
            return p;
        }
        public List<DepartmentManagerModel> GetDepartmentalManagerList(int orgid)
        {
            var p = new List<DepartmentManagerModel>();
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                //var data = _db.lutOrganisations.ToList();
                var data = from a in _db.AspNetUsers
                           join b in _db.AspNetUserRoles on a.Id equals b.UserId
                           join c in _db.tblProfiles on a.Id equals c.UserID
                           where b.OrganisationID == orgid //&& b.RoleId == "2"
                           select new
                           {
                               UserID = a.Id,
                               FullName = c.FirstName + " " + c.Surname
                           };

                foreach (var d in data)
                {
                    DepartmentManagerModel e = new DepartmentManagerModel();
                    e.UserID = Convert.ToString(d.UserID);
                    e.ManagerName = Convert.ToString(d.FullName);
                    p.Add(e);
                }
                return p;
            }
        }

        public int GetOrgId(string userid)
        {
            return _db.AspNetUserRoles.Where(x => x.UserId == userid).Select(x => x.OrganisationID).FirstOrDefault();
        }

        public int CheckIfDepartmentExists(string DepartmentName, int DivisionID)
        {
            return _db.lutDepartments.Where(x => x.DepartmentDiscription == DepartmentName &&
                                            x.DivisionID == DivisionID).Count();
        }

        public void InsertIntoDepartment(string DepartmentName, int DivisionID)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentAddDepartment(DepartmentName, DivisionID);
        }

        public List<DepartmentModel> GetDepartmentForEdit(int id)
        {
            var p = new List<DepartmentModel>();
            _db = new eRecruitmentDataClassesDataContext();
            var data = _db.lutDepartments.Where(x => x.DepartmentID == id).ToList();

            foreach (var d in data)
            {
                DepartmentModel e = new DepartmentModel();
                e.DepartmentID = Convert.ToInt32(d.DepartmentID);
                e.DepartmentName = Convert.ToString(d.DepartmentDiscription);
                e.DivisionID = Convert.ToInt32(d.DivisionID);
                //e.ManagerID = Convert.ToString(d.ManagerID);
                p.Add(e);
            }
            return p;
        }

        public void UpdateIntoDepartment(int id, string DepartmentName, int OrganisationID)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentUpdateDepartment(id, DepartmentName, OrganisationID);

        }

        public void DeleteIntoDepartment(int id)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentDeleteDepartment(id);
        }
        public List<DivisionModel> GetDivisionList(string userid)
        {
            var p = new List<DivisionModel>();
            _db = new eRecruitmentDataClassesDataContext();


            var data = from a in _db.lutDivisions
                       join b in _db.AspNetUserRoles on a.OrganisationID equals b.OrganisationID
                       join c in _db.lutOrganisations on a.OrganisationID equals c.OrganisationID
                       where b.UserId == userid
                       //============Peter 20221027============
                       orderby a.DivisionDiscription ascending
                       //======================================
                       select new
                       {
                           DivisionID = a.DivisionID,
                           DivisionName = a.DivisionDiscription,
                           OrganisationName = c.OrganisationName
                       };

            foreach (var d in data)
            {
                DivisionModel e = new DivisionModel();
                e.DivisionID = Convert.ToInt32(d.DivisionID);
                e.DivisionDiscription = Convert.ToString(d.DivisionName);
                e.OrganisationName = Convert.ToString(d.OrganisationName);
                p.Add(e);
            }
            return p;
        }
        public int CheckIfDivisionExists(string DivisionDiscription, int OrganisationID)
        {
            return _db.lutDivisions.Where(x => x.DivisionDiscription == DivisionDiscription
                                         && x.OrganisationID == OrganisationID).Count();
        }
        //Insert Division
        public void InsertIntoDivision(string Division, int OrganisationID)
        {
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                _db.proc_eRecruitmentAddDivision(Division, OrganisationID);
            }
        }
        //get Division per ORG
        public List<DivisionModel> GetDivisionForEdit(int id)
        {
            var p = new List<DivisionModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = _db.lutDivisions.Where(x => x.DivisionID == id).ToList();

            foreach (var d in data)
            {
                DivisionModel e = new DivisionModel();
                e.DivisionID = Convert.ToInt32(d.DivisionID);
                e.DivisionDiscription = Convert.ToString(d.DivisionDiscription);
                e.OrganisationID = Convert.ToInt32(d.OrganisationID);
                p.Add(e);
            }
            return p;
        }
        //Update Division
        public void UpdateIntoDivision(int id, string DivisionName, int OrganisationID)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentUpdateDivision(id, DivisionName, OrganisationID);

        }
        public void DeleteIntoDivision(int id)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentDeleteDivision(id);
        }

        //Get Disability List
        public List<DisabilityModel> GetDisabilityList()
        {
            var p = new List<DisabilityModel>();
            _db = new eRecruitmentDataClassesDataContext();
            var Disability = _db.lutDisabilities.ToList();

            foreach (var d in Disability)
            {
                DisabilityModel e = new DisabilityModel();
                e.DisabilityID = Convert.ToInt32(d.DisabilityID);
                e.Disability = Convert.ToString(d.Disability);
                p.Add(e);
            }
            return p;
        }

        //Check if Disability Exists
        public int CheckIfDisabilityExists(string Disability)
        {
            return _db.lutDisabilities.Where(x => x.Disability == Disability).Count();
        }

        //Insert Disability
        public void InsertIntoDisability(string Disability)
        {
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                _db.proc_eRecruitmentAddDisability(Disability);
            }
        }

        //Get Disability for Edit
        public List<DisabilityModel> GetDisabilityForEdit(int id)
        {
            var p = new List<DisabilityModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = _db.lutDisabilities.Where(x => x.DisabilityID == id).ToList();

            foreach (var d in data)
            {
                DisabilityModel e = new DisabilityModel();
                e.DisabilityID = Convert.ToInt32(d.DisabilityID);
                e.Disability = Convert.ToString(d.Disability);
                p.Add(e);
            }
            return p;
        }

        //Update Disability
        public void UpdateIntoDisability(int id, string Disability)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentUpdateDisability(id, Disability);

        }

        //Delete Disability
        public void DeleteIntoDisability(int id)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentDeleteDisability(id);
        }

        //Get Employment Type List
        public List<EmploymentTypeModel> GetEmploymentTypeList()
        {
            var p = new List<EmploymentTypeModel>();
            _db = new eRecruitmentDataClassesDataContext();
            var Roles = _db.lutEmployementTypes.OrderBy(x => x.EmploymentType).ToList();
            foreach (var d in Roles)
            {
                EmploymentTypeModel e = new EmploymentTypeModel();
                e.EmploymentTypeID = Convert.ToInt32(d.id);
                e.EmploymentType = Convert.ToString(d.EmploymentType);
                p.Add(e);
            }
            return p;
        }

        //Check if Employment Type Exists
        public int CheckIfEmploymentTypeExists(string EmploymentType)
        {
            return _db.lutEmployementTypes.Where(x => x.EmploymentType == EmploymentType).Count();
        }

        //Peter 20230425
        //Check if Employment Type Exists
        public int CheckIfJobSpecQuestionExists(string JobspecQuestionDesc)
        {
            return _db.lutJobSpecificQuestions.Where(x => x.JobSpecificeQuestionDesc == JobspecQuestionDesc).Count();
        }
        //Insert Employment Type
        public void InsertIntoEmploymentType(string EmploymentType)
        {
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                _db.proc_eRecruitmentAddEmploymentType(EmploymentType);
            }
        }

        //Get Employment Type for Edit
        public List<EmploymentTypeModel> GetEmploymentTypeForEdit(int id)
        {
            var p = new List<EmploymentTypeModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = _db.lutEmployementTypes.Where(x => x.id == id).ToList();

            foreach (var d in data)
            {
                EmploymentTypeModel e = new EmploymentTypeModel();
                e.EmploymentTypeID = Convert.ToInt32(d.id);
                e.EmploymentType = Convert.ToString(d.EmploymentType);
                p.Add(e);
            }
            return p;
        }

        //Peter
        public List<JobJobSpecificQuestionModel> GetJobSpecQuestionForEdit(int id)
        {
            var p = new List<JobJobSpecificQuestionModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = _db.lutJobSpecificQuestions.Where(x => x.JobSpecificeQuestionID == id).ToList();

            foreach (var d in data)
            {
                JobJobSpecificQuestionModel e = new JobJobSpecificQuestionModel();
                e.JobSpecificeQuestionID = d.JobSpecificeQuestionID;
                e.VacancyID = Convert.ToInt32(d.VacancyID);
                e.JobSpecificeQuestionDesc = Convert.ToString(d.JobSpecificeQuestionDesc);
                p.Add(e);
            }
            return p;
        }

        //Update EmploymentType
        public void UpdateIntoEmploymentType(int id, string EmploymentType)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentUpdateEmploymentType(id, EmploymentType);

        }


        //Peter
        //Update Job Specific Question
        public void UpdateIntolutJobSpecificQuestion(int id, string JobSpecificeQuestionDesc)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentUpdateJobSpecificeQuestion(id, JobSpecificeQuestionDesc);

        }
        //Peter
        //Delete EmploymentType
        public void DeleteIntoEmploymentType(int id)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentDeleteEmployementType(id);
        }

        //Delete EmploymentType
        public void DeleteIntoJobSpecQuestion(int id)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentDeleteJobSpecificQuestion(id);
        }

        //Get Quetions banks Per ORG
        public List<QuetionBanksModel> GetQuestionBanksList(string userid)
        {
            var p = new List<QuetionBanksModel>();
            _db = new eRecruitmentDataClassesDataContext();


            var data = from a in _db.lutGeneralQuestions
                       join b in _db.AspNetUserRoles on a.OrganisationID equals b.OrganisationID
                       join c in _db.lutOrganisations on a.OrganisationID equals c.OrganisationID
                       join d in _db.lutQuestionCatergories on a.QCategoryID equals d.QCategoryID
                       where b.UserId == userid
                       select new
                       {
                           id = a.id,
                           GeneralQuestionDesc = a.GeneralQuestionDesc,
                           QCategoryDescr = d.QCategoryDescr,
                           OrganisationName = c.OrganisationName
                       };

            foreach (var d in data)
            {
                QuetionBanksModel e = new QuetionBanksModel();
                e.id = Convert.ToInt32(d.id);
                e.GeneralQuestionDesc = Convert.ToString(d.GeneralQuestionDesc);
                e.QCategoryDescr = Convert.ToString(d.QCategoryDescr);
                e.OrganisationName = Convert.ToString(d.OrganisationName);
                p.Add(e);
            }
            return p;
        }

        //Insert Check If General Question Exist
        public int CheckIfGeneralQuestionExists(string GeneralQuestionDesc, int OrganisationID)
        {
            return _db.lutGeneralQuestions.Where(x => x.GeneralQuestionDesc == GeneralQuestionDesc &&
                                            x.OrganisationID == OrganisationID).Count();
        }

        //Insert General Question
        public void InsertIntoGeneralQuestion(string GeneralQuestionDesc, int OrganisationID, int QCategoryID)
        {
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                _db.proc_eRecruitmentAddQuestionBank(GeneralQuestionDesc, OrganisationID, QCategoryID);
            }
        }

        //Get General Question per ORG For Edit
        public List<GeneralQuestionModel> GetGeneralQuestionForEdit(int id)
        {
            var p = new List<GeneralQuestionModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = _db.lutGeneralQuestions.Where(x => x.id == id).ToList();

            foreach (var d in data)
            {
                GeneralQuestionModel e = new GeneralQuestionModel();
                e.id = Convert.ToInt32(d.id);
                e.GeneralQuestionDesc = Convert.ToString(d.GeneralQuestionDesc);
                e.OrganisationID = Convert.ToInt32(d.OrganisationID);
                e.QCategoryID = Convert.ToInt32(d.QCategoryID);
                p.Add(e);
            }
            return p;
        }

        //Update General Question
        public void UpdateIntoGeneralQuestion(int id, string GeneralQuestionDesc, int OrganisationID, int QCategoryID)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentUpdateQuestionBank(id, GeneralQuestionDesc, OrganisationID, QCategoryID);

        }

        //Delete General Question
        public void DeleteIntoGeneralQuestion(int generalQuestionId)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentDeleteQuetionBank(generalQuestionId);
        }

        //Get Interview Category

        public List<InterviewCategoryModel> GetInterviewCategory()
        {
            var p = new List<InterviewCategoryModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var Roles = _db.lutInterviewCategories.ToList();

            foreach (var d in Roles)
            {
                InterviewCategoryModel e = new InterviewCategoryModel();
                e.InterviewCatID = Convert.ToInt32(d.InterviewCatID);
                e.InterviewCatDescription = Convert.ToString(d.InterviewCatDescription);
                p.Add(e);
            }
            return p;
        }

        //Check if Interview Category Exists
        public int CheckIfInterviewCategoryExists(string InterviewCatDescription)
        {
            return _db.lutInterviewCategories.Where(x => x.InterviewCatDescription == InterviewCatDescription).Count();
        }

        //Insert Interview Category
        public void InsertIntoInterviewCategory(string InterviewCatDescription)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentAddInterviewCategory(InterviewCatDescription);
        }

        //Get Interview Category for Edit
        public List<InterviewCategoryModel> GetInterviewCategoryForEdit(int id)
        {
            var p = new List<InterviewCategoryModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = _db.lutInterviewCategories.Where(x => x.InterviewCatID == id).ToList();

            foreach (var d in data)
            {
                InterviewCategoryModel e = new InterviewCategoryModel();
                e.InterviewCatID = Convert.ToInt32(d.InterviewCatID);
                e.InterviewCatDescription = Convert.ToString(d.InterviewCatDescription);
                p.Add(e);
            }
            return p;
        }

        //Update Interview Category
        public void UpdateIntoInterviewCategory(int id, string InterviewCatDescription)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentUpdateInterviewCategory(id, InterviewCatDescription);

        }

        //Delete Interview Category
        public void DeleteIntoInterviewCategory(int InterviewCategoryid)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentDeleteInterviewCategory(InterviewCategoryid);
        }

        //Get Interview Type
        public List<InterviewTypeModel> GetInterviewType()
        {
            var p = new List<InterviewTypeModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var Roles = _db.lutInterviewTypes.ToList();

            foreach (var d in Roles)
            {
                InterviewTypeModel e = new InterviewTypeModel();
                e.InterviewTypeID = Convert.ToInt32(d.InterviewTypeID);
                e.InterviewTypeDescription = Convert.ToString(d.InterviewTypeDescription);
                p.Add(e);
            }
            return p;
        }

        //Check if Interview Type Exists
        public int CheckIfInterviewTypeExists(string InterviewTypeDescription)
        {
            return _db.lutInterviewTypes.Where(x => x.InterviewTypeDescription == InterviewTypeDescription).Count();
        }

        //Insert Interview Type
        public void InsertIntoInterviewType(string InterviewTypeDescription)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentAddInterviewType(InterviewTypeDescription);
        }

        //Get Interview Type for Edit
        public List<InterviewTypeModel> GetInterviewTypeForEdit(int id)
        {
            var p = new List<InterviewTypeModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = _db.lutInterviewTypes.Where(x => x.InterviewTypeID == id).ToList();

            foreach (var d in data)
            {
                InterviewTypeModel e = new InterviewTypeModel();
                e.InterviewTypeID = Convert.ToInt32(d.InterviewTypeID);
                e.InterviewTypeDescription = Convert.ToString(d.InterviewTypeDescription);
                p.Add(e);
            }
            return p;
        }

        //Update Interview Type
        public void UpdateIntoInterviewType(int id, string InterviewTypeDescription)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentUpdateInterviewType(id, InterviewTypeDescription);

        }

        //Delete Interview Type
        public void DeleteIntoInterviewType(int interviewTypeID)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentDeleteInterviewType(interviewTypeID);
        }


        //Get Job Level Per ORG
        public List<JobLevelViewModel> GetJobLevelList(string userid)
        {
            var p = new List<JobLevelViewModel>();
            _db = new eRecruitmentDataClassesDataContext();


            var data = from a in _db.AspNetUserRoles
                       join b in _db.lutOrganisations on a.OrganisationID equals b.OrganisationID
                       join c in _db.lutJobLevels on b.OrganisationID equals c.OrganisationID
                       where a.UserId == userid

                       select new
                       {
                           JobLevelID = c.JobLevelID,
                           OrganisationName = b.OrganisationName,
                           JobLevelName = c.JobLevelName
                       };

            foreach (var d in data)
            {
                JobLevelViewModel e = new JobLevelViewModel();
                e.JobLevelID = Convert.ToInt32(d.JobLevelID);
                e.OrganisationName = Convert.ToString(d.OrganisationName);
                e.JobLevelName = Convert.ToString(d.JobLevelName);
                p.Add(e);
            }
            return p;
        }
        //Get Job Level Per ORG
        /*      public List<JobLevelViewModel> GetJobLevelList(int SalarySubCategoryID)
              {
                  var p = new List<JobLevelViewModel>();
                  _db = new eRecruitmentDataClassesDataContext();

                  var data = from a in _db.lutJobLevels
                             join b in _db.lutSalarySubCategories on a.SalarySubCategoryID equals b.SalarySubCategoryID
                             join c in _db.lutSalaryCategories on b.SalaryCategoryID equals c.SalaryCategoryID
                             join d in _db.AspNetUserRoles on c.OrganisationID equals d.OrganisationID
                             where a.SalarySubCategoryID == SalarySubCategoryID
                             select new
                             {
                                 JobLevelID = a.JobLevelID,
                                 JobLevelName = a.JobLevelName,
                                 SalarySubCategoryName = b.Descr
                             };

                  foreach (var d in data)
                  {
                      JobLevelViewModel e = new JobLevelViewModel();
                      e.JobLevelID = Convert.ToInt32(d.JobLevelID);
                      e.JobLevelName = Convert.ToString(d.JobLevelName);
                      e.SalarySubCategoryName = Convert.ToString(d.SalarySubCategoryName);
                      p.Add(e);
                  }
                  return p;
              }*/


        //Check if Job Level Per ORG Exists
        public int CheckIfJobLevelExists(int OrganisationID, string JobLevelName)
        {
            return _db.lutJobLevels.Where(x => x.OrganisationID == OrganisationID
                                         && x.JobLevelName == JobLevelName).Count();
        }


        //Insert Job Level
        public void InsertIntoJobLevel(int OrganisationID, string JobLevelName)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentAddJobLevel(OrganisationID, JobLevelName);
        }

        //Get Job Level per ORG For Edit
        public List<JobLevelModel> GetJobLevelForEdit(int id)
        {
            var p = new List<JobLevelModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = _db.lutJobLevels.Where(x => x.JobLevelID == id).ToList();

            foreach (var d in data)
            {
                JobLevelModel e = new JobLevelModel();
                e.JobLevelID = Convert.ToInt32(d.JobLevelID);
                e.JobLevelName = Convert.ToString(d.JobLevelName);
                e.OrganisationID = Convert.ToInt32(d.OrganisationID);
                p.Add(e);
            }
            return p;
        }

        //Update Job Level
        public void UpdateIntoJobLevel(int id, int OrganisationID, string JobLevelName)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentUpdateJobLevel(id, OrganisationID, JobLevelName);

        }

        //Delete Job Level
        public void DeleteIntoJobLevel(int jobLevelID)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentDeleteJobLevel(jobLevelID);
        }

        //Get Job Title 
        public List<JobTitleViewModel> GetJobTitleList(string userid)
        {
            var p = new List<JobTitleViewModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = from a in _db.lutOrganisations
                       join b in _db.AspNetUserRoles on a.OrganisationID equals b.OrganisationID
                       join c in _db.lutJobTitles on b.OrganisationID equals c.OrganisationID
                       where b.UserId == userid
                       select new
                       {
                           JobTitleID = c.JobTitleID,
                           OrganisationName = a.OrganisationName,
                           JobTitle = c.JobTitle
                       };

            foreach (var d in data)
            {
                JobTitleViewModel e = new JobTitleViewModel();
                e.JobTitleID = Convert.ToInt32(d.JobTitleID);
                e.OrganisationName = Convert.ToString(d.OrganisationName);
                e.JobTitle = Convert.ToString(d.JobTitle);
                p.Add(e);
            }


            //var data = from a in _db.lutOrganisations
            //           join b in _db.AspNetUserRoles on a.OrganisationID equals b.OrganisationID
            //           join c in _db.lutJobTitles on b.OrganisationID equals c.OrganisationID
            //           where b.UserId == userid
            //           select new
            //           {
            //               JobTitleID = c.JobTitleID,
            //               OrganisationName = a.OrganisationName,
            //               JobTitle = c.JobTitle
            //           };

            //foreach (var d in data)
            //{
            //    JobTitleViewModel e = new JobTitleViewModel();
            //    e.JobTitleID = Convert.ToInt32(d.JobTitleID);
            //    e.OrganisationName = Convert.ToString(d.OrganisationName);
            //    e.JobTitle = Convert.ToString(d.JobTitle);
            //    p.Add(e);
            //}
            return p;
        }


        //Get All Job Title 
        public List<JobTitleViewModel> GetAllJobTitleList(string userid)
        {
            var p = new List<JobTitleViewModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = from a in _db.lutOrganisations
                       join b in _db.AspNetUserRoles on a.OrganisationID equals b.OrganisationID
                       join c in _db.lutJobTitles on b.OrganisationID equals c.OrganisationID
                       where b.UserId == userid
                       orderby c.JobTitle ascending
                       select new
                       {
                           JobTitleID = c.JobTitleID,
                           JobTitle = c.JobTitle
                       };

            foreach (var d in data)
            {
                JobTitleViewModel e = new JobTitleViewModel();
                e.JobTitleID = Convert.ToInt32(d.JobTitleID);
                e.JobTitle = Convert.ToString(d.JobTitle);
                p.Add(e);
            }
            return p;
        }

        //Get All Job Title From Structure 
        public List<JobTitleViewModel> GetAllJobTitleListForStructure(string userid)
        {
            var p = new List<JobTitleViewModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = from a in _db.lutOrganisations
                       join b in _db.AspNetUserRoles on a.OrganisationID equals b.OrganisationID
                       join c in _db.lutJobTitles on b.OrganisationID equals c.OrganisationID
                       join d in _db.lutSalaryStructures on c.JobTitleID equals d.JobTitleID
                       where b.UserId == userid
                       orderby c.JobTitle.Trim() ascending
                       select new
                       {
                           JobTitleID = c.JobTitleID,
                           JobTitle = c.JobTitle
                       };

            foreach (var d in data)
            {
                JobTitleViewModel e = new JobTitleViewModel();
                e.JobTitleID = Convert.ToInt32(d.JobTitleID);
                e.JobTitle = Convert.ToString(d.JobTitle);
                p.Add(e);
            }
            return p;
        }

        //Get Job Title From Vacancy Profile
        //Get All Job Title From Structure 
        public List<JobTitleViewModel> GetAllJobTitleListFromVacancyProfile(string userid)
        {
            var p = new List<JobTitleViewModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = from a in _db.lutOrganisations
                       join b in _db.AspNetUserRoles on a.OrganisationID equals b.OrganisationID
                       join c in _db.lutJobTitles on b.OrganisationID equals c.OrganisationID
                       //join d in _db.lutSalaryStructures on c.JobTitleID equals d.JobTitleID
                       //join e in _db.tblJobProfiles on d.JobTitleID equals e.JobTitleID
                       where b.UserId == userid
                       //============Peter 20221027============
                       orderby c.JobTitle.Trim() ascending
                       //======================================
                       select new
                       {
                           JobTitleID = c.JobTitleID,
                           JobTitle = c.JobTitle
                       };

            foreach (var d in data)
            {
                JobTitleViewModel e = new JobTitleViewModel();
                e.JobTitleID = Convert.ToInt32(d.JobTitleID);
                e.JobTitle = Convert.ToString(d.JobTitle);
                p.Add(e);
            }
            return p;
        }

        //Get Location 
        public List<LocationModel> GetLocationList(int orgID)
        {
            var p = new List<LocationModel>();
            _db = new eRecruitmentDataClassesDataContext();
            var data = from a in _db.lutOrganisations
                       join b in _db.lutLocations on a.OrganisationID equals b.OrganisationID
                       where b.OrganisationID == orgID
                       //============Peter 20221028============
                       orderby b.LocationDiscription.Trim() ascending
                       //======================================
                       select new
                       {
                           LocationID = b.LocationID,
                           LocationDiscription = b.LocationDiscription
                       };
            foreach (var d in data)
            {
                LocationModel e = new LocationModel();
                e.LocationID = Convert.ToInt32(d.LocationID);
                e.LocationDiscription = Convert.ToString(d.LocationDiscription);
                p.Add(e);
            }
            return p;
        }

        //Check if Location Exists
        public int CheckIfLocationExists(int OrganisationID, string LocationDiscription)
        {
            return _db.lutLocations.Where(x => x.OrganisationID == OrganisationID && x.LocationDiscription == LocationDiscription).Count();
        }

        //Insert Location
        public void InsertIntoLocation(int OrganisationID, string LocationDiscription)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentAddLocation(OrganisationID, LocationDiscription);
        }

        //Get Location for Edit
        public List<LocationModel> GetLocationForEdit(int id)
        {
            var p = new List<LocationModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = _db.lutLocations.Where(x => x.LocationID == id).ToList();

            foreach (var d in data)
            {
                LocationModel e = new LocationModel();
                e.LocationID = Convert.ToInt32(d.LocationID);
                e.LocationDiscription = Convert.ToString(d.LocationDiscription);
                p.Add(e);
            }
            return p;
        }

        //Update Location
        public void UpdateIntoLocation(int id, int OrganisationID, string LocationDiscription)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentUpdateLocation(id, OrganisationID, LocationDiscription);

        }

        //Delete Location
        public void DeleteIntoLocation(int locationID)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentDeleteLocation(locationID);
        }

        //Get Qualification Type List
        public List<QualificationTypeModel> GetQualificationTypeList()
        {
            var p = new List<QualificationTypeModel>();
            _db = new eRecruitmentDataClassesDataContext();
            var QualificationType = _db.lutQualificationTypes.ToList();
            foreach (var d in QualificationType)
            {
                QualificationTypeModel e = new QualificationTypeModel();
                e.QualificationTypeID = Convert.ToInt32(d.QualificationTypeID);
                e.QualificationTypeName = Convert.ToString(d.QualificationTypeName);
                p.Add(e);
            }
            return p;
        }

        //Check if Qualification Type Exists
        public int CheckIfQualificationTypeExists(string QualificationTypeName)
        {
            return _db.lutQualificationTypes.Where(x => x.QualificationTypeName == QualificationTypeName).Count();
        }

        //Insert Qualification Type
        public void InsertIntoQualificationType(string QualificationTypeName)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentAddQualificationType(QualificationTypeName);
        }

        //Get Qualification Type for Edit
        public List<QualificationTypeModel> GetQualificationTypeForEdit(int id)
        {
            var p = new List<QualificationTypeModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = _db.lutQualificationTypes.Where(x => x.QualificationTypeID == id).ToList();

            foreach (var d in data)
            {
                QualificationTypeModel e = new QualificationTypeModel();
                e.QualificationTypeID = Convert.ToInt32(d.QualificationTypeID);
                e.QualificationTypeName = Convert.ToString(d.QualificationTypeName);
                p.Add(e);
            }
            return p;
        }

        //Update Qualification Type
        public void UpdateIntoQualificationType(int id, string QualificationTypeName)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentUpdateQualificationType(id, QualificationTypeName);

        }


        //Delete Qualification Type
        public void DeleteIntoQualificationType(int qualificationTypeID)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentDeleteQualificationType(qualificationTypeID);
        }

        // Get Reject Reason List 
        public List<RejectReasonModel> GetRejectReasonList()
        {
            var p = new List<RejectReasonModel>();
            _db = new eRecruitmentDataClassesDataContext();
            var data = _db.lutRejectReasons.ToList();

            foreach (var d in data)
            {
                RejectReasonModel e = new RejectReasonModel();
                e.RejectReasonID = Convert.ToInt32(d.RejectReasonID);
                e.RejectReason = Convert.ToString(d.RejectReason);
                p.Add(e);
            }
            return p;
        }

        //Check if Reject Reason Exists
        public int CheckIfRejectReasonExists(string RejectReason)
        {
            return _db.lutRejectReasons.Where(x => x.RejectReason == RejectReason).Count();
        }

        //Insert Reject Reason
        public void InsertIntoRejectReason(string RejectReason)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentAddRejectReason(RejectReason);
        }

        //Get Reject Reason for Edit
        public List<RejectReasonModel> GetRejectReasonForEdit(int id)
        {
            var p = new List<RejectReasonModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = _db.lutRejectReasons.Where(x => x.RejectReasonID == id).ToList();

            foreach (var d in data)
            {
                RejectReasonModel e = new RejectReasonModel();
                e.RejectReasonID = Convert.ToInt32(d.RejectReasonID);
                e.RejectReason = Convert.ToString(d.RejectReason);
                p.Add(e);
            }
            return p;
        }

        //Update Reject Reason
        public void UpdateIntoRejectReason(int id, string RejectReason)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentUpdateRejectReason(id, RejectReason);

        }

        //Delete Reject Reason
        public void DeleteIntoRejectReason(int rejectReasonID)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentDeleteRejectReason(rejectReasonID);
        }

        // Get Retract Reason List 
        public List<RetractReasonModel> GetRetractReasonList()
        {
            var p = new List<RetractReasonModel>();
            _db = new eRecruitmentDataClassesDataContext();
            var data = _db.lutRetractReasons.ToList();

            foreach (var d in data)
            {
                RetractReasonModel e = new RetractReasonModel();
                e.RetractReasonID = Convert.ToInt32(d.RetractReasonID);
                e.RetractReason = Convert.ToString(d.RetractReason);
                p.Add(e);
            }
            return p;
        }

        //Check if Retract Reason Exists
        public int CheckIfRetractReasonExists(string RetractReason)
        {
            return _db.lutRetractReasons.Where(x => x.RetractReason == RetractReason).Count();
        }

        //Insert Retract Reason
        public void InsertIntoRetractReason(string RetractReason)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentAddRetractReason(RetractReason);
        }

        //Get Retract Reason for Edit
        public List<RetractReasonModel> GetRetractReasonForEdit(int id)
        {
            var p = new List<RetractReasonModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = _db.lutRetractReasons.Where(x => x.RetractReasonID == id).ToList();

            foreach (var d in data)
            {
                RetractReasonModel e = new RetractReasonModel();
                e.RetractReasonID = Convert.ToInt32(d.RetractReasonID);
                e.RetractReason = Convert.ToString(d.RetractReason);
                p.Add(e);
            }
            return p;
        }

        //Update Retract Reason
        public void UpdateIntoRetractReason(int id, string RetractReason)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentUpdateRetractReasons(id, RetractReason);

        }

        //Delete Retract Reason
        public void DeleteIntoRetractReason(int retractReasonID)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentDeleteRetractReason(retractReasonID);
        }

        //Get Skill List
        public List<SkillModel> GetSkillsList()
        {
            var p = new List<SkillModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var Skills = _db.lutSkills.ToList();

            foreach (var d in Skills)
            {
                SkillModel e = new SkillModel();
                e.skillID = Convert.ToInt32(d.skillID);
                e.skillName = Convert.ToString(d.skillName);
                p.Add(e);
            }
            return p;
        }

        public List<SkillsCategoryModel> GetSkillsCategoryList()
        {
            var p = new List<SkillsCategoryModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var SkillsCategories = _db.lutSkillsCategories.ToList();

            foreach (var d in SkillsCategories)
            {
                SkillsCategoryModel e = new SkillsCategoryModel();
                e.CategoryID = Convert.ToInt32(d.CategoryID);
                e.Description = Convert.ToString(d.Description);
                p.Add(e);
            }
            return p;
        }

        //Check if Skill Exists
        public int CheckIfSkillExists(string skillName, int CategoryID)
        {
            return _db.lutSkills.Where(x => x.skillName == skillName && x.CategoryID == CategoryID).Count();
        }

        //Insert Skill 
        public void InsertIntoSkill(string skillName, int CategoryID)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentAddSkill(skillName, CategoryID);
        }

        //Get Skill for Edit
        public List<SkillModel> GetSkillForEdit(int id)
        {
            var p = new List<SkillModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = _db.lutSkills.Where(x => x.skillID == id).ToList();

            foreach (var d in data)
            {
                SkillModel e = new SkillModel();
                e.skillID = Convert.ToInt32(d.skillID);
                e.skillName = Convert.ToString(d.skillName);
                e.CategoryID = Convert.ToInt32(d.CategoryID);
                //e.Description = Convert.ToString(d.Description);
                p.Add(e);
            }
            return p;
        }

        //Update Skill
        public void UpdateIntoSkill(int id, string skillName, int CategoryID)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentUpdateSkill(id, skillName, CategoryID);

        }

        //Delete Skill
        public void DeleteIntoSkill(int skillID)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentDeleteSkill(skillID);
        }

        //Get Skill Proficiency List
        public List<SkillProficiencyModel> GetSkillProficiencyList()
        {
            var p = new List<SkillProficiencyModel>();
            _db = new eRecruitmentDataClassesDataContext();
            var SkillProficiency = _db.lutSkill_Proficiencies.ToList();

            foreach (var d in SkillProficiency)
            {
                SkillProficiencyModel e = new SkillProficiencyModel();
                e.SkillProficiencyID = Convert.ToInt32(d.SkillProficiencyID);
                e.SkillProficiency = Convert.ToString(d.SkillProficiency);
                p.Add(e);
            }
            return p;
        }

        //Check if Skill Proficiency Exists
        public int CheckIfSkillProficiencyExists(string SkillProficiency)
        {
            return _db.lutSkill_Proficiencies.Where(x => x.SkillProficiency == SkillProficiency).Count();
        }

        //Insert Skill Proficiency 
        public void InsertIntoSkillProficiency(string SkillProficiency)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentAddSkillProficiency(SkillProficiency);
        }

        //Get Skill Proficiency for Edit
        public List<SkillProficiencyModel> GetSkillProficiencyForEdit(int id)
        {
            var p = new List<SkillProficiencyModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = _db.lutSkill_Proficiencies.Where(x => x.SkillProficiencyID == id).ToList();

            foreach (var d in data)
            {
                SkillProficiencyModel e = new SkillProficiencyModel();
                e.SkillProficiencyID = Convert.ToInt32(d.SkillProficiencyID);
                e.SkillProficiency = Convert.ToString(d.SkillProficiency);
                p.Add(e);
            }
            return p;
        }

        //Update Skill Proficiency
        public void UpdateIntoSkillProficiency(int id, string SkillProficiency)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentUpdateSkillProficiency(id, SkillProficiency);

        }

        //Delete Skill Proficiency
        public void DeleteIntoSkillProficiency(int skillProficiencyID)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentDeleteSkillProficiency(skillProficiencyID);
        }

        // Get Withdrawal Reason List 
        public List<WithdrawalReasonModel> GetWithdrawalReasonList()
        {
            var p = new List<WithdrawalReasonModel>();
            _db = new eRecruitmentDataClassesDataContext();
            var data = _db.lutWithdrawalReasons.ToList();

            foreach (var d in data)
            {
                WithdrawalReasonModel e = new WithdrawalReasonModel();
                e.WithdrawalReasonID = Convert.ToInt32(d.WithdrawalReasonID);
                e.WithdrawalReason = Convert.ToString(d.WithdrawalReason);
                p.Add(e);
            }
            return p;
        }

        //Check if Withdrawal Reason Exists
        public int CheckIfWithdrawalReasonExists(string WithdrawalReason)
        {
            return _db.lutWithdrawalReasons.Where(x => x.WithdrawalReason == WithdrawalReason).Count();
        }

        //Insert Withdrawal Reason 
        public void InsertIntoWithdrawalReason(string WithdrawalReason)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentAddWithdrawalReason(WithdrawalReason);
        }

        //Get Withdrawal Reason for Edit
        public List<WithdrawalReasonModel> GetWithdrawalReasonForEdit(int id)
        {
            var p = new List<WithdrawalReasonModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = _db.lutWithdrawalReasons.Where(x => x.WithdrawalReasonID == id).ToList();

            foreach (var d in data)
            {
                WithdrawalReasonModel e = new WithdrawalReasonModel();
                e.WithdrawalReasonID = Convert.ToInt32(d.WithdrawalReasonID);
                e.WithdrawalReason = Convert.ToString(d.WithdrawalReason);
                p.Add(e);
            }
            return p;
        }

        //Update Withdrawal Reason
        public void UpdateIntoWithdrawalReason(int id, string WithdrawalReason)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentUpdatelutWithdrawalReason(id, WithdrawalReason);

        }

        //Delete Withdrawal Reason
        public void DeleteIntoWithdrawalReason(int withdrawalReasonID)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentDeleteWithdrawalReason(withdrawalReasonID);
        }


        #region"--Delete Province By ID--"
        /// <summary>
        /// Delete Province By ID
        /// </summary>
        /// <param name="id"></param>
        /// <remarks><para><b>Created By</b>Ntshengedzeni Badamarema - 2019/03/05</para></remarks>
        public void DeleteProvinceByID(int id)
        {
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                var data = from a in _db.lutProvinces
                           where a.ProvinceID == id
                           select a;
                _db.lutProvinces.DeleteAllOnSubmit(data);
                _db.SubmitChanges();
            }
        }
        #endregion

        #region"--Add Province--"
        /// <summary>
        /// Add Province
        /// </summary>
        /// <param name="province"></param>
        /// <remarks><para><b>Created By</b></para>Ntshengedzeni Badamarema - 2019/03/05</remarks>
        public void AddProvince(ProvinceListModel province)
        {
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                var config = new MapperConfiguration(cfg => { cfg.CreateMap<ProvinceListModel, lutProvince>(); });
                IMapper iMapper = config.CreateMapper();
                var data = iMapper.Map<ProvinceListModel, lutProvince>(province);

                _db.lutProvinces.InsertOnSubmit(data);
                _db.SubmitChanges();

            }
        }
        #endregion

        #region"--Edit Province Data"
        /// <summary>
        /// Edit Province Data
        /// </summary>
        /// <param name="province"></param>
        /// <remarks><para>Created By</para>Ntshengedzeni Badamarema - 2019/03/05</remarks>
        public void EditProvince(ProvinceListModel province)
        {
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                var data = _db.lutProvinces.Where(x => x.ProvinceID == province.ProvinceID).SingleOrDefault();
                data.ProvinceID = province.ProvinceID;
                data.ProvinceName = province.ProvinceName;
                _db.SubmitChanges();
            }
        }
        #endregion

        #region "--Get User List--"
        /// <summary>
        /// The following function returns list of all Users
        /// </summary>
        /// <returns></returns>
        /// <remarks><para><b>Created By:</b>Ntshengedzeni Badamarema - 2019/03/07 </para></remarks>
        public List<UserListModel> GetUserList()
        {
            var p = new List<UserListModel>();
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                var data = (from a in _db.AspNetUsers
                            join b in _db.tblProfiles on a.Id equals b.UserID
                            join c in _db.AspNetUserRoles on a.Id equals c.UserId
                            where a.Email != "eRecruit.Admin@sita.co.za" && a.Email != "portal.admin@sita.co.za" && a.Email != "eRecruitment.Administrator@sita.co.za"
                            select new { a.Id, a.Email, b.Surname, b.FirstName, b.CellNo }).ToList();

                foreach (var d in data)
                {
                    UserListModel e = new UserListModel
                    {
                        UserID = Convert.ToString(d.Id),
                        Email = Convert.ToString(d.Email),
                        FullName = Convert.ToString(d.FirstName) + " " + Convert.ToString(d.Surname),
                        CellNo = Convert.ToString(d.CellNo)
                    };
                    p.Add(e);
                }
                return p;
            }
        }
        #endregion



        #region"--Save User Info--"
        /// <summary>
        /// This function will be used to insert new record or update Existing one
        /// </summary>
        /// <param name="user"></param>
        /// <remarks><para><b>Created By</b>Ntshengedzeni Badamarema 2019/03/12</para>
        /// <para><b>Modified By</b>Lu=ivhuwani Murashia 2019/03/12</para>
        /// </remarks>
        public void SaveUserInfo(UserModel user)
        {
            //int r = _db.AspNetUsers.Where(x => x.Email == user.Email).Count();
            int r = _db.AspNetUsers.Where(x => x.Email == user.Email).Count();
            if (r <= 0)
            {
                var config = new MapperConfiguration(cfg => { cfg.CreateMap<UserModel, AspNetUser>(); });
                IMapper iMapper = config.CreateMapper();
                var data = iMapper.Map<UserModel, AspNetUser>(user);
                _db.AspNetUsers.InsertOnSubmit(data);

                //Create user profile on tblUser
                _db.proc_eRecruitmentCreateUserProfile(user.Id, user.IDNumber, user.Passport, user.Surname, user.FirstName, user.CellNo, user.Email);
            }
            else
            {
                //Perform update
                //Not sure what this line is doing, i am getting an error here and we are not even using the resultset, I have decided to comment it
                //var data = _db.AspNetUsers.Where(x => x.Id == user.Id).SingleOrDefault(); 
                //end comment Line

                //data.Id = user.Id;
                //data.PhoneNumber = user.CellNo;
                //data.LockoutEnabled = false;
                //_db.SubmitChanges();

                //var da = _db.tblProfiles.Where(x => x.UserID == user.Id).SingleOrDefault();
                //da.CellNo = user.CellNo;
                //da.Surname = user.Surname;
                //da.FirstName = user.FirstName;
                //da.IDNumber = user.IDNumber;
                //_db.SubmitChanges();
                _db.proc_eRecruitmentCreateUserProfile(user.Id, user.IDNumber, user.Passport, user.Surname, user.FirstName, user.CellNo, user.Email);
            }

        }
        #endregion


        public bool CheckIfUserExists(string IDNumber)
        {
            bool isExists = false;
            _db = new eRecruitmentDataClassesDataContext();
            //int rCount = _db.AspNetUsers.Where(x => x.Email == email).Count();
            int rCount = (from a in _db.AspNetUsers
                          join b in _db.tblProfiles on a.Id equals b.UserID
                          where b.IDNumber == IDNumber
                          select new
                          {
                              b.IDNumber
                          }).Count();

            if (rCount > 0)
            {
                isExists = true;
            }
            else
            {
                isExists = false;
            }
            return isExists;
        }
        #region"--Edit User Data"
        /// <summary>
        /// Edit User Data
        /// </summary>
        /// <param name="user"></param>
        /// <remarks><para>Created By</para>Ntshengedzeni Badamarema - 2019/03/05</remarks>
        public void EditUserData(UserListModel user)
        {
            _db = new eRecruitmentDataClassesDataContext();

            string RoleID = user.RoleID;

            _db.proc_eRecruitmentInsertUserRole(user.UserID, Convert.ToInt32(RoleID), Convert.ToInt32(user.OrganisationID));
            //_db.proc_eRecruitmentInsertEmployeeNumber(user.UserID, user.EmployeeNO);
            _db.proc_eRecruitmentInsertEmployeeNumberonCreate(user.IDNumber, user.EmployeeNO);
            //string RoleID = user.RoleID;

            //var d = _db.AspNetUserRoles.Where(x => x.UserId == user.UserID).Count();
            //if (d > 0)
            //{
            //    var data = _db.AspNetUserRoles.Where(x => x.UserId == user.UserID).SingleOrDefault();
            //    data.UserId = user.UserID;
            //    data.OrganisationID = Convert.ToInt32(user.OrganisationID);
            //    _db.SubmitChanges();
            //}
            //else
            //{
            //    _db.proc_eRecruitmentInsertUserRole(user.UserID, Convert.ToInt32(RoleID), Convert.ToInt32(user.OrganisationID));
            //}


        }
        #endregion

        #region"--Get User Details For Edit--"
        /// <summary>
        /// Get User Details For Edit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <remarks><para><b>Created By</b>Ntshengedzeni Badamarema 2019/03/07</para></remarks>
        public List<UserListModel> GetUserForEdit(string id)
        {
            var p = new List<UserListModel>();
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                var data = (from a in _db.AspNetUsers
                            join b in _db.tblProfiles on a.Id equals b.UserID
                            join c in _db.AspNetUserRoles on a.Id equals c.UserId into dep
                            from d in dep.DefaultIfEmpty()
                            where a.Id == id
                            select new
                            {
                                a.Id,
                                a.Email,
                                b.Surname,
                                b.FirstName,
                                b.CellNo,
                                OrganisationID = d == null ? "" : d.OrganisationID.ToString()
                            }).FirstOrDefault();

                UserListModel e = new UserListModel
                {
                    UserID = Convert.ToString(data.Id),
                    Email = Convert.ToString(data.Email),
                    FullName = Convert.ToString(data.FirstName) + " " + Convert.ToString(data.Surname),
                    CellNo = Convert.ToString(data.CellNo),
                    OrganisationID = Convert.ToString(data.OrganisationID)
                };
                p.Add(e);
                return p;
            }
        }
        #endregion

        #region"--Get Organisation List--"
        /// <summary>
        /// Get Organisation List
        /// </summary>
        /// <returns>List of All Organisations</returns>
        /// <remarks><para><b>Created By</b>Ntshengedzeni Badamarema 2019/03/07</para></remarks>
        public List<OrganisationModel> GetOrganisationList()
        {
            var p = new List<OrganisationModel>();

            _db = new eRecruitmentDataClassesDataContext();

            var data = _db.lutOrganisations.Where(x => x.OrganisationID != 0).ToList();

            foreach (var d in data)
            {
                OrganisationModel e = new OrganisationModel
                {
                    OrganisationID = Convert.ToInt32(d.OrganisationID),
                    OrganisationCode = Convert.ToString(d.OrganisationCode),
                    OrganisationName = Convert.ToString(d.OrganisationName)
                };
                p.Add(e);
            }
            return p;
        }

        public List<OrganisationModel> GetOrganisationSITA()
        {
            var p = new List<OrganisationModel>();

            _db = new eRecruitmentDataClassesDataContext();

            var data = _db.lutOrganisations.Where(x => x.OrganisationID == 1).ToList();

            foreach (var d in data)
            {
                OrganisationModel e = new OrganisationModel
                {
                    OrganisationID = Convert.ToInt32(d.OrganisationID),
                    OrganisationCode = Convert.ToString(d.OrganisationCode),
                    OrganisationName = Convert.ToString(d.OrganisationName)
                };
                p.Add(e);
            }
            return p;
        }
        #endregion

        public int CheckIfJobTitleExists(int OrganisationID, string JobTitle)
        {
            return _db.lutJobTitles.Where(x => x.OrganisationID == OrganisationID
                                         && x.JobTitle == JobTitle).Count();
        }

        public void InsertIntoJobTitle(int OrganisationID, string JobTitle)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentAddJobTitle(OrganisationID, JobTitle);
        }

        public List<JobTitleModel> GetJobTitleForEdit(int id)
        {
            var p = new List<JobTitleModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = _db.lutJobTitles.Where(x => x.JobTitleID == id).ToList();

            foreach (var d in data)
            {
                JobTitleModel e = new JobTitleModel();
                e.JobTitleID = Convert.ToInt32(d.JobTitleID);
                e.OrganisationID = Convert.ToInt32(d.OrganisationID);
                e.JobTitle = Convert.ToString(d.JobTitle);
                p.Add(e);
            }
            return p;
        }

        //Update Job Title
        public void UpdateIntoJobTitle(int id, int OrganisationID, string JobTitle)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentUpdateJobTilte(id, OrganisationID, JobTitle);

        }

        //Delete Job Title
        public void DeleteIntoJobTitle(int jobTitleID)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentDeleteJobTitle(jobTitleID);
        }

        #region Salary Category
        //Get Salary Category Per ORG
        public List<SalaryCategoryViewModel> GetAllSalaryCategoryList(string userid)
        {
            var p = new List<SalaryCategoryViewModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = from a in _db.AspNetUserRoles
                       join b in _db.lutOrganisations on a.OrganisationID equals b.OrganisationID
                       join c in _db.lutSalaryCategories on a.OrganisationID equals c.OrganisationID
                       where a.UserId == userid

                       //var data = from a in _db.lutSalaryCategories
                       //           join b in _db.AspNetUserRoles on a.OrganisationID equals b.OrganisationID
                       //           join c in _db.lutOrganisations on a.OrganisationID equals c.OrganisationID
                       //           where b.UserId == userid
                       select new
                       {
                           SalaryCategoryID = c.SalaryCategoryID,
                           SalaryCategoryDescr = c.CategoryDescr
                       };

            foreach (var d in data)
            {
                SalaryCategoryViewModel e = new SalaryCategoryViewModel();
                e.SalaryCategoryID = Convert.ToInt32(d.SalaryCategoryID);
                e.CategoryDescr = Convert.ToString(d.SalaryCategoryDescr);
                p.Add(e);
            }
            return p;
        }

        //Get Salary Category Per ORG
        public List<SalaryCategoryViewModel> GetSalaryCategoryList(string userid)
        {
            var p = new List<SalaryCategoryViewModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = from a in _db.lutSalaryCategories
                       join b in _db.AspNetUserRoles on a.OrganisationID equals b.OrganisationID
                       join c in _db.lutOrganisations on a.OrganisationID equals c.OrganisationID
                       where b.UserId == userid
                       select new
                       {
                           SalaryCategoryID = a.SalaryCategoryID,
                           SalaryCategoryDescr = a.CategoryDescr,
                           OrganisationName = c.OrganisationName
                       };

            foreach (var d in data)
            {
                SalaryCategoryViewModel e = new SalaryCategoryViewModel();
                e.SalaryCategoryID = Convert.ToInt32(d.SalaryCategoryID);
                e.CategoryDescr = Convert.ToString(d.SalaryCategoryDescr);
                e.OrganisationName = Convert.ToString(d.OrganisationName);
                p.Add(e);
            }
            return p;
        }

        //Check if Salary Category Per ORG Exists
        public int CheckIfSalaryCategoryExists(string CategoryDescr, int OrganisationID)
        {
            return _db.lutSalaryCategories.Where(x => x.CategoryDescr == CategoryDescr
                                         && x.OrganisationID == OrganisationID).Count();
        }

        //Insert Salary Category
        public void InsertIntoSalaryCategory(int OrganisationID, string SalaryCategoryDescr)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentAddSalaryCategory(OrganisationID, SalaryCategoryDescr);
        }

        //Get Salary Category per ORG For Edit
        public List<SalaryCategoryModel> GetSalaryCategoryForEdit(int id)
        {
            var p = new List<SalaryCategoryModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = _db.lutSalaryCategories.Where(x => x.SalaryCategoryID == id).ToList();

            foreach (var d in data)
            {
                SalaryCategoryModel e = new SalaryCategoryModel();
                e.SalaryCategoryID = Convert.ToInt32(d.SalaryCategoryID);
                e.CategoryDescr = Convert.ToString(d.CategoryDescr);
                e.OrganisationID = Convert.ToInt32(d.OrganisationID);
                p.Add(e);
            }
            return p;
        }

        //Update Salary Category
        public void UpdateIntoSalaryCategory(int id, int OrganisationID, string SalaryCategoryDescr)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentUpdateSalaryCategory(id, OrganisationID, SalaryCategoryDescr);

        }

        //Delete Salary Category
        public void DeleteIntoSalaryCategory(int SalaryCategoryID)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentDeleteSalaryCategory(SalaryCategoryID);
        }
        #endregion
        //----------------------------------------------------------------------------------------------------
        #region Salary Sub-Category
        //Get SubCategoryID per category
        public List<SalarySubCategoryViewModel> GetSubCategoryListforStructure(int SalaryCategoryID)
        {
            var p = new List<SalarySubCategoryViewModel>();
            _db = new eRecruitmentDataClassesDataContext();
            var data = from a in _db.lutSalarySubCategories
                       join b in _db.lutSalaryCategories on a.SalaryCategoryID equals b.SalaryCategoryID
                       where a.SalaryCategoryID == SalaryCategoryID
                       select new
                       {
                           SalarySubCategoryName = a.Descr,
                           SalarySubCategoryID = a.SalarySubCategoryID
                       };

            foreach (var d in data)
            {
                SalarySubCategoryViewModel e = new SalarySubCategoryViewModel();
                e.SalarySubCategoryID = Convert.ToInt32(d.SalarySubCategoryID);
                e.Descr = Convert.ToString(d.SalarySubCategoryName);
                p.Add(e);
            }
            return p;
        }

        //Get SalaryCategoryID per sub category
        public List<SalaryCategoryViewModel> GetCategoryListforStructure(int SalarySubCategoryID)
        {
            var p = new List<SalaryCategoryViewModel>();
            _db = new eRecruitmentDataClassesDataContext();
            var data = from a in _db.lutSalaryCategories
                       join b in _db.lutSalarySubCategories on a.SalaryCategoryID equals b.SalaryCategoryID
                       where b.SalarySubCategoryID == SalarySubCategoryID
                       select new
                       {
                           SalaryCategoryID = b.SalaryCategoryID,
                           CategoryDescr = a.CategoryDescr
                       };

            foreach (var d in data)
            {
                SalaryCategoryViewModel e = new SalaryCategoryViewModel();
                e.SalaryCategoryID = Convert.ToInt32(d.SalaryCategoryID);
                e.CategoryDescr = Convert.ToString(d.CategoryDescr);
                p.Add(e);
            }
            return p;
        }

        //Get SalaryCategoryID per structure id
        public List<SalarySubCategoryViewModel> GetSubCategoryID(int id)
        {
            var p = new List<SalarySubCategoryViewModel>();
            _db = new eRecruitmentDataClassesDataContext();
            var data = from a in _db.lutSalaryStructures
                       join b in _db.lutSalarySubCategories on a.SalarySubCategoryID equals b.SalarySubCategoryID
                       where a.SalaryStructureID == id
                       select new
                       {
                           SalarySubCategoryID = a.SalarySubCategoryID,
                           Descr = b.Descr
                       };

            foreach (var d in data)
            {
                SalarySubCategoryViewModel e = new SalarySubCategoryViewModel();
                e.SalarySubCategoryID = Convert.ToInt32(d.SalarySubCategoryID);
                e.Descr = Convert.ToString(d.Descr);
                p.Add(e);
            }
            return p;
        }

        //Get Salary Category Per ORG
        public List<SalarySubCategoryViewModel> GetSalarySubCategoryList(string userid)
        {
            var p = new List<SalarySubCategoryViewModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = from a in _db.AspNetUserRoles
                       join b in _db.lutOrganisations on a.OrganisationID equals b.OrganisationID
                       join c in _db.lutSalaryCategories on b.OrganisationID equals c.OrganisationID
                       join d in _db.lutSalarySubCategories on c.SalaryCategoryID equals d.SalaryCategoryID
                       where a.UserId == userid
                       orderby d.Descr ascending
                       select new
                       {
                           SalarySubCategoryID = d.SalarySubCategoryID,
                           SalaryCategoryID = d.SalaryCategoryID,
                           SalarySubCategoryDescr = d.Descr,
                           OrganisationName = b.OrganisationName,
                           SalaryCategoryName = c.CategoryDescr
                       };

            foreach (var d in data)
            {
                SalarySubCategoryViewModel e = new SalarySubCategoryViewModel();
                e.SalarySubCategoryID = Convert.ToInt32(d.SalarySubCategoryID);
                e.OrganisationName = Convert.ToString(d.OrganisationName);
                e.SalaryCategoryName = Convert.ToString(d.SalaryCategoryName);
                e.Descr = Convert.ToString(d.SalarySubCategoryDescr);
                e.SalaryCategoryID = Convert.ToInt32(d.SalaryCategoryID);
                p.Add(e);
            }
            return p;
        }

        //Check if Salary Category Per ORG Exists
        public int CheckIfSalarySubCategoryExists(int SalaryCategoryID, string Descr)
        {
            return _db.lutSalarySubCategories.Where(x => x.SalaryCategoryID == SalaryCategoryID
                                         && x.Descr == Descr).Count();
        }

        //Insert Salary Category
        public void InsertIntoSalarySubCategory(int SalaryCategoryID, string SalaryCategoryDescr)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentAddSalarySubCategory(SalaryCategoryID, SalaryCategoryDescr);
        }

        //Get Salary Category per ORG For Edit
        public List<SalarySubCategoryModel> GetSalarySubCategoryForEdit(int id)
        {
            var p = new List<SalarySubCategoryModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = _db.lutSalarySubCategories.Where(x => x.SalarySubCategoryID == id).ToList();

            foreach (var d in data)
            {
                SalarySubCategoryModel e = new SalarySubCategoryModel();
                e.SalarySubCategoryID = Convert.ToInt32(d.SalarySubCategoryID);
                e.Descr = Convert.ToString(d.Descr);
                e.SalaryCategoryID = Convert.ToInt32(d.SalaryCategoryID);
                p.Add(e);
            }
            return p;
        }

        //Update Salary Category
        public void UpdateIntoSalarySubCategory(int id, int SalarySubCategoryID, string SalarySubCategoryDescr)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentUpdateSalarySubCategory(id, SalarySubCategoryID, SalarySubCategoryDescr);

        }

        //Delete Salary Category
        public void DeleteIntoSalarySubCategory(int SalarySubCategoryID)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentDeleteSalarySubCategory(SalarySubCategoryID);
        }
        #endregion
        //----------------------------------------------------------------------------------------------------
        #region Salary Structure
        //Get Salary Structure Per ORG
        public List<SalaryStructureViewModel> GetSalaryStructureList(string userid)
        {
            var p = new List<SalaryStructureViewModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = from a in _db.AspNetUserRoles
                       join b in _db.lutOrganisations on a.OrganisationID equals b.OrganisationID
                       join c in _db.lutSalaryCategories on b.OrganisationID equals c.OrganisationID
                       join d in _db.lutSalarySubCategories on c.SalaryCategoryID equals d.SalaryCategoryID
                       join e in _db.lutSalaryStructures on d.SalarySubCategoryID equals e.SalarySubCategoryID
                       join f in _db.lutJobLevels on e.JobLevelID equals f.JobLevelID
                       join g in _db.lutJobTitles on e.JobTitleID equals g.JobTitleID
                       where a.UserId == userid

                       select new
                       {
                           SalaryCategoryName = c.CategoryDescr,
                           JobLevelName = f.JobLevelName,
                           MaxValue = e.MaxValue,
                           MinValue = e.MinValue,
                           SalaryStructureID = e.SalaryStructureID,
                           SalarySubCategoryName = d.Descr,
                           OrganisationName = b.OrganisationName,
                           JobTitle = g.JobTitle
                       };

            foreach (var d in data)
            {
                SalaryStructureViewModel e = new SalaryStructureViewModel();
                e.SalaryStructureID = Convert.ToInt32(d.SalaryStructureID);
                e.MaxValue = Convert.ToDecimal(d.MaxValue);
                e.MinValue = Convert.ToDecimal(d.MinValue);
                e.JobLevelName = Convert.ToString(d.JobLevelName);
                e.SalaryCategoryName = Convert.ToString(d.SalaryCategoryName);
                e.SalarySubCategoryName = Convert.ToString(d.SalarySubCategoryName);
                e.OrganisationName = Convert.ToString(d.OrganisationName);
                e.JobTitle = Convert.ToString(d.JobTitle);
                p.Add(e);
            }
            return p;
        }

        //Check if Salary Structure Per ORG Exists
        public int CheckIfSalaryStructureExists(int JobTitleID)
        {
            return _db.lutSalaryStructures.Where(x => x.JobTitleID == JobTitleID
                                        ).Count();
        }

        //Insert Salary Structure
        public void InsertIntoSalaryStructure(int JobTitleID, int SalarySubCategoryID, int JobLevelID, decimal MinValue, decimal MaxValue)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentAddSalaryStructure(JobTitleID, SalarySubCategoryID, JobLevelID, MinValue, MaxValue);
        }

        //Get Salary Structure per ORG For Edit
        public List<SalaryStructureModel> GetSalaryStructureForEdit(int id)
        {
            var p = new List<SalaryStructureModel>();
            _db = new eRecruitmentDataClassesDataContext();

            //var data = _db.lutSalaryStructures.Where(x => x.SalaryStructureID == id).ToList();



            var data = from a in _db.lutSalaryCategories
                       join b in _db.lutSalarySubCategories on a.SalaryCategoryID equals b.SalaryCategoryID
                       join c in _db.lutSalaryStructures on b.SalarySubCategoryID equals c.SalarySubCategoryID
                       join d in _db.lutJobLevels on c.JobLevelID equals d.JobLevelID
                       join e in _db.lutJobTitles on c.JobTitleID equals e.JobTitleID
                       where c.SalaryStructureID == id


                       select new
                       {
                           SalaryCategoryID = a.SalaryCategoryID,
                           JobLevelID = d.JobLevelID,
                           MaxValue = c.MaxValue,
                           MinValue = c.MinValue,
                           SalaryStructureID = c.SalaryStructureID,
                           SalarySubCategoryID = b.SalarySubCategoryID,
                           JobTitleID = e.JobTitleID
                       };


            foreach (var d in data)
            {
                SalaryStructureModel e = new SalaryStructureModel();
                e.SalaryCategoryID = Convert.ToInt32(d.SalaryCategoryID);
                e.SalaryStructureID = Convert.ToInt32(d.SalaryStructureID);
                e.SalarySubCategoryID = Convert.ToInt32(d.SalarySubCategoryID);
                e.JobLevelID = Convert.ToInt32(d.JobLevelID);
                e.MinValue = Convert.ToDecimal(d.MinValue);
                e.MaxValue = Convert.ToDecimal(d.MaxValue);
                e.JobTitleID = Convert.ToInt32(d.JobTitleID);
                p.Add(e);
            }
            return p;
        }

        //Update Salary Structure
        public void UpdateIntoSalaryStructure(int id, int JobTitleID, int SalarySubCategoryID, int JobLevelID, decimal MinValue, decimal MaxValue)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentUpdateSalaryStructure(id, JobTitleID, SalarySubCategoryID, JobLevelID, MinValue, MaxValue);

        }

        //Delete Salary Structure
        public void DeleteIntoSalaryStructure(int SalaryStructureID)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentDeleteSalaryStructure(SalaryStructureID);
        }

        #endregion

        //Get All Salary Structure using Job Title
        public List<SalaryStructureViewModel> GetSalaryStructurePerJobTitle(int id)
        {
            var p = new List<SalaryStructureViewModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = from a in _db.lutSalaryCategories
                       join b in _db.lutSalarySubCategories on a.SalaryCategoryID equals b.SalaryCategoryID
                       join c in _db.lutSalaryStructures on b.SalarySubCategoryID equals c.SalarySubCategoryID
                       join d in _db.lutJobLevels on c.JobLevelID equals d.JobLevelID
                       join e in _db.lutJobTitles on c.JobTitleID equals e.JobTitleID
                       where e.JobTitleID == id

                       select new
                       {
                           SalaryCategoryName = a.CategoryDescr,
                           JobLevelName = d.JobLevelName,
                           MaxValue = c.MaxValue,
                           MinValue = c.MinValue,
                           SalaryStructureID = c.SalaryStructureID,
                           SalarySubCategoryName = b.Descr,
                           JobTitle = e.JobTitle,
                           JobTitleID = e.JobTitleID
                       };

            foreach (var d in data)
            {
                SalaryStructureViewModel e = new SalaryStructureViewModel();
                e.SalaryStructureID = Convert.ToInt32(d.SalaryStructureID);
                e.MaxValue = Convert.ToDecimal(d.MaxValue);
                e.MinValue = Convert.ToDecimal(d.MinValue);
                e.JobLevelName = Convert.ToString(d.JobLevelName);
                e.SalaryCategoryName = Convert.ToString(d.SalaryCategoryName);
                e.SalarySubCategoryName = Convert.ToString(d.SalarySubCategoryName);
                e.JobTitle = Convert.ToString(d.JobTitle);
                e.JobTitleID = Convert.ToInt32(d.JobTitleID);
                p.Add(e);
            }
            return p;
        }

        public List<JobProfileViewModel> GetVacancyProfile(int OrgID)
        {
            var p = new List<JobProfileViewModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = _db.proc_eRecruitmentGetJobProfileList(OrgID);
            foreach (var d in data)
            {
                JobProfileViewModel e = new JobProfileViewModel();
                e.JobProfileID = Convert.ToInt32(d.JobProfileID);
                e.OrganisationName = Convert.ToString(d.OrganisationName);
                e.JobTitle = Convert.ToString(d.JobTitle);
                e.JobTitleID = Convert.ToInt32(d.JobTitleID);
                //e.SkillCategory = Convert.ToString(d.CategoryID);
                e.VacancyPurpose = Convert.ToString(d.VacancyPurpose);
                e.QualificationAndExperience = Convert.ToString(d.QualificationAndExperience);
                e.AdditionalRequirements = Convert.ToString(d.AdditonalRequirements);
                e.Disclaimer = Convert.ToString(d.Disclaimer);
                e.Responsibility = Convert.ToString(d.Responsibility);
                p.Add(e);
            }
            return p;
        }

        //GetGeneralQuestionsList
        public List<GeneralQuestionsModel> GetGeneralQuestionsList(int orgID)
        {
            var p = new List<GeneralQuestionsModel>();
            _db = new eRecruitmentDataClassesDataContext();
            var data = from a in _db.lutOrganisations
                       join b in _db.lutGeneralQuestions on a.OrganisationID equals b.OrganisationID
                       join c in _db.lutQuestionCatergories on b.QCategoryID equals c.QCategoryID
                       where a.OrganisationID == orgID && c.QCategoryDescr == "Qualification"

                       select new
                       {
                           QuestionID = b.id,
                           QuestionDesc = b.GeneralQuestionDesc,
                           QCategoryID = c.QCategoryID,
                           QCategoryDescr = c.QCategoryDescr,
                       };

            foreach (var d in data)
            {
                GeneralQuestionsModel e = new GeneralQuestionsModel();
                e.QuestionID = Convert.ToInt32(d.QuestionID);
                e.QuestionDesc = Convert.ToString(d.QuestionDesc);
                e.QCategoryID = Convert.ToInt32(d.QCategoryID);
                e.QCategoryDescr = Convert.ToString(d.QCategoryDescr);
                p.Add(e);
            }
            return p;

        }

        public List<GeneralQuestionsModel> GetGeneralQuestionsExperienceList(int orgID)
        {
            var p = new List<GeneralQuestionsModel>();
            _db = new eRecruitmentDataClassesDataContext();
            var data = from a in _db.lutOrganisations
                       join b in _db.lutGeneralQuestions on a.OrganisationID equals b.OrganisationID
                       join c in _db.lutQuestionCatergories on b.QCategoryID equals c.QCategoryID
                       where a.OrganisationID == orgID && c.QCategoryDescr == "Experience" && b.GeneralQuestionDesc != "Are You Currently Working For SITA?"

                       select new
                       {
                           QuestionID = b.id,
                           QuestionDesc = b.GeneralQuestionDesc,
                           QCategoryID = c.QCategoryID,
                           QCategoryDescr = c.QCategoryDescr,

                       };

            foreach (var d in data)
            {
                GeneralQuestionsModel e = new GeneralQuestionsModel();
                e.QuestionID = Convert.ToInt32(d.QuestionID);
                e.QuestionDesc = Convert.ToString(d.QuestionDesc);
                e.QCategoryID = Convert.ToInt32(d.QCategoryID);
                e.QCategoryDescr = Convert.ToString(d.QCategoryDescr);
                p.Add(e);
            }
            return p;

        }

        public List<GeneralQuestionsModel> GetGeneralQuestionsCertificationList(int orgID)
        {
            var p = new List<GeneralQuestionsModel>();
            _db = new eRecruitmentDataClassesDataContext();
            var data = from a in _db.lutOrganisations
                       join b in _db.lutGeneralQuestions on a.OrganisationID equals b.OrganisationID
                       join c in _db.lutQuestionCatergories on b.QCategoryID equals c.QCategoryID
                       where a.OrganisationID == orgID && c.QCategoryDescr == "Certification"

                       select new
                       {
                           QuestionID = b.id,
                           QuestionDesc = b.GeneralQuestionDesc,
                           QCategoryID = c.QCategoryID,
                           QCategoryDescr = c.QCategoryDescr,

                       };

            foreach (var d in data)
            {
                GeneralQuestionsModel e = new GeneralQuestionsModel();
                e.QuestionID = Convert.ToInt32(d.QuestionID);
                e.QuestionDesc = Convert.ToString(d.QuestionDesc);
                e.QCategoryID = Convert.ToInt32(d.QCategoryID);
                e.QCategoryDescr = Convert.ToString(d.QCategoryDescr);
                p.Add(e);
            }
            return p;

        }

        public List<GeneralQuestionsModel> GetGeneralQuestionsAnnualSalaryList(int orgID)
        {
            var p = new List<GeneralQuestionsModel>();
            _db = new eRecruitmentDataClassesDataContext();
            var data = from a in _db.lutOrganisations
                       join b in _db.lutGeneralQuestions on a.OrganisationID equals b.OrganisationID
                       join c in _db.lutQuestionCatergories on b.QCategoryID equals c.QCategoryID
                       where a.OrganisationID == orgID && c.QCategoryDescr == "Annual Salary"

                       select new
                       {
                           QuestionID = b.id,
                           QuestionDesc = b.GeneralQuestionDesc,
                           QCategoryID = c.QCategoryID,
                           QCategoryDescr = c.QCategoryDescr,

                       };

            foreach (var d in data)
            {
                GeneralQuestionsModel e = new GeneralQuestionsModel();
                e.QuestionID = Convert.ToInt32(d.QuestionID);
                e.QuestionDesc = Convert.ToString(d.QuestionDesc);
                e.QCategoryID = Convert.ToInt32(d.QCategoryID);
                e.QCategoryDescr = Convert.ToString(d.QCategoryDescr);
                p.Add(e);
            }
            return p;

        }

        public List<GeneralQuestionsModel> GetGeneralQuestionsNoticePeriodList(int orgID)
        {
            var p = new List<GeneralQuestionsModel>();
            _db = new eRecruitmentDataClassesDataContext();
            var data = from a in _db.lutOrganisations
                       join b in _db.lutGeneralQuestions on a.OrganisationID equals b.OrganisationID
                       join c in _db.lutQuestionCatergories on b.QCategoryID equals c.QCategoryID
                       where a.OrganisationID == orgID && c.QCategoryDescr == "Notice Period"

                       select new
                       {
                           QuestionID = b.id,
                           QuestionDesc = b.GeneralQuestionDesc,
                           QCategoryID = c.QCategoryID,
                           QCategoryDescr = c.QCategoryDescr,

                       };

            foreach (var d in data)
            {
                GeneralQuestionsModel e = new GeneralQuestionsModel();
                e.QuestionID = Convert.ToInt32(d.QuestionID);
                e.QuestionDesc = Convert.ToString(d.QuestionDesc);
                e.QCategoryID = Convert.ToInt32(d.QCategoryID);
                e.QCategoryDescr = Convert.ToString(d.QCategoryDescr);
                p.Add(e);
            }
            return p;

        }

        //Get Vacancy Type List
        public List<VacancyTypeModel> GetVacancyTypeList()
        {
            var p = new List<VacancyTypeModel>();
            _db = new eRecruitmentDataClassesDataContext();

            //============Peter 20221028============
            var VacancyType = _db.lutVacancyTypes.OrderBy(x => x.VacancyTypeName).ToList();
            //======================================
            //var VacancyType = _db.lutVacancyTypes.ToList();

            foreach (var d in VacancyType)
            {
                VacancyTypeModel e = new VacancyTypeModel();
                e.VancyTypeID = Convert.ToInt32(d.VancyTypeID);
                e.VacancyTypeName = Convert.ToString(d.VacancyTypeName);
                p.Add(e);
            }
            return p;

        }

        public int GetOrganisationID(string userid)
        {
            return _db.AspNetUserRoles.Where(x => x.UserId == userid).Select(x => x.OrganisationID).FirstOrDefault();
        }

        //Insert Vacancy
        public int InsertVacancy(string UserID, int OrganisationID, string BPSVacancyNo, int DivisionID, int DepartmentID, int JobTitleID, int SalaryTypeID, string Recruiter,
                                 string RecruiterEmail, string RecruiterTel, string RecruiterUserId, string Manager, int GenderID, int RaceID, int EmploymentTypeID,
                                 string ContractDuration, DateTime ClosingDate, int NumberOfOpenings, int VancyTypeID, string DeligationReasons, string Location,
                                string AdditionalRequirement, string VacancyPurpose, string QualificationAndExperience
                                , string TechnicalCompetenciesDescription, string Disclaimer, string Responsibility, string Knowledge, string LeadershipCompetencies
                                , string behaviouralCompetencyDesc)
        {
            int id = 0;

            _db = new eRecruitmentDataClassesDataContext();

            var vacancy = _db.proc_eRecruitmentInsertVacancy(UserID, OrganisationID, BPSVacancyNo, DivisionID, DepartmentID, JobTitleID, SalaryTypeID, Recruiter,
                            RecruiterEmail, RecruiterTel, RecruiterUserId, Manager, GenderID, RaceID,
                            EmploymentTypeID, ContractDuration, ClosingDate, NumberOfOpenings, VancyTypeID, DeligationReasons, Location, AdditionalRequirement,
                             VacancyPurpose, QualificationAndExperience, TechnicalCompetenciesDescription
                            , Disclaimer, Responsibility, Knowledge, LeadershipCompetencies, behaviouralCompetencyDesc);
            foreach (var d in vacancy)
            {
                id = (int)d.VacancyID;
            }


            return id;
        }
        //UPdate Vacancy
        public int UpdateVacancy(int id, string UserID, int OrganisationID, string BPSVacancyNo, int DivisionID, int DepartmentID, int JobTitleID, int SalaryTypeID, string Recruiter,
                                 string RecruiterEmail, string RecruiterTel, string RecruiterUserId, string Manager, int GenderID, int RaceID, int EmploymentTypeID,
                                 string ContractDuration, DateTime ClosingDate, int NumberOfOpenings, int VancyTypeID, string DeligationReasons, string Location,
                                 string AdditionalRequirements, string VacancyPurpose, string QualificationAndExperience
                                , string TechnicalCompetenciesDescription, string Disclaimer, string Responsibility, string Knowledge, string LeadershipCompetencies, string behaviouralCompetencyDesc)
        {
            _db = new eRecruitmentDataClassesDataContext();

            var vacancy = _db.proc_eRecruitmentUpdateVacancy(id, UserID, OrganisationID, BPSVacancyNo, DivisionID, DepartmentID, JobTitleID, SalaryTypeID, Recruiter,
                            RecruiterEmail, RecruiterTel, RecruiterUserId, Manager, GenderID, RaceID,
                            EmploymentTypeID, ContractDuration, ClosingDate, NumberOfOpenings, VancyTypeID, DeligationReasons, Location, AdditionalRequirements,
                             VacancyPurpose, QualificationAndExperience, TechnicalCompetenciesDescription, Disclaimer, Responsibility, Knowledge
                            , LeadershipCompetencies, behaviouralCompetencyDesc);


            return id;
        }
        public int ReAdvertiseVacancy(int id, string UserID, int OrganisationID, string BPSVacancyNo, int DivisionID, int DepartmentID, int JobTitleID, int SalaryTypeID, string Recruiter,
                                 string RecruiterEmail, string RecruiterTel, string RecruiterUserId, string Manager, int GenderID, int RaceID, int EmploymentTypeID,
                                 string ContractDuration, DateTime ClosingDate, int NumberOfOpenings, int VancyTypeID, string DeligationReasons, string Location,
                                 string AdditionalRequirements, int VacancyProfileID, string VacancyPurpose, string QualificationAndExperience
                                , string TechnicalCompetenciesDescription, string Disclaimer, string Responsibility, string Knowledge, string LeadershipCompetencies, string behaviouralCompetencyDesc)
        {
            _db = new eRecruitmentDataClassesDataContext();

            var vacancy = _db.proc_eRecruitmentReAdvertiseVacancy(id, UserID, OrganisationID, BPSVacancyNo, DivisionID, DepartmentID, JobTitleID, SalaryTypeID, Recruiter,
                            RecruiterEmail, RecruiterTel, RecruiterUserId, Manager, GenderID, RaceID,
                            EmploymentTypeID, ContractDuration, ClosingDate, NumberOfOpenings, VancyTypeID, DeligationReasons, Location, AdditionalRequirements,
                            VacancyProfileID, VacancyPurpose, QualificationAndExperience, TechnicalCompetenciesDescription, Disclaimer, Responsibility, Knowledge
                            , LeadershipCompetencies, behaviouralCompetencyDesc);


            return id;
        }

        //InsertUpdateVacancyQuestion
        public void InsertUpdateVacancyQuestion(int vacancyid, string questionid)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentInsertUpdateVacancyQuestion(vacancyid, questionid);

        }

        //InsertVacancyDocument
        public void InsertVacancyDocument(int vacancyID, string sFileName, byte[] FileData, string ContentType, string FileExtension)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentInsertVacancyDocument(vacancyID, sFileName, FileData, ContentType, FileExtension);

        }


        //GetVacancyRefNo
        public List<VacancyModels> GetVacancyRefNo(int vacancyID)
        {
            var p = new List<VacancyModels>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = from a in _db.tblVacancies

                       where a.ID == vacancyID

                       select new
                       {
                           ReferenceNo = a.ReferenceNo,

                       };


            foreach (var d in data)
            {
                VacancyModels e = new VacancyModels();
                e.ReferenceNo = Convert.ToString(d.ReferenceNo);
                p.Add(e);
            }
            return p;
        }

        //GetVacancyListByUser
        public List<VacancyListModels> GetVacancyListByUser(string userid)
        {
            var p = new List<VacancyListModels>();
            _db = new eRecruitmentDataClassesDataContext();

            var vacancy = from a in _db.tblVacancies
                          join b in _db.lutStatus on a.StatusID equals b.StatusID
                          join c in _db.lutEmployementTypes on a.EmploymentTypeID equals c.id
                          join d in _db.lutOrganisations on a.OrganisationID equals d.OrganisationID
                          join e in _db.lutDivisions on a.DivisionID equals e.DivisionID
                          join f in _db.lutDepartments on a.DepartmentID equals f.DepartmentID
                          join g in _db.tblVacancyProfiles on a.VacancyProfileID equals g.VacancyProfileID
                          where a.UserID == userid && a.ClosingDate >= DateTime.Now
                          orderby a.CreatedDate descending
                          select new
                          {
                              ID = a.ID,
                              ReferenceNo = a.ReferenceNo,
                              //VacancyName = a.VacancyName,
                              JobTitle = g.VacancyName,
                              EmploymentType = c.EmploymentType,
                              Organisation = d.OrganisationName,
                              CreatedDate = a.CreatedDate,
                              ModifyDate = a.ModifyDate,
                              ClosingDate = a.ClosingDate,
                              Status = b.StatusDescription,
                              NumberOfOpenings = a.NumberOfOpenings

                          };
            foreach (var d in vacancy)
            {
                VacancyListModels e = new VacancyListModels();
                e.ID = Convert.ToInt32(d.ID);
                e.ReferenceNo = Convert.ToString(d.ReferenceNo);
                // e.VacancyName = Convert.ToString(d.VacancyName);
                e.JobTitle = Convert.ToString(d.JobTitle);
                e.EmploymentType = Convert.ToString(d.EmploymentType);
                e.Organisation = Convert.ToString(d.Organisation);
                e.CreatedDate = Convert.ToDateTime(d.CreatedDate).ToShortDateString();
                e.ModifyDate = Convert.ToDateTime(d.ModifyDate).ToShortDateString();
                e.ClosingDate = Convert.ToDateTime(d.ClosingDate).ToShortDateString();
                e.Status = Convert.ToString(d.Status);
                e.NumberOfOpenings = Convert.ToInt32(d.NumberOfOpenings);
                p.Add(e);
            }
            return p;

        }

        //GetJobProfileList
        public List<JobProfileViewModel> GetJobProfileList(int id)
        {
            var p = new List<JobProfileViewModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = _db.sp_JobProfile_Get_PerJobTitleID(id);

            //var data = from a in _db.AspNetUserRoles
            //           join b in _db.lutOrganisations on a.OrganisationID equals b.OrganisationID
            //           join c in _db.tblJobProfiles on b.OrganisationID equals c.OrganisationID
            //           join d in _db.lutSalaryStructures on c.JobTitleID equals d.JobTitleID
            //           join e in _db.lutSalarySubCategories on d.SalarySubCategoryID equals e.SalarySubCategoryID
            //           join f in _db.lutSalaryCategories on e.SalaryCategoryID equals f.SalaryCategoryID
            //           join g in _db.lutJobLevels on d.JobLevelID equals g.JobLevelID
            //           join h in _db.lutDiscalmers on c.OrganisationID equals h.OrginazationID

            //           where c.JobTitleID == id

            //           select new
            //           {
            //               f.SalaryCategoryID,
            //               SalaryCategory = f.CategoryDescr,
            //               SalarySubCategory = e.Descr,
            //               g.JobLevelName,
            //               d.MinValue,
            //               d.MaxValue,
            //               c.VacancyPurpose,
            //               c.Responsibility,
            //               c.QualificationAndExperience,
            //               c.AdditonalRequirements,
            //               h.Disclamer
            //           };

            foreach (var d in data)
            {
                JobProfileViewModel e = new JobProfileViewModel();
                e.SalaryCategoryID = Convert.ToInt32(d.SalaryCategoryID);
                e.CategoryDescr = Convert.ToString(d.CategoryDescr);
                e.Descr = Convert.ToString(d.Descr);
                e.JobLevelID = Convert.ToInt32(d.JobLevelID);
                e.JobLevelName = Convert.ToString(d.JobLevelName);
                e.MinValue = Convert.ToDecimal(d.MinValue);
                e.MaxValue = Convert.ToDecimal(d.MaxValue);
                e.VacancyPurpose = Convert.ToString(d.VacancyPurpose);
                e.Responsibility = Convert.ToString(d.Responsibility);
                e.QualificationAndExperience = Convert.ToString(d.QualificationAndExperience);
                e.Knowledge = Convert.ToString(d.Knowledge);
                e.TechComps = Convert.ToString(d.SelectedTechComps).Replace("#", "\r\n\r\n");
                e.LeadComps = Convert.ToString(d.SelectedLeadComps).Replace("#", "\r\n\r\n");
                e.BehaveComps = Convert.ToString(d.SelectedBehaveComps).Replace("#", "\r\n\r\n");
                e.AdditionalRequirements = Convert.ToString(d.AdditonalRequirements);
                e.Disclaimer = Convert.ToString(d.Disclaimer);
                p.Add(e);
            }
            return p;
        }

        //GetSalaryStructureList
        public List<JobProfileViewModel> GetSalaryStructureList(int id)
        {
            var p = new List<JobProfileViewModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = from a in _db.AspNetUserRoles
                       join b in _db.lutOrganisations on a.OrganisationID equals b.OrganisationID
                       join c in _db.lutJobTitles on b.OrganisationID equals c.OrganisationID
                       join d in _db.lutSalaryStructures on c.JobTitleID equals d.JobTitleID
                       join e in _db.lutSalarySubCategories on d.SalarySubCategoryID equals e.SalarySubCategoryID
                       join f in _db.lutSalaryCategories on e.SalaryCategoryID equals f.SalaryCategoryID
                       join g in _db.lutJobLevels on d.JobLevelID equals g.JobLevelID

                       where c.JobTitleID == id

                       select new
                       {
                           SalaryCategoryID = f.SalaryCategoryID,
                           SalaryCategory = f.CategoryDescr,
                           SalarySubCategory = e.Descr,
                           JobLevelID = g.JobLevelID,
                           JobLevelName = g.JobLevelName,
                           MinValue = d.MinValue,
                           MaxValue = d.MaxValue,
                       };

            foreach (var d in data)
            {
                JobProfileViewModel e = new JobProfileViewModel();
                e.SalaryCategoryID = Convert.ToInt32(d.SalaryCategoryID);
                e.CategoryDescr = Convert.ToString(d.SalaryCategory);
                e.Descr = Convert.ToString(d.SalarySubCategory);
                e.JobLevelID = Convert.ToInt32(d.JobLevelID);
                e.JobLevelName = Convert.ToString(d.JobLevelName);
                e.MinValue = Convert.ToDecimal(d.MinValue);
                e.MaxValue = Convert.ToDecimal(d.MaxValue);
                p.Add(e);
            }
            return p;
        }

        //GetSkillsPerCatergoryList
        public List<SkillModel> GetSkillsPerCatergiryList(int id)
        {
            var p = new List<SkillModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = from a in _db.lutSkillsCategories
                       join b in _db.lutSkills on a.CategoryID equals b.CategoryID

                       where a.CategoryID == id
                       select new
                       {
                           SkillID = b.skillID,
                           SkillName = b.skillName,

                       };

            foreach (var d in data)
            {
                SkillModel e = new SkillModel();
                e.skillID = Convert.ToInt32(d.SkillID);
                e.skillName = Convert.ToString(d.SkillName);
                p.Add(e);
            }
            return p;

        }

        //Get Selected SkillsPerCatergoryList For Edit
        public List<SkillModel> GetSelectedSkillsPerCatergiryList(int id)
        {
            var p = new List<SkillModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = from a in _db.tblVacancySkills
                       join b in _db.lutSkillsCategories on a.CategoryID equals b.CategoryID
                       join c in _db.lutSkills on a.SkillID equals c.skillID
                       join d in _db.tblVacancies on a.VacancyID equals d.ID

                       where a.VacancyID == id
                       select new
                       {
                           SkillID = c.skillID,
                           SkillName = c.skillName,

                       };

            foreach (var d in data)
            {
                SkillModel e = new SkillModel();
                e.skillID = Convert.ToInt32(d.SkillID);
                e.skillName = Convert.ToString(d.SkillName);
                p.Add(e);
            }
            return p;

        }

        //GetRecruiterInfo
        public List<VacancyModels> GetRecruiterInfo(string userid)
        {
            var p = new List<VacancyModels>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = from a in _db.AspNetUserRoles
                       join b in _db.lutOrganisations on a.OrganisationID equals b.OrganisationID
                       join c in _db.tblProfiles on a.UserId equals c.UserID
                       orderby c.FirstName
                       where a.UserId == userid
                       //============Peter 20221028============
                       orderby c.FirstName.Trim() ascending
                       //======================================
                       select new
                       {
                           FullName = c.FirstName + " " + c.Surname,
                           EmailAddress = c.EmailAddress,

                       };

            foreach (var d in data)
            {
                VacancyModels e = new VacancyModels();
                e.Recruiter = Convert.ToString(d.FullName);
                e.RecruiterEmail = Convert.ToString(d.EmailAddress);
                p.Add(e);
            }
            return p;
        }

        //GetVacancyProfileID
        public List<VacancyProfileModel> GetVacancyProfileID(int JobTitleID)
        {
            var p = new List<VacancyProfileModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = from a in _db.lutOrganisations
                       join b in _db.lutJobTitles on a.OrganisationID equals b.OrganisationID
                       join c in _db.tblJobProfiles on b.JobTitleID equals c.JobTitleID

                       where c.JobTitleID == JobTitleID

                       select new
                       {
                           VacancyProfileID = c.JobProfileID,

                       };

            foreach (var d in data)
            {
                VacancyProfileModel e = new VacancyProfileModel();
                e.VacancyProfileID = Convert.ToInt32(d.VacancyProfileID);
                p.Add(e);
            }
            return p;
        }


        //GetVancyList
        public List<VacancyModels> GetVancyList(string userid)
        {
            var p = new List<VacancyModels>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = _db.GetVancyList(userid).ToList();

            foreach (var d in data)
            {
                VacancyModels e = new VacancyModels();
                e.ID = Convert.ToInt32(d.ID);
                e.JobTitle = Convert.ToString(d.JobTitle);
                e.EmploymentType = Convert.ToString(d.EmploymentType);
                e.Status = Convert.ToString(d.StatusDescription);
                e.CreatedDate = Convert.ToDateTime(d.CreatedDate).ToShortDateString();
                e.ModifyDate = Convert.ToDateTime(d.ModifyDate).ToShortDateString();
                e.ClosingDate = Convert.ToDateTime(d.ClosingDate).ToShortDateString();
                e.ReferenceNo = Convert.ToString(d.ReferenceNo);
               
                p.Add(e);
            }
            return p;
        }

        //Insert Job Vacancy
        public int InsertIntoJobProfile(int OrganisationID, int JobTitleID, string VacancyPurpose, string QualificationAndExperience, string Knowledge, string AdditonalRequirements, string SelectedTechComps, string SelectedLeadComps, string SelectedBehaveComps, string Disclaimer, string Responsibility, Guid UserID)
        {
            int id = 0;

            _db = new eRecruitmentDataClassesDataContext();
            var vacancyProfile = _db.proc_eRecruitmentAddJobProfile(OrganisationID, JobTitleID, VacancyPurpose, QualificationAndExperience, Knowledge, AdditonalRequirements, SelectedTechComps, SelectedLeadComps, SelectedBehaveComps, Disclaimer, Responsibility, UserID);

            foreach (var d in vacancyProfile)
            {
                id = (int)d.JobProfileID;
            }


            return id;
        }

        //InsertUpdateVacancySkills
        public void InsertUpdateVacancySkill(int VacancyID, int CategoryID, string SkillID)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentInsertUpdateVacancySkill(VacancyID, CategoryID, SkillID);

        }

        //===========================Peter - 20231101======================================================
        //InsertUpdateLutJobSpecificQuestion
        public void InsertUpdateLutJobSpecificQuestion(int JobTitleID, string JobSpecificeQuestionDesc, DateTime CreatedDate, string CreatedBy, DateTime? ModifyDate, string ModifiedBy)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentInsertUpdateLutJobSpecificQuestion(JobTitleID, JobSpecificeQuestionDesc, CreatedDate, CreatedBy, ModifyDate, ModifiedBy);

        }
        //=================================================================================================

        //GetVancyListForEdit
        public List<VacancyModels> GetVacancyListForEditByID(int id)
        {
            var p = new List<VacancyModels>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = _db.GetVacancyListForEditByID(id);

            var vacancySalary = GetVacancySalary(id);

            foreach (var d in data)
            {
                VacancyModels e = new VacancyModels();
                e.ID = Convert.ToInt32(d.ID);
                e.BPSVacancyNo = Convert.ToString(d.BPSVacancyNo);
                e.DivisionID = Convert.ToInt32(d.DivisionID);
                e.DepartmentID = Convert.ToInt32(d.DepartmentID);
                e.JobTitleID = Convert.ToInt32(d.JobTitleID);
                e.EmploymentTypeID = Convert.ToInt32(d.EmploymentTypeID);
                e.Status = Convert.ToString(d.StatusDescription);
                e.CreatedDate = Convert.ToDateTime(d.CreatedDate).ToShortDateString();
                e.ModifyDate = Convert.ToDateTime(d.ModifyDate).ToShortDateString();
                e.ClosingDate = Convert.ToDateTime(d.ClosingDate).ToShortDateString();
                e.ReferenceNo = Convert.ToString(d.ReferenceNo);
               
                e.JobLevelName = Convert.ToString(vacancySalary.JobLevel);
                e.MinValue = Convert.ToDecimal(vacancySalary.MinValue);
                e.MaxValue = Convert.ToDecimal(vacancySalary.MaxValue);
                e.Recruiter = Convert.ToString(d.Recruiter);
                e.RecruiterEmail = Convert.ToString(d.RecruiterEmail);
                e.RecruiterTel = Convert.ToString(d.RecruiterTel);
                e.RecruiterUserId = Convert.ToString(d.RecruiterUserId);
                e.Manager = Convert.ToString(d.Manager);
                e.ContractDuration = Convert.ToString(d.ContractDuration);
                e.NumberOfOpenings = Convert.ToInt32(d.NumberOfOpenings);
                e.VancyTypeID = Convert.ToInt32(d.VancyTypeID);
                e.Location = Convert.ToString(d.Location);
                e.GenderID = Convert.ToInt32(d.GenderID);
                e.RaceID = Convert.ToInt32(d.RaceID);
                e.SalaryTypeID = Convert.ToInt32(d.SalaryTypeID);
                e.VacancyPurpose = Convert.ToString(d.VacancyPurpose);
                e.DeligationReasons = Convert.ToString(d.DeligationReasons);
                e.Responsibility = Convert.ToString(d.Responsibility);
                e.TechComps = d.TechnicalCompetenciesDescription;
                e.LeadComps = d.LeadershipCompetencies;
                e.BehaveComps = d.BehaviouralCompetency;
                e.QualificationAndExperience = Convert.ToString(d.QualificationAndExperience);
                e.Knowledge = Convert.ToString(d.Knowledge);
                e.AdditonalRequirements = Convert.ToString(d.AdditonalRequirements);
                e.Disclaimer = Convert.ToString(d.Disclaimer);
                p.Add(e);
            }
            return p;
        }

        //Check if Job Profile Exists
        public int CheckIfJobProfileExists(int OrganisationID, int JobTitleID, string VacancyPurpose, string QualificationAndExperience, string AdditonalRequirements, string Disclaimer, string Responsibility)
        {
            return _db.tblJobProfiles.Where(x => x.OrganisationID == OrganisationID
                                         && x.JobTitleID == JobTitleID
                                        //&& x.VacancyPurpose == VacancyPurpose
                                        //&& x.QualificationAndExperience == QualificationAndExperience
                                        //&& x.AdditonalRequirements == AdditonalRequirements
                                        //&& x.Disclaimer == Disclaimer
                                        //&& x.Responsibility == Responsibility
                                        ).Count();
        }

        //Get Selected SkillsPerCatergoryList For Edit
        public List<SkillModel> GetSelectedSkillsPerCatergiryListForJobProfile(int id)
        {
            var p = new List<SkillModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = from a in _db.tblVacancyProfileSkills
                       join b in _db.lutSkillsCategories on a.CategoryID equals b.CategoryID
                       join c in _db.lutSkills on a.SkillID equals c.skillID
                       join d in _db.tblJobProfiles on a.JobProfileID equals d.JobProfileID

                       where a.JobProfileID == id
                       select new
                       {
                           SkillID = c.skillID,
                           SkillName = c.skillName,
                       };

            foreach (var d in data)
            {
                SkillModel e = new SkillModel();
                e.skillID = Convert.ToInt32(d.SkillID);
                e.skillName = Convert.ToString(d.SkillName);
                p.Add(e);
            }
            return p;

        }

        //Get Job Profile For Edit

        public List<JobProfileViewModel> GetVacancyProfileEdit(int id)
        {

            var p = new List<JobProfileViewModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = _db.proc_eRecruitmentGetJobProfile(id);

            foreach (var d in data)
            {
                JobProfileViewModel e = new JobProfileViewModel();
                e.JobProfileID = Convert.ToInt32(d.JobProfileID);
                e.JobTitleID = Convert.ToInt32(d.JobTitleID);
                //e.CategoryID = Convert.ToInt32(d.CategoryID);
                e.CategoryDescr = Convert.ToString(d.CategoryDescr);
                e.Descr = Convert.ToString(d.Descr);
                e.JobLevelID = Convert.ToInt32(d.JobLevelID);
                e.JobLevelName = Convert.ToString(d.JobLevelName);
                e.MinValue = Convert.ToDecimal(d.MinValue);
                e.MaxValue = Convert.ToDecimal(d.MaxValue);
                e.VacancyPurpose = Convert.ToString(d.VacancyPurpose);
                e.QualificationAndExperience = Convert.ToString(d.QualificationAndExperience);
                e.Knowledge = Convert.ToString(d.Knowledge);
                e.AdditionalRequirements = Convert.ToString(d.AdditonalRequirements);
                e.SelectedBehaveComps = Convert.ToString(d.SelectedBehaveComps).Split(';');
                e.SelectedLeadComps = Convert.ToString(d.SelectedLeadComps).Split(';');
                e.SelectedTechComps = Convert.ToString(d.SelectedTechComps).Split(';');
                e.Disclaimer = Convert.ToString(d.Disclaimer);
                e.Responsibility = Convert.ToString(d.Responsibility);
                p.Add(e);
            }
            return p;
        }

        //Update Vacancy Profile
        public int UpdateIntoJobProfile(int id, int OrganisationID, int JobTitleID, string VacancyPurpose, string QualificationAndExperience, string Knowledge, string AdditonalRequirements, string SelectedTechComps, string SelectedLeadComps, string SelectedBehaveComps, string Disclaimer, string Responsibility, Guid UserID)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentUpdateJobProfile(id, OrganisationID, JobTitleID, VacancyPurpose, QualificationAndExperience, Knowledge, AdditonalRequirements, SelectedTechComps, SelectedLeadComps, SelectedBehaveComps, Disclaimer, Responsibility, UserID);

            return id;

        }

        //InsertUpdateVacancyProfileSkills
        public void InsertUpdateVacancyProfileSkill(int JobProfileID, int CategoryID, string SkillID)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentInsertUpdateVacancyProfileSkill(JobProfileID, CategoryID, SkillID);

        }


        //GetApprovedVacancyList
        public List<VacancyListModels> GetApprovedVacancyList(string userid)
        {
            var p = new List<VacancyListModels>();

            _db = new eRecruitmentDataClassesDataContext();

            var data = _db.AspNetUserRoles.Where(x => x.UserId == userid).FirstOrDefault();
            int orgid = data.OrganisationID;

            var vacancy = from a in _db.tblVacancies
                          join b in _db.lutStatus on a.StatusID equals b.StatusID
                          join c in _db.lutEmployementTypes on a.EmploymentTypeID equals c.id
                          join d in _db.lutOrganisations on a.OrganisationID equals d.OrganisationID
                          join e in _db.lutDivisions on a.DivisionID equals e.DivisionID
                          join f in _db.lutDepartments on a.DepartmentID equals f.DepartmentID
                          //join g in _db.tblJobProfiles on a.VacancyProfileID equals g.JobProfileID
                          join h in _db.lutJobTitles on a.JobTitleID equals h.JobTitleID
                          where a.StatusID == 2 && a.OrganisationID == orgid && a.Manager == userid
                          && a.ClosingDate >= DateTime.Now
                          orderby a.CreatedDate descending
                          select new
                          {
                              ID = a.ID,
                              ReferenceNo = a.ReferenceNo,
                              VacancyName = h.JobTitle,
                              JobTitle = h.JobTitle,
                              EmploymentType = c.EmploymentType,
                              Organisation = d.OrganisationName,
                              CreatedDate = a.CreatedDate,
                              ClosingDate = a.ClosingDate,
                              Status = b.StatusDescription
                          };
            foreach (var d in vacancy)
            {
                VacancyListModels e = new VacancyListModels();
                e.ID = Convert.ToInt32(d.ID);
                e.ReferenceNo = Convert.ToString(d.ReferenceNo);
                e.JobTitle = Convert.ToString(d.JobTitle);
                e.JobTitle = Convert.ToString(d.JobTitle);
                e.EmploymentType = Convert.ToString(d.EmploymentType);
                e.Organisation = Convert.ToString(d.Organisation);
                e.CreatedDate = Convert.ToDateTime(d.CreatedDate).ToShortDateString();
                e.ClosingDate = Convert.ToDateTime(d.ClosingDate).ToShortDateString();
                e.Status = Convert.ToString(d.Status);
                p.Add(e);
            }
            return p;

        }

        public List<VacancyListModels> GetVacancyListForView(string userid)
        {
            var p = new List<VacancyListModels>();

            _db = new eRecruitmentDataClassesDataContext();

            var vacancy = _db.sp_tblVacancyListForView_Get(userid).ToList();
            foreach (var d in vacancy)
            {
                VacancyListModels e = new VacancyListModels();
                e.ID = Convert.ToInt32(d.ID);

                //Peter added field on 20220907
                e.BPSVacancyNo = Convert.ToString(d.BPSVacancyNo);

                e.ReferenceNo = Convert.ToString(d.ReferenceNo);

                //Peter added field on 20220907
                e.Recruiter = Convert.ToString(d.Recruiter);

                e.ReferenceNo = Convert.ToString(d.ReferenceNo);
                e.JobTitle = Convert.ToString(d.JobTitle);
                e.JobTitle = Convert.ToString(d.JobTitle);
                e.EmploymentType = Convert.ToString(d.EmploymentType);
                e.Organisation = Convert.ToString(d.OrganisationName);
                e.CreatedDate = Convert.ToDateTime(d.CreatedDate).ToShortDateString();
                e.ClosingDate = Convert.ToDateTime(d.ClosingDate).ToShortDateString();
                e.Status = Convert.ToString(d.Status);
                p.Add(e);
            }
            return p;

        }

        //GetVancyListForEdit
        public List<VacancyModels> GetVacancyForApproval(int id)
        {
            var p = new List<VacancyModels>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = _db.GetVacancyListForEditByID(id);
            var vacancySalry = GetVacancySalary(id);
            foreach (var d in data)
            {
                VacancyModels e = new VacancyModels();
                e.ID = Convert.ToInt32(d.ID);
                e.BPSVacancyNo = Convert.ToString(d.BPSVacancyNo);
                e.DivisionID = Convert.ToInt32(d.DivisionID);
                e.DepartmentID = Convert.ToInt32(d.DepartmentID);
                e.JobTitleID = Convert.ToInt32(d.JobTitleID);
                e.EmploymentTypeID = Convert.ToInt32(d.EmploymentTypeID);
                e.Status = Convert.ToString(d.StatusDescription);
                e.CreatedDate = Convert.ToDateTime(d.CreatedDate).ToShortDateString();
                e.ModifyDate = Convert.ToDateTime(d.ModifyDate).ToShortDateString();
                e.ClosingDate = Convert.ToDateTime(d.ClosingDate).ToShortDateString();
                e.ReferenceNo = Convert.ToString(d.ReferenceNo);
              
                e.JobLevelName = Convert.ToString(vacancySalry.JobLevel);
                e.MinValue = Convert.ToDecimal(vacancySalry.MinValue);
                e.MaxValue = Convert.ToDecimal(vacancySalry.MaxValue);
                e.Recruiter = Convert.ToString(d.Recruiter);
                e.RecruiterEmail = Convert.ToString(d.RecruiterEmail);
                e.RecruiterTel = Convert.ToString(d.RecruiterTel);
                e.RecruiterUserId = Convert.ToString(d.RecruiterUserId);
                e.Manager = Convert.ToString(d.Manager);
                e.ContractDuration = Convert.ToString(d.ContractDuration);
                e.NumberOfOpenings = Convert.ToInt32(d.NumberOfOpenings);
                e.VancyTypeID = Convert.ToInt32(d.VancyTypeID);
                e.Location = Convert.ToString(d.Location);
                e.GenderID = Convert.ToInt32(d.GenderID);
                e.RaceID = Convert.ToInt32(d.RaceID);
                e.SalaryTypeID = Convert.ToInt32(d.SalaryTypeID);
                e.VacancyPurpose = Convert.ToString(d.VacancyPurpose);
                e.DeligationReasons = Convert.ToString(d.DeligationReasons);
                e.Responsibility = Convert.ToString(d.Responsibility);
              
                e.TechComps = d.TechnicalCompetenciesDescription;
                e.LeadComps = d.LeadershipCompetencies;
                e.BehaveComps = d.BehaviouralCompetency;
                e.QualificationAndExperience = Convert.ToString(d.QualificationAndExperience);
                e.Knowledge = Convert.ToString(d.Knowledge);
                e.AdditonalRequirements = Convert.ToString(d.AdditonalRequirements);
                e.Disclaimer = Convert.ToString(d.Disclaimer);
                p.Add(e);
            }
            return p;
        }

        //UpdateVacancyStatus
        public bool UpdateVacancyStatus(int statusid, int id)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentUpdateVacancyStatus(statusid, id);
            return true;

        }

        public bool UpdateReAdvert(int isReAdvert, int id)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentUpdateVacancyIsReAdvert(isReAdvert, id);
            return true;

        }

        //Get Recruiters Email From a Vacancy to Send Notification 
        public List<VacancyModels> GetRecruitersEmail(int id)
        {
            var p = new List<VacancyModels>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = from a in _db.tblVacancies


                       where a.ID == id
                       select new
                       {
                           RecruiterEmail = a.RecruiterEmail,

                       };

            foreach (var d in data)
            {
                VacancyModels e = new VacancyModels();
                e.RecruiterEmail = Convert.ToString(d.RecruiterEmail);
                p.Add(e);
            }
            return p;

        }

        //Get Recruiters Info From a Vacancy to Send Notification 
        public List<NotificationModel> GetRecruitersInfo(string id)
        {
            var p = new List<NotificationModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = (from a in _db.tblVacancies
                        join b in _db.tblProfiles on a.UserID equals b.UserID
                        join c in _db.tblVacancyProfiles on a.VacancyProfileID equals c.VacancyProfileID
                        where a.ID == Convert.ToInt32(id)
                        select new
                        {
                            b.EmailAddress
                                ,
                            b.Surname
                                ,
                            b.FirstName
                                ,
                            a.ReferenceNo
                                ,
                            c.VacancyName
                        }).FirstOrDefault();
            return (p);
        }
        public string GetApproverUserID()
        {
            _db = new eRecruitmentDataClassesDataContext();

            var UsID = _db.AspNetUserRoles.Where(s => s.RoleId == "2").ToList().Select(x => x.UserId).FirstOrDefault();

            return UsID;
        }

        public string GetApproverEmail(string UsID)
        {
            _db = new eRecruitmentDataClassesDataContext();

            var Aemail = _db.AspNetUsers.Where(h => h.Id == UsID).ToList().Select(f => f.Email).FirstOrDefault();

            return Aemail;
        }

        public void UpdateRejectionReason(string reason, int id, string userid, int rejectReasonID)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentUpdateRejectionReason(reason, id, userid, rejectReasonID);

        }
        //UpdateWithdrawalReason
        public void UpdateWithdrawalReason(string reason, int id, int withdrawalReasonID)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentUpdateWithdrawalReason(reason, id, withdrawalReasonID);

        }
        //Update Retraced Reason
        public void UpdateRetractReason(string reason, int id, int retractReasonID)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentUpdateRetractedReason(reason, id, retractReasonID);

        }

        //InsertUpdateVacancyHistory
        public void InsertUpdateVacancyHistory(int ID, string UserID, string ReferenceNo, int OrganisationID, string BPSVacancyNo, int DivisionID, int DepartmentID, int JobTitleID, int SalaryTypeID,
                                               string CategoryDescr, string Descr, string JobLevelName, decimal MinValue, decimal MaxValue, string Recruiter,
                                               string RecruiterEmail, string RecruiterTel, string RecruiterUserId, string Manager, int GenderID, int RaceID, int EmploymentTypeID,
                                               string ContractDuration, DateTime ClosingDate, int NumberOfOpenings, int VancyTypeID, string DeligationReasons, string Location, int StatusID, int VacancyProfileID, string VacancyPurpose, string Responsibility,
                                               string QualificationAndExperience, string TechnicalCompetenciesDescription, string AdditonalRequirements, string Disclaimer)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentInsertUpdateVacancyHistory(ID, UserID, ReferenceNo, OrganisationID, BPSVacancyNo, DivisionID, DepartmentID, JobTitleID, SalaryTypeID,
                                                CategoryDescr, Descr, JobLevelName, MinValue, MaxValue, Recruiter,
                                                RecruiterEmail, RecruiterTel, RecruiterUserId, Manager, GenderID, RaceID, EmploymentTypeID, ContractDuration, ClosingDate,
                                                NumberOfOpenings, VancyTypeID, DeligationReasons, Location, StatusID,
                                                VacancyProfileID, VacancyPurpose, Responsibility, QualificationAndExperience, TechnicalCompetenciesDescription,
                                                AdditonalRequirements, Disclaimer);

        }

        //Get List of Approvers

        public List<ApproverModel> GetListOfApprovers()
        {
            var p = new List<ApproverModel>();
            _db = new eRecruitmentDataClassesDataContext();


            var data = (from a in _db.AspNetUserRoles
                        join b in _db.lutOrganisations on a.OrganisationID equals b.OrganisationID
                        join c in _db.AspNetRoles on a.RoleId equals c.Id
                        join d in _db.tblProfiles on a.UserId equals d.UserID
                        join e in _db.AssignedDivisionDepartments on a.UserId equals e.UserId
                        where Convert.ToInt32(c.Id) == 2
                        orderby d.FirstName.Trim() ascending
                        select new
                        {
                            RoleName = c.Name,
                            ApproverUserIdUserId = d.UserID,
                            Fullname = d.FirstName + " " + d.Surname
                        }).Distinct().OrderBy(x => x.Fullname.Trim()).ToList();

            foreach (var d in data)
            {
                ApproverModel e = new ApproverModel();
                e.RoleName = Convert.ToString(d.RoleName);
                e.ApproverUserId = Convert.ToString(d.ApproverUserIdUserId);
                e.Fullname = Convert.ToString(d.Fullname);
                p.Add(e);
            }
            return p;
        }

        //Get List of Approvers Per Department

        public List<ApproverModel> GetApproverByDepartmentList(int id)
        {
            var p = new List<ApproverModel>();
            _db = new eRecruitmentDataClassesDataContext();


            var data = from a in _db.AspNetUserRoles
                       join b in _db.lutOrganisations on a.OrganisationID equals b.OrganisationID
                       join c in _db.AssignedDivisionDepartments on a.UserId equals c.UserId
                       join d in _db.tblProfiles on a.UserId equals d.UserID
                       join e in _db.AspNetRoles on a.RoleId equals e.Id
                       where Convert.ToInt32(c.DepartmentID) == id && Convert.ToInt32(e.Id) == 2
                       select new
                       {
                           //RoleName = c.Name,
                           ApproverUserIdUserId = d.UserID,
                           Fullname = d.FirstName + " " + d.Surname
                       };

            foreach (var d in data)
            {
                ApproverModel e = new ApproverModel();
                //e.RoleName = Convert.ToString(d.RoleName);
                e.ApproverUserId = Convert.ToString(d.ApproverUserIdUserId);
                e.Fullname = Convert.ToString(d.Fullname);
                p.Add(e);
            }
            return p;
        }



        //Get List of Recruiters

        public List<RecruiterModel> GetListOfRecruiters()
        {
            var p = new List<RecruiterModel>();
            _db = new eRecruitmentDataClassesDataContext();


            var data = (from a in _db.AspNetUserRoles
                        join b in _db.lutOrganisations on a.OrganisationID equals b.OrganisationID
                        join c in _db.AspNetRoles on a.RoleId equals c.Id
                        join d in _db.tblProfiles on a.UserId equals d.UserID
                        join e in _db.AssignedDivisionDepartments on a.UserId equals e.UserId
                        where Convert.ToInt32(c.Id) == 1
                        select new
                        {
                            RoleName = c.Name,
                            RecruiterUserId = d.UserID,
                            Fullname = d.FirstName + " " + d.Surname
                        }).Distinct().OrderBy(x => x.Fullname).ToList();

            foreach (var d in data)
            {
                RecruiterModel e = new RecruiterModel();
                e.RoleName = Convert.ToString(d.RoleName);
                e.RecruiterUserId = Convert.ToString(d.RecruiterUserId);
                e.Fullname = Convert.ToString(d.Fullname);
                p.Add(e);
            }
            return p;
        }

        //Get List of Recruiters Per Department

        public List<RecruiterModel> GetListOfRecruitersByDepartmentList(int id)
        {
            var p = new List<RecruiterModel>();
            _db = new eRecruitmentDataClassesDataContext();


            var data = from a in _db.AspNetUserRoles
                       join b in _db.lutOrganisations on a.OrganisationID equals b.OrganisationID
                       join c in _db.AssignedDivisionDepartments on a.UserId equals c.UserId
                       join d in _db.tblProfiles on a.UserId equals d.UserID
                       join e in _db.AspNetRoles on a.RoleId equals e.Id
                       where Convert.ToInt32(c.DepartmentID) == id && e.Name == "Recruiter"
                       select new
                       {
                           //RoleName = c.Name,
                           RecruiterUserId = d.UserID,
                           Fullname = d.FirstName + " " + d.Surname
                       };

            foreach (var d in data)
            {
                RecruiterModel e = new RecruiterModel();
                //e.RoleName = Convert.ToString(d.RoleName);
                e.RecruiterUserId = Convert.ToString(d.RecruiterUserId);
                e.Fullname = Convert.ToString(d.Fullname);
                p.Add(e);
            }
            return p;
        }

        public string GetKillerQuestionByVIDQBanks(int vacancyId, string questionBank)
        {
            DataTable dt = new DataTable();
            string fQuestions = string.Empty;

            using (SqlConnection sCon = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                using (SqlCommand oCommand = new SqlCommand("proc_eRecruitmentGetKillerQuestionByIDs", sCon))
                {
                    oCommand.CommandType = CommandType.StoredProcedure;
                    oCommand.Parameters.AddWithValue("@VacancyID", vacancyId);
                    oCommand.Parameters.AddWithValue("@QuestionBankID", questionBank);
                    using (SqlDataAdapter da = new SqlDataAdapter(oCommand))
                    {
                        da.Fill(dt);
                    }
                }
            }
            if (dt.Columns.Count > 0)
            {
                fQuestions = (String)dt.Rows[0][0]; //This will always return 1 column, one row
            }
            else
            {
                fQuestions = "No Vacancy Question Selected";
            }
            return fQuestions;
        }

        //================== Removed as per client request -Peter 20230324 ====================
        //public DataTable GeteRecruitmentScreenedCandidateList(string VacancyID, string ProvinceID, string GenderID, string RaceID,
        //                                            int Disability, int CVAttached, int IDAttached, int AgeRange,
        //                                            string questionBank, int professioonallyRegistered,
        //                                            int previouslyEmployedPS, int matricCompleted, int driversLicence)
        public DataTable GeteRecruitmentScreenedCandidateList(string VacancyID, string ProvinceID, string GenderID, string RaceID,
                                             int Disability, int CVAttached, int AgeRange,
                                             string questionBank, int professioonallyRegistered,
                                             int previouslyEmployedPS, int matricCompleted, int driversLicence)
        //========================================================================================
        {
            DataTable dt = new DataTable();
            using (SqlConnection sCon = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                using (SqlCommand oCommand = new SqlCommand("proc_eRecruitmentGetScreenedCandidateList", sCon))
                {
                    oCommand.CommandType = CommandType.StoredProcedure;
                    oCommand.Parameters.AddWithValue("@VacancyID", VacancyID);
                    oCommand.Parameters.AddWithValue("@ProvinceID", ProvinceID);
                    oCommand.Parameters.AddWithValue("@GenderID", GenderID);
                    oCommand.Parameters.AddWithValue("@RaceID", RaceID);
                    oCommand.Parameters.AddWithValue("@Disability", Disability);
                    oCommand.Parameters.AddWithValue("@CVAttached", CVAttached);
                    //================== Removed as per client request -Peter 20230324 ====================
                    //oCommand.Parameters.AddWithValue("@IDAttached", IDAttached);
                    oCommand.Parameters.AddWithValue("@IDAttached", 0);
                    //========================================================================================
                    oCommand.Parameters.AddWithValue("@AgeRange", AgeRange);
                    oCommand.Parameters.AddWithValue("@QuestionBankID", questionBank);
                    oCommand.Parameters.AddWithValue("@ProfessionallyRegisteredID", professioonallyRegistered);
                    oCommand.Parameters.AddWithValue("@PreviouslyEmployedPS", previouslyEmployedPS);
                    oCommand.Parameters.AddWithValue("@matricCompleted", @matricCompleted);
                    oCommand.Parameters.AddWithValue("@driversLicence", @driversLicence);

                    using (SqlDataAdapter da = new SqlDataAdapter(oCommand))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public void UpdateVacancyDocument(int VacancyID, string fileName, byte[] fileData, string contentType, string FileExtension)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentUpdateVacancyDocument(VacancyID, fileName, fileData, contentType, FileExtension);

        }

        public List<ScreenedCandidateModel> GetScreenedCandidateList(string VacancyID, string ProvinceID, string GenderID, string RaceID,
                                                                    int Disability, int @CVAttached, int IDAttached, int AgeRange,
                                                                    string questionBank, int professioonallyRegistered, int previouslyEmployedPS)
        {
            var p = new List<ScreenedCandidateModel>();
            _db = new eRecruitmentDataClassesDataContext();
            var data = _db.proc_eRecruitmentGetScreenedCandidateList(VacancyID, ProvinceID, GenderID, RaceID, Disability
                    , CVAttached, IDAttached, AgeRange, questionBank, professioonallyRegistered, previouslyEmployedPS).ToList();

            foreach (var d in data)
            {
                ScreenedCandidateModel e = new ScreenedCandidateModel();
                e.VacancyID = Convert.ToInt32(d.VacancyID);
                e.ApplicationID = Convert.ToInt32(d.ApplicationID);
                e.UserID = Convert.ToString(d.UserID);
                e.IDNumber = Convert.ToString(d.IDNumber);
                e.Surname = Convert.ToString(d.Surname);
                e.FirstName = Convert.ToString(d.FirstName);
                e.DateOfBirth = Convert.ToString(d.DateOfBirth);
                e.CellNo = Convert.ToString(d.CellNo);
                e.EmailAddress = Convert.ToString(d.EmailAddress);
                e.RaceName = Convert.ToString(d.RaceName);
                e.Gender = Convert.ToString(d.Gender);
                e.Disability = Convert.ToString(d.Disability);
                e.QualificationTypeName = Convert.ToString(d.QualificationTypeName);
                e.jobTitle = Convert.ToString(d.jobTitle);
                e.period = Convert.ToString(d.period);
                e.skillName = Convert.ToString(d.skillName);
                e.ProfessioonallyRegistered = Convert.ToString(d.ProfessionallyRegistered);
                e.PreviouslyEmployedPS = Convert.ToString(d.PreviouslyEmployedPS);
                p.Add(e);
            }
            return p;

        }

        // Get Get Candidate List For Export
        public List<CandidateListToExcelModel> GetCandidateListForExport(int id)
        {
            var p = new List<CandidateListToExcelModel>();
            _db = new eRecruitmentDataClassesDataContext();
            var data = _db.tblScreenedCandidates.Where(x => x.VacancyID == id).ToList();

            foreach (var d in data)
            {
                CandidateListToExcelModel e = new CandidateListToExcelModel();
                e.IDNumber = Convert.ToString(d.IDNumber);
                e.Surname = Convert.ToString(d.Surname);
                e.FirstName = Convert.ToString(d.FirstName);
                e.DateOfBirth = Convert.ToDateTime(d.DateOfBirth).ToString("d");
                e.CellNo = Convert.ToString(d.CellNo);
                e.EmailAddress = Convert.ToString(d.EmailAddress);
                e.RaceName = Convert.ToString(d.RaceName);
                e.Gender = Convert.ToString(d.Gender);
                e.Disability = Convert.ToString(d.Disability);
                e.QualificationTypeName = Convert.ToString(d.QualificationTypeName);
                e.jobTitle = Convert.ToString(d.jobTitle);
                e.period = Convert.ToString(d.period);
                e.skillName = Convert.ToString(d.skillName);
                e.ProfessioonallyRegistered = Convert.ToString(d.ProfessionallyRegistered);
                e.PreviouslyEmployedPS = Convert.ToString(d.PreviouslyEmployedPS);
                p.Add(e);
            }
            return p;
        }

        //Get Gender List
        public List<GenderModel> GetGenderList()
        {
            var p = new List<GenderModel>();
            _db = new eRecruitmentDataClassesDataContext();
            var Gender = _db.lutGenders.Where(x => x.GenderID != -1).OrderBy(x => x.Gender).ToList();

            foreach (var d in Gender)
            {
                GenderModel e = new GenderModel();
                e.GenderID = Convert.ToInt32(d.GenderID);
                e.Gender = Convert.ToString(d.Gender);
                p.Add(e);
            }
            return p;
        }

        //Get Race List
        public List<RaceModel> GetRaceList()
        {
            var p = new List<RaceModel>();
            _db = new eRecruitmentDataClassesDataContext();

            //============Peter 20221028============
            var Race = _db.lutRaces.Where(x => x.RaceID != -1).OrderBy(x => x.RaceName).ToList();
            //======================================
            //var Race = _db.lutRaces.Where(x => x.RaceID != -1).ToList();

            foreach (var d in Race)
            {
                RaceModel e = new RaceModel();
                e.RaceID = Convert.ToInt32(d.RaceID);
                e.RaceName = Convert.ToString(d.RaceName);
                p.Add(e);
            }
            return p;
        }


        //Get Vacancy For Candidate Screening
        public List<VacancyModels> GetVacancyListForScreening(string userid)
        {
            var p = new List<VacancyModels>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = from a in _db.tblVacancies
                       join b in _db.lutJobTitles on a.JobTitleID equals b.JobTitleID
                       where a.RecruiterUserId == userid
                       //============Peter 20221028============
                       orderby a.ReferenceNo.Trim() ascending
                       //======================================
                       select new
                       {
                           VacancyID = a.ID,
                           JobTitle = a.ReferenceNo + " " + "(" + b.JobTitle + ")"
                       };


            foreach (var d in data)
            {
                VacancyModels e = new VacancyModels();
                e.ID = Convert.ToInt32(d.VacancyID);
                e.JobTitle = Convert.ToString(d.JobTitle);
                p.Add(e);
            }
            return p;
        }

        //GetQuestionBanksPerVacancy
        public List<GeneralQuestionModel> GetQuestionBanksPerVacancy(int id)
        {
            var p = new List<GeneralQuestionModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = from a in _db.lutGeneralQuestions
                       join b in _db.tblVacancyQuestions on a.id equals b.QuestionID
                       where b.VacancyID == id
                       //Peter Added orderby on 20220908 to sort qualifications
                       orderby a.QCategoryID, b.id descending
                       select new
                       {
                           QuestionID = a.id,
                           QuestionDescr = a.GeneralQuestionDesc,

                       };

            foreach (var d in data)
            {
                GeneralQuestionModel e = new GeneralQuestionModel();
                e.id = Convert.ToInt32(d.QuestionID);
                e.GeneralQuestionDesc = " " + Convert.ToString(d.QuestionDescr);
                p.Add(e);
            }
            return p;

        }

        //GetDisclamer
        public List<DisclaimerModel> GetDisclamer(int orgID)
        {
            var p = new List<DisclaimerModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = from a in _db.lutDiscalmers.Where(x => x.OrginazationID == orgID).ToList()

                       select new
                       {
                           DisclaimerID = a.DisclamerID,
                           DisclaimerDescr = a.Disclamer,

                       };

            foreach (var d in data)
            {
                DisclaimerModel e = new DisclaimerModel();
                e.DisclaimerID = Convert.ToInt32(d.DisclaimerID);
                e.DisclaimerDescr = Convert.ToString(d.DisclaimerDescr);
                p.Add(e);
            }
            return p;

        }

        //Get Question Category List
        public List<QuestionCatergoryModel> GetQuestionCategoryList()
        {
            var p = new List<QuestionCatergoryModel>();
            _db = new eRecruitmentDataClassesDataContext();
            var QuestionCatergory = _db.lutQuestionCatergories.ToList();

            foreach (var d in QuestionCatergory)
            {
                QuestionCatergoryModel e = new QuestionCatergoryModel();
                e.QCategoryID = Convert.ToInt32(d.QCategoryID);
                e.QCategoryDescr = Convert.ToString(d.QCategoryDescr);
                p.Add(e);
            }
            return p;
        }

        //Check If Vacancy Exist
        public int CheckIfVacancyExists(int JobTitleID, int OrganisationID)
        {
            return _db.tblVacancies.Where(x => x.JobTitleID == JobTitleID
                            && x.OrganisationID == OrganisationID).Count();
        }

        //Check If Vacancy Profile Exist
        public int CheckIfVacancyProfileExists(int JobTitleID, int OrganisationID)
        {
            return _db.tblVacancies.Where(x => x.JobTitleID == JobTitleID
                            && x.OrganisationID == OrganisationID).Count();
        }

        //Get Publish Days List
        public List<PublishDaysModel> GetPublishDaysList()
        {
            var p = new List<PublishDaysModel>();
            _db = new eRecruitmentDataClassesDataContext();
            var QuestionCatergory = _db.lutPublishDays.ToList();

            foreach (var d in QuestionCatergory)
            {
                PublishDaysModel e = new PublishDaysModel();
                e.NumberOfDaysID = Convert.ToInt32(d.NumberOfDaysID);
                e.NumberOfDays = Convert.ToString(d.NumberOfDays);
                p.Add(e);
            }
            return p;
        }

        //DeleteJobProfile
        public void DeleteJobProfile(int JobProfileID)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentDeleteJobProfile(JobProfileID);
        }

        //Check If Vacancy Profile Exist
        public int CheckIfVacancyProfileExistsOnAdvert(int JobProfileID, int OrganisationID)
        {
            return _db.tblVacancies.Where(x => x.VacancyProfileID == JobProfileID
                            && x.OrganisationID == OrganisationID).Count();
        }

        //Get GetCandidateList
        public List<ScreenedCandidateModel> GetCandidateList(string userid)
        {

            var p = new List<ScreenedCandidateModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = (from a in _db.tblVacancies
                        join b in _db.lutJobTitles on a.JobTitleID equals b.JobTitleID
                        join c in _db.tblCandidateVacancyApplications on a.ID equals c.VacancyID

                        where a.RecruiterUserId == userid

                        select new
                        {
                            VacancyID = a.ID,
                            JobTitle = a.ReferenceNo + " " + "(" + b.JobTitle + ")",
                            ApplicationID = c.ApplicationID

                        }).FirstOrDefault();

            if (data != null)
            {
                ScreenedCandidateModel e = new ScreenedCandidateModel();

                {
                    e.VacancyID = Convert.ToInt32(data.VacancyID);
                    e.jobTitle = Convert.ToString(data.JobTitle);
                    e.ApplicationID = data.ApplicationID;

                }
                p.Add(e);
            }
            return p;
        }


        public List<ScreenedCandidateModel> GetCandidateListUsingVacancyID(int VacancyID)
        {
            var p = new List<ScreenedCandidateModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = from a in _db.tblCandidateVacancyApplications
                       join b in _db.tblProfiles on a.UserID equals b.UserID
                       join c in _db.lutRaces on b.fkRaceID equals c.RaceID
                       join d in _db.lutGenders on b.fkGenderID equals d.GenderID
                       where a.VacancyID == VacancyID

                       select new
                       {
                           IDNumber = b.IDNumber,
                           Surname = b.Surname,
                           FirstName = b.FirstName,
                           DateOfBirth = b.DateOfBirth,
                           CellNo = b.CellNo,
                           EmailAddress = b.EmailAddress,
                           Race = c.RaceName,
                           Gender = d.Gender,
                           ApplicationID = a.ApplicationID,
                           PassportNo = b.PassportNo,
                           UserID = b.UserID,
                           ProfileID = b.pkProfileID
                       };

            foreach (var d in data)
            {
                ScreenedCandidateModel e = new ScreenedCandidateModel();
                if (d.IDNumber != null)
                {
                    e.IDNumber = Convert.ToString(d.IDNumber);
                }
                else
                {
                    e.IDNumber = Convert.ToString(d.PassportNo);
                }

                e.Surname = Convert.ToString(d.Surname);
                e.FirstName = Convert.ToString(d.FirstName);
                e.DateOfBirth = Convert.ToDateTime(d.DateOfBirth).ToShortDateString();
                e.CellNo = Convert.ToString(d.CellNo);
                e.EmailAddress = Convert.ToString(d.EmailAddress);
                e.RaceName = Convert.ToString(d.Race);
                e.Gender = Convert.ToString(d.Gender);
                e.ApplicationID = Convert.ToInt32(d.ApplicationID);
                e.UserID = Convert.ToString(d.UserID);
                e.ProfileID = int.Parse(d.ProfileID.ToString());
                p.Add(e);
            }
            return p;
        }


        //Get GetCandidateProfileInfo
        public List<ProfileViewModel> GetCandidateProfileInfo(int ID)
        {
            var p = new List<ProfileViewModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = from a in _db.tblCandidateVacancyApplications
                       join b in _db.tblProfiles on a.UserID equals b.UserID
                       join c in _db.lutRaces on b.fkRaceID equals c.RaceID
                       join d in _db.lutGenders on b.fkGenderID equals d.GenderID
                       join e in _db.lutProvinces on b.fkProvinceID equals e.ProvinceID
                       join f in _db.lutDisabilities on b.NatureOfDisability equals f.DisabilityID
                       join g in _db.lutYesorNos on b.MatricID equals g.AnswerID
                       join h in _db.lutYesorNos on b.DriversLicenseID equals h.AnswerID
                       where a.ApplicationID == ID
                       select new
                       {
                           IDNumber = b.IDNumber,
                           b.PassportNo,
                           Surname = b.Surname,
                           FirstName = b.FirstName,
                           DateOfBirth = b.DateOfBirth,
                           fkRaceID = c.RaceID,
                           Race = c.RaceName,
                           fkGenderID = d.GenderID,
                           Gender = d.Gender,
                           CellNo = b.CellNo,
                           AlternativeNo = b.AlternativeNo,
                           EmailAddress = b.EmailAddress,
                           Matric = g.Answer,
                           DriversLicense = h.Answer,

                           UnitNo = b.UnitNo,
                           StreetNo = b.StreetNo,
                           StreetName = b.StreetName,
                           SuburbName = b.SuburbName,
                           fkProvinceID = b.fkProvinceID,
                           PostalCode = b.PostalCode,
                           fkDisabilityID = b.fkDisabilityID,

                           NatureOfDisability = b.NatureOfDisability,
                           DisabilityDesc = f.Disability,
                           OtherNatureOfDisability = b.OtherNatureOfDisability,
                           SACitizen = b.SACitizen,
                           fkNationalityID = b.fkNationalityID,
                           fkWorkPermitID = b.fkWorkPermitID,
                           WorkPermitNo = b.WorkPermitNo,
                           pkCriminalOffenseID = b.pkCriminalOffenseID,
                           fkLanguageForCorrespondenceID = b.fkLanguageForCorrespondenceID,
                           TelNoDuringWorkingHours = b.TelNoDuringWorkingHours,

                           MethodOfCommunicationID = b.MethodOfCommunicationID,
                           CorrespondanceDetails = b.CorrespondanceDetails,


                           ProfessionallyRegisteredID = b.ProfessionallyRegisteredID,
                           RegistrationDate = b.RegistrationDate,
                           RegistrationNumber = b.RegistrationNumber,
                           RegistrationBody = b.RegistrationBody,
                           PreviouslyEmployedPS = b.PreviouslyEmployedPS,
                           b.ConditionsThatPreventsReEmploymentID,
                           ReEmployment = b.ReEmployment,
                           PreviouslyEmployedDepartment = b.PreviouslyEmployedDepartment,
                           ProfileID = b.pkProfileID,
                           ApplicationID = a.ApplicationID

                       };

            foreach (var d in data)
            {
                ProfileViewModel e = new ProfileViewModel();
                e.IDNumber = Convert.ToString(d.IDNumber);
                e.PassportNumber = Convert.ToString(d.PassportNo);
                e.Surname = Convert.ToString(d.Surname);
                e.FirstName = Convert.ToString(d.FirstName);
                e.DateOfBirth = Convert.ToDateTime(d.DateOfBirth).ToShortDateString();
                e.Race = Convert.ToString(d.Race);
                e.Gender = Convert.ToString(d.Gender);
                e.CellNo = Convert.ToString(d.CellNo);
                e.AlternativeNo = Convert.ToString(d.AlternativeNo);
                e.EmailAddress = Convert.ToString(d.EmailAddress);
                e.Matric = Convert.ToString(d.Matric);
                e.DriversLicense = Convert.ToString(d.DriversLicense);
                e.UnitNo = Convert.ToString(d.UnitNo);
                e.StreetNo = Convert.ToString(d.StreetNo);
                e.StreetName = Convert.ToString(d.StreetName);
                e.SuburbName = Convert.ToString(d.SuburbName);
                e.fkProvinceID = Convert.ToInt32(d.fkProvinceID);
                e.PostalCode = Convert.ToString(d.PostalCode);
                e.fkDisabilityID = Convert.ToInt32(d.fkDisabilityID);
                e.SACitizen = Convert.ToInt32(d.SACitizen);
                e.NatureOfDisability = Convert.ToInt32(d.NatureOfDisability);
                e.OtherNatureOfDisability = Convert.ToString(d.OtherNatureOfDisability);
                e.Disability = Convert.ToString(d.DisabilityDesc);
                e.fkNationalityID = Convert.ToInt32(d.fkNationalityID);
                e.fkWorkPermitID = Convert.ToInt32(d.fkWorkPermitID);
                e.WorkPermitNo = Convert.ToString(d.WorkPermitNo);
                e.pkCriminalOffenseID = Convert.ToInt32(d.pkCriminalOffenseID);
                e.fkLanguageForCorrespondenceID = Convert.ToInt32(d.fkLanguageForCorrespondenceID);
                e.TelNoDuringWorkingHours = Convert.ToString(d.TelNoDuringWorkingHours);
                e.EmailAddress = Convert.ToString(d.EmailAddress);

                e.MethodOfCommunicationID = Convert.ToInt32(d.MethodOfCommunicationID);
                e.CorrespondanceDetails = Convert.ToString(d.CorrespondanceDetails);

                e.ProfessionallyRegisteredID = Convert.ToInt32(d.ProfessionallyRegisteredID);
                e.RegistrationDate = Convert.ToDateTime(d.RegistrationDate);
                e.RegistrationNumber = Convert.ToString(d.RegistrationNumber);
                e.RegistrationBody = Convert.ToString(d.RegistrationBody);
                e.PreviouslyEmployedPS = Convert.ToInt32(d.PreviouslyEmployedPS);
                e.ConditionsThatPreventsReEmploymentID = Convert.ToInt32(d.ConditionsThatPreventsReEmploymentID);
                e.ReEmployment = Convert.ToString(d.ReEmployment);
                e.PreviouslyEmployedDepartment = Convert.ToString(d.PreviouslyEmployedDepartment);
                e.pkProfileID =int.Parse( Convert.ToString(d.ProfileID));
                e.ApplicationID = d.ApplicationID;
                p.Add(e);
            }
            return p;
        }

        //Get Get Candidate Education List
        public List<EducationModel> GetEducationList(int ID)
        {
            var p = new List<EducationModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = from a in _db.tblCandidateVacancyApplications
                       join b in _db.tblProfiles on a.UserID equals b.UserID
                       join c in _db.tblCandidateEducations on b.pkProfileID equals c.ProfileID
                       join d in _db.lutQualificationTypes on c.QualificationTypeID equals d.QualificationTypeID

                       where a.ApplicationID == ID
                       select new
                       {
                           institutionName = c.InstitutionName,
                           qualificationName = c.QualificationName,
                           QualificationTypeName = d.QualificationTypeName,
                           CertificateNumber = c.CertificateNumber,
                           startDate = c.StartDate,
                           endDate = c.EndDate

                       };

            foreach (var d in data)
            {
                EducationModel e = new EducationModel();
                e.institutionName = Convert.ToString(d.institutionName);
                e.qualificationName = Convert.ToString(d.qualificationName);
                e.QualificationTypeName = Convert.ToString(d.QualificationTypeName);
                e.certificateNumber = Convert.ToString(d.CertificateNumber);
                e.startDate = Convert.ToDateTime(d.startDate).ToShortDateString();
                e.endDate = Convert.ToDateTime(d.endDate).ToShortDateString();
                p.Add(e);
            }
            return p;
        }

        //Get Get Candidate Work History List
        public List<WorkHistoryModel> GetWorkHistoryList(int ID)
        {
            var p = new List<WorkHistoryModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = from a in _db.tblCandidateVacancyApplications
                       join b in _db.tblProfiles on a.UserID equals b.UserID
                       join c in _db.tblWorkHistories on b.pkProfileID equals c.profileID

                       where a.ApplicationID == ID
                       select new
                       {
                           companyName = c.companyName,
                           jobTitle = c.jobTitle,
                           positionHeld = c.positionHeld,
                           department = c.department,
                           startDate = c.startDate,
                           endDate = c.endDate,
                           reasonForLeaving = c.reasonForLeaving,

                       };

            foreach (var d in data)
            {
                WorkHistoryModel e = new WorkHistoryModel();
                e.companyName = Convert.ToString(d.companyName);
                e.jobTitle = Convert.ToString(d.jobTitle);
                string text = Convert.ToString(d.positionHeld);

                string heldPosition = Convert.ToString(MvcHtmlString.Create(HttpUtility.HtmlEncode(text.Replace("•", ".").Replace("’", "'").Replace("“", "\"").Replace("”", "\"").Replace("–", "-"))));
                e.positionHeld = Convert.ToString(this.RemoveSpecialCharacters(heldPosition));
                e.department = Convert.ToString(d.department);
                e.startDate = Convert.ToDateTime(d.startDate).ToShortDateString();
                e.endDate = Convert.ToDateTime(d.endDate).ToShortDateString();
                e.reasonForLeaving = Convert.ToString(d.reasonForLeaving);
                p.Add(e);
            }
            return p;
        }

        //=============================Peter 202020414==========================
        public List<JobJobSpecificQuestionModel> GetJobSpecificQuestionsList(int ID)
        {
            var p = new List<JobJobSpecificQuestionModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = (from a in _db.lutJobSpecificQuestions
                        where a.VacancyID == ID
                        select new
                        {
                            JobSpecificeQuestionID = a.JobSpecificeQuestionID,
                            VacancyID = a.VacancyID,
                            jobSpecificQuestion = a.JobSpecificeQuestionDesc
                        });

            foreach (var d in data)
            {
                JobJobSpecificQuestionModel e = new JobJobSpecificQuestionModel();
                e.JobSpecificeQuestionID = d.JobSpecificeQuestionID;
                e.VacancyID = Convert.ToInt32(d.VacancyID);
                e.JobSpecificeQuestionDesc = Convert.ToString(d.jobSpecificQuestion);
                p.Add(e);
            }
            return p;
        }
        //=====================================================================

        //Get Get Candidate Skill List
        public List<CandidateSkillsProfileModel> GetCandidateSkillList(int ID)
        {
            var p = new List<CandidateSkillsProfileModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = from a in _db.tblCandidateVacancyApplications
                       join b in _db.tblProfiles on a.UserID equals b.UserID
                       join c in _db.tblSkillsProfiles on b.pkProfileID equals c.profileID
                       join d in _db.lutSkills on c.skillID equals d.skillID
                       join e in _db.lutSkill_Proficiencies on c.SkillProficiencyID equals e.SkillProficiencyID

                       where a.ApplicationID == ID
                       select new
                       {
                           skillName = d.skillName,
                           SkillProficiency = e.SkillProficiency

                       };

            foreach (var d in data)
            {
                CandidateSkillsProfileModel e = new CandidateSkillsProfileModel();
                e.skillName = Convert.ToString(d.skillName);
                e.SkillProficiency = Convert.ToString(d.SkillProficiency);
                p.Add(e);
            }
            return p;
        }

        //Get Get Candidate Language List
        public List<CandidateLanguageProfileModel> GetCandidateLanguageList(int ID)
        {
            var p = new List<CandidateLanguageProfileModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = from a in _db.tblCandidateVacancyApplications
                       join b in _db.tblProfiles on a.UserID equals b.UserID
                       join c in _db.tblProfileLangauages on b.pkProfileID equals c.profileID
                       join d in _db.lutLanguages on c.languageID equals d.languageID
                       join e in _db.lutLaguage_Proficiencies on c.LanguageProficiencyID equals e.LanguageProficiencyID

                       where a.ApplicationID == ID
                       select new
                       {
                           LanguageName = d.LanguageName,
                           LanguageProficiency = e.LanguageProficiency

                       };

            foreach (var d in data)
            {
                CandidateLanguageProfileModel e = new CandidateLanguageProfileModel();
                e.LanguageName = Convert.ToString(d.LanguageName);
                e.LanguageProficiency = Convert.ToString(d.LanguageProficiency);
                p.Add(e);
            }
            return p;
        }

        //Get Get Candidate Reference List
        public List<ReferenceModel> GetReferenceList(int ID)
        {
            var p = new List<ReferenceModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = from a in _db.tblCandidateVacancyApplications
                       join b in _db.tblProfiles on a.UserID equals b.UserID
                       join c in _db.tblReferences on b.pkProfileID equals c.ProfileID

                       where a.ApplicationID == ID
                       select new
                       {
                           RefName = c.RefName,
                           CompanyName = c.CompanyName,
                           PositionHeld = c.PositionHeld,
                           TelNo = c.TelNo,
                           EmailAddress = c.EmailAddress

                       };

            foreach (var d in data)
            {
                ReferenceModel e = new ReferenceModel();
                e.refName = Convert.ToString(d.RefName);
                e.companyName = Convert.ToString(d.CompanyName);
                e.positionHeld = Convert.ToString(d.PositionHeld);
                e.telNo = Convert.ToString(d.TelNo);
                e.emailAddress = Convert.ToString(d.EmailAddress);
                p.Add(e);
            }
            return p;
        }

        //Get Get Candidate Attachment List
        public List<AttachmentModel> GetCandidateAttachmentList(int ID)
        {
            var p = new List<AttachmentModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = from a in _db.tblCandidateVacancyApplications
                       join b in _db.tblProfiles on a.UserID equals b.UserID
                       join c in _db.Attachments on b.pkProfileID equals c.ProfileID

                       where a.ApplicationID == ID
                       select new
                       {
                           fileName = c.fileName,
                           attachmentID = c.AttachmentID


                       };

            foreach (var d in data)
            {
                AttachmentModel e = new AttachmentModel();
                e.fileName = Convert.ToString(d.fileName);
                e.attachmentID = Convert.ToInt32(d.attachmentID);
                p.Add(e);
            }
            return p;
        }

        // Get Skills For User Profile
        public List<SkillsModel> SkillsProf(string userid)
        {
            var p = new List<SkillsModel>();
            var skillsData = from a in _db.tblSkillsProfiles
                             join b in _db.lutSkills on a.skillID equals b.skillID
                             join c in _db.tblProfiles on a.profileID equals c.pkProfileID
                             where c.UserID == userid
                             select new
                             {
                                 SkillName = b.skillName
                             };
            foreach (var d in skillsData)
            {
                SkillsModel e = new SkillsModel();
                e.skillName = Convert.ToString(d.SkillName);

                p.Add(e);
            }
            return p;
        }

        //Get Yes or No List
        public List<YesorNoModel> GetYesorNoList()
        {
            var p = new List<YesorNoModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var YesorNo = _db.lutYesorNos.ToList();

            foreach (var d in YesorNo)
            {
                YesorNoModel e = new YesorNoModel();
                e.AnswerID = Convert.ToInt32(d.AnswerID);
                e.Answer = Convert.ToString(d.Answer);
                p.Add(e);
            }
            return p;
        }

        //Get Country List
        public List<CountryModel> GetCountryList()
        {
            var p = new List<CountryModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var Country = _db.lutCountries.ToList();

            foreach (var d in Country)
            {
                CountryModel e = new CountryModel();
                e.CountryID = Convert.ToInt32(d.CountryID);
                e.CountryName = Convert.ToString(d.CountryName);
                p.Add(e);
            }
            return p;

        }

        //Get Language List
        public List<LanguageModel> GetLanguageList()
        {
            var p = new List<LanguageModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var Language = _db.lutLanguages.ToList();

            foreach (var d in Language)
            {
                LanguageModel e = new LanguageModel();
                e.LanguageID = Convert.ToInt32(d.languageID);
                e.LanguageName = Convert.ToString(d.LanguageName);
                p.Add(e);
            }
            return p;
        }

        //Get Language Proficiency List
        public List<LaguageProficiencyModel> GetLanguageProficiencyList()
        {
            var p = new List<LaguageProficiencyModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var LanguageProficiency = _db.lutLaguage_Proficiencies.ToList();

            foreach (var d in LanguageProficiency)
            {
                LaguageProficiencyModel e = new LaguageProficiencyModel();
                e.LanguageProficiencyID = Convert.ToInt32(d.LanguageProficiencyID);
                e.LanguageProficiency = Convert.ToString(d.LanguageProficiency);
                p.Add(e);
            }
            return p;
        }

        public List<MethodOfCummunicationModel> GetMethodOfCommunication()
        {
            var p = new List<MethodOfCummunicationModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = _db.lutMethodOfCommunications.ToList();

            foreach (var d in data)
            {
                MethodOfCummunicationModel e = new MethodOfCummunicationModel();
                e.MethodID = Convert.ToInt32(d.MethodID);
                e.MethodName = Convert.ToString(d.MethodOfCommunication);
                p.Add(e);
            }
            return p;
        }

        public int CheckIfVacancyExists(int DivisionID, int DepartmentID, int JobTitleID, int OrganisationID)
        {
            return _db.tblVacancies.Where(x => x.DivisionID == DivisionID
                            && x.DepartmentID == DepartmentID
                            && x.JobTitleID == JobTitleID
                            && x.OrganisationID == OrganisationID).Count();
        }

        public int CheckIfVacancyNumberExists(string BPSVacancyNo)
        {
            return _db.tblVacancies.Where(x => x.BPSVacancyNo == BPSVacancyNo).Count();
        }
        //public String ValidateBPSNumberOnBPSTracking(string BPSVacancyNo)
        //{
            

        //    String[] BPSNumber = BPSVacancyNo.Split(',');
        //    ArrayList notValid = new ArrayList();

           
           
        //    for (int x = 0; x < BPSNumber.Length; x++)
        //    {
        //        HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["BPSTrackerAPIURL"] + BPSNumber[x]);
        //        myRequest.Timeout = 5000;
        //        ServicePointManager.DefaultConnectionLimit = 500;
        //        HttpWebResponse response = (HttpWebResponse)myRequest.GetResponse();
        //        if (response.StatusCode == HttpStatusCode.OK)
        //        {
        //            Stream dataStream = response.GetResponseStream();
        //            // Open the stream using a StreamReader for easy access.
        //            StreamReader reader = new StreamReader(dataStream);
        //            // Read the content.
        //            string responseFromServer = reader.ReadToEnd();

        //            dynamic data = JsonConvert.DeserializeObject(responseFromServer);


        //            if (data.data == null)
        //            {
        //                notValid.Add(BPSNumber[x]);

        //            }
        //            // Display the content.

                    
        //            response.Close();
        //            //return true;
        //        }


               


        //    }

        //    StringBuilder builder = new StringBuilder();

          

        //    if (notValid != null)
        //    {

        //        foreach (String str in notValid)
        //        {
        //            builder.Append(str);

        //        }
        //    }

        //    return builder.ToString();
        //}

        public List<SalaryTypeModel> GetSalaryTypeList()
        {
            var p = new List<SalaryTypeModel>();
            _db = new eRecruitmentDataClassesDataContext();

            //============Peter 20221028============
            var data = _db.lutSalaryTypes.Where(y => y.SalaryTypeDescr == "Salary").OrderBy(x => x.SalaryTypeDescr).ToList();
            //======================================
            //var data = _db.lutSalaryTypes.ToList();

            foreach (var d in data)
            {
                SalaryTypeModel e = new SalaryTypeModel();
                e.SalaryTypeID = Convert.ToInt32(d.SalaryTypeID);
                e.SalaryTypeDescr = Convert.ToString(d.SalaryTypeDescr);
                p.Add(e);
            }

            return p;
        }

        public int CheckIfJobLevelExistsInSalaryStructure(int jobLevelID)
        {
            return _db.lutSalaryStructures.Where(x => x.JobLevelID == jobLevelID).Count();
        }

        public int CheckIfJobTitleExistsInSalaryStructure(int JobTitleID)
        {
            return _db.lutSalaryStructures.Where(x => x.JobTitleID == JobTitleID).Count();
        }

        public int CheckIfSalaryCategoryExistsInSalarySubCategory(int SalaryCategoryID)
        {
            return _db.lutSalarySubCategories.Where(x => x.SalaryCategoryID == SalaryCategoryID).Count();
        }

        public int CheckIfSalarySubCategoryExistsInSalaryStructure(int SalarySubCategoryID)
        {
            return _db.lutSalaryStructures.Where(x => x.SalarySubCategoryID == SalarySubCategoryID).Count();
        }

        public int CheckIfSalaryStructureExistsInJobProfile(int SalaryStructureID)
        {
            var info = (from a in _db.lutSalaryStructures

                        where a.SalaryStructureID == SalaryStructureID
                        select new
                        {
                            a.JobTitleID

                        }).FirstOrDefault();

            return _db.tblJobProfiles.Where(x => x.JobTitleID == info.JobTitleID).Count();
        }

        public int CheckIfDivisionExistsInDepartment(int id)
        {
            return _db.lutDepartments.Where(x => x.DivisionID == id).Count();
        }

        public int CheckIfDepartmentExistsInVacancy(int id)
        {
            return _db.tblVacancies.Where(x => x.DepartmentID == id).Count();
        }

        //InsertUpdateVacancyBPSNumber
        public void InsertUpdateVacancyBPSNumber(int vacancyid, string BPSNumber)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentInsertUpdateVacancyBPSNumber(vacancyid, BPSNumber);

        }

        //Get Disability List
        public List<DisclamerModel> GetDisclamerList(int UserOrganizationID)
        {
            var p = new List<DisclamerModel>();
            _db = new eRecruitmentDataClassesDataContext();
            var Disclamer = _db.lutDiscalmers.Where(x => x.OrginazationID == UserOrganizationID);

            foreach (var d in Disclamer)
            {
                DisclamerModel e = new DisclamerModel();
                e.DisclamerID = Convert.ToInt32(d.DisclamerID);
                e.OrginazationID = Convert.ToInt32(d.OrginazationID);
                e.Disclamer = Convert.ToString(d.Disclamer);
                p.Add(e);
            }
            return p;
        }

        public int CheckIfDisclamerExists(int OrginazationID)
        {
            return _db.lutDiscalmers.Where(x => x.OrginazationID == OrginazationID).Count();
        }

        public List<DisclamerModel> GetDisclaimerForEdit(int id)
        {
            var p = new List<DisclamerModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = _db.lutDiscalmers.Where(x => x.DisclamerID == id).ToList();

            foreach (var d in data)
            {
                DisclamerModel e = new DisclamerModel();
                e.DisclamerID = Convert.ToInt32(d.DisclamerID);
                e.OrginazationID = Convert.ToInt32(d.OrginazationID);
                e.Disclamer = Convert.ToString(d.Disclamer);
                p.Add(e);
            }
            return p;
        }

        public void InsertIntoDisclaimer(int OrginazationID, string Disclamer)
        {
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                _db.proc_eRecruitmentAddDisclamer(OrginazationID, Disclamer);
            }
        }

        public void UpdateIntoDisclamer(int id, int OrginazationID, string Disclamer)
        {
            _db = new eRecruitmentDataClassesDataContext();

            _db.proc_eRecruitmentUpdateDisclamer(id, OrginazationID, Disclamer);

        }

        public void DeleteIntoDisclaimer(int id)
        {
            _db = new eRecruitmentDataClassesDataContext();
            _db.proc_eRecruitmentDeleteDisclamer(id);
        }

        public VacancyModels GetVacancyRef(int VacancyID)
        {
            var p = new VacancyModels();
            _db = new eRecruitmentDataClassesDataContext();

            var data = from a in _db.tblVacancies
                       where a.ID == VacancyID
                       select new
                       {
                           ReferenceNo = a.ReferenceNo,

                       };
            VacancyModels e = new VacancyModels();
            foreach (var d in data)
            {
                e.ReferenceNo = Convert.ToString(d.ReferenceNo);
            }
            return e;
        }

        public List<DefinitionModel> GetDefinitions(int jobprofileid)
        {
            var p = new List<DefinitionModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var Definitions = _db.sp_Competency_Get_PerJobProfile(jobprofileid);

            foreach (var d in Definitions)
            {
                DefinitionModel e = new DefinitionModel();
                e.Type = Convert.ToString(d.Type);
                e.Name = Convert.ToString(d.Name);
                e.Definition = Convert.ToString(d.Definition);
                p.Add(e);
            }
            return p;
        }

        public List<BehaviouralCompModel> GetBehaveCompList(int orgid)
        {
            var p = new List<BehaviouralCompModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = from a in _db.BehaviouralCompetencies
                       join b in _db.lutOrganisations on a.OrganisationID equals b.OrganisationID
                       where b.OrganisationID == orgid
                       select new
                       {
                           a.BehaviouralCompetencyID,
                           b.OrganisationID,
                           b.OrganisationName,
                           a.BehaviouralComp,
                           a.BehaviouralCompDesc
                       };

            foreach (var d in data)
            {
                BehaviouralCompModel e = new BehaviouralCompModel();
                e.BehaviouralCompetencyID = Convert.ToInt32(d.BehaviouralCompetencyID);
                e.OrganisationID = Convert.ToInt32(d.OrganisationID);
                e.OrganisationName = Convert.ToString(d.OrganisationName);
                e.BehaviouralComp = Convert.ToString(d.BehaviouralComp);
                e.BehaviouralCompDesc = Convert.ToString(d.BehaviouralCompDesc);
                p.Add(e);
            }
            return p;
        }

        public List<BehaviouralCompModel> GetBehaveCompPerOrgID(int OrganisationID)
        {
            var p = new List<BehaviouralCompModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var BehaveComps = _db.BehaviouralCompetencies.OrderBy(x => x.BehaviouralComp).Where(x => x.OrganisationID == OrganisationID).ToList();

            foreach (var d in BehaveComps)
            {
                BehaviouralCompModel e = new BehaviouralCompModel();
                e.BehaviouralCompetencyID = Convert.ToInt32(d.BehaviouralCompetencyID);
                e.BehaviouralComp = Convert.ToString(d.BehaviouralComp);
                e.BehaviouralCompDesc = Convert.ToString(d.BehaviouralCompDesc);
                p.Add(e);
            }
            return p;
        }

        public int CheckIfBehaveCompExists(string BehaviouralComp, int OrganisationID)
        {
            int count = 0;
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                count = _db.BehaviouralCompetencies.Where(x => x.BehaviouralComp == BehaviouralComp
                                                      && x.OrganisationID == OrganisationID).Count();
            }
            return count;
        }

        public void InsertIntoBehaveComp(string BehaviouralComp, string BehaviouralCompDesc, int OrganisationID, Guid userid)
        {
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                _db.sp_BehaviouralCompetency_Post(OrganisationID, BehaviouralComp, BehaviouralCompDesc, userid);
            }
        }

        public BehaviouralCompModel GetBehaveCompForEdit(int id)
        {
            var p = new BehaviouralCompModel();
            _db = new eRecruitmentDataClassesDataContext();

            var data = _db.BehaviouralCompetencies.Where(x => x.BehaviouralCompetencyID == id).FirstOrDefault();

            p.BehaviouralCompetencyID = Convert.ToInt16(data.BehaviouralCompetencyID);
            p.OrganisationID = Convert.ToInt32(data.OrganisationID);
            p.BehaviouralComp = Convert.ToString(data.BehaviouralComp);
            p.BehaviouralCompDesc = Convert.ToString(data.BehaviouralCompDesc);

            return p;
        }

        public List<BehaviouralCompModel> GetBehaveCompPerIDs(string ids)
        {
            var p = new List<BehaviouralCompModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var BehaveComps = _db.sp_BehaviouralCompetency_Get_PerIDs(ids);

            foreach (var d in BehaveComps)
            {
                BehaviouralCompModel e = new BehaviouralCompModel();
                e.BehaviouralCompetencyID = Convert.ToInt32(d.BehaviouralCompetencyID);
                e.BehaviouralComp = Convert.ToString(d.BehaviouralComp);
                e.BehaviouralCompDesc = Convert.ToString(d.BehaviouralCompDesc);
                p.Add(e);
            }
            return p;
        }

        public void UpdateIntoBehaveComp(int id, int OrganisationID, string BehaviouralComp, string BehaviouralCompDesc, Guid userid)
        {
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                _db.sp_BehaviouralCompetency_Put(id, OrganisationID, BehaviouralComp, BehaviouralCompDesc, userid);
            }
        }

        public int CheckIfBehaveCompExistsInJobProfile(int BehaveCompID)
        {
            int count = 0;
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                count = _db.JobProfileBehaveComps.Where(x => x.BehaviouralCompetencyID == BehaveCompID).Count();
            }
            return count;
        }

        public void DeleteIntoBehaveComp(int id, Guid userid)
        {
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                _db.sp_BehaviouralCompetency_Delete(id, userid);
            }
        }

        public List<LeadershipCompModel> GetLeadCompList(int orgid)
        {
            var p = new List<LeadershipCompModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = from a in _db.LeadershipCompetencies
                       join b in _db.lutOrganisations on a.OrganisationID equals b.OrganisationID
                       where b.OrganisationID == orgid
                       select new
                       {
                           a.LeadershipCompetencyID,
                           b.OrganisationID,
                           b.OrganisationName,
                           a.LeadershipComp,
                           a.LeadershipCompDesc
                       };

            foreach (var d in data)
            {
                LeadershipCompModel e = new LeadershipCompModel();
                e.LeadershipCompetencyID = Convert.ToInt32(d.LeadershipCompetencyID);
                e.OrganisationID = Convert.ToInt32(d.OrganisationID);
                e.OrganisationName = Convert.ToString(d.OrganisationName);
                e.LeadershipComp = Convert.ToString(d.LeadershipComp);
                e.LeadershipCompDesc = Convert.ToString(d.LeadershipCompDesc);
                p.Add(e);
            }
            return p;
        }

        public List<LeadershipCompModel> GetLeadCompPerOrgID(int OrganisationID)
        {
            var p = new List<LeadershipCompModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var LeadComps = _db.LeadershipCompetencies.OrderBy(x => x.LeadershipComp).Where(x => x.OrganisationID == OrganisationID).ToList();

            foreach (var d in LeadComps)
            {
                LeadershipCompModel e = new LeadershipCompModel();
                e.LeadershipCompetencyID = Convert.ToInt32(d.LeadershipCompetencyID);
                e.LeadershipComp = Convert.ToString(d.LeadershipComp);
                e.LeadershipCompDesc = Convert.ToString(d.LeadershipCompDesc);
                p.Add(e);
            }
            return p;
        }

        public int CheckIfLeadCompExists(string LeadershipComp, int OrganisationID)
        {
            int count = 0;
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                count = _db.LeadershipCompetencies.Where(x => x.LeadershipComp == LeadershipComp
                                                      && x.OrganisationID == OrganisationID).Count();
            }
            return count;
        }

        public void InsertIntoLeadComp(string LeadershipComp, string LeadershipCompDesc, int OrganisationID, Guid userid)
        {
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                _db.sp_LeadershipCompetency_Post(OrganisationID, LeadershipComp, LeadershipCompDesc, userid);
            }
        }

        public LeadershipCompModel GetLeadCompForEdit(int id)
        {
            var p = new LeadershipCompModel();
            _db = new eRecruitmentDataClassesDataContext();

            var data = _db.LeadershipCompetencies.Where(x => x.LeadershipCompetencyID == id).FirstOrDefault();

            p.LeadershipCompetencyID = Convert.ToInt32(data.LeadershipCompetencyID);
            p.OrganisationID = Convert.ToInt32(data.OrganisationID);
            p.LeadershipComp = Convert.ToString(data.LeadershipComp);
            p.LeadershipCompDesc = Convert.ToString(data.LeadershipCompDesc);

            return p;
        }

        public List<LeadershipCompModel> GetLeadCompPerIDs(string ids)
        {
            var p = new List<LeadershipCompModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var LeadComps = _db.sp_LeadershipCompetency_Get_PerIDs(ids);

            foreach (var d in LeadComps)
            {
                LeadershipCompModel e = new LeadershipCompModel();
                e.LeadershipCompetencyID = Convert.ToInt32(d.LeadershipCompetencyID);
                e.LeadershipComp = Convert.ToString(d.LeadershipComp);
                e.LeadershipCompDesc = Convert.ToString(d.LeadershipCompDesc);
                p.Add(e);
            }
            return p;
        }

        public void UpdateIntoLeadComp(int id, int OrganisationID, string LeadershipComp, string LeadershipCompDesc, Guid userid)
        {
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                _db.sp_LeadershipCompetency_Put(id, OrganisationID, LeadershipComp, LeadershipCompDesc, userid);
            }
        }

        public int CheckIfLeadCompExistsInJobProfile(int LeadCompID)
        {
            int count = 0;
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                count = _db.JobProfileLeadComps.Where(x => x.LeadershipCompetencyID == LeadCompID).Count();
            }
            return count;
        }

        public void DeleteIntoLeadComp(int id, Guid userid)
        {
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                _db.sp_LeadershipCompetency_Delete(id, userid);
            }
        }

        public List<TechnicalCompModel> GetTechCompList(int orgid)
        {
            var p = new List<TechnicalCompModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var data = from a in _db.TechnicalCompetencies
                       join b in _db.lutOrganisations on a.OrganisationID equals b.OrganisationID
                       where b.OrganisationID == orgid
                       select new
                       {
                           a.TechnicalCompetencyID,
                           b.OrganisationID,
                           b.OrganisationName,
                           a.TechnicalComp,
                           a.TechnicalCompDesc
                       };

            foreach (var d in data)
            {
                TechnicalCompModel e = new TechnicalCompModel();
                e.TechnicalCompetencyID = Convert.ToInt32(d.TechnicalCompetencyID);
                e.OrganisationID = Convert.ToInt32(d.OrganisationID);
                e.OrganisationName = Convert.ToString(d.OrganisationName);
                e.TechnicalComp = Convert.ToString(d.TechnicalComp);
                e.TechnicalCompDesc = Convert.ToString(d.TechnicalCompDesc);
                p.Add(e);
            }
            return p;
        }

        public List<TechnicalCompModel> GetTechCompPerOrgID(int OrganisationID)
        {
            var p = new List<TechnicalCompModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var TechComps = _db.TechnicalCompetencies.OrderBy(x => x.TechnicalComp).Where(x => x.OrganisationID == OrganisationID).ToList();

            foreach (var d in TechComps)
            {
                TechnicalCompModel e = new TechnicalCompModel();
                e.TechnicalCompetencyID = Convert.ToInt32(d.TechnicalCompetencyID);
                e.TechnicalComp = Convert.ToString(d.TechnicalComp);
                e.TechnicalCompDesc = Convert.ToString(d.TechnicalCompDesc);
                p.Add(e);
            }
            return p;
        }

        public int CheckIfTechCompExists(string TechnicalComp, int OrganisationID)
        {
            int count = 0;
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                count = _db.TechnicalCompetencies.Where(x => x.TechnicalComp == TechnicalComp
                                                      && x.OrganisationID == OrganisationID).Count();
            }
            return count;
        }

        public void InsertIntoTechComp(string TechnicalComp, string TechnicalCompDesc, int OrganisationID, Guid userid)
        {
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                _db.sp_TechnicalCompetency_Post(OrganisationID, TechnicalComp, TechnicalCompDesc, userid);
            }
        }

        public TechnicalCompModel GetTechCompForEdit(int id)
        {
            var p = new TechnicalCompModel();
            _db = new eRecruitmentDataClassesDataContext();

            var data = _db.TechnicalCompetencies.Where(x => x.TechnicalCompetencyID == id).FirstOrDefault();

            p.TechnicalCompetencyID = Convert.ToInt32(data.TechnicalCompetencyID);
            p.OrganisationID = Convert.ToInt32(data.OrganisationID);
            p.TechnicalComp = Convert.ToString(data.TechnicalComp);
            p.TechnicalCompDesc = Convert.ToString(data.TechnicalCompDesc);

            return p;
        }

        public List<TechnicalCompModel> GetTechCompPerIDs(string ids)
        {
            var p = new List<TechnicalCompModel>();
            _db = new eRecruitmentDataClassesDataContext();

            var TechComps = _db.sp_TechnicalCompetency_Get_PerIDs(ids);

            foreach (var d in TechComps)
            {
                TechnicalCompModel e = new TechnicalCompModel();
                e.TechnicalCompetencyID = Convert.ToInt32(d.TechnicalCompetencyID);
                e.TechnicalComp = Convert.ToString(d.TechnicalComp);
                e.TechnicalCompDesc = Convert.ToString(d.TechnicalCompDesc);
                p.Add(e);
            }
            return p;
        }

        public void UpdateIntoTechComp(int id, int OrganisationID, string TechnicalComp, string TechnicalCompDesc, Guid userid)
        {
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                _db.sp_TechnicalCompetency_Put(id, OrganisationID, TechnicalComp, TechnicalCompDesc, userid);
            }
        }

        public int CheckIfTechCompExistsInJobProfile(int TechCompID)
        {
            int count = 0;
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                count = _db.JobProfileTechComps.Where(x => x.TechnicalCompetencyID == TechCompID).Count();
            }
            return count;
        }

        public void DeleteIntoTechComp(int id, Guid userid)
        {
            using (eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext())
            {
                _db.sp_TechnicalCompetency_Delete(id, userid);
            }
        }

        public List<TechnicalCompModel> GetTechnicalCompList(int VacancyProfileID)
        {
            _db = new eRecruitmentDataClassesDataContext();

            var data = (from jp in _db.tblJobProfiles
                        join jptc in _db.JobProfileTechComps on jp.JobProfileID equals jptc.JobProfileID
                        join tc in _db.TechnicalCompetencies on jptc.TechnicalCompetencyID equals tc.TechnicalCompetencyID
                        orderby tc.TechnicalComp
                        where jp.JobProfileID == VacancyProfileID
                        select new TechnicalCompModel()
                        {
                            TechnicalCompetencyID = tc.TechnicalCompetencyID,
                            TechnicalComp = tc.TechnicalComp,
                            TechnicalCompDesc = tc.TechnicalCompDesc
                        }).ToList();

            return data;
        }

        public List<LeadershipCompModel> GetLeadershipCompList(int VacancyProfileID)
        {
            _db = new eRecruitmentDataClassesDataContext();

            var data = (from jp in _db.tblJobProfiles
                        join jplc in _db.JobProfileLeadComps on jp.JobProfileID equals jplc.JobProfileID
                        join lc in _db.LeadershipCompetencies on jplc.LeadershipCompetencyID equals lc.LeadershipCompetencyID
                        orderby lc.LeadershipComp
                        where jp.JobProfileID == VacancyProfileID
                        select new LeadershipCompModel()
                        {
                            LeadershipCompetencyID = lc.LeadershipCompetencyID,
                            LeadershipComp = lc.LeadershipComp,
                            LeadershipCompDesc = lc.LeadershipCompDesc
                        }).ToList();

            return data;
        }

        public List<BehaviouralCompModel> GetBehaviouralCompList(int VacancyProfileID)
        {
            _db = new eRecruitmentDataClassesDataContext();

            var data = (from jp in _db.tblJobProfiles
                        join jpbc in _db.JobProfileBehaveComps on jp.JobProfileID equals jpbc.JobProfileID
                        join bc in _db.BehaviouralCompetencies on jpbc.BehaviouralCompetencyID equals bc.BehaviouralCompetencyID
                        orderby bc.BehaviouralComp
                        where jp.JobProfileID == VacancyProfileID
                        select new BehaviouralCompModel()
                        {
                            BehaviouralCompetencyID = bc.BehaviouralCompetencyID,
                            BehaviouralComp = bc.BehaviouralComp,
                            BehaviouralCompDesc = bc.BehaviouralCompDesc
                        }).ToList();

            return data;
        }

        private string RemoveSpecialCharacters(string value)
        {
            //var data = _db.SpecialCharacters.ToList();
            //foreach (var d in data)
            //{
            //    value = Convert.ToString(MvcHtmlString.Create(HttpUtility.HtmlEncode(Regex.Replace(value, d.SC_Unicode, d.SC_UnicodeReplacementValue, RegexOptions.IgnoreCase))));
            //}
            //return value;
            //return Regex.Replace(value, @"[^0-9A-Za-z ,]", " ").Replace("&#61623", ".").Replace("  61623", ".").Replace("&#61553", ".").Replace("  61553", ".").Replace(" ", " ").Trim();
            //return Regex.Replace(value, @"[^0-9A-Za-z ,]", ".").Replace("\u0095", ".").Replace("\u0092", "'").Trim();
            return value.Replace("\u0095", ".").Replace("\u0092", "'").Replace("\u0096", "-").Replace("•", ".").Replace("amp;amp;amp;", "").Replace("&#39;", "'").Replace("&amp;amp;amp;", "&").Replace("‘", "'").Replace("’", "'").Replace("#39", "'").Replace("&quot;", "\"").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&").Trim();
        }

        public void saveVacancySalary(tblVacancySalary salary)
        {
            _db.tblVacancySalaries.InsertOnSubmit(salary);

            _db.SubmitChanges();
        }

        public void updateVacancySalary(tblVacancySalary vacancySalary)
        {

            tblVacancySalary vacancySalaryTable = _db.tblVacancySalaries.Where(x => x.VacancyID == vacancySalary.VacancyID).FirstOrDefault();
            vacancySalaryTable.JobTitleID = vacancySalary.JobTitleID;
            vacancySalaryTable.JobLevel = vacancySalary.JobLevel;
            vacancySalaryTable.MinValue = vacancySalary.MinValue;
            vacancySalaryTable.MaxValue = vacancySalary.MaxValue;
            vacancySalaryTable.VacancyID = vacancySalary.VacancyID;


            _db.SubmitChanges();
        
        }

        public VacancySalary GetVacancySalary(int VacancyID)
        {
           return (from vS in _db.tblVacancySalaries
                           
                           where vS.VacancyID == VacancyID
                           select new VacancySalary()
                           {
                               SalaryID = vS.SalaryID,
                               JobTitleID = int.Parse(vS.JobTitleID.ToString()),
                               JobLevel =vS.JobLevel,
                               MinValue = (decimal)vS.MinValue,
                               MaxValue = (decimal)vS.MaxValue,
                               VacancyID= int.Parse(vS.VacancyID.ToString())
                               
                           }).FirstOrDefault();


            
        }

    }
}

