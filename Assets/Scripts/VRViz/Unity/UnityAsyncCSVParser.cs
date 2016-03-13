using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using VRViz.Core;
using UnityEngine;

namespace VRViz {
  namespace Unity {
    /// <summary>
    /// Implementors must define a concrete type for T
    /// </summary>
    /// <typeparam name="T">The type returned by the CSV parser.</typeparam>
    public class AsyncCSVParser<T> {

      private object m_outputLock = new object();
      private CSVParser<T>.ParserOutput m_output;

      public CSVParser<T>.ParserOutput ParserOutput {
        get {
          lock (m_outputLock) {
            return m_output;
          }
        }
      }

      public void InitializeParsing(string rawCSV, MarshalledSignal parsingCompleteSignal, CSVParser<T>.RowToObjectFactory factory, CSVParser<T>.ErrorOutDelegate errorOutput = null) {
        // Setup Parsing thread 
        Thread parsingThread = new Thread((object parsingObj) => {
          CSVParser<T>.ParserOutput output = CSVParser<T>.ParseCSVRowsToType(rawCSV, factory, errorOutput);
          lock (m_outputLock) {
            m_output = output;
          }         
          parsingCompleteSignal.Signal();
        });

        // Run parsing thread
        parsingThread.Start();
      }
    }
  }
}
