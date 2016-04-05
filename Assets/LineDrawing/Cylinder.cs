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


    // Cylinder intrinsic properties
    private int m_facesAroundU;
    private int m_subdivisionsV;
    private float m_radius;
    private Mesh m_mesh;

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