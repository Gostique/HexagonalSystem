using System;

namespace HexagonSystem
{
    [Serializable]
    public struct CubeCoordinate
    {

        #region Fields
        // in oder x, z, y
        public int _q, _r, _s;
        private static CubeCoordinate[] CubeCoordinate_directions = new CubeCoordinate[]
        {
        new CubeCoordinate(1, 0, -1),
        new CubeCoordinate(1, -1, 0),
        new CubeCoordinate(0, -1, 1),
        new CubeCoordinate(-1, 0, 1),
        new CubeCoordinate(-1, 1, 0),
        new CubeCoordinate(0, 1, -1)
        };
        #endregion

        #region Properties
        public int Q { get => _q; }
        public int R { get => _r; }
        public int S { get => _s; }
        #endregion

        #region Constructors
        public CubeCoordinate(int q, int r)
        {
            _q = q;
            _r = r;
            _s = -q - r;

            if (_q + _r + _s != 0) throw new ArgumentException("CubeCoordinate(int q, int r, int s) Error. The sum of q, r and s must be equal to 0.");
        }
        public CubeCoordinate(int q, int r, int s)
        {
            _q = q;
            _r = r;
            _s = s;

            if (_q + _r + _s != 0) throw new ArgumentException("CubeCoordinate(int q, int r, int s) Error. The sum of q, r and s must be equal to 0.");
        }
        #endregion

        #region Static Methods
        public static CubeCoordinate CubeCoordinate_direction(int direction)
        {
            return CubeCoordinate_directions[direction % 6];
        }
        public static CubeCoordinate CubeCoordinate_neighbor(CubeCoordinate hex, int direction)
        {
            return CubeCoordinate_add(hex, CubeCoordinate_direction(direction));
        }
        public static CubeCoordinate CubeCoordinate_add(CubeCoordinate a, CubeCoordinate b)
        {
            return new CubeCoordinate(a._q + b._q, a._r + b._r, a._s + b._s);
        }
        public static CubeCoordinate CubeCoordinate_subtract(CubeCoordinate a, CubeCoordinate b)
        {
            return new CubeCoordinate(a._q - b._q, a._r - b._r, a._s - b._s);
        }
        public static CubeCoordinate CubeCoordinate_multiply(CubeCoordinate a, int k)
        {
            return new CubeCoordinate(a._q * k, a._r * k, a._s * k);
        }

        public static int CubeCoordinate_length(CubeCoordinate cube)
        {
            return (int)((Math.Abs(cube._q) + Math.Abs(cube._r) + Math.Abs(cube._s)) / 2);
        }
        public static int CubeCoordinate_distance(CubeCoordinate a, CubeCoordinate b)
        {
            return CubeCoordinate_length(CubeCoordinate_subtract(a, b));
        }

        public override bool Equals(object obj)
        {
            return obj is CubeCoordinate coordinate &&
                   this == coordinate;
        }
        public override int GetHashCode()
        {
            int hashCode = -1835713032;
            hashCode = hashCode * -1521134295 + _q.GetHashCode();
            hashCode = hashCode * -1521134295 + _r.GetHashCode();
            hashCode = hashCode * -1521134295 + _s.GetHashCode();
            return hashCode;
        }
        #endregion

        #region Operator
        public static CubeCoordinate operator +(CubeCoordinate a, CubeCoordinate b)
        {
            return CubeCoordinate_add(a, b);
        }
        public static CubeCoordinate operator -(CubeCoordinate a, CubeCoordinate b)
        {
            return CubeCoordinate_subtract(a, b);
        }
        public static CubeCoordinate operator *(CubeCoordinate a, int b)
        {
            return CubeCoordinate_multiply(a, b);
        }
        public static bool operator ==(CubeCoordinate a, CubeCoordinate b)
        {
            return a._q == b._q && a._r == b._r && a._s == b._s;
        }
        public static bool operator !=(CubeCoordinate a, CubeCoordinate b)
        {
            return !(a == b);
        }
        #endregion

    }
}