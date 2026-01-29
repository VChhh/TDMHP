using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using TDMHP.Combat;
using TDMHP.Combat.Weapons;
using TDMHP.Input;

namespace TDMHP.Editor.Weapons
{
    internal sealed class MoveNode : Node
    {
        private readonly Dictionary<CombatIntent, Port> _outputs = new();

        public MoveNode(AttackMoveData move, CombatIntent[] intents)
        {
            Move = move;
            title = move != null ? move.name : "Move";

            InputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(AttackMoveData));
            InputPort.portName = "In";
            inputContainer.Add(InputPort);
            EdgeConnectorUtils.AddConnector(InputPort);

            for (int i = 0; i < intents.Length; i++)
            {
                var intent = intents[i];
                // One output per intent; capacity single to keep routing unambiguous.
                var port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(AttackMoveData));
                port.portName = intent.ToString();
                port.userData = intent;
                _outputs[intent] = port;
                outputContainer.Add(port);
                EdgeConnectorUtils.AddConnector(port);
            }

            RefreshExpandedState();
            RefreshPorts();
        }

        public AttackMoveData Move { get; }
        public Port InputPort { get; }

        public Port GetOutputPort(CombatIntent intent)
        {
            return _outputs.TryGetValue(intent, out var port) ? port : null;
        }

        public bool TryGetIntent(Port port, out CombatIntent intent)
        {
            if (port != null && port.userData is CombatIntent storedIntent)
            {
                intent = storedIntent;
                return true;
            }

            intent = default;
            return false;
        }
    }
}
