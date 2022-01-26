// Decompiled with JetBrains decompiler
// Type: StardewValley.StringBuilderFormatEx
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using System;
using System.Globalization;
using System.Text;

namespace StardewValley
{
  /// <summary>
  /// StringBuilder extension methods for garbage free append and format of numeric types.
  /// </summary>
  /// <remarks>
  /// Based on the work of Gavin Pugh.
  /// http://www.gavpugh.com/2010/04/05/xnac-a-garbage-free-stringbuilder-format-method/
  /// </remarks>
  public static class StringBuilderFormatEx
  {
    private static readonly char[] MsDigits = new char[16]
    {
      '0',
      '1',
      '2',
      '3',
      '4',
      '5',
      '6',
      '7',
      '8',
      '9',
      'A',
      'B',
      'C',
      'D',
      'E',
      'F'
    };
    private const uint MsDefaultDecimalPlaces = 5;
    private const char MsDefaultPadChar = '0';
    private static char[] _buffer;

    public static bool StringsEqual(this StringBuilder sb, string value)
    {
      if (sb == null != (value == null))
        return false;
      if (value == null)
        return true;
      if (sb.Length != value.Length)
        return false;
      for (int index = 0; index < value.Length; ++index)
      {
        if ((int) value[index] != (int) sb[index])
          return false;
      }
      return true;
    }

    private static char[] _getBuffer(int len)
    {
      if (StringBuilderFormatEx._buffer == null || StringBuilderFormatEx._buffer.Length < len)
        StringBuilderFormatEx._buffer = new char[len];
      return StringBuilderFormatEx._buffer;
    }

    public static StringBuilder AppendEx(
      this StringBuilder stringBuilder,
      StringBuilder value)
    {
      int length = value.Length;
      char[] buffer = StringBuilderFormatEx._getBuffer(length);
      value.CopyTo(0, buffer, 0, length);
      stringBuilder.Append(buffer, 0, length);
      return stringBuilder;
    }

    static StringBuilderFormatEx() => StringBuilderFormatEx.Init();

    public static void Init()
    {
    }

    /// <summary>
    /// Convert an unsigned integer value to a string and concatenate into StringBuilder. Any base value allowed.
    /// </summary>
    public static StringBuilder AppendEx(
      this StringBuilder stringBuilder,
      uint uintVal,
      uint padAmount,
      char padChar,
      uint baseVal)
    {
      uint val2 = 0;
      uint num = uintVal;
      do
      {
        num /= baseVal;
        ++val2;
      }
      while (num > 0U);
      stringBuilder.Append(padChar, (int) Math.Max(padAmount, val2));
      int length = stringBuilder.Length;
      for (; val2 > 0U; --val2)
      {
        --length;
        stringBuilder[length] = StringBuilderFormatEx.MsDigits[(int) (uintVal % baseVal)];
        uintVal /= baseVal;
      }
      return stringBuilder;
    }

    /// <summary>
    /// Convert an unsigned integer value to a string and concatenate into StringBuilder. Assumes no padding and base ten.
    /// </summary>
    public static StringBuilder AppendEx(
      this StringBuilder stringBuilder,
      uint uintVal)
    {
      stringBuilder.AppendEx(uintVal, 0U, '0', 10U);
      return stringBuilder;
    }

    /// <summary>
    /// Convert an unsigned integer value to a string and concatenate into StringBuilder. Assumes base ten.
    /// </summary>
    public static StringBuilder AppendEx(
      this StringBuilder stringBuilder,
      uint uintVal,
      uint padAmount)
    {
      stringBuilder.AppendEx(uintVal, padAmount, '0', 10U);
      return stringBuilder;
    }

    /// <summary>
    /// Convert an unsigned integer value to a string and concatenate into StringBuilder. Assumes base ten.
    /// </summary>
    public static StringBuilder AppendEx(
      this StringBuilder stringBuilder,
      uint uintVal,
      uint padAmount,
      char padChar)
    {
      stringBuilder.AppendEx(uintVal, padAmount, padChar, 10U);
      return stringBuilder;
    }

    /// <summary>
    /// Converts a signed integer value to a string and concatenate into StringBuilder. Any base value allowed.
    /// </summary>
    public static StringBuilder AppendEx(
      this StringBuilder stringBuilder,
      int intVal,
      uint padAmount,
      char padChar,
      uint baseVal)
    {
      if (intVal < 0)
      {
        stringBuilder.Append('-');
        uint uintVal = (uint) (-1 - intVal + 1);
        stringBuilder.AppendEx(uintVal, padAmount, padChar, baseVal);
      }
      else
        stringBuilder.AppendEx((uint) intVal, padAmount, padChar, baseVal);
      return stringBuilder;
    }

