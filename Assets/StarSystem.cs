using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace KeplerData {
  public class StarSystem : MonoBehaviour {
    public delegate void VisibilityChangedHandler(StarSystem system, Visibility visibility);
    public event VisibilityChangedHandler OnVisibilityChanged;

    public delegate void PlanetRegistrationHandler(Planet planet);
    public event PlanetRegistrationHandler OnPlanetRegistered;

    private KeplerNode m_dataNode;
    private Text m_cachedNameLabel;
    private Vector3 m_fullScale;
    private string m_systemName;
    private List<Planet> m_planets = new List<Planet>();
    private uint m_visiblePlanetCount = 0;
    private Visibility m_visibility = Visibility.HIDDEN;

    public enum Visibility {
      SHOW,
      HIDDEN
    }

    public KeplerData.KeplerNode DataNode {
      get {
        return m_dataNode;
      }

      set {
        if (m_dataNode != null)
          throw new System.Exception("Cannot change data-node after first setting.");

        m_dataNode = value;
        configureFromDataNode();
      }
    }

    public Text NameLabel {
      get {
        if (m_cachedNameLabel == null)
          m_cachedNameLabel = GetComponentInChildren<Text>();

        return m_cachedNameLabel;
      }
    }

    public string SystemName {
      get {
        return m_systemName;
      }
      set {
        m_systemName = value;
        //NameLabel.text = m_systemName;
      }
    }

    public Visibility SystemVisibility {
      get {
        return m_visibility;
      }
      private set {
        if (m_visibility == value)
          return;

        m_visibility = value;

        VisibilityChangedHandler e = OnVisibilityChanged;
        if (e != null)
          e(this, m_visibility);

        if (m_visibility == Visibility.SHOW)
          show();
        else
          hide();
      }
    }

    public uint VisiblePlanets {
      get {
        return m_visiblePlanetCount;
      }
    }

    private uint VisiblePlanetCount {
      get {
        return m_visiblePlanetCount;
      }
      set {
        m_visiblePlanetCount = value;
        if (m_visiblePlanetCount == 0) {
          SystemVisibility = Visibility.HIDDEN;
        }
        else {
          SystemVisibility = Visibility.SHOW;
        }
      }
    }

    public static StarSystem ConstructNewStarSystem(KeplerNode dataNode, Transform parent, bool startHidden) {
      GameObject newSystemVisual = new GameObject(dataNode.HostName);
      newSystemVisual.transform.parent = parent;
      if (startHidden) {
        newSystemVisual.SetActive(false);
      }
      StarSystem newSystem = newSystemVisual.AddComponent<StarSystem>();
      newSystem.DataNode = dataNode;
      return newSystem;
    }

    void Awake() {
      m_fullScale = Vector3.one;
    }

    private void configureFromDataNode() {
      SystemName = name;
    }

    private void show() {
      transform.localScale = Vector3.zero;
      gameObject.SetActive(true);
      StartCoroutine(ScaleUniform(m_fullScale.x, 1.0f));
    }

    private void hide() {
      StopAllCoroutines();
      StartCoroutine(ScaleUniform(0.0f, 2.0f));
    }

    private IEnumerator ScaleUniform(float scale, float time) {
      float startTime = Time.time;
      Vector3 startScale = transform.localScale;
      Vector3 newScale = new Vector3(scale, scale, scale);
      if (time <= 0) {
        transform.localScale = newScale;
      }

      while (true) {
        float percent = Mathf.Min(1.0f, (Time.time - startTime) / time);
        transform.localScale = Vector3.Lerp(startScale, newScale, percent);
        if (percent >= 1)
          break;
        yield return null;
      }
    }

    public StellarCoordinates StellarPosition {
      get {
        return DataNode.Position;
      }
    }

    public void RegisterPlanet(Planet planet) {
      if(m_planets.Contains(planet))
        throw new System.Exception("Cannot re-register a planet.");
      m_planets.Add(planet);
      planet.OnVisibilityChanged += onPlanetVisibiltiyChanged;
    }

    public static StarSystem operator +(StarSystem system, Planet p) {
      system.RegisterPlanet(p);
      return system;
    }

    private void onPlanetVisibiltiyChanged(Planet p, Planet.Visibility visibility) {
      if (visibility == Planet.Visibility.SHOWN)
        VisiblePlanetCount += 1;
      else
        VisiblePlanetCount -= 1;
    }
  }
}