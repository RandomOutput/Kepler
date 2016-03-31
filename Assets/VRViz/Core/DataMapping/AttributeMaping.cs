using System;
using System.Collections;
using System.Collections.Generic;

namespace VRViz {
  namespace Core {
    public delegate void AutoUpdateChangedHandler(bool newAutoUpdateSetting);

    public interface IAttributeMapper<DataType> {
      event MappingChangedHandler OnMappingChanged;
      event AutoUpdateChangedHandler OnAutoUpdateChanged;

      bool AutoUpdate {
        get;
        set;
      }

      void ApplyMapping(DataType dataNode);
      void ApplyMapping(List<DataType> dataNodes);
    }

    public class AttributeMapper<DataType, InputType, OutputType> :
      IAttributeMapper<DataType>
    {
      public delegate InputType InputAccessor(DataType dataNode);
      public delegate void AtributeApplier(DataType target, OutputType valueToApply);

      public event MappingChangedHandler OnMappingChanged;
      public event AutoUpdateChangedHandler OnAutoUpdateChanged;

      private InputAccessor m_getInput;
      private IMapping<InputType, OutputType> m_mapping;
      private AtributeApplier m_applier;
      private bool m_autoUpdate;

      public bool AutoUpdate {
        get {
          return m_autoUpdate;
        }
        set {
          if (value == m_autoUpdate)
            return;

          m_autoUpdate = value;

          AutoUpdateChangedHandler e = OnAutoUpdateChanged;
          if (e != null)
            e(m_autoUpdate);
        }
      }

      public AttributeMapper(InputAccessor getInput, IMapping<InputType, OutputType> dataMapping, AtributeApplier applier, bool autoUpdate = true) {
        m_getInput = getInput;
        m_mapping = dataMapping;
        m_applier = applier;
        m_autoUpdate = autoUpdate;

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

      public void ApplyMapping(List<DataType> dataNodes) {
        for (int i = 0; i < dataNodes.Count; i++) {
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