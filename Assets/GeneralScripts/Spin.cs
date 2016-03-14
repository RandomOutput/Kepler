using UnityEngine;
using System.Collections;

public class Spin : MonoBehaviour {
  public float degreesPerSecond;
  	
	// Update is called once per frame
	void Update () {
    rotateAboutYAxis();
	}

  private void rotateAboutYAxis() {
    Quaternion rotation = Quaternion.Euler(0.0f, degreesPerSecond * Time.deltaTime, 0.0f);
    Quaternion newRotation = transform.localRotation * rotation;
    transform.localRotation = newRotation;
  }
}
