using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRecruitment.Sita.BackEnd.Models
{
    public class RoleMenuItemCheckboxModel
    {
        //Cat Id
        public int MenuItemID { get; set; }
        public int ParentID { get; set; }
        public List<RoleMenuItemCheckboxModel> Children { get; set; }
        //Cat Name
        public string Name { get; set; }

        ////Cat Description
        public bool IsChecked { get; set; }

        //represnts Parent ID and it's nullable


    }

    public class JsTreeModel
    { //help sort data for treeview 
        public int MenuItemID { get; set; }
        public int ParentID { get; set; }
        public List<RoleMenuItemCheckboxModel> Children { get; set; }
        //Cat Name
        public string Name { get; set; }

        ////Cat Description
        public bool IsChecked { get; set; }
    }
}