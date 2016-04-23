using UnityEngine;
using System.Collections;

namespace LineDrawing {
  public class BezzierSection {
    public const int NUM_CONTROL_POINTS = 4;

    private Vector3 m_p0;
    private Vector3 m_p1;
    private Vector3 m_p2;
    private Vector3 m_p3;

    private int m_pointsInSection;

    private Vector3[] m_points;
    Line m_line;

    public Vector3 this[int point] {
      get {
        if (point >= NUM_CONTROL_POINTS || point < 0)
          throw new System.ArgumentOutOfRangeException("Bezzier Section only has 4 points (0 - 3). Requested point index: " + point);

        if (point == 0)
          return m_p0;
        else if (point == 1)
          return m_p1;
        else if (point == 2)
          return m_p2;
        else if (point == 3)
          return m_p3;
        else
          throw new System.ArgumentException("point index not understood.");
      }

      set {
        if (point >= NUM_CONTROL_POINTS || point < 0)
          throw new System.ArgumentOutOfRangeException("Bezzier Section only has 4 points (0 - 3). Requested point index: " + point);

        if (point == 0)
          m_p0 = value;
        else if (point == 1)
          m_p1 = value;
        else if (point == 2)
          m_p2 = value;
        else if (point == 3)
          m_p3 = value;
        else
          throw new System.ArgumentException("point index not understood.");

        UpdateCurve();
      }
    }

    public int PointsInSection {
      get {
        return m_pointsInSection;
      }

      set {
        if (m_pointsInSection == value)
          return;
        m_pointsInSection = value;
        UpdateCurve();
      }
    }

    public BezzierSection(int pointsInSection, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
      m_p0 = p0;
      m_p1 = p1;
      m_p2 = p2;
      m_p3 = p3;
      m_pointsInSection = pointsInSection;
      m_line = new Line(m_pointsInSection, 3, 0.02f);
      m_points = m_line.Verticies;
      UpdateCurve();
    }

    void UpdateCurve() {
      for (int point = 0; point < m_pointsInSection; point++) {
        float t = point / (float)(m_pointsInSection - 1);
        float invT = 1 - t;
        //https://en.wikipedia.org/wiki/B%C3%A9zier_curve
        Vector3 pos = (invT * invT * invT) * m_p0 + 3 * (invT * invT) * t * m_p1 + 3 * invT * t * t * m_p2 + t * t * t * m_p3;
        m_points[point] = pos;
      }

      m_line.SetVerticies(m_points);
    }
  }
}
