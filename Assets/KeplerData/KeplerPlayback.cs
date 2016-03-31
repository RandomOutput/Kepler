using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VRViz.Core;
using VRViz.Unity;

namespace KeplerData {
  public class KeplerPlayback : MonoBehaviour {
    [Header("DistanceMapping")]
    [Range(0, 1000.0f)]
    public float distance_inputMin;
    [Range(0, 1000.0f)]
    public float distance_inputMax;
    [Range(0, 1000.0f)]
    public float distance_outputMin;
    [Range(0, 1000.0f)]
    public float distance_outputMax;

    [SerializeField]
    private GameObject m_planetPrefab;
    [SerializeField]
    private Transform m_systemRoot;

    private Dictionary<string, StarSystem> m_nameToStarSystem = new Dictionary<string, StarSystem>();
    private Dictionary<uint, Planet> m_idToPlanet = new Dictionary<uint,Planet>();
    private List<StarSystem> m_starSystems = new List<StarSystem>();
    private List<Planet> m_planets = new List<Planet>();

    private LinearMapping m_distanceMapping;

    private List<string> m_uniqueFacilities;
    private Dictionary<string, Color> m_facilityColors;

    private MappingManager<Planet> m_planetMappingManager;
    private MappingManager<StarSystem> m_starSystemMappingManager;

    void Awake() {
      KeplerParser.ParseKeplerDataAsync(OnParsingComplete);
    }

    private void OnParsingComplete(KeplerNode[] nodes, List<string> uniqueFalcilities) {
      m_uniqueFacilities = uniqueFalcilities;

      NodeStoreDefault<KeplerNode> nodeStore = new NodeStoreDefault<KeplerNode>(nodes);

      configureMappings();

      m_facilityColors = new Dictionary<string, Color>();
      for (int i = 0; i < m_uniqueFacilities.Count; i++) {
        m_facilityColors.Add(m_uniqueFacilities[i], new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value));
      }

      constructStarSystems(nodeStore);

      // Create playhead
      Playhead<KeplerNode> playhead = new Playhead<KeplerNode>(nodeStore, 1990.0f, 2017.0f, true);
      playhead.OnStep += (Playhead<KeplerNode> head, float playheadTime, float stepSize) => { Debug.Log(playheadTime); };

      // Configure Node Birth and Death Behaviors
      playhead.OnNodeBorn += (TimeboxedNode node) => {
        StartCoroutine(showPlanetAfterTime(m_idToPlanet[node.UID], UnityEngine.Random.Range(0.0f, 1.0f)));
      };

      playhead.OnNodeDied += (TimeboxedNode node) => {
        m_idToPlanet[node.UID].VisibilityState = Planet.Visibility.HIDDEN;
      };

      // Start Data Playback
      playhead.InitializePlayhead();
      StartCoroutine(moveTimeForward(playhead));
    }

    void Update() {
      updateDistanceMapping();
    }

    private void updateDistanceMapping() {
      if (m_distanceMapping == null)
        return;

      m_distanceMapping.Input.Min = distance_inputMin;
      m_distanceMapping.Input.Max = distance_inputMax;
      m_distanceMapping.Output.Min = distance_outputMin;
      m_distanceMapping.Output.Max = distance_outputMax;
    }

    private IEnumerator showPlanetAfterTime(Planet planet, float seconds) {
      yield return new WaitForSeconds(seconds);
      planet.VisibilityState = Planet.Visibility.SHOWN;
    }

    private IEnumerator moveTimeForward(Playhead<KeplerNode> playhead) {
      while (true) {
        playhead.StepPlayhead(1f);
        yield return new WaitForSeconds(1.5f);
      }
    }

    private void constructStarSystems(NodeStoreDefault<KeplerNode> nodeStore) {
      foreach (KeplerNode node in nodeStore.Nodes.Values) {
        bool systemSpawned = m_nameToStarSystem.ContainsKey(node.HostName);
        StarSystem system;

        if (!m_nameToStarSystem.TryGetValue(node.HostName, out system)) {
          system = StarSystem.ConstructNewStarSystem(node, m_systemRoot, true);
          m_starSystems.Add(system);
          m_nameToStarSystem.Add(node.HostName, system);
        }

        GameObject newPlanetVisual = GameObject.Instantiate(m_planetPrefab);
        Planet planet = newPlanetVisual.AddComponent<Planet>();
        planet.DataNode = node;
        planet += system;
        m_planets.Add(newPlanetVisual.GetComponent<Planet>());
        m_idToPlanet.Add(node.UID, planet);
      }

      m_starSystemMappingManager.UpdateAllMappers();
      m_planetMappingManager.UpdateAllMappers();
    }

    private void configureMappings() {
      m_planetMappingManager = new MappingManager<Planet>(() => m_planets );
      m_starSystemMappingManager = new MappingManager<StarSystem>(() => m_starSystems );

      AttributeMapper<StarSystem, StellarCoordinates, ComparableVec3> stellarMapper = new AttributeMapper<StarSystem, StellarCoordinates, ComparableVec3>(
        (StarSystem system) => system.StellarPosition,

        new FunctionalMapping<StellarCoordinates, ComparableVec3>((StellarCoordinates stellarCoords) => {
          return Quaternion.Euler(-stellarCoords.declination, stellarCoords.rightAscention, 0.0f) * Vector3.forward;
        }),

        (StarSystem system, ComparableVec3 newPosition) => {
          float distance = system.transform.localPosition.magnitude;
          system.transform.localPosition = (Vector3)newPosition * distance;
        }
      );

      AttributeMapper<StarSystem, float, float> distanceMapper = new AttributeMapper<StarSystem, float, float>(

        (StarSystem system) => system.StellarPosition.distance,

        new LinearMapping(
          new VRViz.Core.Range<float>(distance_inputMin, distance_inputMax),
          new VRViz.Core.Range<float>(distance_outputMin, distance_outputMax)
        ),

        (StarSystem system, float newDistance) => { system.transform.localPosition = system.transform.localPosition.normalized * newDistance; }
      );

      AttributeMapper<Planet, string, Color> facillityColorMapper = new AttributeMapper<Planet, string, Color>(

        (Planet planet) => planet.DataNode.DiscoveringFaciltiy,

        new FunctionalMapping<string, Color>((string facilityName) => {
          Color returnColor = Color.magenta;
          try {
            returnColor = m_facilityColors[facilityName.ToLower()];
          }
          catch (System.Collections.Generic.KeyNotFoundException e) {
            Debug.Log(e.Message + " | Could not find key: " + facilityName.ToLower());
          }
          return returnColor;
        }),

        (Planet planet, Color color) => {
          planet.GetComponent<Renderer>().material.color = color;
          planet.GetComponent<Renderer>().material.SetColor("_EmissionColor", color);
        },
        false
      );

      m_distanceMapping = (LinearMapping)distanceMapper.Mapping;

      m_starSystemMappingManager += stellarMapper;
      m_starSystemMappingManager += distanceMapper;
      m_planetMappingManager += facillityColorMapper;
    }
  }
}