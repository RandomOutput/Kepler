using System.Collections;
using System.Collections.Generic;

namespace VRViz {
  namespace Core {
    public class MappingManager<T> {
      public delegate List<T> DataAccessor();

      private List<IAttributeMapper<T>> m_mappers;
      private DataAccessor m_dataAccessor;

      public MappingManager(DataAccessor dataAccessor) {
        m_mappers = new List<IAttributeMapper<T>>();
        m_dataAccessor = dataAccessor;
      }

      public static MappingManager<T> operator +(MappingManager<T> manager, IAttributeMapper<T> mapper) {
        manager.AddMapper(mapper);
        return manager;
      }

      public static MappingManager<T> operator -(MappingManager<T> manager, IAttributeMapper<T> mapper) {
        manager.RemoveMapper(mapper);
        return manager;
      }

      public void AddMapper(IAttributeMapper<T> mapper) {
        m_mappers.Add(mapper);
        mapper.OnMappingChanged += AutoUpdate;
      }

      public void RemoveMapper(IAttributeMapper<T> mapper) {
        m_mappers.Remove(mapper);
        mapper.OnMappingChanged -= AutoUpdate;
      }

      public void UpdateAllMappers() {
        UpdateMappers(true);
      }

      private void AutoUpdate() {
        UpdateMappers(false);
      }

      public virtual void UpdateMappers(bool updateAllMappers = false) {
        List<T> data = m_dataAccessor();

        for (int i = 0; i < m_mappers.Count; i++) {
          if (!m_mappers[i].AutoUpdate && !updateAllMappers)
            return;
          m_mappers[i].ApplyMapping(data);
        }
      }
    }
  }
}