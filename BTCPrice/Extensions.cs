using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;

public static partial class Extensions
{
    static DateTime zeroTime = new DateTime(1, 1, 1);
    public static int TotalYears(this TimeSpan span)
    {
        int years = (zeroTime + span).Year - 1;
        return years;
    }
    public static decimal Sqrt(this decimal x, decimal epsilon = 0.0M)
    {
        if (x < 0) throw new OverflowException("Cannot calculate square root from a negative number");

        decimal current = (decimal)Math.Sqrt((double)x), previous;
        do
        {
            previous = current;
            if (previous == 0.0M) return 0;
            current = (previous + x / previous) / 2;
        }
        while (Math.Abs(previous - current) > epsilon);
        return current;
    }
    public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size)
    {
        for (var i = 0; i < (float)array.Length / size; i++)
        {
            yield return array.Skip(i * size).Take(size);
        }
    }
    public static bool TryToDouble(this string text, out double value)
    {
        if (string.IsNullOrEmpty(text))
            text = "0";
        return double.TryParse(
            text.Replace(NumberFormatInfo.CurrentInfo.CurrencyGroupSeparator, NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator), out value);
    }
    public static double ToDouble(this decimal number)
    {
        return Convert.ToDouble(number);
    }
    public static double ToDouble(this object number)
    {
        return Convert.ToDouble(number);
    }
    public static double ToDouble(this string text)
    {
        if (string.IsNullOrEmpty(text))
            text = "0";
        return Convert.ToDouble(
            text.Replace(NumberFormatInfo.CurrentInfo.CurrencyGroupSeparator, NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator));
    }
    public static decimal ToDecimal(this double doub)
    {
        return Convert.ToDecimal(doub);
    }
    public static decimal ToDecimal(this string text)
    {
        if (string.IsNullOrEmpty(text))
            text = "0";
        return Convert.ToDecimal(
            text.Replace(NumberFormatInfo.CurrentInfo.CurrencyGroupSeparator, NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator));
    }
    public static bool ToBool(this string text)
    {
        return Convert.ToBoolean(text);
    }
    public static bool ToBoolean(this object obj)
    {
        return Convert.ToBoolean(obj);
    }
    public static string BasamakTamamlaSol(this int rakam, int basamak)
    {
        string text = "" + rakam;
        while (text.Length < basamak)
        {
            text = "0" + text;
        }
        return text;
    }
    public static string BasamakTamamlaSag(this int rakam, int basamak)
    {
        string text = "" + rakam;
        while (text.Length < basamak)
        {
            text += "0";
        }
        return text;
    }
    public static int ToInt(this object obj)
    {
        return Convert.ToInt32(obj);
    }
    public static int ToInt(this Newtonsoft.Json.Linq.JValue obj)
    {
        return Convert.ToInt32(obj.Value);
    }
    public static long ToLong(this string text)
    {
        if (string.IsNullOrEmpty(text))
            text = "0";
        return Convert.ToInt64(
            text.Replace(NumberFormatInfo.CurrentInfo.CurrencyGroupSeparator, NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator));
    }
    public static int ToInt(this string text)
    {
        if (string.IsNullOrEmpty(text))
            text = "0";
        return Convert.ToInt32(
            text.Replace(NumberFormatInfo.CurrentInfo.CurrencyGroupSeparator, NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator));
    }

    public static void AddIfNotEmptyOrNull(this List<string> list, string Item)
    {
        if (!string.IsNullOrEmpty(Item))
        {
            list.Add(Item);
        }
    }
}