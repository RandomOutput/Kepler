using UnityEngine;
using VRViz.Core;
using System.Collections;

namespace KeplerData {
  public class KeplerNode : TimeboxedNode {
    public readonly string HostName;
    public readonly string PlanetLetter;
    public readonly StellarCoordinates Position;
    public readonly string DiscoveringFaciltiy;

    public KeplerNode(string hostName, string planetLetter, StellarCoordinates position, string discoveringFaciltiy, float startTime, float endTime = float.MaxValue)
      : base(startTime, endTime) {
        HostName = hostName;
        PlanetLetter = planetLetter;
        Position = position;
        DiscoveringFaciltiy = discoveringFaciltiy;
    }
  }
}