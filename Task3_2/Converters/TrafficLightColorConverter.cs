using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Task3_2.Models;

namespace Task3_2.Converters
{
    public class TrafficLightColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string state)
            {
                if (state == TrafficLight.LightState.RedForCars.ToString())
                    return Brushes.Red;  // Красный для машин (зеленый для пешеходов)
                else 
                    return Brushes.Green; // Зеленый для машин (красный для пешеходов)
            }
            return Brushes.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
     public class CarColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isEmergency && isEmergency)
            {
                return Brushes.Red; // Для аварийных машин
            }
            
            return Brushes.DarkBlue; // Для обычных машин
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
        public class AddValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue && parameter is string paramString)
            {
                if (double.TryParse(paramString, out double paramValue))
                {
                    return doubleValue + paramValue;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}