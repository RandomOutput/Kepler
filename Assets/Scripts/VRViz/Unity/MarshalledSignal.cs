using UnityEngine;
using System.Collections;

public class MarshalledSignal : MonoBehaviour {
  public delegate void SignalHandler();
  public event SignalHandler OnSignal;

  private bool m_signal = false;
  private object m_signalLock = new object();
	
	void Update () {
    checkSignal();
	}

  private void checkSignal() {
    lock (m_signalLock) {
      if (m_signal == true) {
        SignalHandler e = OnSignal;
        if (e != null)
          e();
        m_signal = false;
      }
    }
  }

  public void Signal() {
    lock (m_signalLock) {
      m_signal = true;
    }
  }
}
