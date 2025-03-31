using System.ComponentModel.DataAnnotations;

namespace SimpleBoardTest.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "사용자 이름을 입력하세요.")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "이메일을 입력하세요.")]
        [EmailAddress(ErrorMessage = "이메일 형식이 올바르지 않습니다.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "비밀번호를 입력하세요.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "비밀번호는 6자 이상이어야 합니다.")]
        public string Password { get; set; } = null!;

        [Compare("Password", ErrorMessage = "비밀번호가 일치하지 않습니다.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = null!;
    }
}
