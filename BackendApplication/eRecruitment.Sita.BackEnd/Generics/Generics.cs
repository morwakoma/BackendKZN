using eRecruitment.Sita.BackEnd.App_Data.Entities.DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRecruitment.Sita.BackEnd.Generics
{
    public class UserMenuItems
    {
        public static ArrayList GetUserMenuList(string userID)
        {
            eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext();
            //FunctionRepositoryBLL funcRep = new FunctionRepositoryBLL(new FunctionRepository());
            //FunctionRepository FuncRep = new FunctionRepository();
            ArrayList result = new ArrayList();

            //Query you Database and Add MenuItemId below
            var menuItems = _db.sp_GetMenuItemsByUserId_Get(userID).ToList();
            foreach (var d in menuItems)
            {
                //result.Add("number");
                result.Add(d.MenuItemID.ToString());
            }

            return result;
        }
    }
}