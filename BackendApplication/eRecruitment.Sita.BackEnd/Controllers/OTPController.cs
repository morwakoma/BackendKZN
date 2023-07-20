using eRecruitment.BusinessDomain.DAL.Entities.AppModels;
using Newtonsoft.Json;
using OTPClient.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace eRecruitment.Sita.BackEnd.Controllers
{
    public class OTPController : Controller
    {
        // GET: OTPSelection
        public ActionResult Index(string returnUrl, string id)
        {
            //id = "eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJudHNoZW5nZWR6ZW5pLmJhZGFtYXJlbWFAc2l0YS5jby56YSJ9.wgRvPMV_2FO2vSqtBE8cJK7aeoWKtaQ327ZOar4viwM";

            User userModel = new User();

            Console.Write("First hit from Portal");
            if (id != null)
            {
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["WebAPIURL"] + id);
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
                    if (data != null)
                    {
                       // User userModel = new User();
                        userModel = new User
                        {
                            Name = Convert.ToString(data["name"]),
                            Name2 = Convert.ToString(data["secondName"]),
                            Name3 = Convert.ToString(data["otherName"]),
                            Surname = Convert.ToString(data["surname"]),
                            Email = Convert.ToString(data["email"]),
                            IDNumber = Convert.ToString(data["idNumber"]),
                            PhoneNumber = Convert.ToString(data["phoneNumber"]),
                            // PhoneNumber = "0843436415",
                        };
                        response.Close();
                    }

                    else
                    {
                        response.Close();
                        // userModel = conn.GetUserInformation("123");
                    }
                }
                else
                {
                    response.Close();
                }
            }

            //if(userModel.PhoneNumber.Length > 0)
            //{
            //    userModel.tempPhoneNumber = userModel.PhoneNumber.Substring(0, 2) + "******";
            //    Console.WriteLine(userModel.tempPhoneNumber);
            //    userModel.tempPhoneNumber = userModel.tempPhoneNumber + userModel.PhoneNumber.Substring(8, 2) + "(default)";

            //}
            //if (userModel.Email.Length > 0)
            //{
            //    userModel.tempEmail = userModel.Email.Substring(0, 2) + "****";
            //    int indx = userModel.Email.IndexOf('@');
            //    userModel.tempEmail = userModel.tempEmail + userModel.Email.Substring(indx, 2) + "****";
            //    Console.WriteLine(userModel.tempEmail);
            //    //tempEmail=tempEmail+Email.Substring(Email.IndexOf("@"), Email.IndexOf("@") + 1) + "****" + Email.Substring(Email.LastIndexOf("."));
            //}
            userModel.formatEmailCell(userModel);
            ViewBag.ReturnUrl = returnUrl;
            TempData["Userdetails"] = userModel;

            return View("Selection", userModel);


        }
        public ActionResult Selection()
        {
            return View();
        }
        public ActionResult OTPSelection(FormCollection collection)
        {
            User userModel = new User();
            ClientService service = new ClientService();
            string selectedValue;

            userModel = TempData["Userdetails"] as User;
            //TempData["userdetails"] = userModel.getfakeuser("123");
            // userModel = new Models.OTP.User();
             //userModel = userModel.getfakeuser("123");
            TempData.Keep();
            if (collection["CellphoneSelected"] == "Cellphone")
            {
                selectedValue = userModel.PhoneNumber;
                TempData["EmailSelected"] = false;
            }
            else
            {
                selectedValue = userModel.Email;
                TempData["EmailSelected"] = true;
                // userModel.CellphoneSelected = false;
            }
            string publictoken;
            service = new ClientService();
            publictoken = service.sendotp(selectedValue);
            if (!(string.IsNullOrEmpty(publictoken)))
            {
                userModel.Token = publictoken;
                TempData["Userdetails"] = userModel;
                return View("Verification", userModel);
            }
            else
            {
                return null;
            }
        }
        public ActionResult ValidateOTP(FormCollection collection)
        {
            User userModel = new User();
            ClientService service = new ClientService();
            string validateOTPErrorCode;
            userModel = TempData["Userdetails"] as User;
            TempData.Keep();
            string OTPEntered = collection["optValue"];

            if (string.IsNullOrEmpty(OTPEntered))
            {
                ModelState.AddModelError("", "Please enter OTP before clicking submit.");
                return View("Verification", userModel);
            }
            try
            {

                if (userModel.resendCounter >= 0)
                {
                    validateOTPErrorCode = service.validateTheOTP(userModel.Token, OTPEntered);

                    if (validateOTPErrorCode == "100")
                    {
                        TempData["Userdetails"] = userModel;
                       // return RedirectToAction("Index", "Admin",);
                        return RedirectToAction("Index", "Admin", new {userModel});
                    }
                    else if (validateOTPErrorCode == "30")
                    {
                        userModel.resendCounter--;
                        if (userModel.resendCounter > 0)
                        {
                            userModel.enableResend = false;
                            ModelState.AddModelError("", "OTP Failure Count Depleted!,Please select Resend button to receive new OTP.");
                            userModel.enableResend = true;
                            return View("Verification", userModel);
                        }
                    }
                    else if (validateOTPErrorCode == "20")
                    {
                        userModel.resendCounter--;
                        if (userModel.resendCounter > 0)
                        {
                            userModel.enableResend = false;
                            ModelState.AddModelError("", "OTP Expired!, Please select Resend button to receive new OTP.");
                            userModel.enableResend = true;
                            return View("Verification", userModel);
                        }
                        else
                        {
                            ModelState.AddModelError("", "Resends Depleted!,The number of Resends has been depleted!, Click Cancel to exit and  try again after 24 hours...");
                            return RedirectToAction("GoToPortal");
                        }
                    }
                    else if (validateOTPErrorCode == "40")
                    {
                        userModel.failureCounter--;
                        if (userModel.failureCounter < 0)
                        {
                            userModel.enableResend = false;
                            ModelState.AddModelError("", "Failures Exceeded!,The number of failures has been exceeded!, Click Cancel to exit ");

                            return RedirectToAction("GoToPortal");
                        }
                        else if (userModel.failureCounter > 0)
                        {
                            ModelState.AddModelError("", "Wrong OTP Entered!, Authentication Failed, Please try again. Retries Left: " + userModel.failureCounter);
                            return View("Verification", userModel);
                        }
                        else
                        {
                            ModelState.AddModelError("", "Failures Exceeded!,The number of failures has been exceeded!, Click Cancel to exit");
                            return View("Verification", userModel);
                        }

                    }
                    else if (validateOTPErrorCode == "50")
                    {
                        ModelState.AddModelError("", "OTP Invalidated!, The OTP has already been validated/rejected.");
                        return RedirectToAction("GoToPortal");
                    }
                }

                return View("Verification", userModel);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return RedirectToAction("GoToPortal");
            }

        }
        public void GoToPortal()
        {
            Response.Redirect(System.Configuration.ConfigurationManager.AppSettings["LogoutURL"].ToString());
        }

        public ActionResult Verification()
        {
            return View();
        }

        public ActionResult onResendClicked()
        {

            User userModel = new User();
            ClientService service = new ClientService();
            userModel = TempData["Userdetails"] as User;
            string selectedValue;

            if (TempData["EmailSelected"].ToString() == "true")
            {
                selectedValue = userModel.Email;
            }
            else
            {
                selectedValue = userModel.PhoneNumber;
            }

            userModel.failureCounter = userModel.resendCounter > 0 ? 3 : userModel.failureCounter;
            userModel.resendCounter--;

            if (userModel.Token == null)
            {
                ModelState.AddModelError("", "Please enter OTP before clicking submit.");
                TempData["Userdetails"] = userModel;
                return View("Verification", userModel);
            }
            try
            {
                if (userModel.resendCounter < 0)
                {
                    ModelState.AddModelError("", "Your resedn counter has been depleted, Please exit the application.");
                    TempData["Userdetails"] = userModel;
                    return View("Verification", userModel);
                }
                else
                {
                    userModel.enableResend = true;
                    userModel.Token = service.reSendOtp(userModel.Token, selectedValue);
                    TempData["Userdetails"] = userModel;
                    return View("Verification", userModel);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                TempData["Userdetails"] = userModel;
                return RedirectToAction("GoToPortal");
            }
        }
    }

}