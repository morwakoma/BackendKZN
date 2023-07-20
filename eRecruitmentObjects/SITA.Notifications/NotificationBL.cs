using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SITA.Notifications
{
   public class NotificationBL
    {
        #region"--Declarations--"
        /// <summary>
        /// Declarations
        /// <para><b>Created By: </b> Ntshengedzeni Badamarema - 03/05/2020</para>
        /// <para><b>Modified By: </b> Modiefied By and Date</para>
        /// </summary>
        public INotification _notify;
        #endregion

        public NotificationBL(INotification notification)
        {
            this._notify = notification;
        }

        public bool SendSMS(string cellPhone, string message)
        {
            _notify = new Notification();
            return _notify.SendSMS(cellPhone, message);
        }

        public bool SendEmail(string to, string subject, string bodyMessage)
        {
            _notify = new Notification();
            return _notify.SendEmail(to, subject, bodyMessage);
        }

    }
}
