using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace KeplerData {
  public class Planet : MonoBehaviour {
    public delegate void VisibilityChangedHandler(Planet planet, Visibility visibility);
    public event VisibilityChangedHandler OnVisibilityChanged;

    public enum Visibility {
      SHOWN,
      HIDDEN
    }

    private KeplerData.KeplerNode m_dataNode;
    private Text m_cachedNameLabel;
    private Vector3 m_fullScale;
    private StarSystem m_starSystem;
    private Visibility m_visibility = Visibility.HIDDEN;

    public Visibility VisibilityState {
      get{
        return m_visibility;
      }
      set {
        if(value == m_visibility)
          return;

        m_visibility = value;

        VisibilityChangedHandler e = OnVisibilityChanged;
        if (e != null)
          e(this, m_visibility);

        if(m_visibility == Visibility.SHOWN) {
          show();
        }
        else {
          hide();
        }
      }
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
          m_cachedNameLabel = GetComponentInChildren<Text>(true);

        return m_cachedNameLabel;
      }
    }

    public static Planet ConstructNewPlanet() {
      return new Planet();
    }

    void Awake() {
      m_fullScale = transform.localScale;
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

    private void configureFromDataNode() {
      NameLabel.text = m_dataNode.PlanetLetter;
    }

    public void RegisterStarSystem(StarSystem system) {
      transform.parent = system.transform;
      transform.localPosition = Vector3.zero;
      m_starSystem = system;
      m_starSystem.RegisterPlanet(this);
    }

    public static Planet operator +(Planet p, StarSystem s) {
      p.RegisterStarSystem(s);
      return p;
    }
  }
}