using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VRViz.Core;
using VRViz.Unity;

namespace KeplerData {
public class KeplerParser {
  public delegate void ParsingCompleteHandler(KeplerNode[] nodes, List<string> uniqueFalcilities);

  private const string KeplerFilename = "kepler_planets";
  private static KeplerNode invalidNode = new KeplerNode("", "", new StellarCoordinates(0,0,0), "", 0);

  public static CSVErrorCode KeplerDataToKeplerNode(string[] fields, out KeplerNode node) {

    if (fields.Length < 151) {
      node = invalidNode;
      return CSVErrorCode.ERR_NOT_ENOUGH_FIELDS;
    }

    string hostName = fields[1];
    string planetLetter = fields[2];

    string discoveringFacility = fields[155];

    int discoveryYear = 0;
    if (!int.TryParse(fields[152], out discoveryYear)) {
      node = invalidNode;
      return CSVErrorCode.ERR_COULD_NOT_CAST_FIELD;
    }

    float ra = 0;
    if (!float.TryParse(fields[39], out ra)) {
      node = invalidNode;
      return CSVErrorCode.ERR_COULD_NOT_CAST_FIELD;
    }

    float dec = 0;
    if (!float.TryParse(fields[41], out dec)) {
      node = invalidNode;
      return CSVErrorCode.ERR_COULD_NOT_CAST_FIELD;
    }

    float dist = 0;
    if (!float.TryParse(fields[42], out dist)) {
      node = invalidNode;
      return CSVErrorCode.ERR_COULD_NOT_CAST_FIELD;
    }

    StellarCoordinates position = new StellarCoordinates(ra, dec, dist);

    node = new KeplerNode(hostName, planetLetter, position, discoveringFacility, discoveryYear, float.MaxValue);
    return CSVErrorCode.ERR_OK;
  }

  public static void ParseKeplerDataAsync(ParsingCompleteHandler onComplete) {
    string keplerCSV = Resources.Load<UnityEngine.TextAsset>(KeplerFilename).text;
    List<string>[] uniqueColumnValues = new List<string>[0];
    KeplerNode[] planets = new KeplerNode[0];

    // Create a signal object to know when parsing is complete.
    GameObject signalObject = new GameObject("parsing signal");
    MarshalledSignal signal = signalObject.AddComponent<MarshalledSignal>();

    AsyncWorker<CSVParser<KeplerNode>.ParserOutput> asyncParser = new AsyncWorker<CSVParser<KeplerNode>.ParserOutput>();

    signal.OnSignal += () => {
      GameObject.Destroy(signalObject);
      signal = null;
      CSVParser<KeplerNode>.ParserOutput output = asyncParser.ParserOutput;
      planets = output.parsedObjects;
      uniqueColumnValues = output.uniqueValuesInRows;
      List<string> uniqueFacilities = uniqueColumnValues[155];
      onComplete(planets, uniqueFacilities);
    };

    asyncParser.StartWork(signal, () => {
      return CSVParser<KeplerNode>.ParseCSVRowsToType(keplerCSV, KeplerDataToKeplerNode, (string err) => Debug.LogWarning(err));
    });
  }

  static void signal_OnSignal() {
    throw new System.NotImplementedException();
  }
}
}