    /// <summary>
    /// Converts a signed integer value to a string and concatenate into StringBuilder. Assumes no padding and base ten.
    /// </summary>
    public static StringBuilder AppendEx(this StringBuilder stringBuilder, int intVal)
    {
      stringBuilder.AppendEx(intVal, 0U, '0', 10U);
      return stringBuilder;
    }

    /// <summary>
    /// Convert a signed integer value to a string and concatenate into StringBuilder. Assumes base ten.
    /// </summary>
    public static StringBuilder AppendEx(
      this StringBuilder stringBuilder,
      int intVal,
      uint padAmount)
    {
      stringBuilder.AppendEx(intVal, padAmount, '0', 10U);
      return stringBuilder;
    }

    /// <summary>
    /// Convert a signed integer value to a string and concatenate into StringBuilder. Assumes base ten.
    /// </summary>
    public static StringBuilder AppendEx(
      this StringBuilder stringBuilder,
      int intVal,
      uint padAmount,
      char padChar)
    {
      stringBuilder.AppendEx(intVal, padAmount, padChar, 10U);
      return stringBuilder;
    }

    /// <summary>
    /// Convert an unsigned long value to a string and concatenate into StringBuilder. Any base value allowed.
    /// </summary>
    public static StringBuilder AppendEx(
      this StringBuilder stringBuilder,
      ulong uintVal,
      uint padAmount,
      char padChar,
      uint baseVal)
    {
      uint val2 = 0;
      ulong num = uintVal;
      do
      {
        num /= (ulong) baseVal;
        ++val2;
      }
      while (num > 0UL);
      stringBuilder.Append(padChar, (int) Math.Max(padAmount, val2));
      int length = stringBuilder.Length;
      for (; val2 > 0U; --val2)
      {
        --length;
        stringBuilder[length] = StringBuilderFormatEx.MsDigits[uintVal % (ulong) baseVal];
        uintVal /= (ulong) baseVal;
      }
      return stringBuilder;
    }

    /// <summary>
    /// Convert an unsigned long value to a string and concatenate into StringBuilder. Assumes no padding and base ten.
    /// </summary>
    public static StringBuilder AppendEx(
      this StringBuilder stringBuilder,
      ulong uintVal)
    {
      stringBuilder.AppendEx(uintVal, 0U, '0', 10U);
      return stringBuilder;
    }

    /// <summary>
    /// Convert an unsigned long value to a string and concatenate into StringBuilder. Assumes base ten.
    /// </summary>
    public static StringBuilder AppendEx(
      this StringBuilder stringBuilder,
      ulong uintVal,
      uint padAmount)
    {
      stringBuilder.AppendEx(uintVal, padAmount, '0', 10U);
      return stringBuilder;
    }

    /// <summary>
    /// Convert an unsigned long value to a string and concatenate into StringBuilder. Assumes base ten.
    /// </summary>
    public static StringBuilder AppendEx(
      this StringBuilder stringBuilder,
      ulong uintVal,
      uint padAmount,
      char padChar)
    {
      stringBuilder.AppendEx(uintVal, padAmount, padChar, 10U);
      return stringBuilder;
    }

    /// <summary>
    /// Converts a signed long value to a string and concatenate into StringBuilder. Any base value allowed.
    /// </summary>
    public static StringBuilder AppendEx(
      this StringBuilder stringBuilder,
      long intVal,
      uint padAmount,
      char padChar,
      uint baseVal)
    {
      if (intVal < 0L)
      {
        stringBuilder.Append('-');
        uint uintVal = (uint) (-1 - (int) (uint) intVal + 1);
        stringBuilder.AppendEx(uintVal, padAmount, padChar, baseVal);
      }
      else
        stringBuilder.AppendEx((uint) intVal, padAmount, padChar, baseVal);
      return stringBuilder;
    }

    /// <summary>
    /// Converts a signed long value to a string and concatenate into StringBuilder. Assumes no padding and base ten.
    /// </summary>
    public static StringBuilder AppendEx(this StringBuilder stringBuilder, long intVal)
    {
      stringBuilder.AppendEx(intVal, 0U, '0', 10U);
      return stringBuilder;
    }

    /// <summary>
    /// Convert a signed long value to a string and concatenate into StringBuilder. Assumes base ten.
    /// </summary>
    public static StringBuilder AppendEx(
      this StringBuilder stringBuilder,
      long intVal,
      uint padAmount)
    {
      stringBuilder.AppendEx(intVal, padAmount, '0', 10U);
      return stringBuilder;
    }

