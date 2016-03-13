using UnityEngine;
using System;
using System.Collections;

public struct ComparableVec3 : IEquatable<ComparableVec3>, IComparable<ComparableVec3> {
  public float x;
  public float y;
  public float z;

  public static implicit operator Vector3(ComparableVec3 vec) {
    return new Vector3(vec.x, vec.y, vec.z);
  }

  public static implicit operator ComparableVec3(Vector3 vec) {
    return new ComparableVec3(vec.x, vec.y, vec.z);
  }

  public ComparableVec3(float _x, float _y, float _z) {
    x = _x;
    y = _y;
    z = _z;
  }

  public bool Equals(ComparableVec3 vec2) {
    return x == vec2.x && y == vec2.y && z == vec2.z;
  }

  public int CompareTo(ComparableVec3 vec2) {
    float thisManhattan = x + y + z;
    float vec2Manhattan = vec2.x + vec2.y + vec2.z;

    if (thisManhattan > vec2Manhattan)
      return 1;
    else if (thisManhattan == vec2Manhattan)
      return 0;
    else
      return -1;
  }
}
