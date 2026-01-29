using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using TDMHP.Combat;
using TDMHP.Combat.Weapons;
using TDMHP.Input;

namespace TDMHP.Editor.Weapons
{
    internal sealed class WeaponGraphView : GraphView
    {
        private readonly WeaponGraphEditorWindow _window;
        private readonly Dictionary<AttackMoveData, MoveNode> _moveNodes = new();
        private readonly Dictionary<string, ConnectionNode> _connectionNodes = new();
        private EntryNode _lightEntryNode;
        private EntryNode _heavyEntryNode;
        private WeaponData _currentWeapon;

        private static readonly CombatIntent[] SupportedIntents =
        {
            CombatIntent.LightAttack,
            CombatIntent.HeavyAttack
        };

        public WeaponGraphView(WeaponGraphEditorWindow window)
        {
            _window = window;
            style.flexGrow = 1;

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            graphViewChanged += OnGraphViewChanged;
        }

        public override void AddToSelection(ISelectable selectable)
        {
            base.AddToSelection(selectable);
            LogEdgeSelection();
        }

        public override void RemoveFromSelection(ISelectable selectable)
        {
            base.RemoveFromSelection(selectable);
            LogEdgeSelection();
        }

        public override void ClearSelection()
        {
            base.ClearSelection();
            LogEdgeSelection();
        }

        public Vector2 GetCanvasCenter()
        {
            var view = contentViewContainer;
            var size = view.layout.size;
            if (float.IsNaN(size.x) || float.IsNaN(size.y) || size == Vector2.zero)
                size = new Vector2(400, 300);

            return size * 0.5f;
        }

        public void LoadFromWeapon(WeaponData weapon)
        {
            _currentWeapon = weapon;
            DeleteElements(graphElements.ToList());
            _moveNodes.Clear();
            _connectionNodes.Clear();
            _lightEntryNode = null;
            _heavyEntryNode = null;

            if (weapon == null)
                return;

            var moves = new HashSet<AttackMoveData>();
            if (weapon.lightEntry != null) moves.Add(weapon.lightEntry);
            if (weapon.heavyEntry != null) moves.Add(weapon.heavyEntry);

            for (int i = 0; i < weapon.transitions.Count; i++)
            {
                var tr = weapon.transitions[i];
                if (tr.from != null) moves.Add(tr.from);
                if (tr.to != null) moves.Add(tr.to);
            }

            int index = 0;
            foreach (var move in moves)
            {
                AddMoveNode(move, GetSavedPosition(move, index));
                index++;
            }

            // Recreate saved connection joints
            if (weapon.connectionLayout != null)
            {
                foreach (var joint in weapon.connectionLayout)
                {
                    AddConnectionNode(joint.position, joint.id);
                }
            }

            _lightEntryNode = AddEntryNode(EntrySlot.Light, new Vector2(-260f, 20f));
            _heavyEntryNode = AddEntryNode(EntrySlot.Heavy, new Vector2(-260f, 180f));

            ConnectEntry(_lightEntryNode, weapon.lightEntry);
            ConnectEntry(_heavyEntryNode, weapon.heavyEntry);

            if (weapon.graphEdges != null && weapon.graphEdges.Count > 0)
            {
                foreach (var edgeData in weapon.graphEdges)
                {
                    TryAddGraphEdge(edgeData);
                }
            }
            else
            {
                // Fallback: rebuild simple edges from transitions
                for (int i = 0; i < weapon.transitions.Count; i++)
                {
                    AddEdgeForTransition(weapon.transitions[i]);
                }
            }

            if (_moveNodes.Count > 0)
                FrameAll();
        }

