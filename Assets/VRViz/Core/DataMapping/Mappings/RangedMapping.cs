using System;
using System.Collections;

namespace VRViz {
  namespace Core {
    public abstract class RangedMapping<InputType, OutputType> :
          IMapping<InputType, OutputType>
      where InputType : struct, IComparable<InputType>, IEquatable<InputType>
      where OutputType : struct, IComparable<OutputType>, IEquatable<OutputType> {
      public event MappingChangedHandler OnMappingChanged;

      private Range<InputType> m_input;
      private Range<OutputType> m_output;

      public Range<InputType> Input {
        get {
          return m_input;
        }
        set {
          if (m_input == value)
            return;

          m_input = value;

          fireMappingChanged();
        }
      }

      public Range<OutputType> Output {
        get {
          return m_output;
        }
        set {
          if (m_output == value)
            return;

          m_output = value;

          fireMappingChanged();
        }
      }

      public RangedMapping(Range<InputType> mappingInput, Range<OutputType> mappingOutput) {
        m_input = mappingInput;
        m_output = mappingOutput;

        m_input.OnBoundsChagned += OnBoundsChanged<InputType>;
        m_output.OnBoundsChagned += OnBoundsChanged<OutputType>;
      }

      private void OnBoundsChanged<T>(T min, T max) {
        MappingChangedHandler e = OnMappingChanged;
        if (e != null)
          e();
      }

      protected void fireMappingChanged() {
        MappingChangedHandler e = OnMappingChanged;
        if (e != null)
          e();
      }

      abstract public OutputType MapValue(InputType value);
    }
  }
}