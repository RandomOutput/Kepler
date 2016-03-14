using UnityEngine;
using UnityEngine.VR;
using System.Collections;

public class BillboardToCamera : MonoBehaviour {
  public Vector3 WorldUp;
  public Camera TargetCamera;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
    faceTargetCamera();
	}

  private void faceTargetCamera() {
    Vector3 objectToCamera = transform.position - TargetCamera.transform.position;
    Quaternion rotation = transform.rotation;
    rotation.SetLookRotation(objectToCamera, WorldUp);
    transform.rotation = rotation;
  }
}
