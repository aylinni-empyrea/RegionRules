using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RegionRules.Extensions
{
  public static class StringExtensions
  {
    public static bool CultureSensitiveContains(this string src, string value)
      => CultureInfo.CurrentCulture.CompareInfo.IndexOf(src, value, CompareOptions.IgnoreCase) >= 0;

    public static string FormatWith(this string input, params object[] args) => string.Format(input, args);

    public static string JoinWithVerb(this IEnumerable<string> src, string verb = "and")
    {
      if (verb == null)
        throw new ArgumentNullException(nameof(verb));

      var tmp = src.ToArray();
      var sb = new StringBuilder(string.Join(", ", tmp, 0, tmp.Length - 1));
      return sb.Append($" {verb} {tmp[tmp.Length - 1]}").ToString();
    }

    public static Color ToColor(this string hexString)
    {
      if (hexString.StartsWith("#"))
        hexString = hexString.Substring(1);

      var hex = uint.Parse(hexString, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

      var color = Color.White;

      switch (hexString.Length)
      {
        case 8:
          color.A = (byte) (hex >> 24);
          color.R = (byte) (hex >> 16);
          color.G = (byte) (hex >> 8);
          color.B = (byte) hex;
          break;
        case 6:
          color.R = (byte) (hex >> 16);
          color.G = (byte) (hex >> 8);
          color.B = (byte) hex;
          break;
        default:
          throw new InvalidOperationException("Invald hex representation of an ARGB or RGB color value.");
      }

      return color;
    }
  }
}