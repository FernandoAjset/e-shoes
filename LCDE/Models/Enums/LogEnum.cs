using System.ComponentModel.DataAnnotations;

namespace LCDE.Models.Enums
{
    public enum LogEnum
    {
        [Display(Name = "Error")]
        ERROR = 1,
        [Display(Name = "Warning")]
        WARNING = 2,
        [Display(Name = "Info")]
        INFO = 3
    }

    public static class LogEnumExtensions
    {
        public static string? GetDisplayName(this LogEnum logEnum)
        {
            var type = logEnum.GetType();
            var memberInfo = type.GetMember(logEnum.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);
            return (attributes.Length > 0) ? ((DisplayAttribute)attributes[0]).Name : logEnum.ToString();
        }
    }
}