using UnityEngine;
using System.Collections;

namespace LineDrawing {
  public class CylinderCreationTest : MonoBehaviour {
    [SerializeField]
    private Material m_cylinderMaterial;

    // Use this for initialization
    void Start() {
      Cylinder cylinder = Cylinder.MakeCylinder("TestCylinder", 3, 3, 5.0f, 0.005f, m_cylinderMaterial);
    }

    // Update is called once per frame
    void Update() {

    }
  }
}