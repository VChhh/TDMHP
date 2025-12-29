using UnityEngine;

namespace TDMHP.AI.Motor
{
    public sealed class EnemyMotorStub : MonoBehaviour
    {
        public void Stop()
        {
            // no-op for stationary dummy
        }

        public void FaceTowards(Vector3 worldPos)
        {
            var dir = worldPos - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f) return;

            transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
        }
    }
}
