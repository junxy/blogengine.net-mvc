using System.Web.Mvc;

namespace BlogEngine.MVC.Framework
{
    public class CustomWebFormViewEngine : WebFormViewEngine
    {
        public CustomWebFormViewEngine()
        {
            MasterLocationFormats = new[] {
                "~/Views/{1}/{0}.master",
                "~/Views/Shared/{0}.master"
            };

            AreaMasterLocationFormats = new[] {
                "~/Administration/Views/{1}/{0}.master",
                "~/Administration/Views/Shared/{0}.master",

                "~/Areas/{2}/Views/{1}/{0}.master",
                "~/Areas/{2}/Views/Shared/{0}.master",
            };

            ViewLocationFormats = new[] {
                "~/Views/{1}/{0}.aspx",
                "~/Views/{1}/{0}.ascx",
                "~/Views/Shared/{0}.aspx",
                "~/Views/Shared/{0}.ascx"
            };

            AreaViewLocationFormats = new[] {
                "~/Administration/Views/{1}/{0}.aspx",
                "~/Administration/Views/{1}/{0}.ascx",
                "~/Administration/Views/Shared/{0}.aspx",
                "~/Administration/Views/Shared/{0}.ascx",

                "~/Areas/{2}/Views/{1}/{0}.aspx",
                "~/Areas/{2}/Views/{1}/{0}.ascx",
                "~/Areas/{2}/Views/Shared/{0}.aspx",
                "~/Areas/{2}/Views/Shared/{0}.ascx",
            };

            PartialViewLocationFormats = ViewLocationFormats;
            AreaPartialViewLocationFormats = AreaViewLocationFormats;
        }
    }
}