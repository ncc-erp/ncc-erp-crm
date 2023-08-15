using System.ComponentModel.DataAnnotations;

namespace CRM.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}