        public WeaponGraphSnapshot BuildSnapshot()
        {
            var snapshot = new WeaponGraphSnapshot
            {
                transitions = new List<ComboTransition>(),
                layout = new List<MoveNodeLayout>(),
                connectionLayout = new List<ConnectionNodeLayout>(),
                graphEdges = new List<GraphEdgeData>()
            };

            if (_lightEntryNode != null)
                snapshot.lightEntry = GetEntryTarget(_lightEntryNode);
            if (_heavyEntryNode != null)
                snapshot.heavyEntry = GetEntryTarget(_heavyEntryNode);

            var edgeList = edges.ToList();
            foreach (var edge in edgeList)
            {
                // Save raw graph edges for reconstruction
                var edgeData = BuildEdgeData(edge);
                if (edgeData != null)
                    snapshot.graphEdges.Add(edgeData.Value);

                // Build gameplay transitions by resolving through joints
                if (edge.output?.node is MoveNode from)
                {
                    if (from.TryGetIntent(edge.output, out var intent))
                    {
                        var target = ResolveEdgeDestination(edge, new HashSet<Edge>());
                        if (target != null)
                        {
                            snapshot.transitions.Add(new ComboTransition
                            {
                                from = from.Move,
                                intent = intent,
                                to = target.Move
                            });
                        }
                    }
                }
            }

            foreach (var kvp in _moveNodes)
            {
                var rect = kvp.Value.GetPosition();
                snapshot.layout.Add(new MoveNodeLayout
                {
                    move = kvp.Key,
                    position = rect.position
                });
            }

            foreach (var kvp in _connectionNodes)
            {
                var rect = kvp.Value.GetPosition();
                snapshot.connectionLayout.Add(new ConnectionNodeLayout
                {
                    id = kvp.Key,
                    position = rect.position
                });
            }

            return snapshot;
        }

        public void AddMoveNode(AttackMoveData move, Vector2 position)
        {
            if (move == null)
            {
                _window.NotifyStatus("Cannot add a null move.");
                return;
            }

            if (_moveNodes.TryGetValue(move, out var existing))
            {
                existing.SetPosition(new Rect(position, existing.GetPosition().size));
                return;
            }

            var node = new MoveNode(move, SupportedIntents);
            node.SetPosition(new Rect(position, new Vector2(220, 160)));
            AddElement(node);
            _moveNodes[move] = node;
        }

        public void AddConnectionNode(Vector2 position, string id = null)
        {
            var node = new ConnectionNode(id);
            node.SetPosition(new Rect(position, new Vector2(180, 120)));
            AddElement(node);
            _connectionNodes[node.Id] = node;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            evt.menu.AppendAction("Add Move Node (from selection)", _ =>
            {
                var selectedMove = Selection.activeObject as AttackMoveData;
                if (selectedMove != null)
                {
                    var graphPos = contentViewContainer.WorldToLocal(evt.localMousePosition);
                    AddMoveNode(selectedMove, graphPos);
                }
                else
                {
                    _window.NotifyStatus("Select an AttackMoveData asset in the Project window to add.");
                }
            });
            evt.menu.AppendAction("Add Connection Node", _ =>
            {
                var graphPos = contentViewContainer.WorldToLocal(evt.localMousePosition);
                AddConnectionNode(graphPos);
            });
        }

        public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter)
        {
            var list = new List<Port>();
            ports.ForEach(port =>
            {
                if (startAnchor != port &&
                    startAnchor.node != port.node &&
                    startAnchor.direction != port.direction &&
                    startAnchor.portType == port.portType)
                {
                    list.Add(port);
                }
            });
            return list;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange change)
        {
            if (change.elementsToRemove != null)
            {
                foreach (var element in change.elementsToRemove)
                {
                    if (element is MoveNode moveNode && moveNode.Move != null)
                    {
                        _moveNodes.Remove(moveNode.Move);
                    }

                    if (element is EntryNode entryNode)
                    {
                        if (entryNode.Slot == EntrySlot.Light) _lightEntryNode = null;
                        if (entryNode.Slot == EntrySlot.Heavy) _heavyEntryNode = null;
                    }

                    if (element is ConnectionNode connectionNode && !string.IsNullOrEmpty(connectionNode.Id))
                    {
                        _connectionNodes.Remove(connectionNode.Id);
                    }
                }
            }

            // DetectDisconnectAll();
            // Clean orphaned edges after GraphView finishes its internal updates.
            schedule.Execute(CleanupOrphanEdges);
            return change;
        }