    /// <summary>
    /// Convert a signed long value to a string and concatenate into StringBuilder. Assumes base ten.
    /// </summary>
    public static StringBuilder AppendEx(
      this StringBuilder stringBuilder,
      long intVal,
      uint padAmount,
      char padChar)
    {
      stringBuilder.AppendEx(intVal, padAmount, padChar, 10U);
      return stringBuilder;
    }

    /// <summary>
    /// Convert a float value to a string and concatenate into StringBuilder.
    /// </summary>
    public static StringBuilder AppendEx(
      this StringBuilder stringBuilder,
      float floatVal,
      uint decimalPlaces,
      uint padAmount,
      char padChar)
    {
      if (decimalPlaces == 0U)
      {
        int intVal = (double) floatVal < 0.0 ? (int) ((double) floatVal - 0.5) : (int) ((double) floatVal + 0.5);
        stringBuilder.AppendEx(intVal, padAmount, padChar, 10U);
      }
      else
      {
        int intVal1 = (int) floatVal;
        stringBuilder.AppendEx(intVal1, padAmount, padChar, 10U);
        stringBuilder.Append('.');
        float intVal2 = Math.Abs(floatVal - (float) intVal1);
        for (int index = 0; (long) index < (long) decimalPlaces; ++index)
          intVal2 *= 10f;
        stringBuilder.AppendEx((int) intVal2, decimalPlaces, '0', 10U);
      }
      return stringBuilder;
    }

    /// <summary>
    /// Convert a float value to a string and concatenate into StringBuilder. Assumes five decimal places, and no padding.
    /// </summary>
    public static StringBuilder AppendFormatEx(
      this StringBuilder stringBuilder,
      float floatVal)
    {
      stringBuilder.AppendEx(floatVal, 5U, 0U, '0');
      return stringBuilder;
    }

    public static StringBuilder AppendFormatEx(
      this StringBuilder stringBuilder,
      float floatVal,
      uint decimalPlaces)
    {
      stringBuilder.AppendEx(floatVal, decimalPlaces, 0U, '0');
      return stringBuilder;
    }

    /// <summary>
    /// Convert a float value to a string and concatenate into StringBuilder.
    /// </summary>
    public static StringBuilder AppendFormatEx(
      this StringBuilder stringBuilder,
      float floatVal,
      uint decimalPlaces,
      uint padAmount)
    {
      stringBuilder.AppendEx(floatVal, decimalPlaces, padAmount, '0');
      return stringBuilder;
    }

    /// <summary>Concatenate a formatted string with arguments.</summary>
    public static StringBuilder AppendFormatEx<TA>(
      this StringBuilder stringBuilder,
      string formatString,
      TA arg1)
      where TA : IConvertible
    {
      return stringBuilder.AppendFormatEx<TA, int, int, int, int>(formatString, arg1, 0, 0, 0, 0);
    }

    /// <summary>Concatenate a formatted string with arguments.</summary>
    public static StringBuilder AppendFormatEx<TA, TB>(
      this StringBuilder stringBuilder,
      string formatString,
      TA arg1,
      TB arg2)
      where TA : IConvertible
      where TB : IConvertible
    {
      return stringBuilder.AppendFormatEx<TA, TB, int, int, int>(formatString, arg1, arg2, 0, 0, 0);
    }

    /// <summary>Concatenate a formatted string with arguments.</summary>
    public static StringBuilder AppendFormatEx<TA, TB, TC>(
      this StringBuilder stringBuilder,
      string formatString,
      TA arg1,
      TB arg2,
      TC arg3)
      where TA : IConvertible
      where TB : IConvertible
      where TC : IConvertible
    {
      return stringBuilder.AppendFormatEx<TA, TB, TC, int, int>(formatString, arg1, arg2, arg3, 0, 0);
    }

    /// <summary>Concatenate a formatted string with arguments.</summary>
    public static StringBuilder AppendFormatEx<TA, TB, TC, TD>(
      this StringBuilder stringBuilder,
      string formatString,
      TA arg1,
      TB arg2,
      TC arg3,
      TD arg4)
      where TA : IConvertible
      where TB : IConvertible
      where TC : IConvertible
      where TD : IConvertible
    {
      return stringBuilder.AppendFormatEx<TA, TB, TC, TD, int>(formatString, arg1, arg2, arg3, arg4, 0);
    }

