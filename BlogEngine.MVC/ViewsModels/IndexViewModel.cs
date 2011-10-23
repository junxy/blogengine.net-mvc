using System.Collections.Generic;
using BlogEngine.Core;

namespace BlogEngine.MVC.ViewsModels
{
    public class IndexViewModel : BaseViewModel
    {
        public IndexViewModel() { }

        public BaseViewModel Base { get; set; }

        public ServingContentBy ContentBy { get; set; }

        public List<IPublishable> Posts { get; set; }
    }
}