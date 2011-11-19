using System.Web.Mvc;

namespace BlogEngine.MVC.Framework
{
    public class CustomRazorViewEngine : RazorViewEngine
    {
        public CustomRazorViewEngine() : base() { }

        public CustomRazorViewEngine(IViewPageActivator viewPageActivator)
            : base(viewPageActivator)
        {
            //AreaViews

            AreaViewLocationFormats = new[] {
                "~/Administration/Views/{1}/{0}.cshtml",
                "~/Administration/Views/Shared/{0}.cshtml"
            };
            AreaMasterLocationFormats = new[] {
                "~/Administration/Views/{1}/{0}.cshtml",
                "~/Administration/Views/Shared/{0}.cshtml"
            };
            AreaPartialViewLocationFormats = new[] {
                "~/Administration/Views/{1}/{0}.cshtml",
                "~/Administration/Views/Shared/{0}.cshtml"
            };

            ViewLocationFormats = new[] {
                "~/Views/{1}/{0}.cshtml",
                "~/Views/Shared/{0}.cshtml"
            };
            MasterLocationFormats = new[] {
                "~/Views/{1}/{0}.cshtml",
                "~/Views/Shared/{0}.cshtml"
            };
            PartialViewLocationFormats = new[] {
                "~/Views/{1}/{0}.cshtml",
                "~/Views/Shared/{0}.cshtml"
            };

            FileExtensions = new[] {
                "cshtml",
                //"vbhtml",
            };
        }
    }
}