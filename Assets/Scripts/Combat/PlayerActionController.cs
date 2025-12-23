using UnityEngine;
using TDMHP.Input;
using TDMHP.Combat.Weapons;
using TDMHP.Combat.Hit;


namespace TDMHP.Combat
{
    public sealed class PlayerActionController : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private InputReader _input;
        [SerializeField] private IntentBuffer _buffer;
        [SerializeField] private PlayerMotor _motor;
        [SerializeField] private MeleeHitDetector _hitDetector;
        

        [Header("Weapon")]
        [SerializeField] private WeaponData _startingWeapon;
        

        private PlayerAction _current;

        public InputReader Input => _input;
        public IntentBuffer Buffer => _buffer;
        public PlayerMotor Motor => _motor;
        public MeleeHitDetector HitDetector => _hitDetector;

        public WeaponData Weapon { get; private set; }

        private void Reset()
        {
            _input = GetComponent<InputReader>();
            _buffer = GetComponent<IntentBuffer>();
            _motor = GetComponent<PlayerMotor>();
            _hitDetector = GetComponent<MeleeHitDetector>();

        }

        private void Awake()
        {
            if (_input == null) _input = GetComponent<InputReader>();
            if (_buffer == null) _buffer = GetComponent<IntentBuffer>();
            if (_motor == null) _motor = GetComponent<PlayerMotor>();
            if (_hitDetector == null) _hitDetector = GetComponent<MeleeHitDetector>();

        }

        private void OnEnable()
        {
            if (_startingWeapon != null)
                SetWeapon(_startingWeapon);

            SwitchTo(new IdleAction(this));
        }

        private void Update()
        {
            _current?.Tick(Time.deltaTime);
        }

        public void SwitchTo(PlayerAction next)
        {
            _current?.Exit();
            _current = next;
            _current?.Enter();
        }

        public void SetWeapon(WeaponData weapon)
        {
            Weapon = weapon;

            // Keep Input in sync with weapon mode (Melee vs Ranged action map).
            if (_input != null && Weapon != null)
                _input.SetCombatMode(Weapon.inputMode);
        }
    }
}
