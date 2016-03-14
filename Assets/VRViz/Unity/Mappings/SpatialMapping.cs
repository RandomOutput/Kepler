using UnityEngine;
using System;
using System.Collections;
using VRViz.Core;

namespace VRViz {
  namespace Unity {
    public class SpatialMapper<DataType, InputType> :
      AttributeMapper<DataType, InputType, ComparableVec3>
      where DataType : MonoBehaviour
      where InputType : struct, IComparable<InputType>, IEquatable<InputType>
    {

      public SpatialMapper(InputAccessor getInput, IMapping<InputType, ComparableVec3> mapValue)
        : base(getInput, mapValue, SetLocation) { }

      static protected void SetLocation(DataType target, ComparableVec3 location) {
        target.transform.localPosition = location;
      }
    }
  }
}