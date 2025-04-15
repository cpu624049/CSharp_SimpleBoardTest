using System.ComponentModel.DataAnnotations;

namespace SimpleBoardTest.ViewModels
{
    public class UserEditViewModel
    {
        public int UserId { get; set; }
        [Required(ErrorMessage = "사용자 이름을 입력하세요.")]
        public string UserName { get; set; } = null!;
        [Required(ErrorMessage = "이메일을 입력하세요.")]
        [EmailAddress(ErrorMessage = "이메일 형식이 올바르지 않습니다.")]
        public string Email { get; set; } = null!;
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; } = null!;
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "비밀번호가 일치하지 않습니다.")]
        public string? ConfirmPassword { get; set; } = null!;
    }
}
