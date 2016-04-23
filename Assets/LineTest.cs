using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LineDrawing {
  public class LineTest : MonoBehaviour {
    public Transform[] vertTransforms;

    // Use this for initialization
    void Start() {
      Vector3[] verts = new Vector3[vertTransforms.Length];

      for(int i=0;i<verts.Length;i++) {
        verts[i] = vertTransforms[i].position;
      }

      Line line = new Line(verts, 3, 0.02f);
    }
  }
}
