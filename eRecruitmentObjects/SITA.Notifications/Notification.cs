using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SITA.Notifications
{
    public class Notification : INotification
    {

        public bool SendEmail(string to, string subject, string bodyMessage)
        {
            bool Status = true;
            try

            {
                Task t = Task.Run(async () =>
                {
                    // You should use using block so .NET can clean up resources
                    using (var client = new SmtpClient())
                    {
                        MailMessage msg = new MailMessage();
                        //string  fromEmail = string.Format("Agro Industry <{0}>", System.Configuration.ConfigurationManager.AppSettings["AgroEmail"]);

                        msg.From = new MailAddress(string.Format("e-Recruitment Online <{0}>", System.Configuration.ConfigurationManager.AppSettings["MemberEmail"]));
                        msg.To.Add(new MailAddress(to));
                        msg.Body = bodyMessage;
                        msg.Subject = subject;

                        //client.Host = "smtp.naroba.co.za";
                        client.Host = System.Configuration.ConfigurationManager.AppSettings["SMTPServer"];
                        client.Port = 25;
                        client.Credentials = new NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["SMTPUserName"]
                            , System.Configuration.ConfigurationManager.AppSettings["SMTPPassword"]);

                        msg.IsBodyHtml = true;

                        await client.SendMailAsync(msg);
                        Status = true;
                    }
                });
                t.Wait(); // Wait until the above task is complete, email is sent
            }
            catch (Exception ex)
            {
                string Message = ex.Message.ToString();
                Status = false;
            }

            return Status;
        }

        public bool SendSMS(string cellNo, string message)
        {
            try
            {
                string Proxy = System.Configuration.ConfigurationManager.AppSettings["SMS_PROXY"];
                string sms_port = System.Configuration.ConfigurationManager.AppSettings["SMS_PORT"];
                string Password = System.Configuration.ConfigurationManager.AppSettings["SMS_PASSWORD"];
                string sms_Username = System.Configuration.ConfigurationManager.AppSettings["SMS_USERNAME"];

                String urlStr1 = "http://sms.wasp.gov.za/smpp/submit_multi?sysid=EMATRIC&to=" + cellNo + "&msg=" + message + "&from=" + sms_Username + "&pwd=" + Password + "&priority=1&ref=88&acc=SITA";

                bool Status = false;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlStr1);
                WebProxy myproxy = new WebProxy(Proxy, Convert.ToInt32(sms_port));
                myproxy.BypassProxyOnLocal = false;
                request.Proxy = myproxy;
                request.Method = "GET";
                request.Timeout = 100000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusDescription == "OK") { Status = true; }
                else { Status = false; }
                return Status;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return true;
        }

    }
}
