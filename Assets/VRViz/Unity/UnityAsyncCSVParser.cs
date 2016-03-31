using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using VRViz.Core;
using UnityEngine;

namespace VRViz {
  namespace Unity {
    public class AsyncWorker<T> {
      public delegate T WorkDelegate();

      private object m_outputLock = new object();
      private T m_output;

      public T ParserOutput {
        get {
          lock (m_outputLock) {
            return m_output;
          }
        }
      }

      public void StartWork(MarshalledSignal finishedSignal, WorkDelegate work) {
        Thread workThread = new Thread(()=> {
          T output = work();
          lock (m_outputLock) {
            m_output = output;
          }
          finishedSignal.Signal();
        });

        // Run parsing thread
        workThread.Start();
      }
    }
  }
}