    /// <summary>Concatenate a formatted string with arguments.</summary>
    public static StringBuilder AppendFormatEx<TA, TB, TC, TD, TE>(
      this StringBuilder stringBuilder,
      string formatString,
      TA arg1,
      TB arg2,
      TC arg3,
      TD arg4,
      TE arg5)
      where TA : IConvertible
      where TB : IConvertible
      where TC : IConvertible
      where TD : IConvertible
      where TE : IConvertible
    {
      int startIndex = 0;
      for (int index1 = 0; index1 < formatString.Length; ++index1)
      {
        if (formatString[index1] == '{')
        {
          if (startIndex < index1)
            stringBuilder.Append(formatString, startIndex, index1 - startIndex);
          uint baseValue = 10;
          uint padding = 0;
          uint decimalPlaces = 5;
          int index2 = index1 + 1;
          char ch = formatString[index2];
          if (ch == '{')
          {
            stringBuilder.Append('{');
            index1 = index2 + 1;
          }
          else
          {
            index1 = index2 + 1;
            if (formatString[index1] == ':')
            {
              ++index1;
              while (formatString[index1] == '0')
              {
                ++index1;
                ++padding;
              }
              if (formatString[index1] == 'X')
              {
                ++index1;
                baseValue = 16U;
                if (formatString[index1] >= '0' && formatString[index1] <= '9')
                {
                  padding = (uint) formatString[index1] - 48U;
                  ++index1;
                }
              }
              else if (formatString[index1] == '.')
              {
                ++index1;
                decimalPlaces = 0U;
                while (formatString[index1] == '0')
                {
                  ++index1;
                  ++decimalPlaces;
                }
              }
            }
            while (formatString[index1] != '}')
              ++index1;
            switch (ch)
            {
              case '0':
                stringBuilder.AppendFormatValue<TA>(arg1, padding, baseValue, decimalPlaces);
                break;
              case '1':
                stringBuilder.AppendFormatValue<TB>(arg2, padding, baseValue, decimalPlaces);
                break;
              case '2':
                stringBuilder.AppendFormatValue<TC>(arg3, padding, baseValue, decimalPlaces);
                break;
              case '3':
                stringBuilder.AppendFormatValue<TD>(arg4, padding, baseValue, decimalPlaces);
                break;
              case '4':
                stringBuilder.AppendFormatValue<TE>(arg5, padding, baseValue, decimalPlaces);
                break;
            }
          }
          startIndex = index1 + 1;
        }
      }
      if (startIndex < formatString.Length)
        stringBuilder.Append(formatString, startIndex, formatString.Length - startIndex);
      return stringBuilder;
    }

    private static void AppendFormatValue<T>(
      this StringBuilder stringBuilder,
      T arg,
      uint padding,
      uint baseValue,
      uint decimalPlaces)
      where T : IConvertible
    {
      switch ((object) arg != null ? arg.GetTypeCode() : (!((object) arg is string) ? TypeCode.Object : TypeCode.String))
      {
        case TypeCode.Object:
        case TypeCode.Boolean:
          stringBuilder.Append(Convert.ToString((object) arg));
          break;
        case TypeCode.SByte:
          stringBuilder.AppendEx(arg.ToInt32((IFormatProvider) NumberFormatInfo.CurrentInfo), padding, '0', baseValue);
          break;
        case TypeCode.Byte:
          stringBuilder.AppendEx(arg.ToUInt32((IFormatProvider) NumberFormatInfo.CurrentInfo), padding, '0', baseValue);
          break;
        case TypeCode.Int16:
          stringBuilder.AppendEx(arg.ToInt32((IFormatProvider) NumberFormatInfo.CurrentInfo), padding, '0', baseValue);
          break;
        case TypeCode.UInt16:
          stringBuilder.AppendEx(arg.ToUInt32((IFormatProvider) NumberFormatInfo.CurrentInfo), padding, '0', baseValue);
          break;
        case TypeCode.Int32:
          stringBuilder.AppendEx(arg.ToInt32((IFormatProvider) NumberFormatInfo.CurrentInfo), padding, '0', baseValue);
          break;
        case TypeCode.UInt32:
          stringBuilder.AppendEx(arg.ToUInt32((IFormatProvider) NumberFormatInfo.CurrentInfo), padding, '0', baseValue);
          break;
        case TypeCode.Int64:
          stringBuilder.AppendEx(arg.ToInt64((IFormatProvider) NumberFormatInfo.CurrentInfo), padding, '0', baseValue);
          break;
        case TypeCode.UInt64:
          stringBuilder.AppendEx(arg.ToUInt64((IFormatProvider) NumberFormatInfo.CurrentInfo), padding, '0', baseValue);
          break;
        case TypeCode.Single:
        case TypeCode.Double:
          stringBuilder.AppendEx(arg.ToSingle((IFormatProvider) NumberFormatInfo.CurrentInfo), decimalPlaces, padding, '0');
          break;
        case TypeCode.String:
          stringBuilder.Append((object) arg);
          break;
      }
    }
  }
}
