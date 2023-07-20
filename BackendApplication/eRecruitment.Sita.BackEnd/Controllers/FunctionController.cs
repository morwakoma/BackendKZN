using eRecruitment.Sita.BackEnd.App_Data.Entities.DAL;
using eRecruitment.Sita.BackEnd.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eRecruitment.Sita.BackEnd.Controllers
{
    public class FunctionController : Controller
    {
        eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext();

        // GET: Function
        public ActionResult Index()
        {
            ViewBag.Roles = _db.AspNetRoles.ToList();
            return View();
        }

        public ActionResult SystemRoles()
        {
            ViewBag.Roles = _db.AspNetRoles.Where(x=>x.Name != "Citizen").ToList();
            return View();
        }

        public ActionResult RoleMenuItems(string RoleID)
        {// get menuitem based on selected roles
            var Manager = User.IsInRole("System Admin");
            var p = new List<RoleMenuItemCheckboxModel>();
            if (RoleID == null)
            {
                RoleID = "";
            }
            ViewBag.RoleList = _db.AspNetRoles.Where(x => x.Name != "Citizen").OrderBy(a=> a.Name).ToList();

            var data = _db.sp_GetMenuItemsByRoleID(RoleID).ToList();

            foreach (var d in data)
            {
                //List<RoleMenuItemCheckboxModel> 
                RoleMenuItemCheckboxModel e = new RoleMenuItemCheckboxModel
                {
                    MenuItemID = Convert.ToInt32(d.MenuItemID),
                    Name = Convert.ToString(d.name),
                    IsChecked = Convert.ToBoolean(d.IsChecked),
                    ParentID = Convert.ToInt32(d.ParentID)
                };
                p.Add(e);
            }
            //var Menulist = _db.sp_GetMenuItemsByRoleID(RoleID).ToList();
            var Menulist = p;
            return View(Menulist.OrderBy(x=> x.Name).ToList());

      

        }

        //Recursion method for recursively get all child nodes
        public void GetTreeview(List<RoleMenuItemCheckboxModel> list, RoleMenuItemCheckboxModel current, ref List<RoleMenuItemCheckboxModel> returnList)
        {
            var childs = list.Where(a => a.ParentID == current.MenuItemID).ToList();
            current.Children = new List<RoleMenuItemCheckboxModel>();
            current.Children.AddRange(childs);
            foreach (var i in childs)
            {
                GetTreeview(list, i, ref returnList);
            }
        }

        public List<RoleMenuItemCheckboxModel> BuildTree(List<RoleMenuItemCheckboxModel> list)
        {
            List<RoleMenuItemCheckboxModel> returnList = new List<RoleMenuItemCheckboxModel>();
            //find top levels items
            var topLevels = list.Where(a => a.ParentID == list.OrderBy(b => b.ParentID).FirstOrDefault().ParentID);
            returnList.AddRange(topLevels);
            foreach (var i in topLevels)
            {
                GetTreeview(list, i, ref returnList);
            }
            return returnList;
        }

        public JsonResult UpdateRoleMenuItemByID(string list)
        {
            //By Ntshengedzeni
            dynamic d = JsonConvert.DeserializeObject(list);
            int roleId = d.RoleID;
            int menuItemId = d.MenuItemID;
            bool IsChecked = d.IsSelected;
            string userId = User.Identity.GetUserId();

            //Perform update and return results back to the View
            //funcRep.ManageRoleFunction(roleID, accessLevelID, IsChecked, userID); //Save the selected Record From the Form
            //var data = funcRep.GetSystemFunctionsByRoleID(roleID); //Return the list of all Functions for the selected role

            if (IsChecked)
            {
                //This procedure sp_UpdateRoleMenuItems_Post still needs to be modified, for now it looks working fine
                _db.sp_UpdateRoleMenuItems_Post(roleId, menuItemId, Guid.Parse(userId), IsChecked); 
            }
            else
            {
                _db.sp_RemoveRoleMenuItems_Post(roleId, menuItemId);
            }
            
            string data = string.Empty;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}