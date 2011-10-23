using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BlogEngine.Core;

namespace BlogEngine.MVC.ViewsModels
{
    public class ArchiveViewModel
    {
        public BaseViewModel Base { get; set; }

        public List<Category> Categories { get; set; }

        public List<Post> NoCatList { get; set; }
    }
}