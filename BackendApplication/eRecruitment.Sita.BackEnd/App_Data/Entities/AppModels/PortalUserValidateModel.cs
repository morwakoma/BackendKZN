using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRecruitment.BusinessDomain.DAL.Entities.AppModels
{
    public class PortalUserValidateModel
    {
        public user UserDetails { get; set; }
        public contact ContactDetails { get; set; }
        public address AddressDetails { get; set; }
    }
    public class userStatus
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int indicator { get; set; }
        public int categoryId { get; set; }
    }
    public class address
    {
        public int addressId { get; set; }
        public string area { get; set; }
        public string municipality { get; set; }
        public string province { get; set; }
        public string line1 { get; set; }
        public string line2 { get; set; }
        public string line3 { get; set; }
        public string line4 { get; set; }
        public string line5 { get; set; }
        public DateTime dateCreated { get; set; }
        public string other { get; set; }
    }
    public class contact
    {
        public int contactId { get; set; }
        public string cellHome { get; set; }
        public string cellWork { get; set; }
        public string emailHome { get; set; }
        public string emailWork { get; set; }
        public string landlineHome { get; set; }
        public string landlineWork { get; set; }
        public DateTime dateCreated { get; set; }
    }
    public class user
    {
        public int userId { get; set; }
        public string name { get; set; }
        public string secondName { get; set; }
        public string thirdName { get; set; }
        public string surname { get; set; }
        public string idNumber { get; set; }
        public string passport { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public int token { get; set; }
        public DateTime dateCreated { get; set; }
        public userStatus usersatatus { set; get; }
    }
}