using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RconClient
{
  [ValueConversion(typeof (Status), typeof (SolidColorBrush))]
  public class StatusToBrushConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      switch ((Status) value)
      {
        case Status.Ok:
          return (object) Brushes.ForestGreen;
        case Status.Warning:
          return (object) Brushes.Yellow;
        case Status.Error:
          return (object) Brushes.OrangeRed;
        default:
          return (object) null;
      }
    }

    public object ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      return (object) null;
    }
  }
}
