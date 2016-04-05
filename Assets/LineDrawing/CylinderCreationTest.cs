using UnityEngine;
using System.Collections;

namespace LineDrawing {
  public class CylinderCreationTest : MonoBehaviour {
    [SerializeField]
    private Material m_cylinderMaterial;

    public Vector3 ToMove;

    Cylinder m_cylinder;

    // Use this for initialization
    void Start() {
      m_cylinder = Cylinder.MakeCylinder("TestCylinder", 3, 3, 5.0f, 0.005f, m_cylinderMaterial);
    }

    // Update is called once per frame
    void Update() {
      m_cylinder[1].Position = m_cylinder[1].Position + (ToMove * Time.deltaTime);
    }
  }
}