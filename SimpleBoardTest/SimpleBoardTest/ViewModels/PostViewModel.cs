using System.ComponentModel.DataAnnotations;

namespace SimpleBoardTest.ViewModels
{
    public class PostViewModel
    {
        [Required(ErrorMessage = "제목을 입력하세요.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "내용을 입력하세요.")]
        public string Content { get; set; } = string.Empty;
    }
}
