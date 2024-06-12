using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            var displayAttribute = enumValue.GetType()
                                            .GetField(enumValue.ToString())
                                            .GetCustomAttributes(typeof(DisplayAttribute), false)
                                            .SingleOrDefault() as DisplayAttribute;

            return displayAttribute?.Name ?? enumValue.ToString();
        }
    }
}
