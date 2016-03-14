using UnityEngine;
using VRViz.Core;
using System.Collections;

namespace KeplerData {
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