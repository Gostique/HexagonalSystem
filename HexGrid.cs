using System;
using System.Collections.Generic;
using UnityEngine;

namespace HexagonSystem
{
    public class HexGrid
    {

        #region Fields
        private Orientation _orientation;
        internal Vector2 m_size;
        internal Vector2 m_origin;

        public enum HexagonOrientation
        {
            Pointy,
            Flat
        }
        public enum OffsetRaw
        {
            Right = -1,
            Left = 1
        }
        #endregion

        #region Properties
        public HexagonOrientation HexOrientation { get; private set; }
        public OffsetRaw Offset { get; private set; }
        #endregion

        #region Constructors
        public HexGrid(HexagonOrientation orientation, Vector2 size, Vector2 origin, OffsetRaw offset = OffsetRaw.Right)
        {
            HexOrientation = orientation;
            Offset = offset;
            _orientation = orientation == HexagonOrientation.Pointy ? Orientation.pointyOrientation : Orientation.flatOrientation;
            m_size = size;
            m_origin = origin;
        }
        #endregion

        #region Methods
        public Vector2 CubeCoordinateToWorld(CubeCoordinate cubeCoordinate)
        {
            double x = (_orientation._f0 * cubeCoordinate.Q + _orientation._f1 * cubeCoordinate.R) * m_size.x;
            double y = (_orientation._f2 * cubeCoordinate.Q + _orientation._f3 * cubeCoordinate.R) * m_size.y;
            return new Vector2((float)x + m_origin.x, (float)y + m_origin.y);
        }
        public CubeCoordinate WorldToCoordinate(Vector2 point)
        {
            Vector2 pt = new Vector2(
                                (point.x - m_origin.x) / m_size.x,
                                (point.y - m_origin.y) / m_size.y
                                );
            double q = _orientation._b0 * pt.x + _orientation._b1 * pt.y;
            double r = _orientation._b2 * pt.x + _orientation._b3 * pt.y;
            return new FractionalCubeCoordinate(q, r, -q - r).ToCubeCoordinate();
        }



        internal Vector2 CubeCoordinateCornerOffset(int corner)
        {
            double angle = 2.0 * Math.PI * (_orientation._startAngle + corner) / 6;
            return new Vector2((float)(m_size.x * Math.Cos(angle)), (float)(m_size.y * Math.Sin(angle)));
        }
        public Vector2[] CubeCoordinateCorners(CubeCoordinate cubeCoordinate)
        {
            Vector2[] corners = new Vector2[6];
            Vector2 center = CubeCoordinateToWorld(cubeCoordinate);
            for (int i = 0; i < corners.Length; i++)
            {
                Vector2 offset = CubeCoordinateCornerOffset(i);
                corners[i] = new Vector2(center.x + offset.x, center.y + offset.y);
            }
            return corners;
        }



        public OffsetCoordinate CubeToOffset(CubeCoordinate cubeCoordinate)
        {
            if (HexOrientation == HexagonOrientation.Flat)
                return CubeToFlatOffset(cubeCoordinate);
            else //HexOrientation == HexagonOrientation.Pointy
                return CubeToPointyOffset(cubeCoordinate);
        }
        private OffsetCoordinate CubeToFlatOffset(CubeCoordinate cubeCoordinate)
        {
            int col = cubeCoordinate.Q;
            int row = cubeCoordinate.R + (int)((cubeCoordinate.Q + (int)Offset * (cubeCoordinate.Q & 1)) / 2);
            return new OffsetCoordinate(col, row);
        }
        private OffsetCoordinate CubeToPointyOffset(CubeCoordinate cubeCoordinate)
        {
            int col = cubeCoordinate.Q + (int)((cubeCoordinate.R + (int)Offset * (cubeCoordinate.R & 1)) / 2);
            int row = cubeCoordinate.R;
            return new OffsetCoordinate(col, row);
        }

