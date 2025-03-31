using System.ComponentModel.DataAnnotations;

namespace SimpleBoardTest.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "이메일을 입력하세요.")]
        [EmailAddress(ErrorMessage = "이메일 형식이 올바르지 않습니다.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "비밀번호를 입력하세요.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
