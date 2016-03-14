using System;
using System.Collections;

namespace VRViz {
  namespace Core {
    public delegate void MappingChangedHandler();

    public interface IMapping<InputType, OutputType> {
      event MappingChangedHandler OnMappingChanged;
      OutputType MapValue(InputType value);
    }

    public abstract class RangedMapping<InputType, OutputType> :
      IMapping<InputType, OutputType>
      where InputType : struct, IComparable<InputType>, IEquatable<InputType>
      where OutputType : struct, IComparable<OutputType>, IEquatable<OutputType>
    {
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

    public class LinearMapping : RangedMapping<float, float> {
      public LinearMapping(Range<float> mappingInput, Range<float> mappingOutput) :
        base(mappingInput, mappingOutput) { }

      public override float MapValue(float value) {
        float noralized = (value - Input.Min) / (Input.Max - Input.Min);
        float mapped = Output.Min + (noralized * (Output.Max - Output.Min));
        return mapped;
      }
    }

    public class FunctionalMapping<InputType, OutputType> :
      IMapping<InputType, OutputType>
    {
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
        base()
      {
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