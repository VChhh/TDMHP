using UnityEngine;

namespace TDMHP.Combat.Damage
{
    public sealed class DamageDebugHUD : MonoBehaviour
    {
        [SerializeField] private HealthComponent _playerHealth;
        [SerializeField] private StaggerMeter _playerPoise;


        [SerializeField] private HealthComponent _enemyHealth;
        [SerializeField] private StaggerMeter _enemyPoise;

        private void OnGUI()
        {
            var style = new GUIStyle(GUI.skin.label) { fontSize = 18 };
            int x = 20;
            int width = 500;
            int height = 30;
            int y = 20;
            int spacing = 36;

            if (_playerHealth != null)
            {
            string hp = $"Player HP: {_playerHealth.Current:0.0}/{_playerHealth.Max:0.0}";
            string poise = _playerPoise != null ? $"Player Poise: {_playerPoise.Current:0.0}/{_playerPoise.Max:0.0}" : "Player Poise: (none)";
            GUI.Label(new Rect(x, y, width, height), hp, style);
            GUI.Label(new Rect(x, y + spacing, width, height), poise, style);

            // move y down so enemy HUD won't overlap
            y += spacing * 2 + 10;
            }

            if (_enemyHealth != null)
            {
            string hp = $"Enemy HP: {_enemyHealth.Current:0.0}/{_enemyHealth.Max:0.0}";
            string poise = _enemyPoise != null ? $"Enemy Poise: {_enemyPoise.Current:0.0}/{_enemyPoise.Max:0.0}" : "Enemy Poise: (none)";
            GUI.Label(new Rect(x, y, width, height), hp, style);
            GUI.Label(new Rect(x, y + spacing, width, height), poise, style);
            }
        }
    }
}
