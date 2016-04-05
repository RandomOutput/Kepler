using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LineDrawing {
  public class RingTransform {
    private Vector3 m_position;
    private Vector3 m_normal;
    private Mesh m_mesh;
    private List<Vector3> m_vertCache;
    private readonly int m_firstVertexIndex;
    private readonly int m_lastVertexIndex;
    private readonly int m_vertexCount;

    public RingTransform(int firstVertexIndex, int lastVertexIndex, Mesh mesh) {
      m_mesh = mesh;
      m_vertCache = new List<Vector3>(mesh.vertices);
      m_firstVertexIndex = firstVertexIndex;
      m_lastVertexIndex = lastVertexIndex;
      m_vertexCount = lastVertexIndex - firstVertexIndex;
      m_position = calcRingCenter();
      m_normal = calcRingNormal();
    }

    public Vector3 Position {
      get {
        return m_position;
      }
      set {
        Vector3 toNewCenter = value - m_position;
        for (int i = m_firstVertexIndex; i <= m_lastVertexIndex; i++) {
          m_vertCache[i] = m_mesh.vertices[i] + toNewCenter;
        }
        m_mesh.SetVertices(m_vertCache);
        m_position = value;
      }
    }

    public Vector3 Normal {
      get {
        return m_normal;
      }
      set {
        Quaternion oldNormalToNew = Quaternion.FromToRotation(m_normal, value);
        for (int i = m_firstVertexIndex; i <= m_lastVertexIndex; i++) {
          Vector3 oldFromCenter = m_mesh.vertices[i] - m_position;
          Vector3 newFromCenter = oldNormalToNew * oldFromCenter;
          m_vertCache[i] = newFromCenter + m_position;
        }
        m_mesh.SetVertices(m_vertCache);
      }
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
      m_ringTransforms = new RingTransform[m_subdivisionsV + CylinderMeshGenerator.FACES_BEFORE_SUBDIVISION];
      for (int i = 0; i < m_facesAroundU; i++) {
        int start, end;
        RingIndicies(i, out start, out end);
        m_ringTransforms[i] = new RingTransform(start, end, m_mesh);
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