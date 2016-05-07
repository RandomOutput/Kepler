using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LineDrawing {
  public class RingTransform {
    private Vector3 m_position;
    private Vector3 m_normal;
    private Mesh m_mesh;
    private Vector3[] m_vertCache;
    private Transform m_parent;
    private readonly int m_firstVertexIndex;
    private readonly int m_lastVertexIndex;
    private readonly int m_vertexCount;
    private readonly float m_radius;

    public RingTransform(int firstVertexIndex, int lastVertexIndex, float radius, Mesh mesh, Transform parent) {
      m_parent = parent;
      m_mesh = mesh;
      m_firstVertexIndex = firstVertexIndex;
      m_lastVertexIndex = lastVertexIndex;
      m_vertexCount = 1 + lastVertexIndex - firstVertexIndex;
      m_position = calcRingCenter();
      m_normal = calcRingNormal();
      m_radius = radius;
    }

    public Vector3 Position {
      get {
        return m_position;
      }
      set {
        m_vertCache = m_mesh.vertices;
        Vector3 toNewCenter = value - m_position;
        for (int i = m_firstVertexIndex; i <= m_lastVertexIndex; i++) {
          m_vertCache[i] = m_mesh.vertices[i] + toNewCenter;
        }
        m_mesh.vertices = m_vertCache;
        m_position = m_position + toNewCenter;
        m_mesh.RecalculateNormals();
      }
    }

    public Vector3 Normal {
      get {
        return m_normal;
      }
    }

    public Vector3 ToRootVertex {
      get {
        return (m_vertCache[m_firstVertexIndex] - m_position);
      }
    }

    public void SetNormal(Vector3 normal) {
      Vector3 up = Vector3.up;
      Vector3.OrthoNormalize(ref normal, ref up);
      SetNormal(normal, up);
      m_mesh.RecalculateNormals();
    }

    public void SetNormal(Vector3 normal, Vector3 rootVertexDirection) {
      Debug.DrawRay(Position, rootVertexDirection, Color.red, float.MaxValue);
      Vector3 right = rootVertexDirection;
      Vector3 up = Vector3.Cross(normal, right).normalized;

      for (int i = m_firstVertexIndex; i <= m_lastVertexIndex; i++) {
        int indexInRing = i - m_firstVertexIndex;
        float circlePercentage = (indexInRing / (float)((m_lastVertexIndex - m_firstVertexIndex) + 1));
        float radians = CylinderMeshGenerator.FULL_CIRCLE_RADIANS * circlePercentage;
        Debug.Log(indexInRing + " / " + "((" + m_lastVertexIndex + " - " + m_firstVertexIndex + ") + 1 = " + circlePercentage);
        Debug.Log(indexInRing + " radians: " + radians);
        Vector3 vertPosition = CylinderMeshGenerator.placeVert(m_position, up, right, m_radius, radians);
        m_vertCache[i] = vertPosition;
      }

      m_mesh.vertices = m_vertCache;
      m_normal = normal;
    }

    public static Vector3 PlaneProjection(Vector3 planePoint, Vector3 planeNormal, Vector3 pointToProject) {
      Vector3 pointToPoint = pointToProject - planePoint;
      float distFromPlane = Vector3.Dot(pointToPoint, planeNormal.normalized);
      Vector3 projectedPoint = pointToProject + (-planeNormal * distFromPlane);
      return projectedPoint - planePoint;
    }

    public static Vector3 PlaneProjectionAlongVector(Vector3 planePoint, Vector3 planeNormal, Vector3 pointToProject, Vector3 projectionVector) {
      float denom = Vector3.Dot(planeNormal, projectionVector);
      if (denom <= Mathf.Epsilon)
        return Vector3.zero;

      Vector3 p0l0 = planePoint - pointToProject;
      float dist = Vector3.Dot(p0l0, planeNormal) / denom;
      Vector3 projectionRay = (projectionVector.normalized * dist);
      return projectionRay;
    }

    public static float RadiansBetweenPoints(Vector2 p1, Vector2 p2) {
      Vector2 toP2 = p2 - p1;
      return Mathf.Atan2(toP2.y, toP2.x);
    }

    private Vector3 calcRingCenter() {
      Vector3 vectorSum = Vector3.zero;
      for (int i = m_firstVertexIndex; i <= m_lastVertexIndex; i++) {
        vectorSum += m_mesh.vertices[i];
      }
      Vector3 center = vectorSum / m_vertexCount;
      return center;
    }

    private Vector3 calcRingNormal() {
      Vector3 center = calcRingCenter();
      Vector3 v1 = m_mesh.vertices[m_firstVertexIndex] - center;
      Vector3 v2 = m_mesh.vertices[m_firstVertexIndex + ((m_lastVertexIndex - m_firstVertexIndex) / 2)] - center;
      Vector3 normal = Vector3.Cross(v1, v2);
      return normal;
    }
  }

  public class Cylinder : MonoBehaviour {
    private int m_facesAroundU;
    private int m_subdivisionsV;
    private float m_radius;
    private Mesh m_mesh;
    private RingTransform[] m_ringTransforms;

    public Material CylinderMaterial {
      get {
        return gameObject.GetComponent<MeshRenderer>().material;
      }
      set {
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
          meshRenderer = gameObject.AddComponent<MeshRenderer>();

        meshRenderer.material = value;
      }
    }

    public int Count {
      get {
        return m_ringTransforms.Length;
      }
    }

    public void SetMesh(Mesh mesh, int facesAroundU, int subdivisionsV, float radius) {
      MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
      if (meshFilter == null)
        meshFilter = gameObject.AddComponent<MeshFilter>();

      meshFilter.mesh = mesh;
      m_mesh = mesh;
      m_facesAroundU = facesAroundU;
      m_subdivisionsV = subdivisionsV;
      m_radius = radius;
      ConstructRingTransforms();
    }

    public RingTransform this[int ring] {
      get {
        if (ring > m_ringTransforms.Length)
          throw new System.IndexOutOfRangeException();
        return m_ringTransforms[ring];
      }
    }

    private void ConstructRingTransforms() {
      m_ringTransforms = new RingTransform[m_subdivisionsV + CylinderMeshGenerator.RINGS_BEFORE_SUBDIVISION];
      for (int i = 0; i < m_ringTransforms.Length; i++) {
        int start, end;
        RingIndicies(i, out start, out end);
        m_ringTransforms[i] = new RingTransform(start, end, m_radius, m_mesh, transform);
      }
    }

    public static Cylinder MakeCylinder(int facesAroundU, int subdivisionsV, float radius) { return MakeCylinder(facesAroundU, subdivisionsV, 1.0f, radius); }

    public static Cylinder MakeCylinder(int facesAroundU, int subdivisionsV, float height, float radius) { return MakeCylinder("cylinder", facesAroundU, subdivisionsV, height, radius, new Material(Shader.Find("Standard"))); }

    public static Cylinder MakeCylinder(string name, int facesAroundU, int subdivisionsV, float height, float radius, Material material) {
      GameObject newCylinderObject = new GameObject(name);
      Cylinder newCylinder = newCylinderObject.AddComponent<Cylinder>();
      Mesh cylinderMesh = CylinderMeshGenerator.GenerateCylinderMesh(facesAroundU, subdivisionsV, height, radius);
      newCylinder.SetMesh(cylinderMesh, facesAroundU, subdivisionsV, radius);
      newCylinder.CylinderMaterial = material;
      return newCylinder;
    }

    private void RingIndicies(int ringIndex, out int ringStart, out int ringEnd) {
      ringStart = ringIndex * m_facesAroundU;
      ringEnd = ringStart + m_facesAroundU - 1;
    }
  }
}
