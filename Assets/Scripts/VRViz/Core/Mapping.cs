using System;
using System.Collections;

namespace VRViz {
  namespace Core {
    public abstract class Mapping<InputType, OutputType> 
      where InputType : struct, IComparable<InputType>, IEquatable<InputType>
      where OutputType : struct, IComparable<OutputType>, IEquatable<OutputType>
    {
      public delegate void MappingChangedHandler(Range<InputType> input, Range<OutputType> output);
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

      public Mapping(Range<InputType> mappingInput, Range<OutputType> mappingOutput) {
        m_input = mappingInput;
        m_output = mappingOutput;

        m_input.OnBoundsChagned += OnBoundsChanged<InputType>;
        m_output.OnBoundsChagned += OnBoundsChanged<OutputType>;
      }

      private void OnBoundsChanged<T>(T min, T max) {
        MappingChangedHandler e = OnMappingChanged;
        if (e != null)
          e(m_input, m_output);
      }

      protected void fireMappingChanged() {
        MappingChangedHandler e = OnMappingChanged;
        if (e != null)
          e(Input, Output);
      }

      abstract public OutputType MapValue(InputType value);
    }

    public class LinearMapping : Mapping<float, float> {
      public LinearMapping(Range<float> mappingInput, Range<float> mappingOutput) :
        base(mappingInput, mappingOutput) { }

      public override float MapValue(float value) {
        float noralized = (value - Input.Min) / (Input.Max - Input.Min);
        float mapped = Output.Min + (noralized * (Output.Max - Output.Min));
        return mapped;
      }
    }

    public class FunctionalMapping<Input, Output> : 
      Mapping<Input, Output>
      where Input : struct, IComparable<Input>, IEquatable<Input>
      where Output : struct, IComparable<Output>, IEquatable<Output>
    {
      public delegate Output MappingFunction(Input input);
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

      public FunctionalMapping(Range<Input> mappingInput, Range<Output> mappingOutput, MappingFunction mapper) :
        base(mappingInput, mappingOutput) 
      {
        m_mapper = mapper;
      }

      public override Output MapValue(Input value) {
        return m_mapper(value);
      }
    }
  }
}