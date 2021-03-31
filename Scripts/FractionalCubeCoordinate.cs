using System;

namespace HexagonSystem
{
    internal struct FractionalCubeCoordinate
    {
        #region Fields
        private double _q, _r, _s;
        #endregion

        #region Properties
        public double Q { get => _q; }
        public double R { get => _r; }
        public double S { get => _s; }
        #endregion

        #region Methods
        public CubeCoordinate ToCubeCoordinate()
        {
            int q = (int)Math.Round(_q);
            int r = (int)Math.Round(_r);
            int s = (int)Math.Round(_s);
            double q_diff = Math.Abs(q - _q);
            double r_diff = Math.Abs(r - _r);
            double s_diff = Math.Abs(s - _s);
            if (q_diff > r_diff && q_diff > s_diff) {
                q = -r - s;
            } else if (r_diff > s_diff)
            {
                r = -q - s;
            }
            else
            {
                s = -q - r;
            }
            return new CubeCoordinate(q, r, s);
        }

        private static float Lerp(double a, double b, double t)
        {
            return (float)(a * (1 - t) + b * t);
        }
        public static FractionalCubeCoordinate Lerp(FractionalCubeCoordinate a, FractionalCubeCoordinate b, double t)
        {
            return new FractionalCubeCoordinate(Lerp(a.Q, b.Q, t),
                                                Lerp(a.R, b.R, t),
                                                Lerp(a.S, b.S, t));
        }
        public static FractionalCubeCoordinate Lerp(CubeCoordinate a, CubeCoordinate b, double t)
        {
            return new FractionalCubeCoordinate(Lerp(a.Q, b.Q, t),
                                                Lerp(a.R, b.R, t),
                                                Lerp(a.S, b.S, t));
        }
        #endregion

        #region Constructor
        public FractionalCubeCoordinate(double q, double r, double s)
        {
            _q = q;
            _r = r;
            _s = s;
        }
        #endregion
    }
}
