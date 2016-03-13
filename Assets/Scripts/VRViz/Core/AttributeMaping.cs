using System;
using System.Collections;

namespace VRViz {
  namespace Core {
    public interface IAttributeMapper<DataType> {
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

      private InputAccessor m_getInput;
      private Mapping<InputType, OutputType> m_mapping;
      private AtributeApplier m_applier;

      public AttributeMapper(InputAccessor getInput, Mapping<InputType, OutputType> dataMapping, AtributeApplier applier) {
        m_getInput = getInput;
        m_mapping = dataMapping;
        m_applier = applier;
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
    }
  }
}