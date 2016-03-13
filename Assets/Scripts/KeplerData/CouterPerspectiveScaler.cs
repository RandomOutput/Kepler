using UnityEngine;
using System.Collections;

public class CouterPerspectiveScaler : MonoBehaviour {
  public float prescaler;
  [SerializeField]
  private Camera m_targetCamera;

  private float m_fovConversion;

  void Awake() {
    m_fovConversion = (2 * Mathf.Tan(m_targetCamera.fieldOfView / 2 * Mathf.Deg2Rad)) * m_targetCamera.pixelHeight;
  }
	
	// Update is called once per frame
	void Update () {
    counterLinearPerspective();
	}

  private void counterLinearPerspective() {
    if (m_targetCamera == null)
      return;

    float scale = calculateScaleRatio() * prescaler;
    transform.localScale = new Vector3(scale, scale, scale);
  }

  private float calculateScaleRatio() {
    Vector3 cameraToThis = transform.position - m_targetCamera.transform.position;
    float distanceAlongCameraForward = Vector3.Dot(m_targetCamera.transform.forward, cameraToThis);
    float heightAtDistanceOne = 1.0f / m_fovConversion;
    float heightAtActualDistance = distanceAlongCameraForward / distanceAlongCameraForward;
    float scaleRatio = heightAtDistanceOne / heightAtActualDistance;
    return scaleRatio;
  }
}
