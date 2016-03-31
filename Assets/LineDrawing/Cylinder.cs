using UnityEngine;
using System.Collections;
using ComponentCaching;

namespace LineDrawing {
  public class Cylinder : MonoBehaviour {
    public struct RingTransform {
      public readonly Vector3 Position;
      public readonly Vector3 Normal;

      public RingTransform(Vector3 position, Vector3 normal) {
        Position = position;
        Normal = normal;
      }
    }

    // Cached Components
    private ComponentCacher m_componentCache;

    // Cylinder intrinsic properties
    private int m_facesAroundU;
    private int m_subdivisionsV;
    private int m_radius;
    private Mesh m_mesh;

    private ComponentCacher ComponentCache  {
      get {
        if (m_componentCache == null)
          m_componentCache = new ComponentCacher(gameObject);

        return m_componentCache;
      }
    }

    public void SetMesh(Mesh mesh, int facesAroundU, int subdivisionsV, int radius) {
      ComponentCache.GetComponent<MeshFilter>().mesh = mesh;
      m_facesAroundU = facesAroundU;
      m_subdivisionsV = subdivisionsV;
      m_radius = radius;
    }

    public RingTransform this[int ring] {
      get {
        return new RingTransform(calcRingNormal(ring), calcRingCenter(ring));
      }
      set {
        setRingNormal(ring, value.Normal);
        setRingPosition(ring, value.Position);
      }
    }

    private void setRingPosition(int ringIndex, Vector3 center) {
      throw new System.NotImplementedException();
    }

    private void setRingNormal(int ringIndex, Vector3 normal) {
      throw new System.NotImplementedException();
    }

    private Vector3 calcRingCenter(int ringIndex) {
      throw new System.NotImplementedException();
    }

    private Vector3 calcRingNormal(int ringIndex) {
      throw new System.NotImplementedException();
    }

  }
}