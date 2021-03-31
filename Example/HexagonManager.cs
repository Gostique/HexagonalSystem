using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexagonSystem;

public class HexagonManager : MonoBehaviour
{

    #region Fields
    public Mode m_mode;
    public int m_rangeCenter, m_rangeA, m_rangeB;
    private Mode _previousMode;
    private Transform _center, _pointA, _pointB;
    private HexGrid m_grid;
    public enum Mode
    {
        DrawArea,
        DrawIntersectArea,
        RotateCoordinate,
        DrawRing,
        DrawLargeRing
    }
    #endregion

    #region Methods
    private void ChangeMode(Mode newMode)
    {
        _previousMode = m_mode;
        _center.gameObject.SetActive(false);
        _pointA.gameObject.SetActive(false);
        _pointB.gameObject.SetActive(false);

        switch (m_mode)
        {
            case Mode.DrawArea:
            case Mode.DrawRing:
            case Mode.DrawLargeRing:
                _center.gameObject.SetActive(true);
                break;
            case Mode.DrawIntersectArea:
                _pointA.gameObject.SetActive(true);
                _pointB.gameObject.SetActive(true);
                break;
            case Mode.RotateCoordinate:
                _center.gameObject.SetActive(true);
                _pointA.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }
    private void DrawHexagon(CubeCoordinate[] coordinates, Color color)
    {
        foreach (CubeCoordinate coordinate in coordinates)
        {
            m_grid.DebugDrawHexagon(coordinate, color);
        }
    }
    private void DrawHexagonColorRamp(CubeCoordinate[] coordinates, Color colorA, Color colorB)
    {
        for (int i = 0; i < coordinates.Length; i++)
        {
            m_grid.DebugDrawHexagon(coordinates[i], Color.Lerp(colorA, colorB, (float)i/(coordinates.Length-1)));
        }
    }
    #endregion

    #region Unity API
    private void Awake()
    {
        m_grid = new HexGrid(
            HexGrid.HexagonOrientation.Pointy,
            Vector2.one,
            Vector2.zero,
            HexGrid.OffsetRaw.Right
            );
    }
    private void Start()
    {
        _center = new GameObject("Center").GetComponent<Transform>();
        _center.gameObject.SetActive(false);
        _pointA = new GameObject("Point A").GetComponent<Transform>();
        _pointA.gameObject.SetActive(false);
        _pointB = new GameObject("Point B").GetComponent<Transform>();
        _pointB.gameObject.SetActive(false);

        ChangeMode(m_mode);
    }
    private void Update()
    {
        if (_previousMode != m_mode)
        {
            ChangeMode(m_mode);
        }

        switch (m_mode)
        {
            case Mode.DrawArea:
                DrawHexagon(HexGrid.DrawArea(
                    m_grid.WorldToCoordinate(new Vector2(_center.position.x, _center.position.z)),
                    m_rangeCenter),
                    Color.cyan
                    );
                break;



            case Mode.DrawIntersectArea:
                DrawHexagon(
                    HexGrid.DrawArea(
                        m_grid.WorldToCoordinate(new Vector2(_pointA.position.x, _pointA.position.z)),
                        m_rangeA),
                    Color.blue
                    );
                DrawHexagon(
                    HexGrid.DrawArea(
                        m_grid.WorldToCoordinate(new Vector2(_pointB.position.x, _pointB.position.z)),
                        m_rangeB),
                    Color.yellow
                    );
                DrawHexagon(
                    HexGrid.DrawIntersectArea(
                        m_grid.WorldToCoordinate(new Vector2(_pointA.position.x, _pointA.position.z)),
                        m_rangeA,
                        m_grid.WorldToCoordinate(new Vector2(_pointB.position.x, _pointB.position.z)),
                        m_rangeB),
                    Color.green
                    );
                break;



            case Mode.RotateCoordinate:
                CubeCoordinate a = m_grid.WorldToCoordinate(new Vector2(_pointA.position.x, _pointA.position.z));
                CubeCoordinate centerRotation = m_grid.WorldToCoordinate(new Vector2(_center.position.x, _center.position.z));
                DrawHexagonColorRamp(
                    new CubeCoordinate[] {
                        centerRotation,
                        a,
                        HexGrid.RotateCoordinate(a, centerRotation, HexGrid.CubeRotation.CCW_60),
                        HexGrid.RotateCoordinate(a, centerRotation, HexGrid.CubeRotation.CCW_120),
                        HexGrid.RotateCoordinate(a, centerRotation, HexGrid.CubeRotation.CCW_180),
                        HexGrid.RotateCoordinate(a, centerRotation, HexGrid.CubeRotation.CCW_240),
                        HexGrid.RotateCoordinate(a, centerRotation, HexGrid.CubeRotation.CCW_300)
                    },
                    Color.white,
                    Color.blue
                    );
                break;



            case Mode.DrawRing:
                CubeCoordinate centerRing = m_grid.WorldToCoordinate(new Vector2(_center.position.x, _center.position.z));

                DrawHexagon(
                    HexGrid.DrawRing(centerRing, m_rangeCenter),
                    Color.blue
                    );

                m_grid.DebugDrawHexagon(centerRing, Color.white);
                break;



            case Mode.DrawLargeRing:
                CubeCoordinate centerLargeRing = m_grid.WorldToCoordinate(new Vector2(_center.position.x, _center.position.z));

                DrawHexagon(
                    HexGrid.DrawLargeRing(centerLargeRing, m_rangeA, m_rangeB),
                    Color.blue
                    );

                m_grid.DebugDrawHexagon(centerLargeRing, Color.white);
                break;



            default:
                break;
        }
    }
    #endregion

}