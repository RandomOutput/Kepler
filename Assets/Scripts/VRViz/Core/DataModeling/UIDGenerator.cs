using System;

namespace VRViz {
  namespace Core {
    public class UIDGenerator {
      private static uint nextID = 0;

      public static uint GetNewUID() {
        if (nextID == uint.MaxValue)
          throw new OverflowException("too many unique IDs generated. uint can only hold " + uint.MaxValue + " ids.");

        return nextID++;
      }
    }
  }
}