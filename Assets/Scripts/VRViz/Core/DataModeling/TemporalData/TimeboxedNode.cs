using System;
namespace VRViz {
  namespace Core {
    public class TimeboxedNode : INode, ITimeboxed {
      public event NodeLifetimeHandler OnBirth;
      public event NodeLifetimeHandler OnDeath;

      private readonly uint m_uid;
      private readonly float m_startTime;
      private readonly float m_endTime;

      private bool m_alive;

      public float StartTime {
        get {
          return m_startTime;
        }
      }

      public float EndTime {
        get {
          return m_endTime;
        }
      }

      public uint UID {
        get {
          return m_uid;
        }
      }

      virtual public bool Alive {
        get {
          return m_alive;
        }
        set {
          if (value == m_alive)
            return;

          m_alive = value;

          NodeLifetimeHandler e = m_alive ? OnBirth : OnDeath;
          if (e != null)
            e(this);
        }
      }

      public TimeboxedNode(float startTime, float endTime = float.MaxValue)
        : base() {
        if (endTime <= startTime)
          throw new System.ArgumentException("End time must be after start time: " + endTime + " <= " + startTime);

        m_uid = UIDGenerator.GetNewUID();
        m_startTime = startTime;
        m_endTime = endTime;
        m_alive = false;
      }
    }
  }
}