using System;
using UnityEditor.Experimental.GraphView;
using TDMHP.Combat;

// Utility/routing node; no gameplay data, just lets you branch/merge visually.
namespace TDMHP.Editor.Weapons
{
    internal sealed class ConnectionNode : Node
    {
        public ConnectionNode(string id = null, string titleText = "Joint")
        {
            title = string.IsNullOrEmpty(titleText) ? "Connection" : titleText;
            Id = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString("N") : id;
            capabilities &= ~Capabilities.Collapsible;
            capabilities &= ~Capabilities.Deletable; // allow delete via selection + DEL; prevents little close button

            InputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(AttackMoveData));
            InputPort.portName = "In";
            inputContainer.Add(InputPort);
            EdgeConnectorUtils.AddConnector(InputPort);

            OutputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(AttackMoveData));
            OutputPort.portName = "Out";
            outputContainer.Add(OutputPort);
            EdgeConnectorUtils.AddConnector(OutputPort);

            RefreshExpandedState();
            RefreshPorts();

            style.minWidth = 60f;
            style.maxWidth = 100f;
            style.minHeight = 40f;
            style.maxHeight = 60f;
            RefreshExpandedState();
        }

        public Port InputPort { get; }
        public Port OutputPort { get; }
        public string Id { get; }
    }
}
