using System.ComponentModel.DataAnnotations;

namespace ContestMeter.Web.Site.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required(ErrorMessage = "Необходимо ввести имя пользователя")]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }
    }

    public class ManageUserViewModel
    {
        [Required(ErrorMessage = "Необходимо ввести текущий пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Текущий пароль")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Необходимо ввести новый пароль")]
        [StringLength(100, ErrorMessage = "{0} должен содержать не менее {2} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите новый пароль")]
        [Compare("NewPassword", ErrorMessage = "Новый пароль и его подтверждение не совпадают.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Необходимо ввести имя пользователя")]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Необходимо ввести пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterTeacherViewModel
    {
        [Required(ErrorMessage = "Необходимо ввести имя пользователя")]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Необходимо ввести пароль")]
        [StringLength(100, ErrorMessage = "{0} должен содержать не менее {2} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите пароль")]
        [Compare("Password", ErrorMessage = "Пароль и его подтверждение не совпадают.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Необходимо ввести email")]
        [RegularExpression(@"^[a-zA-Z0-9.-]{1,20}@[a-zA-Z0-9]{1,20}\.[A-Za-z]{2,4}", ErrorMessage = "Неверный формат Email")]
        [Display(Name = "Email адрес")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Необходимо ввести имя")]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Необходимо ввести фамилию")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Необходимо ввести отчество")]
        [Display(Name = "Отчество")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Необходимо ввести школу (ВУЗ)")]
        [MaxLength(150, ErrorMessage = "{0} должна содержать не более {1} символов")]
        [Display(Name = "Школа (ВУЗ)")]
        public string School { get; set; }

        [Required(ErrorMessage = "Необходимо ввести город (поселок)")]
        [MaxLength(150, ErrorMessage = "{0} должен содержать не более {1} символов")]
        [Display(Name = "Город (поселок)")]
        public string HomeTown { get; set; }

        public bool UseRecaptcha { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Необходимо ввести имя пользователя")]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Необходимо ввести пароль")]
        [StringLength(100, ErrorMessage = "{0} должен содержать не менее {2} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите пароль")]
        [Compare("Password", ErrorMessage = "Пароль и его подтверждение не совпадают.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Необходимо ввести email")]
        [RegularExpression(@"^[a-zA-Z0-9.-]{1,20}@[a-zA-Z0-9]{1,20}\.[A-Za-z]{2,4}", ErrorMessage = "Неверный формат Email")]
        [Display(Name = "Email адрес")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Необходимо ввести имя")]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Необходимо ввести фамилию")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Необходимо ввести отчество")]
        [Display(Name = "Отчество")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Необходимо ввести школу (ВУЗ)")]
        [MaxLength(150, ErrorMessage = "{0} должна содержать не более {1} символов")]
        [Display(Name = "Школа (ВУЗ)")]
        public string School { get; set; }

        [Required(ErrorMessage = "Необходимо ввести класс (курс)")]
        // ToDo: нужно ли вводиить еще и группу? Если да, то убрать проверку на регулярное выражение
        [RegularExpression(@"\d{1,2}", ErrorMessage = "Вы ввели некорректный класс (курс)")]
        [Display(Name = "Класс (курс)")]
        public int Grade { get; set; }

        [Required(ErrorMessage = "Необходимо ввести город (поселок)")]
        [MaxLength(150, ErrorMessage = "{0} должен содержать не более {1} символов")]
        [Display(Name = "Город (поселок)")]
        public string HomeTown { get; set; }

        public bool UseRecaptcha { get; set; }
    }
}
