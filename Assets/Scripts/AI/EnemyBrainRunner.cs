using UnityEngine;

namespace TDMHP.AI
{
    public sealed class EnemyBrainRunner : MonoBehaviour
    {
        [SerializeField] private EnemyBrainAsset _brainAsset;

        private EnemyContext _ctx;
        private IEnemyBrain _brain;

        private void Awake()
        {
            var actor = GetComponent<EnemyActor>();
            _ctx = actor.BuildContext();
        }

        private void OnEnable()
        {
            if (_brainAsset == null) return;
            _brain = _brainAsset.CreateBrain();
            _brain.Initialize(_ctx);

            // wire combat events
            var events = GetComponent<EnemyEvents>();
            if (events != null) events.EventRaised += OnEnemyEvent;
        }

        private void OnDisable()
        {
            var events = GetComponent<EnemyEvents>();
            if (events != null) events.EventRaised -= OnEnemyEvent;

            _brain?.Shutdown();
            _brain = null;
        }

        private void Update()
        {
            _brain?.Tick(Time.deltaTime);
        }

        private void OnEnemyEvent(EnemyEvent e) => _brain?.OnEvent(in e);
    }
}
