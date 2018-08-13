using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ContestMeter.Web.Site.Database.Entities;

namespace ContestMeter.Web.Site.Models
{
    public class AdministratorMainViewModel
    {
        public bool AllowRegister { get; set; }

        public bool AllowRegisterTeacher { get; set; }

        public bool UseRecaptcha { get; set; }

        [Required(ErrorMessage = "Необходимо ввести временный каталог для присылаемых решений")]
        public string PostedSolutionsRootFolder { get; set; }

        [Required(ErrorMessage = "Необходимо ввести корневой каталог с чекерами и тестами")]
        public string SiteConfigFolder { get; set; }

        [Required(ErrorMessage = "Необходимо ввести публичный ключ Recaptcha")]
        public string RecaptchaPublicKey { get; set; }

        [Required(ErrorMessage = "Необходимо ввести приватный ключ Recaptcha")]
        public string RecaptchaPrivateKey { get; set; }
    }

    public class AdministratorNewsItemCreateViewModel
    {
        [Required(ErrorMessage = "Необходимо ввести заголовок новости")]
        [Display(Name = "Заголовок новости")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Необходимо ввести текст новости")]
        [Display(Name = "Текст новости")]
        public string Text { get; set; }
    }

    public class AdministratorNewsItemEditViewModel
    {
        public Guid? NewsItemId { get; set; }

        [Required(ErrorMessage = "Необходимо ввести заголовок новости")]
        [Display(Name = "Заголовок новости")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Необходимо ввести текст новости")]
        [Display(Name = "Текст новости")]
        public string Text { get; set; }
    }

    public class AdministratorAccountsViewModel
    {
        public IQueryable<ApplicationUser> DeletedUsers { get; set; }
        public IQueryable<ApplicationUser> NotDeletedUsers { get; set; }

        public string AdministratorsRoleId { get; set; }
        public string TeacherRoleId { get; set; }
        public string ParticipantId { get; set; }
    }
}