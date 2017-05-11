using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using miniCoreFrame.DbProvider;

namespace efwplusWebsite.App_Code
{
    public abstract class BaseController
    {
        public AbstractDbHelper DbHelper { get; set; }

        //public string Args { get; set; }
        public HttpRequest Request { get; set; }
    }
}