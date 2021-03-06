﻿//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.Drawing
{
    /// <summary>Represents an ARGB color.</summary>
    [Serializable, StructLayout(LayoutKind.Sequential), DebuggerDisplay("\\{ARGB = ({A}, {R}, {G}, {B})\\}")]
    public struct ColorRgb : IEquatable<Color>
    {
        [DebuggerStepThrough]
        internal static void Checkdouble(double value, string name)
        {
            Checkdouble(value, name, 0.0, 1.0);
        }

        [DebuggerStepThrough]
        internal static void Checkdouble(double value, string name, double min, double max)
        {
            if ((value < min) || (value > max))
            {
                throw new ArgumentException(string.Format("Check Float Exception", name, value, min, max));
            }
        }

        /// <summary>Initializes a new instance of the <see cref="T:LukeSw.Drawing.ColorRgb"/> structure
        /// from the specified double values.</summary>
        /// <param name="alpha">The alpha component value. Valid values are 0 through 1.</param>
        /// <param name="red">The red component value. Valid values are 0 through 1.</param>
        /// <param name="green">The green component value. Valid values are 0 through 1.</param>
        /// <param name="blue">The blue component value. Valid values are 0 through 1.</param>
        public ColorRgb(double alpha, double red, double green, double blue)
        {
            Checkdouble(alpha, "alpha");
            Checkdouble(red, "red");
            Checkdouble(green, "green");
            Checkdouble(blue, "blue");
            this.A = alpha;
            this.R = red;
            this.G = green;
            this.B = blue;
        }

        /// <summary>Initializes a new instance of the <see cref="T:LukeSw.Drawing.ColorRgb"/> structure
        /// from the specified double values. The alpha value is implicitly 1 (fully opaque).</summary>
        /// <param name="red">The red component value. Valid values are 0 through 1.</param>
        /// <param name="green">The green component value. Valid values are 0 through 1.</param>
        /// <param name="blue">The blue component value. Valid values are 0 through 1.</param>
        public ColorRgb(double red, double green, double blue)
        {
            Checkdouble(red, "red");
            Checkdouble(green, "green");
            Checkdouble(blue, "blue");
            this.A = 1.0;
            this.R = red;
            this.G = green;
            this.B = blue;
        }

        /// <summary>Gets the alpha component value.</summary>
        public readonly double A;
        /// <summary>Gets the red component value.</summary>
        public readonly double R;
        /// <summary>Gets the green component value.</summary>
        public readonly double G;
        /// <summary>Gets the blue component value.</summary>
        public readonly double B;

        /// <summary>Converts this <see cref="T:LukeSw.Drawing.ColorRgb" /> structure to a human-readable string.</summary>
        /// <returns>String that consists of the ARGB component names and their values.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x20);
            builder.Append(GetType().Name);
            builder.Append(" [");
            builder.Append("A=");
            builder.Append(this.A);
            builder.Append(", R=");
            builder.Append(this.R);
            builder.Append(", G=");
            builder.Append(this.G);
            builder.Append(", B=");
            builder.Append(this.B);
            builder.Append("]");
            return builder.ToString();
        }

        public override bool Equals(object obj)
        {
            return (obj is Color && ToColor() == (Color)obj) ||
                (obj is ColorRgb && ToColor() == (ColorRgb)obj);
        }

        public static bool operator ==(ColorRgb c1, ColorRgb c2)
        {
            return c1.A == c2.A && c1.R == c2.R && c1.G == c2.G && c1.B == c2.B;
        }

        public static bool operator !=(ColorRgb c1, ColorRgb c2)
        {
            return !(c1 == c2);
        }

        public bool Equals(Color other)
        {
            return ToColor() == other;
        }

        public override int GetHashCode()
        {
            return A.GetHashCode() ^ R.GetHashCode() ^ G.GetHashCode() ^ B.GetHashCode();
        }

        public static ColorRgb FromColor(Color color)
        {
            return new ColorRgb(
                color.A / 255.0,
                color.R / 255.0,
                color.G / 255.0,
                color.B / 255.0);
        }

        public static ColorRgb FromColor(double alpha, Color baseColor)
        {
            return new ColorRgb(
                alpha,
                baseColor.R / 255.0,
                baseColor.G / 255.0,
                baseColor.B / 255.0);
        }

        public static implicit operator ColorRgb(Color color) { return ColorRgb.FromColor(color); }

        /// <summary>Returns a <see cref="T:System.Drawing.Color" /> reprezentation of current instance.</summary>
        /// <returns>A <see cref="T:System.Drawing.Color" /> reprezentation of current instance</returns>
        public Color ToColor()
        {
            return Color.FromArgb((int)(255.0 * A + .5), (int)(255.0 * R + .5), (int)(255.0 * G + .5), (int)(255.0 * B + .5));
        }

        public static explicit operator Color(ColorRgb rgb) { return rgb.ToColor(); }

        public static ColorRgb FromColor(ColorHsv hsv)
        {
            if (hsv.S == 0.0)
            {
                return new ColorRgb(hsv.A, hsv.V, hsv.V, hsv.V);
            }
            double H = hsv.H / 60.0;
            int h = (int)H;
            double f = H - h;
            double v1 = hsv.V * (1.0 - hsv.S);
            double v2 = hsv.V * (1.0 - hsv.S * f);
            double v3 = hsv.V * (1.0 - hsv.S * (1.0 - f));
            switch (h)
            {
                case 0:
                    return new ColorRgb(hsv.A, hsv.V, v3, v1);
                case 1:
                    return new ColorRgb(hsv.A, v2, hsv.V, v1);
                case 2:
                    return new ColorRgb(hsv.A, v1, hsv.V, v3);
                case 3:
                    return new ColorRgb(hsv.A, v1, v2, hsv.V);
                case 4:
                    return new ColorRgb(hsv.A, v3, v1, hsv.V);
                case 5:
                    return new ColorRgb(hsv.A, hsv.V, v1, v2);
                default:
                    throw new InvalidOperationException();
            }
        }

        public static implicit operator ColorRgb(ColorHsv hsv) { return ColorRgb.FromColor(hsv); }

        /// <summary>Returns a <see cref="T:LukeSw.Drawing.ColorHsv" /> reprezentation of current instance.</summary>
        /// <returns>A <see cref="T:LukeSw.Drawing.ColorHsv" /> reprezentation of current instance</returns>
        public ColorHsv ToColorHsv()
        {
            double max = Math.Max(R, Math.Max(G, B));
            double min = Math.Min(R, Math.Min(G, B));
            if (max == min)
            {
                return new ColorHsv(A, 0.0, 0.0, max);
            }
            return new ColorHsv(A, RgbToHue(max, min), 1.0 - min / max, max);
        }

        public static implicit operator ColorHsv(ColorRgb rgb) { return rgb.ToColorHsv(); }

        private static double HueToRgb(double value1, double value2, double hue)
        {
            if (hue < 0.0)
            {
                hue += 360.0;
            }
            if (hue > 360.0)
            {
                hue -= 360.0;
            }
            if (hue < 60.0)
            {
                return value1 + (value2 - value1) * hue / 60.0;
            }
            if (hue < 180.0)
            {
                return value2;
            }
            if (hue < 240.0)
            {
                return value1 + (value2 - value1) * (240.0 - hue) / 60.0;
            }
            return value1;
        }

        public static ColorRgb FromColor(ColorHsl hsl)
        {
            if (hsl.S == 0.0)
            {
                return new ColorRgb(hsl.A, hsl.L, hsl.L, hsl.L);
            }
            double value2 = hsl.L * hsl.S;
            if (hsl.L <= .5)
            {
                value2 += hsl.L;
            }
            else
            {
                value2 = hsl.L + hsl.S - value2;
            }
            double value1 = 2f * hsl.L - value2;
            return new ColorRgb(hsl.A,
                HueToRgb(value1, value2, hsl.H + 120.0),
                HueToRgb(value1, value2, hsl.H),
                HueToRgb(value1, value2, hsl.H - 120.0));
        }

        public static implicit operator ColorRgb(ColorHsl hsl) { return ColorRgb.FromColor(hsl); }

        private double RgbToHue(double max, double min)
        {
            double del = max - min;
            if (del == 0.0)
            {
                return 0.0;
            }
            if (max == R && G >= B)
            {
                return 60.0 * (G - B) / del;
            }
            if (max == R)
            {
                return 60.0 * (G - B) / del + 360.0;
            }
            if (max == G)
            {
                return 60.0 * (B - R) / del + 120.0;
            }
            if (max == B)
            {
                return 60.0 * (R - G) / del + 240.0;
            }
            return 0.0;
        }

        /// <summary>Returns a <see cref="T:LukeSw.Drawing.ColorHsl" /> reprezentation of current instance.</summary>
        /// <returns>A <see cref="T:LukeSw.Drawing.ColorHsl" /> reprezentation of current instance</returns>
        public ColorHsl ToColorHsl()
        {
            double max = Math.Max(R, Math.Max(G, B));
            double min = Math.Min(R, Math.Min(G, B));
            double sum = max + min;
            double l = sum / 2f;
            if (max == min)
            {
                return new ColorHsl(A, 0.0, 0.0, l);
            }
            double s = 0.0;
            if (l <= .5)
            {
                s = 1.0 - 2f * min / sum; // del / sum;
            }
            else if (l > .5)
            {
                s = 1.0 - 2.0 * (1.0 - max) / (2.0 - sum); // del / (2f - sum);
            }
            return new ColorHsl(A, RgbToHue(max, min), s, l);
        }

        public static implicit operator ColorHsl(ColorRgb rgb) { return rgb.ToColorHsl(); }
    }
}
