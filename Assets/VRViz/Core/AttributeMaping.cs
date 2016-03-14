using System;
using System.Collections;

namespace VRViz {
  namespace Core {
    public interface IAttributeMapper<DataType> {
      event MappingChangedHandler OnMappingChanged;
      void ApplyMapping(DataType dataNode);
      void ApplyMapping(DataType[] dataNodes);
    }

    public class AttributeMapper<DataType, InputType, OutputType> :
      IAttributeMapper<DataType>
      where InputType : struct, IComparable<InputType>, IEquatable<InputType>
      where OutputType : struct, IComparable<OutputType>, IEquatable<OutputType>
    {
      public delegate InputType InputAccessor(DataType dataNode);
      public delegate void AtributeApplier(DataType target, OutputType valueToApply);

      public event MappingChangedHandler OnMappingChanged;

      private InputAccessor m_getInput;
      private IMapping<InputType, OutputType> m_mapping;
      private AtributeApplier m_applier;

      public AttributeMapper(InputAccessor getInput, IMapping<InputType, OutputType> dataMapping, AtributeApplier applier) {
        m_getInput = getInput;
        m_mapping = dataMapping;
        m_applier = applier;

        m_mapping.OnMappingChanged += fireMappingChanged;
      }

      public IMapping<InputType, OutputType> Mapping {
        get {
          return m_mapping;
        }
      }

      public void ApplyMapping(DataType dataNode) {
        InputType input = m_getInput(dataNode);
        OutputType mappedValue = m_mapping.MapValue(input);
        m_applier(dataNode, mappedValue);
      }

      public void ApplyMapping(DataType[] dataNodes) {
        for (int i = 0; i < dataNodes.Length; i++) {
          ApplyMapping(dataNodes[i]);
        }
      }

      protected void fireMappingChanged() {
        MappingChangedHandler e = OnMappingChanged;
        if (e != null)
          e();
      }
    }
  }
}