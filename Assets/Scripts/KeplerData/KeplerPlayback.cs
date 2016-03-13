using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VRViz.Core;

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

    private LinearMapping m_distanceMapping;

    private List<string> m_uniqueFacilities;
    private Dictionary<string, Color> m_facilityColors;

    void Awake() {
      KeplerParser.ParseKeplerDataAsync((KeplerNode[] nodes, List<string> uniqueFalcilities) => {
        m_uniqueFacilities = uniqueFalcilities;
        
        NodeStoreDefault<KeplerNode> nodeStore = new NodeStoreDefault<KeplerNode>(nodes);

        // Configure Distance Mapping
        m_distanceMapping = new LinearMapping(
          new VRViz.Core.Range<float>(distance_inputMin, distance_inputMax),
          new VRViz.Core.Range<float>(distance_outputMin, distance_outputMax)
         );

        for (int i = 0; i < m_uniqueFacilities.Count; i++) {
          Debug.Log("Facility " + i + ": " + m_uniqueFacilities[i]);
        }

        m_facilityColors = new Dictionary<string, Color>();
        for (int i = 0; i < m_uniqueFacilities.Count; i++) {
          m_facilityColors.Add(m_uniqueFacilities[i], new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value));
        }

        // Spawn planets
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
          projectPlanet(newPlanet);
          m_spawnedPlanets.Add(node.UID, newPlanetVisual.GetComponent<Planet>());
        }

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

        m_distanceMapping.OnMappingChanged += ReprojectPlanets;

        // Start Data Playback
        playhead.InitializePlayhead();
        StartCoroutine(moveTimeForward(playhead));
      });
    }

    public void ReprojectPlanets(Range<float> input, Range<float> output) {
      Planet[] planets = new Planet[m_spawnedPlanets.Values.Count];
      m_spawnedPlanets.Values.CopyTo(planets, 0);
      for (int i = 0; i < planets.Length; i++) {
        projectPlanet(planets[i]);
      }
    }

    private void projectPlanet(Planet planet) {
      planet.transform.localPosition = planet.DataNode.Position.ToPosition(m_distanceMapping);
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
  }
}