using UnityEngine;
using System.Collections;

namespace VRViz {
  namespace Core {
    public class FunctionalMapping<InputType, OutputType> :
          IMapping<InputType, OutputType> {
      public delegate OutputType MappingFunction(InputType input);

      public event MappingChangedHandler OnMappingChanged;

      private MappingFunction m_mapper;

      public MappingFunction Mapper {
        get {
          return m_mapper;
        }

        set {
          if (value == m_mapper)
            return;

          if (value == null)
            throw new System.ArgumentNullException("Cannot set mapping function to null");

          m_mapper = value;

          fireMappingChanged();
        }
      }

      public FunctionalMapping(MappingFunction mapper) :
        base() {
        m_mapper = mapper;
      }

      public OutputType MapValue(InputType value) {
        return m_mapper(value);
      }

      protected void fireMappingChanged() {
        MappingChangedHandler e = OnMappingChanged;
        if (e != null)
          e();
      }
    }
  }
}