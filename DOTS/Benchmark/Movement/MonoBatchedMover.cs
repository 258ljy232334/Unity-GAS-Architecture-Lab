using System.Diagnostics;
using UnityEngine;
using Unity.Profiling;

namespace Benchmark.Movement
{
    /// <summary>单脚本批量移动；用于与 DOTS 批处理强度更接近的对比。</summary>
    public class MonoBatchedMover : MonoBehaviour
    {
        static readonly ProfilerMarker s_LoopMarker = new ProfilerMarker("Benchmark.Mono.BatchedMoveLoop");

        public Transform[] Targets;
        public Vector3[] Velocities;

        /// <summary>上一帧纯循环耗时（秒），供 UI 显示。</summary>
        public double LastLoopSeconds { get; private set; }

        void Update()
        {
            if (Targets == null || Velocities == null || Targets.Length == 0)
                return;

            float dt = Time.deltaTime;
            var sw = Stopwatch.StartNew();
            using (s_LoopMarker.Auto())
            {
                for (int i = 0; i < Targets.Length; i++)
                {
                    var t = Targets[i];
                    if (t != null)
                        t.position += Velocities[i] * dt;
                }
            }

            sw.Stop();
            LastLoopSeconds = sw.Elapsed.TotalSeconds;
        }
    }
}
