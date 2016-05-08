using UnityEngine;
using System.Collections;
using LineDrawing;

public class BezzierSectionTest : MonoBehaviour {
  public int m_pointCount;
  public Transform m_p0;
  public Transform m_p1;
  public Transform m_p2;
  public Transform m_p3;

  private BezzierSection m_bezzierSection;

	// Use this for initialization
	void Start () {
    m_bezzierSection = new BezzierSection(m_pointCount, m_p0.position, m_p1.position, m_p2.position, m_p3.position);
	}

	// Update is called once per frame
  void Update () {
    m_bezzierSection[0] = m_p0.position;
    m_bezzierSection[1] = m_p1.position;
    m_bezzierSection[2] = m_p2.position;
    m_bezzierSection[3] = m_p3.position;
	}
}
