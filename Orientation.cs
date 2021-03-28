using System;

namespace HexagonSystem
{
    internal struct Orientation
    {
        public double _f0, _f1, _f2, _f3;
        public double _b0, _b1, _b2, _b3;
        public double _startAngle;

        public Orientation(
                double f0, double f1, double f2, double f3,
                double b0, double b1, double b2, double b3,
                double startAngle)
        {
            _f0 = f0; _f1 = f1; _f2 = f2; _f3 = f3;
            _b0 = b0; _b1 = b1; _b2 = b2; _b3 = b3;
            _startAngle = startAngle;
        }

        public static Orientation pointyOrientation = new Orientation(
                Math.Sqrt(3.0), Math.Sqrt(3.0) / 2.0, 0.0, 3.0 / 2.0,
                Math.Sqrt(3.0) / 3.0, -1.0 / 3.0, 0.0, 2.0 / 3.0,
                0.5);
        public static Orientation flatOrientation = new Orientation(
                3.0 / 2.0, 0.0, Math.Sqrt(3.0) / 2.0, Math.Sqrt(3.0),
                2.0 / 3.0, 0.0, -1.0 / 3.0, Math.Sqrt(3.0) / 3.0,
                0.0);

    }
}