        private Vector2 GetSavedPosition(AttackMoveData move, int index)
        {
            if (_currentWeapon != null && _currentWeapon.nodeLayout != null)
            {
                for (int i = 0; i < _currentWeapon.nodeLayout.Count; i++)
                {
                    var layout = _currentWeapon.nodeLayout[i];
                    if (layout.move == move)
                        return layout.position;
                }
            }

            float x = 120f + (index % 3) * 240f;
            float y = 40f + (index / 3) * 180f;
            return new Vector2(x, y);
        }

        private EntryNode AddEntryNode(EntrySlot slot, Vector2 position)
        {
            var node = new EntryNode(slot);
            node.SetPosition(new Rect(position, new Vector2(180, 120)));
            AddElement(node);

            if (slot == EntrySlot.Light) _lightEntryNode = node;
            if (slot == EntrySlot.Heavy) _heavyEntryNode = node;
            return node;
        }

        private void ConnectEntry(EntryNode entryNode, AttackMoveData target)
        {
            if (entryNode == null || target == null)
                return;

            if (!_moveNodes.TryGetValue(target, out var targetNode))
            {
                AddMoveNode(target, GetSavedPosition(target, _moveNodes.Count));
                targetNode = _moveNodes[target];
            }

            var edge = entryNode.OutputPort.ConnectTo(targetNode.InputPort);
            AddElement(edge);
        }

        private void AddEdgeForTransition(ComboTransition transition)
        {
            if (transition.from == null || transition.to == null)
                return;

            if (!_moveNodes.TryGetValue(transition.from, out var fromNode))
            {
                AddMoveNode(transition.from, GetSavedPosition(transition.from, _moveNodes.Count));
                fromNode = _moveNodes[transition.from];
            }

            if (!_moveNodes.TryGetValue(transition.to, out var toNode))
            {
                AddMoveNode(transition.to, GetSavedPosition(transition.to, _moveNodes.Count));
                toNode = _moveNodes[transition.to];
            }

            var outputPort = fromNode.GetOutputPort(transition.intent);
            if (outputPort == null)
                return;

            var edge = outputPort.ConnectTo(toNode.InputPort);
            AddElement(edge);
        }

        private AttackMoveData GetEntryTarget(EntryNode entryNode)
        {
            if (entryNode == null)
                return null;

            var outgoing = edges.ToList().Where(e => e.output == entryNode.OutputPort);
            foreach (var edge in outgoing)
            {
                var target = ResolveEdgeDestination(edge, new HashSet<Edge>());
                if (target != null)
                    return target.Move;
            }

            return null;
        }

        private MoveNode ResolveEdgeDestination(Edge edge, HashSet<Edge> visited)
        {
            if (edge == null || visited.Contains(edge))
                return null;

            visited.Add(edge);

            var targetNode = edge.input?.node;
            switch (targetNode)
            {
                case MoveNode moveNode:
                    return moveNode;
                case ConnectionNode connectionNode:
                    foreach (var next in edges)
                    {
                        if (next.output == connectionNode.OutputPort)
                        {
                            var resolved = ResolveEdgeDestination(next, visited);
                            if (resolved != null)
                                return resolved;
                        }
                    }
                    break;
            }

            return null;
        }

        private void CleanupOrphanEdges()
        {
            var toRemove = new List<Edge>();
            edges.ForEach(e =>
            {
                if (e == null ||
                    e.output == null || e.output.node == null ||
                    e.input == null || e.input.node == null)
                {
                    toRemove.Add(e);
                }
            });

            if (toRemove.Count > 0)
                DeleteElements(toRemove);
        }

        private GraphEdgeData? BuildEdgeData(Edge edge)
        {
            if (edge == null) return null;

            var fromEndpoint = BuildEndpoint(edge.output);
            var toEndpoint = BuildEndpoint(edge.input);
            if (fromEndpoint == null || toEndpoint == null)
                return null;

            var data = new GraphEdgeData
            {
                from = fromEndpoint.Value,
                to = toEndpoint.Value,
                hasIntent = false
            };

            if (edge.output?.node is MoveNode moveNode)
            {
                if (moveNode.TryGetIntent(edge.output, out var intent))
                {
                    data.hasIntent = true;
                    data.intent = intent;
                }
            }

            return data;
        }

