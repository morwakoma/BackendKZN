using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eRecruitment.BusinessDomain.DAL.Entities.AppModels
{
    public class User
    {
        public string ID { get; set; }
        public string PhoneNumber { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Password { get; set; }
        public string IDNumber { get; set; }
        public string MemberStatus { get; set; }
        public string Name2 { get; set; }
        public string Name3 { get; set; }

        public int resendCounter = 4;
        public int failureCounter = 3;
        public bool enableResend = true;
        public string tempEmail { get; set; }
        public string tempPhoneNumber { get; set; }
        [Required(ErrorMessage = "Please enter OTP")]
        public string optValue { get; set; }
        public bool EmailSelected { get; set; }
        public bool CellphoneSelected { get; set; }

        public String toString()
        {
            return "PortalUser [id = " + ID + ", phoneNumber = " + PhoneNumber + ", token = " + Token + ", email = " + Email + ", name = " + Name + ", surname = " + Surname + ", password = " + Password + ", idNumber = " + IDNumber + ",memberStatus =" + MemberStatus + ",name2 = " + Name2 + ",name3 = " + Name3 + "]";
        }
        public User getfakeuser(String id)
        {
            User user = null;
            user = new User();
            //user.PhoneNumber = "0815412802";
            //user.PhoneNumber = "0722779905";
            // user.PhoneNumber = "0762299190";
            user.PhoneNumber = "0725365413";
            user.Email = "ntshengedzeni.badamarema@gmail.com";
            user.Surname = "Badamarema";
            user.Name2 = "Emma";
            user.Name3 = "Done have";
            user.Name = "Ntshengedzeni";
            user.IDNumber = "7907265091081";
            //user.IDNumber = "9610260778086";
            // user.IDNumber = "9611030621085";
            //user.IDNumber = "9504016183081";
            return user;
        }

        public void formatEmailCell(User user)
        {
            if (PhoneNumber.Length > 0)
            {
                // cellNrString = cellnumber.substring(0, 2) + "******" + cellnumber.substring(9, 10) + " (default)";
                tempPhoneNumber = PhoneNumber.Substring(0, 2) + "******";
                Console.WriteLine(tempPhoneNumber);
                tempPhoneNumber = tempPhoneNumber + PhoneNumber.Substring(8, 2) + "(default)";
            }
            if (Email.Length > 0)
            {
                tempEmail = Email.Substring(0, 2) + "****";
                int indx = Email.IndexOf('@');
                tempEmail = tempEmail + Email.Substring(indx, 2) + "****";
                Console.WriteLine(tempEmail);
                //tempEmail=tempEmail+Email.Substring(Email.IndexOf("@"), Email.IndexOf("@") + 1) + "****" + Email.Substring(Email.LastIndexOf("."));
            }
        }

    }
}
