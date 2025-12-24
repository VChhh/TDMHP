using UnityEngine;

namespace TDMHP.Combat.Damage
{
    public sealed class DamageDebugHUD : MonoBehaviour
    {
        [SerializeField] private HealthComponent _health;
        [SerializeField] private StaggerMeter _poise;

        private void OnGUI()
        {
            if (_health == null) return;

            string hp = $"HP: {_health.Current:0.0}/{_health.Max:0.0}";
            string poise = _poise != null ? $"Poise: {_poise.Current:0.0}/{_poise.Max:0.0}" : "Poise: (none)";
            GUI.Label(new Rect(20, 20, 400, 30), hp);
            GUI.Label(new Rect(20, 45, 400, 30), poise);
        }
    }
}
