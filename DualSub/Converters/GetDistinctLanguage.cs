using DualSub.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DualSub.Converters
{
    public class GetDistinctLanguageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return new [] { (string)parameter };

          
            IEnumerable<SubtitleMetadata> subtitles = (IEnumerable<SubtitleMetadata>)value;

            if (!subtitles.Any())
                return new[] { (string)parameter };

            return subtitles.Select(x => x.Language).Distinct().OrderBy(x => x).Select(x => x.ToUpper());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
