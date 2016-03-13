using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace VRViz {
  namespace Core {
    public class NodeStoreDefault<NodeType> :
      NodeStore<uint, NodeType>
      where NodeType : INode {
      public NodeStoreDefault(INode[] nodes)
        : base(new Dictionary<uint, NodeType>()) {
        foreach (NodeType node in nodes) {
          Nodes.Add(node.UID, node);
        }
      }
    }

    public class NodeStore<Keytype, NodeType> where NodeType : INode {
      public readonly Dictionary<Keytype, NodeType> Nodes;

      public NodeStore(Dictionary<Keytype, NodeType> nodes) {
        Nodes = nodes;
      }
    }
  }
}