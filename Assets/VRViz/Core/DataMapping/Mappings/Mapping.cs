using System;
using System.Collections;

namespace VRViz {
  namespace Core {
    public delegate void MappingChangedHandler();

    public interface IMapping<InputType, OutputType> {
      event MappingChangedHandler OnMappingChanged;
      OutputType MapValue(InputType value);
    }
  }
}