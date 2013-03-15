using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;

namespace GdutWeixin.Utils
{
    public static class ApplicationLogger
    {
        public static ILog GetLogger(this HttpApplication app)
        {
            return LogManager.GetLogger("log4net");
        }

    }
}