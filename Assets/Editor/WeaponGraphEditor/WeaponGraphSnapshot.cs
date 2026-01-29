using System.Collections.Generic;
using TDMHP.Combat;
using TDMHP.Combat.Weapons;

namespace TDMHP.Editor.Weapons
{
    internal sealed class WeaponGraphSnapshot
    {
        public AttackMoveData lightEntry;
        public AttackMoveData heavyEntry;
        public List<ComboTransition> transitions;
        public List<MoveNodeLayout> layout;
        public List<ConnectionNodeLayout> connectionLayout;
        public List<GraphEdgeData> graphEdges;
    }
}
