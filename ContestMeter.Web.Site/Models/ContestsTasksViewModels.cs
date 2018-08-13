using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ContestMeter.Web.Site.Database.Entities;

namespace ContestMeter.Web.Site.Models
{
    public class ContestsTasksListViewModel
    {
        public Guid? ContestId { get; set; }

        public IQueryable<Task> Tasks { get; set; } 
    }

    public class ContestsTasksCreateViewModel
    {
        public Guid? ContestId { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать название задачи")]
        [MaxLength(50, ErrorMessage = "{0} не должно превышать {1} символов")]
        [Display(Name = "Название задачи")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать краткое название")]
        [MaxLength(50, ErrorMessage = "{0} не должно превышать {1} символов")]
        [Display(Name = "Краткое название")]
        public string ExecutableName { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать проверяющую программу")]
        [MaxLength(50, ErrorMessage = "{0} не должна превышать {1} символов")]
        [Display(Name = "Проверяющая программа (с расширением)")]
        public string CheckerName { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать папку с тестами")]
        [MaxLength(256, ErrorMessage = "{0} не должна превышать {1} символов")]
        [Display(Name = "Папка с тестами")]
        public string TestsFolder { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать предел выполнения по времени")]
        [Display(Name = "Предел выполнения по времени (в сек.)")]
        [Range(0, int.MaxValue, ErrorMessage = "{0} не может быть меньше {1} и больше {2}")]
        public int TimeLimit { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать цену одного теста")]
        [Display(Name = "Цена одного теста")]
        [Range(0, int.MaxValue, ErrorMessage = "{0} не может быть меньше {1} и больше {2}")]
        public int Weight { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать максимальный размер исходника")]
        [Display(Name = "Максимальный размер исходника (в байтах). 0 - не ограничен")]
        [Range(0, int.MaxValue, ErrorMessage = "{0} не может быть меньше {1} и больше {2}")]
        public int MaxSourceSize { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать максимальный размер используемой памяти")]
        [Display(Name = "Максимальный размер используемой памяти (в байтах). 0 - не ограничен")]
        [Range(0, int.MaxValue, ErrorMessage = "{0} не может быть меньше {1} и больше {2}")]
        public int MaxMemorySize { get; set; }
    }

    public class ContestsTasksEditViewModel
    {
        public Guid? ContestId { get; set; }
        
        public Guid? TaskId { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать название задачи")]
        [MaxLength(50, ErrorMessage = "{0} не должно превышать {1} символов")]
        [Display(Name = "Название задачи")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать краткое название")]
        [MaxLength(50, ErrorMessage = "{0} не должно превышать {1} символов")]
        [Display(Name = "Краткое название")]
        public string ExecutableName { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать проверяющую программу")]
        [MaxLength(50, ErrorMessage = "{0} не должна превышать {1} символов")]
        [Display(Name = "Проверяющая программа (с расширением)")]
        public string CheckerName { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать папку с тестами")]
        [MaxLength(256, ErrorMessage = "{0} не должна превышать {1} символов")]
        [Display(Name = "Папка с тестами")]
        public string TestsFolder { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать предел выполнения по времени")]
        [Display(Name = "Предел выполнения по времени (в сек.)")]
        [Range(0, int.MaxValue, ErrorMessage = "{0} не может быть меньше {1} и больше {2}")]
        public int TimeLimit { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать цену одного теста")]
        [Display(Name = "Цена одного теста")]
        [Range(0, int.MaxValue, ErrorMessage = "{0} не может быть меньше {1} и больше {2}")]
        public int Weight { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать максимальный размер исходника")]
        [Display(Name = "Максимальный размер исходника (в байтах). 0 - не ограничен")]
        [Range(0, int.MaxValue, ErrorMessage = "{0} не может быть меньше {1} и больше {2}")]
        public int MaxSourceSize { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать максимальный размер используемой памяти")]
        [Display(Name = "Максимальный размер используемой памяти (в байтах). 0 - не ограничен")]
        [Range(0, int.MaxValue, ErrorMessage = "{0} не может быть меньше {1} и больше {2}")]
        public int MaxMemorySize { get; set; }

        public int Rating { get; set; }
    }

    public class ContestsTasksDeleteViewModel
    {
        public Guid? ContestId { get; set; }

        public Guid? TaskId { get; set; }

        public string TaskDescriptionFileName { get; set; }

        [Display(Name = "Название задачи")]
        public string Name { get; set; }

        [Display(Name = "Краткое название")]
        public string ExecutableName { get; set; }

        [Display(Name = "Проверяющая программа (с расширением)")]
        public string CheckerName { get; set; }

        [Display(Name = "Папка с тестами")]
        public string TestsFolder { get; set; }

        [Display(Name = "Предел выполнения по времени (в сек.)")]
        public int TimeLimit { get; set; }

        [Display(Name = "Цена одного теста")]
        public int Weight { get; set; }

        [Display(Name = "Максимальный размер исходника (в байтах). 0 - не ограничен")]
        public int MaxSourceSize { get; set; }

        [Display(Name = "Максимальный размер используемой памяти (в байтах). 0 - не ограничен")]
        public int MaxMemorySize { get; set; }
        [Display(Name = "Рейтинг задачи")]
        public int Rating { get; set; }
    }
}