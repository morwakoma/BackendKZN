using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eRecruitment.BusinessDomain.DAL.Entities.AppModels
{
    public class UserListModel
    {
        public string UserID { get; set; }
        public string OrganisationID { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string IDNumber { get; set; }
        public string CellNo { get; set; }
        public string RoleID { get; set; }
        public string RoleName { get; set; }
        public string EmployeeNO { get; set; }
        public int DivisionID { get; set; }
        public int DepartmentID { get; set; }
        public bool UserStatusID { get; set; }
        public string Status { get; set; }
        [NotMapped]
        public string[] SelectedDivisions { get; set; }
        [NotMapped]
        public string[] SelectedDepartments { get; set; }

    }

    public class UserModel
    {
        //Personal Infromation
        public string IDNumber { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string CellNo { get; set; }
        public string UserName { get; set; }
        //Additional Portal Fields
        public string Passport { get; set; }
        public string area { get; set; }
        public string municipality { get; set; }
        public string province { get; set; }
        //AspNetUser Table
        public string Id { get; set; }
        public string Password { get; set; }
        public string SecurityStamp { get; set; }

        public string UserID { get; set; }
        public string OrganisationID { get; set; }
        public string RoleID { get; set; }
        public string RoleName { get; set; }
        public string EmployeeNO { get; set; }
        public int DivisionID { get; set; }
        public int DepartmentID { get; set; }

    }
}
