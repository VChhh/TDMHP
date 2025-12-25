using UnityEngine;
using System;
using TDMHP.Input;
using TDMHP.Combat.Weapons;
using TDMHP.Combat.Hit;
using TDMHP.Combat.Resources;
using TDMHP.Combat.Damage;
using TDMHP.Combat.Aiming;


namespace TDMHP.Combat
{
    public sealed class PlayerActionController : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private InputReader _input;
        [SerializeField] private IntentBuffer _buffer;
        [SerializeField] private PlayerMotor _motor;
        [SerializeField] private MeleeHitDetector _hitDetector;
        [SerializeField] private ResourceContainer _resources;
        [SerializeField] private AimProvider _aim;
        

        

        [Header("Weapon")]
        [SerializeField] private WeaponData _startingWeapon;

        [Header("Dodge")]
        [SerializeField] private DodgeData _dodgeData;

        [SerializeField] private Invulnerability _invulnerability;
        


        // Optional: global action costs (not tied to a move)
        [Header("optional Global Action Costs")]
        [SerializeField] private ResourceCost[] _dodgeCosts;
        

        private PlayerAction _current;

        public InputReader Input => _input;
        public IntentBuffer Buffer => _buffer;
        public PlayerMotor Motor => _motor;
        public MeleeHitDetector HitDetector => _hitDetector;
        public ResourceContainer Resources => _resources;
        public AimProvider Aim => _aim;


        public WeaponData Weapon { get; private set; }

        public event Action<string> OnActionRejected; // e.g., "Not enough stamina"
        public ResourceCost[] DodgeCosts => _dodgeCosts;

        public DodgeData DodgeData => _dodgeData;
        public Invulnerability Invulnerability => _invulnerability;

        private void Reset()
        {
            _input = GetComponent<InputReader>();
            _buffer = GetComponent<IntentBuffer>();
            _motor = GetComponent<PlayerMotor>();
            _hitDetector = GetComponent<MeleeHitDetector>();
            _resources = GetComponent<ResourceContainer>();
            _invulnerability = GetComponent<Invulnerability>();
            _aim = GetComponent<AimProvider>();
        }

        private void Awake()
        {
            if (_input == null) _input = GetComponent<InputReader>();
            if (_buffer == null) _buffer = GetComponent<IntentBuffer>();
            if (_motor == null) _motor = GetComponent<PlayerMotor>();
            if (_hitDetector == null) _hitDetector = GetComponent<MeleeHitDetector>();
            if (_resources == null) _resources = GetComponent<ResourceContainer>();
            if (_invulnerability == null) _invulnerability = GetComponent<Invulnerability>();
            if (_aim == null) _aim = GetComponent<AimProvider>();
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

        public void Reject(string reason)
        {
            Debug.Log($"[ActionRejected] {reason}", this);
            OnActionRejected?.Invoke(reason);
        }
    }
}
