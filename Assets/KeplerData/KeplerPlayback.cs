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
    private Transform m_exoplanetRoot;

    private Dictionary<uint, Planet> m_spawnedPlanets = new Dictionary<uint, Planet>();

    private FunctionalMapping<StellarCoordinates, ComparableVec3> m_stellarMapping;
    private LinearMapping m_distanceMapping;

    private List<string> m_uniqueFacilities;
    private Dictionary<string, Color> m_facilityColors;

    private MappingManager<Planet> m_planetMappingManager;

    void Awake() {
      KeplerParser.ParseKeplerDataAsync((KeplerNode[] nodes, List<string> uniqueFalcilities) => {
        m_uniqueFacilities = uniqueFalcilities;

        NodeStoreDefault<KeplerNode> nodeStore = new NodeStoreDefault<KeplerNode>(nodes);

        configureMappings();

        m_facilityColors = new Dictionary<string, Color>();
        for (int i = 0; i < m_uniqueFacilities.Count; i++) {
          m_facilityColors.Add(m_uniqueFacilities[i], new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value));
        }

        spawnPlanets(nodeStore);

        // Create playhead
        Playhead<KeplerNode> playhead = new Playhead<KeplerNode>(nodeStore, 1991.0f, 2017.0f, true);
        playhead.OnStep += (Playhead<KeplerNode> head, float playheadTime, float stepSize) => { Debug.Log(playheadTime); };

        // Configure Node Birth and Death Behaviors
        playhead.OnNodeBorn += (TimeboxedNode node) => {
          StartCoroutine(showPlanetAfterTime(m_spawnedPlanets[node.UID], UnityEngine.Random.Range(0.0f, 1.0f)));
        };

        playhead.OnNodeDied += (TimeboxedNode node) => {
          m_spawnedPlanets[node.UID].Hide();
        };

        // Start Data Playback
        playhead.InitializePlayhead();
        StartCoroutine(moveTimeForward(playhead));
      });
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
      planet.Show();
    }

    private IEnumerator moveTimeForward(Playhead<KeplerNode> playhead) {
      while (true) {
        playhead.StepPlayhead(1f);
        yield return new WaitForSeconds(1.5f);
      }
    }

    private void spawnPlanets(NodeStoreDefault<KeplerNode> nodeStore) {
      foreach (KeplerNode node in nodeStore.Nodes.Values) {
        GameObject newPlanetVisual = GameObject.Instantiate(m_planetPrefab);
        newPlanetVisual.name = node.Name;
        newPlanetVisual.transform.parent = m_exoplanetRoot;
        newPlanetVisual.SetActive(false);
        try {
          newPlanetVisual.GetComponent<Renderer>().material.color = m_facilityColors[node.DiscoveringFaciltiy.ToLower()];
          newPlanetVisual.GetComponent<Renderer>().material.SetColor("_EmissionColor", m_facilityColors[node.DiscoveringFaciltiy.ToLower()]);
        }
        catch (System.Collections.Generic.KeyNotFoundException e) {
          Debug.Log(e.Message + " | Could not find key: " + node.DiscoveringFaciltiy.ToLower());
        }
        Planet newPlanet = newPlanetVisual.GetComponent<Planet>();
        newPlanet.DataNode = node;
        m_spawnedPlanets.Add(node.UID, newPlanetVisual.GetComponent<Planet>());
      }

      m_planetMappingManager.UpdateMappers();
    }

    private void configureMappings() {
      // TODO:  This is horrible. Giant array allocation every time you need to get data.
      //        I really need to fix this up.
      m_planetMappingManager = new MappingManager<Planet>(
        () => {
          Planet[] planets = new Planet[m_spawnedPlanets.Values.Count];
          m_spawnedPlanets.Values.CopyTo(planets, 0);
          return planets;
        }
      );

      AttributeMapper<Planet, StellarCoordinates, ComparableVec3> stellarMapper = new AttributeMapper<Planet, StellarCoordinates, ComparableVec3>(
        (Planet planet) => planet.DataNode.Position,
        new FunctionalMapping<StellarCoordinates, ComparableVec3>((StellarCoordinates stellarCoords) => {
          return Quaternion.Euler(-stellarCoords.declination, stellarCoords.rightAscention, 0.0f) * Vector3.forward;
        }),
        (Planet planet, ComparableVec3 newPosition) => {
          float distance = planet.transform.localPosition.magnitude;
          planet.transform.localPosition = (Vector3)newPosition * distance;
        }
      );

      AttributeMapper<Planet, float, float> distanceMapper = new AttributeMapper<Planet, float, float>(
        (Planet planet) => planet.DataNode.Position.distance,
        new LinearMapping(
          new VRViz.Core.Range<float>(distance_inputMin, distance_inputMax),
          new VRViz.Core.Range<float>(distance_outputMin, distance_outputMax)
        ),
        (Planet planet, float newDistance) => { planet.transform.localPosition = planet.transform.localPosition.normalized * newDistance; }
      );

      m_distanceMapping = (LinearMapping)distanceMapper.Mapping;

      m_planetMappingManager += stellarMapper;
      m_planetMappingManager += distanceMapper;
    }
  }
}