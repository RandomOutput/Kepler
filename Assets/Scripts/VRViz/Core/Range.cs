using System;
using System.Collections;

namespace VRViz {
  namespace Core {
    public class Range<T> where T : struct, IComparable<T>, IEquatable<T> {
      public delegate void RangeChangedHandler(T min, T max);
      public event RangeChangedHandler OnBoundsChagned;

      private T m_min;
      private T m_max;

      public T Min {
        get {
          return m_min;
        }
        set {
          if (value.Equals(m_min))
            return;

          if (value.Equals(m_max))
            throw new System.ArgumentException("Bounds may not be identical");

          m_min = value;

          RangeChangedHandler e = OnBoundsChagned;
          if (e != null)
            e(m_min, m_max);
        }
      }

      public T Max {
        get {
          return m_max;
        }
        set {
          if (value.Equals(m_max))
            return;

          if (value.Equals(m_min))
            throw new System.ArgumentException("Bounds may not be identical");

          m_max = value;

          RangeChangedHandler e = OnBoundsChagned;
          if (e != null)
            e(m_min, m_max);
        }
      }

      public Range(T boundsMin, T boundsMax) {
        if (boundsMin.Equals(boundsMax))
          throw new System.ArgumentException("Bounds may not be identical");

        m_min = boundsMin;
        m_max = boundsMax;
      } 
    }
  }
}