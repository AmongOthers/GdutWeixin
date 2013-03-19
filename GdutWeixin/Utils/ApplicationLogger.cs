using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;

namespace GdutWeixin.Utils
{
    public class ApplicationLogger
    {
        public static ILog GetLogger()
        {
            return LogManager.GetLogger("log4net");
        }

    }
}