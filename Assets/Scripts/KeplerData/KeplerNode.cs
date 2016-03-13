using UnityEngine;
using VRViz.Core;
using System.Collections;

namespace KeplerData {

  public struct StellarCoordinates {
    public float rightAscention;
    public float declination;
    public float distance;

    public StellarCoordinates(float ra, float dec, float dist) {
      rightAscention = ra;
      declination = dec;
      distance = dist;
    }

    public Quaternion ToQuaternion() {
      // declination is flipped to fit unity's axis
      return Quaternion.Euler(-declination, rightAscention, 0.0f);
    }

    public Vector3 ToPosition(Mapping<float, float> distanceMapping) {
      if (distanceMapping == null)
        throw new System.NullReferenceException("Null distance mapping provided.");
      return ToQuaternion() * Vector3.forward * distanceMapping.MapValue(distance);
    }
  }

  public class KeplerNode : TimeboxedNode {
    public readonly string Name;
    public readonly StellarCoordinates Position;
    public readonly string DiscoveringFaciltiy;

    public KeplerNode(string name, StellarCoordinates position, string discoveringFaciltiy, float startTime, float endTime = float.MaxValue)
      : base(startTime, endTime) {
        Name = name;
        Position = position;
        DiscoveringFaciltiy = discoveringFaciltiy;
    }
    
  }
}