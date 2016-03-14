using UnityEngine;
using System;
using System.Collections;
using VRViz.Core;
using VRViz.Unity;

namespace KeplerData {
  public struct StellarCoordinates :
      IEquatable<StellarCoordinates>,
      IComparable<StellarCoordinates> {
    public float rightAscention;
    public float declination;
    public float distance;

    public StellarCoordinates(float ra, float dec, float dist) {
      rightAscention = ra;
      declination = dec;
      distance = dist;
    }

    public bool Equals(StellarCoordinates coords2) {
      return rightAscention == coords2.rightAscention && declination == coords2.declination && distance == coords2.distance;
    }

    public int CompareTo(StellarCoordinates coords2) {
      float thisManhattan = rightAscention + declination + distance;
      float coords2Manhattan = coords2.rightAscention + coords2.declination + coords2.distance;

      if (thisManhattan > coords2Manhattan)
        return 1;
      else if (thisManhattan == coords2Manhattan)
        return 0;
      else
        return -1;
    }
  }
}