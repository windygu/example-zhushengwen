namespace CCWin.Imaging
{
    using System;

    public sealed class ColorConverterEx
    {
        private static readonly int[] BT907 = new int[] { 0x84d, 0x1bf2, 0x2d1, 0x2710 };
        private static readonly int[] RMY = new int[] { 500, 0x1a3, 0x51, 0x3e8 };
        private static readonly int[] Y = new int[] { 0x12b, 0x24b, 0x72, 0x3e8 };

        private ColorConverterEx()
        {
        }

        private static byte GetGray(RGB rgb, int[] coefficient)
        {
            return (byte) ((((rgb.R * coefficient[0]) + (rgb.G * coefficient[1])) + (rgb.B * coefficient[2])) / coefficient[3]);
        }

        public static RGB HslToRgb(HSL hsl)
        {
            RGB rgb = new RGB();
            HslToRgb(hsl, rgb);
            return rgb;
        }

        public static void HslToRgb(HSL hsl, RGB rgb)
        {
            if (hsl.Saturation == 0.0)
            {
                rgb.R = rgb.G = rgb.B = (byte) (hsl.Luminance * 255.0);
            }
            else
            {
                double hue = ((double) hsl.Hue) / 360.0;
                double v2 = (hsl.Luminance < 0.5) ? (hsl.Luminance * (1.0 + hsl.Saturation)) : ((hsl.Luminance + hsl.Saturation) - (hsl.Luminance * hsl.Saturation));
                double v1 = (2.0 * hsl.Luminance) - v2;
                rgb.R = (byte) (255.0 * HueToRGB(v1, v2, hue + 0.33333333333333331));
                rgb.G = (byte) (255.0 * HueToRGB(v1, v2, hue));
                rgb.B = (byte) (255.0 * HueToRGB(v1, v2, hue - 0.33333333333333331));
            }
        }

        private static double HueToRGB(double v1, double v2, double vH)
        {
            if (vH < 0.0)
            {
                vH++;
            }
            if (vH > 1.0)
            {
                vH--;
            }
            if ((6.0 * vH) < 1.0)
            {
                return (v1 + (((v2 - v1) * 6.0) * vH));
            }
            if ((2.0 * vH) < 1.0)
            {
                return v2;
            }
            if ((3.0 * vH) < 2.0)
            {
                return (v1 + (((v2 - v1) * (0.66666666666666663 - vH)) * 6.0));
            }
            return v1;
        }

        public static RGB RgbToGray(RGB source)
        {
            RGB dest = new RGB();
            RgbToGray(source, dest);
            return dest;
        }

        public static RGB RgbToGray(RGB source, GrayscaleStyle style)
        {
            RGB dest = new RGB();
            RgbToGray(source, dest, style);
            return dest;
        }

        public static void RgbToGray(RGB source, RGB dest)
        {
            RgbToGray(source, dest, GrayscaleStyle.BT907);
        }

        public static void RgbToGray(RGB source, RGB dest, GrayscaleStyle style)
        {
            byte gray = 0x7f;
            switch (style)
            {
                case GrayscaleStyle.BT907:
                    gray = GetGray(source, BT907);
                    break;

                case GrayscaleStyle.RMY:
                    gray = GetGray(source, RMY);
                    break;

                case GrayscaleStyle.Y:
                    gray = GetGray(source, Y);
                    break;
            }
            dest.R = dest.G = dest.B = gray;
        }

        public static HSL RgbToHsl(RGB rgb)
        {
            HSL hsl = new HSL();
            RgbToHsl(rgb, hsl);
            return hsl;
        }

        public static void RgbToHsl(RGB rgb, HSL hsl)
        {
            double r = ((double) rgb.R) / 255.0;
            double g = ((double) rgb.G) / 255.0;
            double b = ((double) rgb.G) / 255.0;
            double min = Math.Min(Math.Min(r, g), b);
            double max = Math.Max(Math.Max(r, g), b);
            double delta = max - min;
            hsl.Luminance = (max + min) / 2.0;
            if (delta == 0.0)
            {
                hsl.Hue = 0;
                hsl.Saturation = 0.0;
            }
            else
            {
                double hue;
                hsl.Saturation = (hsl.Luminance < 0.5) ? (delta / (max + min)) : (delta / ((2.0 - max) - min));
                double del_r = (((max - r) / 6.0) + (delta / 2.0)) / delta;
                double del_g = (((max - g) / 6.0) + (delta / 2.0)) / delta;
                double del_b = (((max - b) / 6.0) + (delta / 2.0)) / delta;
                if (r == max)
                {
                    hue = del_b - del_g;
                }
                else if (g == max)
                {
                    hue = (0.33333333333333331 + del_r) - del_b;
                }
                else
                {
                    hue = (0.66666666666666663 + del_g) - del_r;
                }
                if (hue < 0.0)
                {
                    hue++;
                }
                if (hue > 1.0)
                {
                    hue--;
                }
                hsl.Hue = (int) (hue * 360.0);
            }
        }
    }
}

