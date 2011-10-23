using BlogEngine.Core;

namespace BlogEngine.MVC.ViewsModels
{
    public class PostViewModel : BaseViewModel
    {
        public PostViewModel() { }

        public BaseViewModel Base { get; set; }

        public Post Post { get; set; }
    }
}