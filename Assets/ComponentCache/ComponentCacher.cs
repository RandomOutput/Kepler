using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ComponentCaching {
  public class ComponentCacher {
    private readonly GameObject m_gameObject;
    private Dictionary<System.Type, Component> m_componentCache;

    public ComponentCacher(GameObject gameObject) {
      m_gameObject = gameObject;
      m_componentCache = new Dictionary<System.Type, Component>();
    }

    public T GetComponent<T>() where T : Component {
      System.Type type = typeof(T);
      if (m_componentCache.ContainsKey(type) && m_componentCache[type] != null)
        return (T)m_componentCache[type];

      T component;

      if (m_componentCache.ContainsKey(type)) {
        component = m_gameObject.GetComponent<T>();
      }
      else {
        component = m_gameObject.GetComponent<T>();
      }

      if (component == null)
        return null;

      m_componentCache.Add(type, m_gameObject.GetComponent<T>());
      return component;
    }

    public void InvalidateCache() {
      m_componentCache.Clear();
    }

    public void InvalidateCache<T>() {
      m_componentCache.Remove(typeof(T));
    }
  }
}