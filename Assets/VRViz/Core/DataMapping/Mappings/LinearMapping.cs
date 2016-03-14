using System.Collections;

namespace VRViz {
  namespace Core {
    public class LinearMapping : RangedMapping<float, float> {
      public LinearMapping(Range<float> mappingInput, Range<float> mappingOutput) :
        base(mappingInput, mappingOutput) { }

      public override float MapValue(float value) {
        float noralized = (value - Input.Min) / (Input.Max - Input.Min);
        float mapped = Output.Min + (noralized * (Output.Max - Output.Min));
        return mapped;
      }
    }
  }
}