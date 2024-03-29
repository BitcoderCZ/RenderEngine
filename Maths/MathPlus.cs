﻿using GameEngine.Maths.Vectors;
using System;

namespace GameEngine.Maths
{
    public static class MathPlus
    {
        public static Vector2I Abs(this Vector2I v) => new Vector2I(Math.Abs(v.X), Math.Abs(v.Y));
        public static Vector2F Abs(this Vector2F v) => new Vector2F(Math.Abs(v.X), Math.Abs(v.Y));
        public static Vector2D Abs(this Vector2D v) => new Vector2D(Math.Abs(v.X), Math.Abs(v.Y));

        public static float Lenght(this Vector2I v)
            => (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);
        public static float Lenght(this Vector2F v)
            => (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);
        public static double Lenght(this Vector2D v)
            => Math.Sqrt(v.X * v.X + v.Y * v.Y);

        public static float Distance(this Vector2I a, Vector2I b)
        {
            int dx = a.X - b.X;
            int dy = a.Y - b.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
        public static float Distance(this Vector2F a, Vector2F b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
        public static double Distance(this Vector2D a, Vector2D b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static Vector2F Normalized(this Vector2F v)
        {
            float length = v.Lenght();
            if (length != 0f)
                return v / length;
            else
                return v;
        }
        public static Vector2D Normalized(this Vector2D v)
        {
            double length = v.Lenght();
            if (length != 0f)
                return v / length;
            else
                return v;
        }

        public static int Dot(this Vector2I a, Vector2I b)
            => a.X * b.X + a.Y * b.Y;
        public static float Dot(this Vector2F a, Vector2F b)
            => a.X * b.X + a.Y * b.Y;
        public static double Dot(this Vector2D a, Vector2D b)
            => a.X * b.X + a.Y * b.Y;

        public static int Cross(this Vector2I a, Vector2I b)
            => a.X * b.Y - a.Y * b.X;
        public static float Cross(this Vector2F a, Vector2F b)
            => a.X * b.Y - a.Y * b.X;
        public static double Cross(this Vector2D a, Vector2D b)
            => a.X * b.Y - a.Y * b.X;


        public static float Round(float value)
        {
            float dec = value % 1f;
            if (dec < 0.5f)
                return value - dec;
            else
                return value + (1f - dec);
        }
        public static double Round(double value)
        {
            double dec = value % 1d;
            if (dec < 0.5d)
                return value - dec;
            else
                return value + (1d - dec);
        }

        public static float Floor(float value)
        {
            float dec = value % 1f;

            return value - dec;
        }
        public static double Floor(double value)
        {
            double dec = value % 1d;

            return value - dec;
        }

        public static float Ceil(float value)
        {
            float dec = value % 1f;

            if (dec != 0f)
                return value + (1f - dec);
            else
                return value;
        }
        public static double Ceil(double value)
        {
            double dec = value % 1d;

            if (dec != 0d)
                return value + (1d - dec);
            else
                return value;
        }


        public static float Round(float value, int decimalPlaces)
        {
            float pow = (float)Math.Pow(10f, decimalPlaces);

            value *= pow;

            float dec = value % 1f;
            if (dec < 0.5f)
                return (value - dec) / pow;
            else
                return (value + (1f - dec)) / pow;
        }
        public static double Round(double value, int decimalPlaces)
        {
            double pow = Math.Pow(10d, decimalPlaces);

            value *= pow;

            double dec = value % 1d;
            if (dec < 0.5d)
                return (value - dec) / pow;
            else
                return (value + (1d - dec)) / pow;
        }

        public static float Floor(float value, int decimalPlaces)
        {
            float pow = (float)Math.Pow(10f, decimalPlaces);

            value *= pow;

            float dec = value % 1f;

            return (value - dec) / pow;
        }
        public static double Floor(double value, int decimalPlaces)
        {
            double pow = Math.Pow(10d, decimalPlaces);

            value *= pow;

            double dec = value % 1d;

            return (value - dec) / pow;
        }

        public static float Ceil(float value, int decimalPlaces)
        {
            float pow = (float)Math.Pow(10f, decimalPlaces);

            value *= pow;

            float dec = value % 1f;

            if (dec != 0f)
                return (value + (1f - dec)) / pow;
            else
                return value / pow;
        }
        public static double Ceil(double value, int decimalPlaces)
        {
            double pow = Math.Pow(10d, decimalPlaces);

            value *= pow;

            double dec = value % 1d;

            if (dec != 0d)
                return (value + (1d - dec)) / pow;
            else
                return value / pow;
        }



        public static int RoundToInt(float value)
        {
            float dec = value % 1f;
            if (dec < 0.5f)
                return (int)(value - dec);
            else
                return (int)(value + (1f - dec));
        }
        public static int RoundToInt(double value)
        {
            double dec = value % 1d;
            if (dec < 0.5d)
                return (int)(value - dec);
            else
                return (int)(value + (1d - dec));
        }

        public static int FloorToInt(float value)
        {
            float dec = value % 1f;

            return (int)(value - dec);
        }
        public static int FloorToInt(double value)
        {
            double dec = value % 1d;

            return (int)(value - dec);
        }

        public static int CeilToInt(float value)
        {
            float dec = value % 1f;

            if (dec != 0f)
                return (int)(value + (1f - dec));
            else
                return (int)value;
        }
        public static int CeilToInt(double value)
        {
            double dec = value % 1d;

            if (dec != 0d)
                return (int)(value + (1d - dec));
            else
                return (int)value;
        }



        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else return value;
        }
        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else return value;
        }
        public static double Clamp(double value, double min, double max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else return value;
        }



        public static int FlipAround(int value, int center)
        {
            int i = value - center;
            i = -i;
            return i + center;
        }
        public static float FlipAround(float value, float center)
        {
            float i = value - center;
            i = -i;
            return i + center;
        }
        public static double FlipAround(double value, double center)
        {
            double i = value - center;
            i = -i;
            return i + center;
        }



        public static int FindOverlapping(int start1, int end1, int start2, int end2)
            => Math.Max(0, Math.Min(end1, end2) - Math.Max(start1, start2) + 1);
        public static float FindOverlapping(float start1, float end1, float start2, float end2)
            => Math.Max(0, Math.Min(end1, end2) - Math.Max(start1, start2) + 1);
        public static double FindOverlapping(double start1, double end1, double start2, double end2)
            => Math.Max(0, Math.Min(end1, end2) - Math.Max(start1, start2) + 1);
    }
}
