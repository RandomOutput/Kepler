using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LineDrawing {
  public class Line {
    private Cylinder m_cylinder;
    private Vector3[] m_vertCache;

    public Line(int vertexCount, int facesAround, float radius) {
      m_cylinder = Cylinder.MakeCylinder(facesAround, vertexCount - 2, radius);
      initializeVerts(vertexCount);
    }

    public Line(Vector3[] verticies, int facesAround, float radius) {
      m_cylinder = Cylinder.MakeCylinder(facesAround, verticies.Length - 2, radius);
      initializeVerts(verticies);
    }

    private void initializeVerts(int vertCount) {
      m_vertCache = new Vector3[vertCount];
      for (int i = 0; i < vertCount; i++) {
        m_vertCache[i] = i * Vector3.down;
      }
      updateVerts(m_vertCache);
    }

    private void initializeVerts(Vector3[] verts) {
      m_vertCache = verts;
      updateVerts(m_vertCache);
    }

    public Vector3 this[int vertex] {
      get {
        return m_cylinder[vertex].Position;
      }
      set {
        m_vertCache[vertex] = value;
        updateVerts(m_vertCache);
      }
    }

    public Vector3[] Verticies {
      get {
        return m_vertCache;
      }
    }

    public void SetVerticies(Vector3[] verts) {
      m_vertCache = verts;
      updateVerts(m_vertCache);
    }

    private void updateVerts(Vector3[] verticies) {
      if (verticies.Length != m_cylinder.Count) {
        Debug.Log("verts: " + verticies.Length + " | cyl rings: " + m_cylinder.Count);
        throw new System.ArgumentOutOfRangeException();
        // TODO: Update this to just replace the mesh.
      }

      // Special case for first vert
      var initialDirection = (verticies[1] - verticies[0]).normalized;
      m_cylinder[0].Position = verticies[0];
      m_cylinder[0].Normal = initialDirection;

      for (int i = 1; i < verticies.Length - 1; i++) {
        var inDirection = verticies[i] - verticies[i - 1];
        var outDirection = verticies[i + 1] - verticies[i];
        var averageDirection = ((inDirection + outDirection) / 2.0f).normalized;

        m_cylinder[i].Position = verticies[i];
        m_cylinder[i].Normal = averageDirection;
      }

      // Special case for last vert
      int lastVertIndex = verticies.Length - 1;
      var finalDirection = (verticies[lastVertIndex] - verticies[lastVertIndex - 1]).normalized;
      m_cylinder[lastVertIndex].Position = verticies[lastVertIndex];
      m_cylinder[lastVertIndex].Normal = finalDirection;
    }
  }
}