        public CubeCoordinate OffsetToCube(OffsetCoordinate offsetCoordinate)
        {
            if (HexOrientation == HexagonOrientation.Flat)
                return FlatOffsetToCube(offsetCoordinate);
            else //HexOrientation == HexagonOrientation.Pointy
                return PointyOffsetToCube(offsetCoordinate);
        }
        private CubeCoordinate FlatOffsetToCube(OffsetCoordinate offsetCoordinate)
        {
            int q = offsetCoordinate.m_col;
            int r = offsetCoordinate.m_row - (int)((offsetCoordinate.m_col + (int)Offset * (offsetCoordinate.m_col & 1)) / 2);
            int s = -q - r;
            return new CubeCoordinate(q, r, s);
        }
        private CubeCoordinate PointyOffsetToCube(OffsetCoordinate offsetCoordinate)
        {
            int q = offsetCoordinate.m_col - (int)((offsetCoordinate.m_row + (int)Offset * (offsetCoordinate.m_row & 1)) / 2);
            int r = offsetCoordinate.m_row;
            int s = -q - r;
            return new CubeCoordinate(q, r, s);
        }



        /// <summary>
        /// Build the line of hexagon between two hexagons.
        /// </summary>
        /// <param name="start">The CubeCoordinate of the first hexagon of the line.</param>
        /// <param name="end">The CubeCoordinate of the last hexagon of the line.</param>
        /// <returns>An array of CubeCoordinate of each hexagon between start (included) and end (included)</returns>
        public static CubeCoordinate[] DrawLinedraw(CubeCoordinate start, CubeCoordinate end)
        {
            int N = CubeCoordinate.CubeCoordinate_distance(start, end);
            CubeCoordinate[] results = new CubeCoordinate[N];
            double step = 1.0 / Math.Max(N, 1);
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = FractionalCubeCoordinate.Lerp(start, end, step * i).ToCubeCoordinate();
            }
            return results;
        }
        /// <summary>
        /// Build the area of CubeCoordinate around the center in a defined range.
        /// </summary>
        /// <param name="center">The center of the area.</param>
        /// <param name="range">The distance in hexagon to reach the edge of the area.</param>
        /// <returns>An array of CubeCoordinate representing the area around the center.</returns>
        public static CubeCoordinate[] DrawArea(CubeCoordinate center, int range)
        {
            if (range < 0) range = -range;
            List<CubeCoordinate> results = new List<CubeCoordinate>();
            for (int x = -range; x <= range; x++)
            {
                for (int y = Mathf.Max(-range, -x-range); y <= Mathf.Min(range, -x+range); y++)
                {
                    results.Add(center + new CubeCoordinate(x,y));
                }
            }
            return results.ToArray();
        }
        /// <summary>
        /// Build the area of CubeCoordinate that intersect two areas.
        /// </summary>
        /// <param name="aCenter">The center of the first area.</param>
        /// <param name="aRange">The distance in hexagon to reach the edge of the first area./param>
        /// <param name="bCenter">The center of the second area.</param>
        /// <param name="bRange">The distance in hexagon to reach the edge of the second area.</param>
        /// <returns>An array of CubeCoordinate representing the intersection between the two areas</returns>
        public static CubeCoordinate[] DrawIntersectArea(CubeCoordinate aCenter, int aRange, CubeCoordinate bCenter, int bRange)
        {
            if (aRange < 0) aRange = -aRange;
            if (bRange < 0) bRange = -bRange;

            List<CubeCoordinate> results = new List<CubeCoordinate>();

            int xMin = Mathf.Max(aCenter.Q - aRange, bCenter.Q - bRange);
            int xMax = Mathf.Min(aCenter.Q + aRange, bCenter.Q + bRange);
            int yMin = Mathf.Max(aCenter.R - aRange, bCenter.R - bRange);
            int yMax = Mathf.Min(aCenter.R + aRange, bCenter.R + bRange);
            int zMin = Mathf.Max(aCenter.S - aRange, bCenter.S - bRange);
            int zMax = Mathf.Min(aCenter.S + aRange, bCenter.S + bRange);

            for (int x = xMin; x <= xMax; x++)
            {
                for (int y = Mathf.Max(yMin, -x-zMax); y <= Mathf.Min(yMax, -x-zMin); y++)
                {
                    results.Add(new CubeCoordinate(x,y));
                }
            }

            return results.ToArray();
        }
        /// <summary>
        /// Rotate the CubeCoordinate around a center on the hexagonal grid.
        /// </summary>
        /// <param name="target">The CubeCoordinate to rotate.</param>
        /// <param name="center">The center of the rotation.</param>
        /// <param name="rotation">The angle of rotation.</param>
        /// <returns>A CubeCoordinate representing the rotation around the center on the hexagonal grid</returns>
        public static CubeCoordinate RotateCoordinate(CubeCoordinate target, CubeCoordinate center, CubeRotation rotation)
        {
            CubeCoordinate vectorIN = target - center;

            CubeCoordinate vectorOut = vectorIN;
            switch (rotation)
            {
                case CubeRotation.CW_60:
                case CubeRotation.CCW_300:
                    vectorOut = new CubeCoordinate(-vectorIN.S, -vectorIN.Q, -vectorIN.R);
                    break;
                case CubeRotation.CW_120:
                case CubeRotation.CCW_240:
                    vectorOut = new CubeCoordinate(vectorIN.R, vectorIN.S, vectorIN.Q);
                    break;
                case CubeRotation.CW_180:
                case CubeRotation.CCW_180:
                    vectorOut = vectorIN * -1;
                    break;
                case CubeRotation.CW_240:
                case CubeRotation.CCW_120:
                    vectorOut = new CubeCoordinate(vectorIN.S, vectorIN.Q, vectorIN.R);
                    break;
                case CubeRotation.CW_300:
                case CubeRotation.CCW_60:
                    vectorOut = new CubeCoordinate(-vectorIN.R, -vectorIN.S, -vectorIN.Q);
                    break;
            }

            return center + vectorOut;
        }
        public enum CubeRotation
        {
            CW_60,
            CCW_300,
            CW_120,
            CCW_240,
            CW_180,
            CCW_180,
            CW_240,
            CCW_120,
            CW_300,
            CCW_60
        }
        /// <summary>
        /// Build a ring around the center with a specified radius;
        /// </summary>
        /// <param name="center">The center of the ring.</param>
        /// <param name="radius">The radius of the ring.</param>
        /// <returns>An array of CubeCoordinate representing the ring.</returns>
        public static CubeCoordinate[] DrawRing(CubeCoordinate center, int radius)
        {
            if (radius == 0) return new CubeCoordinate[] { center };
            if (radius < 0) radius = -radius;

            List<CubeCoordinate> results = new List<CubeCoordinate>();

            CubeCoordinate current = center + CubeCoordinate.CubeCoordinate_direction(4) * radius;
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < radius; j++)
                {
                    results.Add(current);
                    current = CubeCoordinate.CubeCoordinate_neighbor(current, i);
                }
            }

            return results.ToArray();
        }
        /// <summary>
        /// Build a ring around the center with an inner and outer radius;
        /// </summary>
        /// <param name="center">The center of the ring.</param>
        /// <param name="minRadius">The min radius of the ring.</param>
        /// <param name="maxRadius">The max radius of the ring.</param>
        /// <returns>An array of CubeCoordinate representing the ring</returns>
        public static CubeCoordinate[] DrawLargeRing (CubeCoordinate center, int innerRadius, int outerRadius)
        {
            if (innerRadius < 0) innerRadius = -innerRadius;
            if (outerRadius < 0) outerRadius = -outerRadius;

            if (innerRadius == 0 && outerRadius == 0) return new CubeCoordinate[] { center };
            if (innerRadius == 0 || outerRadius == 0) return DrawArea(center, Mathf.Max(innerRadius, outerRadius));
            if (innerRadius == outerRadius) return DrawRing(center, innerRadius);
            if (outerRadius < innerRadius)
            {
                int swap = innerRadius;
                innerRadius = outerRadius;
                outerRadius = swap;
            }

            List<CubeCoordinate> results = new List<CubeCoordinate>();
            for (int i = innerRadius; i <= outerRadius; i++)
            {
                results.AddRange(DrawRing(center, i));
            }
            return results.ToArray();
        }
        #endregion

#if UNITY_EDITOR
        #region Debug
        public void DebugDrawHexagon(CubeCoordinate coordinate, Color color, float? duration = null)
        {
            Vector2[] corners = CubeCoordinateCorners(coordinate);
            for (int i = 0; i < corners.Length; i++)
            {
                int y = (i + 1) % 6;
                Debug.DrawLine(
                    new Vector3(corners[i].x, 0f, corners[i].y),
                    new Vector3(corners[y].x, 0f, corners[y].y),
                    color,
                    duration.HasValue ? duration.Value : Time.deltaTime
                    );
            }
        }
        #endregion
#endif

    }
}