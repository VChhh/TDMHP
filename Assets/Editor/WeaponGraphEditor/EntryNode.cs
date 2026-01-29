using UnityEditor.Experimental.GraphView;
using UnityEngine;
using TDMHP.Combat;
using TDMHP.Combat.Weapons;

namespace TDMHP.Editor.Weapons
{
    internal sealed class EntryNode : Node
    {
        public EntryNode(EntrySlot slot)
        {
            Slot = slot;
            title = slot == EntrySlot.Light ? "Light Entry" : "Heavy Entry";

            OutputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(AttackMoveData));
            OutputPort.portName = "Start";
            outputContainer.Add(OutputPort);
            EdgeConnectorUtils.AddConnector(OutputPort);

            RefreshExpandedState();
            RefreshPorts();
        }

        public EntrySlot Slot { get; }
        public Port OutputPort { get; }
    }

    internal enum EntrySlot
    {
        Light,
        Heavy
    }
}
