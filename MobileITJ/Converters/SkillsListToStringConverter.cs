using System.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace MobileITJ.Converters
{
    public class SkillsListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<string> skills)
            {
                if (skills == null || !skills.Any())
                    return "No specific skills listed.";

                return "Skills: " + string.Join(", ", skills);
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}