using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LinkCleaner.Presentation.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public Brush TrueBrush { get; set; } = Brushes.SteelBlue;
        public Brush FalseBrush { get; set; } = Brushes.LightGray;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isSelected = (bool)value;
            return isSelected ? TrueBrush : FalseBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;
    }
}
