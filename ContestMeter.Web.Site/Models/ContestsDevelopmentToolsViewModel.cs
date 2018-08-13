using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ContestMeter.Web.Site.Database.Entities;

namespace ContestMeter.Web.Site.Models
{
    public class ContestsDevelopmentToolsListViewModel
    {
        public Guid? ContestId { get; set; }

        public IQueryable<DevelopmentTool> DevelopmentTools { get; set; } 
    }

    public class ContestsDevelopmentToolsCreateViewModel
    {
        public Guid? ContestId { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать название компилятора")]
        [MaxLength(150, ErrorMessage = "{0} не должно превышать {1} символов")]
        [Display(Name = "Название компилятора")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать исполняемый файл")]
        [MaxLength(255, ErrorMessage = "{0} не должно превышать {1} символов")]
        [Display(Name = "Исполняемый файл")]
        public string CompileCommand { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать шаблон командной строки")]
        [MaxLength(255, ErrorMessage = "{0} не должно превышать {1} символов")]
        [Display(Name = "Шаблон командной строки")]
        public string CommandArgs { get; set; }
    }

    public class ContestsDevelopmentToolsEditViewModel
    {
        public Guid? ContestId { get; set; }

        public Guid? DevelopmentToolId { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать название компилятора")]
        [MaxLength(150, ErrorMessage = "{0} не должно превышать {1} символов")]
        [Display(Name = "Название компилятора")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать исполняемый файл")]
        [MaxLength(255, ErrorMessage = "{0} не должно превышать {1} символов")]
        [Display(Name = "Исполняемый файл")]
        public string CompileCommand { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать шаблон командной строки")]
        [MaxLength(255, ErrorMessage = "{0} не должно превышать {1} символов")]
        [Display(Name = "Шаблон командной строки")]
        public string CommandArgs { get; set; }
    }

    public class ContestsDevelopmentToolsDeleteViewModel
    {
        public Guid? ContestId { get; set; }

        public Guid? DevelopmentToolId { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать название компилятора")]
        [MaxLength(150, ErrorMessage = "{0} не должно превышать {1} символов")]
        [Display(Name = "Название компилятора")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать исполняемый файл")]
        [MaxLength(255, ErrorMessage = "{0} не должно превышать {1} символов")]
        [Display(Name = "Исполняемый файл")]
        public string CompileCommand { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать шаблон командной строки")]
        [MaxLength(255, ErrorMessage = "{0} не должно превышать {1} символов")]
        [Display(Name = "Шаблон командной строки")]
        public string CommandArgs { get; set; }
    }
}