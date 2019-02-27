using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RideShareWebServices.Models
{
    public class AndroidFCMPushNotificationStatus
    {
        public bool Successful
        {
            get;
            set;
        }

        public String Response
        {
            get;
            set;
        }
        public Exception Error
        {
            get;
            set;
        }

    }
}