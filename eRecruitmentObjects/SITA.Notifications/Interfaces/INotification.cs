using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SITA.Notifications
{
   public interface INotification
    {
        bool SendSMS(string cellPhone, string message);
        bool SendEmail(string to, string subject, string bodyMessage);
    }
}
