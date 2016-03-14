using System;
using System.Collections;

namespace VRViz {
  namespace Core {
    public delegate void NodeLifetimeHandler(INode node);

    public interface INode {
      event NodeLifetimeHandler OnBirth;
      event NodeLifetimeHandler OnDeath;

      uint UID {
        get;
      }

      bool Alive {
        get;
        set;
      }
    }
  }
}