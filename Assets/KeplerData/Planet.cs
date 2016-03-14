using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Planet : MonoBehaviour {
  private KeplerData.KeplerNode m_dataNode;
  private Text m_cachedNameLabel;
  private Vector3 m_fullScale;

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

  void Awake() {
    m_fullScale = transform.localScale;
  }

  public void Show() {
    transform.localScale = Vector3.zero;
    gameObject.SetActive(true);
    StartCoroutine(ScaleUniform(m_fullScale.x, 1.0f));
  }

  public void Hide() {
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
    NameLabel.text = m_dataNode.Name;
  }
}
