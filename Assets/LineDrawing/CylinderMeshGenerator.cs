using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LineDrawing {
  public class CylinderMeshGenerator {
    private const float FULL_CIRCLE_RADIANS = Mathf.PI * 2.0f;
    private const int TRIS_PER_FACE = 2;
    private const int VERTS_PER_TRI = 3;
    private const int RINGS_BEFORE_SUBDIVISION = 2;
    private const int FACES_BEFORE_SUBDIVISION = RINGS_BEFORE_SUBDIVISION / 2;
    private static Vector3 MESH_GENERATION_NORMAL = new Vector3(0.0f, -1.0f, 0.0f);

    private struct VertexAndUVArgs {
      public readonly int TotalVerts;
      public readonly int FacesAroundU;
      public readonly float HeightStep;
      public readonly float UvStep;
      public readonly Vector3 TopRingStart;
      public readonly Vector3 Up;
      public readonly Vector3 Right;
      public readonly float Radius;

      public VertexAndUVArgs(int facesAroundU, int subdivisionsV, float height, float radius) {
        FacesAroundU = facesAroundU;
        Radius = radius;

        basisVectorsFromNormal(MESH_GENERATION_NORMAL, out Right, out Up);

        //Generate args for filling the vertex and UV arrays
        TopRingStart = -MESH_GENERATION_NORMAL * (height / 2.0f);
        TotalVerts = facesAroundU * (RINGS_BEFORE_SUBDIVISION + subdivisionsV);
        HeightStep = height / (subdivisionsV + 1);
        UvStep = 1.0f / (float)(subdivisionsV + 1);
      }
    }

    private struct TriArgs {
      public readonly int FacesAroundU;
      public readonly int TotalFaces;
      public readonly int TotalTris;
      public readonly int TriArraySize;

      public TriArgs(int facesAroundU, int subdivisionsV) {
        FacesAroundU = facesAroundU;
        TotalFaces = facesAroundU * (subdivisionsV + FACES_BEFORE_SUBDIVISION);
        TotalTris = TotalFaces * TRIS_PER_FACE;
        TriArraySize = TotalTris * VERTS_PER_TRI;
      }
    }

    public static Mesh GenerateCylinderMesh(int facesAroundU, int subdivisionsV, float height, float radius) {
      Mesh newMesh = new Mesh();

      // Arrays to hold the mesh data
      Vector3[] verts;
      Vector2[] uvs;
      int[] tris;

      VertexAndUVArgs vertAndUvArgs = new VertexAndUVArgs(facesAroundU, subdivisionsV, height, radius);
      fillVertAndUVArrays(vertAndUvArgs, out verts, out uvs);

      TriArgs triArgs = new TriArgs(facesAroundU, subdivisionsV);
      fillTrisArray(triArgs, out tris);

      // Appply mesh data
      newMesh.SetVertices(new List<Vector3>(verts));
      newMesh.SetTriangles(tris, 0);
      newMesh.SetUVs(0, new List<Vector2>(uvs));
      newMesh.RecalculateBounds();
      newMesh.RecalculateNormals();

      return newMesh;
    }

    private static void fillVertAndUVArrays(VertexAndUVArgs args, out Vector3[] verts, out Vector2[] uvs) {
      verts = new Vector3[args.TotalVerts];
      uvs = new Vector2[args.TotalVerts];

      // Vars to be updated per ring
      int currentRingIndex = 0;
      float ringHeight = 0;
      Vector3 ringCenter = Vector3.zero;

      // Fill in verts and UV arrays
      for (int i = 0; i < args.TotalVerts; i++) {
        int vertInRingIndex = i % args.FacesAroundU;
        bool isStartOfNewRing = vertInRingIndex == 0;

        if (isStartOfNewRing) {
          currentRingIndex = i / args.FacesAroundU;
          ringHeight = currentRingIndex * args.HeightStep;
          ringCenter = args.TopRingStart + (MESH_GENERATION_NORMAL * ringHeight);
        }

        // Calculated per vert
        float radians = FULL_CIRCLE_RADIANS * (vertInRingIndex / (float)args.FacesAroundU);
        Vector3 vertPosition = placeVert(ringCenter, args.Up, args.Right, args.Radius, radians);
        verts[i] = vertPosition;

        // Handle UVs
        float uCoordiate = 0;
        float vCoordinate = args.UvStep * currentRingIndex;
        uvs[i] = new Vector2(uCoordiate, vCoordinate);
      }
    }

    private static void fillTrisArray(TriArgs args, out int[] tris) {
      tris = new int[args.TriArraySize];

      // Vars to be calculated each ring
      int currentRingIndex_tris = 0;
      int currentRingFirstVert = 0;

      for (int i = 0; i < args.TotalFaces; i++) {
        int faceInRingIndex = i % args.FacesAroundU;
        bool isStartOfNewRing = faceInRingIndex == 0;

        if (isStartOfNewRing) {
          // Can be calculated per ring
          currentRingIndex_tris = i / args.FacesAroundU;
          currentRingFirstVert = args.FacesAroundU * currentRingIndex_tris;
        }

        // Calculated per vert
        int currentTri = i * TRIS_PER_FACE;
        int startTriIndex = currentTri * VERTS_PER_TRI;

        // 0 -> 1 -> 2 & 1 -> 3 -> 2
        tris[startTriIndex + 0] = faceInRingIndex + currentRingFirstVert;
        tris[startTriIndex + 1] = ((faceInRingIndex + 1) % args.FacesAroundU) + currentRingFirstVert;
        tris[startTriIndex + 2] = faceInRingIndex + currentRingFirstVert + args.FacesAroundU;

        tris[startTriIndex + 3] = ((faceInRingIndex + 1) % args.FacesAroundU) + currentRingFirstVert;
        tris[startTriIndex + 4] = (((faceInRingIndex + 1) % args.FacesAroundU) + args.FacesAroundU) + currentRingFirstVert;
        tris[startTriIndex + 5] = faceInRingIndex + currentRingFirstVert + args.FacesAroundU;
      }
    }

    private static Vector3 placeVert(Vector3 center, Vector3 up, Vector3 right, float radius, float radians) {
      Vector3 newVert = Vector3.zero;
      float xComponent = Mathf.Cos(radians);
      float yComponent = Mathf.Sin(radians);

      Vector3 directionToVertex = (up * yComponent) + (right * xComponent);
      Vector3 rayToVertex = directionToVertex * radius;
      newVert = center + rayToVertex;
      return newVert;
    }

    private static void basisVectorsFromNormal(Vector3 normalVector, out Vector3 right, out Vector3 up) {
      // Make sure our vector for cross product is dis-simmilar to our normal.
      Vector3 crossVector = Vector3.forward;
      if (Vector3.Dot(crossVector, normalVector) > 0.75f)
        crossVector = Vector3.right;

      // Generate basis vectors for vertex generation operations.
      right = Vector3.Cross(normalVector, crossVector);
      up = Vector3.Cross(normalVector, right);
    }
  }
}