using System.Globalization;
using Povala.Shared.DTOs.Boulder;
using Povala.Shared.Enums;

namespace povala_app.Converters;

public class BoulderToRectConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is BoulderDto boulder)
        {
            return new Rect(boulder.CoordX - 10, boulder.CoordY - 10, 20, 20);
        }
        return new Rect(0, 0, 20, 20);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class BoulderColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is BoulderColor colorEnum)
        {
            return colorEnum switch
            {
                BoulderColor.White => Colors.White,
                BoulderColor.Yellow => Colors.Yellow,
                BoulderColor.Green => Colors.LimeGreen,
                BoulderColor.Blue => Colors.DodgerBlue,
                BoulderColor.Red => Colors.Red,
                BoulderColor.Black => Colors.Black,
                BoulderColor.Purple => Colors.Purple,
                _ => Colors.Gray
            };
        }
        return Colors.Gray;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}
