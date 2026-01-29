using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TDMHP.Editor.Weapons
{
    internal static class EdgeConnectorUtils
    {
        private sealed class SimpleEdgeConnectorListener : IEdgeConnectorListener
        {
            public void OnDropOutsidePort(Edge edge, Vector2 position)
            {
                if (edge?.parent != null)
                {
                    edge.parent.Remove(edge);
                }
            }

            public void OnDrop(GraphView graphView, Edge edge)
            {
                if (graphView != null && edge != null)
                {
                    graphView.AddElement(edge);
                }
            }
        }

        private static readonly SimpleEdgeConnectorListener Listener = new();

        public static void AddConnector(Port port)
        {
            if (port == null) return;
            var connector = new EdgeConnector<Edge>(Listener);
            port.AddManipulator(connector);
        }
    }
}
