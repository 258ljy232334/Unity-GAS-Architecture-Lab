using UnityEngine;

namespace Benchmark.Movement
{
    /// <summary>每物体独立 Update，体现 GameObject + 托管调度开销。</summary>
    public class MonoPerObjectMover : MonoBehaviour
    {
        public Vector3 Velocity;

        void Update()
        {
            transform.position += Velocity * Time.deltaTime;
        }
    }
}
