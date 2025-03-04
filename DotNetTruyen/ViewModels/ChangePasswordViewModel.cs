﻿using System.ComponentModel.DataAnnotations;

namespace DotNetTruyen.ViewModels
{
    public class ChangePasswordViewModel
    {
        public UpdatePasswordViewModel UpdatePasswordViewModel { get; set; }
        public AddPasswordViewModel AddPasswordViewModel { get; set; }
    }

    public class AddPasswordViewModel
    {
        [Required(ErrorMessage = "Phải nhập {0}")]

        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).{6,}$",
        ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự, bao gồm chữ thường, chữ hoa, số và ký tự đặc biệt.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string NewPassword { get; set; }
    }

    public class UpdatePasswordViewModel
    {
        [Required(ErrorMessage = "Phải nhập {0}")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu cũ")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Phải nhập {0}")]

        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).{6,}$",
        ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự, bao gồm chữ thường, chữ hoa, số và ký tự đặc biệt.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu lặp lại không trùng khớp")]
        public string ConfirmPassword { get; set; }
    }
}