        // private void DetectDisconnectAll()
        // {
        //     // Detect edges that were orphaned (Disconnect All sets one end to null).
        //     var orphaned = edges.ToList().Where(e =>
        //         e == null ||
        //         e.output == null || e.output.node == null ||
        //         e.input == null || e.input.node == null);

        //     if (orphaned.Any())
        //     {
        //         Debug.Log("Detected disconnect-all action; cleaning orphaned edges.");
        //     }
        // }

        private GraphEndpoint? BuildEndpoint(Port port)
        {
            if (port == null || port.node == null) return null;

            switch (port.node)
            {
                case MoveNode moveNode:
                    return new GraphEndpoint
                    {
                        type = GraphEndpointType.Move,
                        move = moveNode.Move
                    };
                case EntryNode entryNode:
                    return new GraphEndpoint
                    {
                        type = entryNode.Slot == EntrySlot.Light ? GraphEndpointType.EntryLight : GraphEndpointType.EntryHeavy
                    };
                case ConnectionNode connectionNode:
                    return new GraphEndpoint
                    {
                        type = GraphEndpointType.Joint,
                        jointId = connectionNode.Id
                    };
                default:
                    return null;
            }
        }

        private void TryAddGraphEdge(GraphEdgeData data)
        {
            var fromPort = ResolvePort(data.from, isOutput: true, data.hasIntent ? data.intent : (CombatIntent?)null);
            var toPort = ResolvePort(data.to, isOutput: false, null);

            if (fromPort == null || toPort == null)
                return;

            var edge = fromPort.ConnectTo(toPort);
            AddElement(edge);
        }

        private Port ResolvePort(GraphEndpoint endpoint, bool isOutput, CombatIntent? intent)
        {
            switch (endpoint.type)
            {
                case GraphEndpointType.EntryLight:
                    return isOutput ? _lightEntryNode?.OutputPort : null;
                case GraphEndpointType.EntryHeavy:
                    return isOutput ? _heavyEntryNode?.OutputPort : null;
                case GraphEndpointType.Move:
                    if (endpoint.move == null) return null;
                    if (!_moveNodes.TryGetValue(endpoint.move, out var moveNode))
                    {
                        AddMoveNode(endpoint.move, GetSavedPosition(endpoint.move, _moveNodes.Count));
                        moveNode = _moveNodes[endpoint.move];
                    }
                    if (isOutput)
                    {
                        if (intent.HasValue)
                            return moveNode.GetOutputPort(intent.Value);
                        return null;
                    }
                    return moveNode.InputPort;
                case GraphEndpointType.Joint:
                    if (string.IsNullOrEmpty(endpoint.jointId))
                        return null;
                    if (!_connectionNodes.TryGetValue(endpoint.jointId, out var joint))
                    {
                        // If not found, create at default position
                        AddConnectionNode(Vector2.zero, endpoint.jointId);
                        joint = _connectionNodes[endpoint.jointId];
                    }
                    return isOutput ? joint.OutputPort : joint.InputPort;
                default:
                    return null;
            }
        }

        private void LogEdgeSelection()
        {
            var edge = selection.OfType<Edge>().FirstOrDefault();
            if (edge == null)
                return;

            var fromName = DescribeNode(edge.output?.node as Node);
            var toName = DescribeNode(edge.input?.node as Node);
            var intentLabel = edge.output?.portName ?? string.Empty;
            Debug.Log($"Selected edge: {fromName} --[{intentLabel}]--> {toName}");
        }

        private string DescribeNode(Node node)
        {
            switch (node)
            {
                case MoveNode moveNode:
                    return moveNode.Move != null ? moveNode.Move.name : "Move";
                case EntryNode entryNode:
                    return entryNode.Slot == EntrySlot.Light ? "Light Entry" : "Heavy Entry";
                case ConnectionNode connectionNode:
                    return $"Joint ({connectionNode.Id})";
                default:
                    return node != null ? (string.IsNullOrEmpty(node.title) ? node.name : node.title) : "null";
            }
        }
    }
}
