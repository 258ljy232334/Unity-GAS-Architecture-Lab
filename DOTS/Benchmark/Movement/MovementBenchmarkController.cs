using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Benchmark.Movement
{
    /// <summary>
    /// 在运行模式下生成指定数量的“仅位移”物体，切换 DOTS / Mono 两种路径。
    /// CPU：MonoBatched 会在屏幕左上角显示上一帧 for 循环耗时；DOTS 请在 Profiler 中查看 SimpleMoveJob 与 Worker Thread。
    /// </summary>
    [DefaultExecutionOrder(-500)]
    public class MovementBenchmarkController : MonoBehaviour
    {
        [Header("规模")]
        [Min(1)]
        [SerializeField]
        int objectCount = 10000;

        [SerializeField]
        float spawnRadius = 50f;

        [SerializeField]
        float moveSpeed = 3f;

        [Header("模式")]
        [SerializeField]
        MovementBenchmarkMode mode = MovementBenchmarkMode.Dots;

        [Tooltip("进入 Play 后自动按当前模式生成")]
        [SerializeField]
        bool applyOnPlay = true;

        Transform _monoRoot;
        MonoBatchedMover _batchedMover;

        void Awake()
        {
            EnsureMonoInfrastructure();
        }

        void Start()
        {
            if (applyOnPlay)
                ApplyCurrentMode();
        }

        void OnDestroy()
        {
            ClearAll();
        }

        void EnsureMonoInfrastructure()
        {
            if (_monoRoot != null)
                return;

            var rootGo = new GameObject("MovementBenchmark_MonoRoot");
            rootGo.transform.SetParent(transform, false);
            _monoRoot = rootGo.transform;

            _batchedMover = rootGo.AddComponent<MonoBatchedMover>();
        }

        [ContextMenu("Apply Current Mode")]
        public void ApplyCurrentMode()
        {
            ClearAll();
            switch (mode)
            {
                case MovementBenchmarkMode.None:
                    break;
                case MovementBenchmarkMode.Dots:
                    SpawnDots();
                    break;
                case MovementBenchmarkMode.MonoBatched:
                    SpawnMonoBatched();
                    break;
                case MovementBenchmarkMode.MonoPerObject:
                    SpawnMonoPerObject();
                    break;
            }
        }

        [ContextMenu("Clear")]
        public void ClearAll()
        {
            ClearDots();
            ClearMono();
        }

        void ClearDots()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
                return;

            var em = world.EntityManager;
            using var query = em.CreateEntityQuery(ComponentType.ReadOnly<MovementBenchmarkTag>());
            if (!query.IsEmpty)
                em.DestroyEntity(query);
        }

        void ClearMono()
        {
            EnsureMonoInfrastructure();
            for (int i = _monoRoot.childCount - 1; i >= 0; i--)
                Destroy(_monoRoot.GetChild(i).gameObject);

            _batchedMover.Targets = null;
            _batchedMover.Velocities = null;
        }

        void SpawnDots()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
            {
                Debug.LogError("DefaultGameObjectInjectionWorld 未就绪，无法生成 ECS 实体。");
                return;
            }

            var em = world.EntityManager;
            var archetype = em.CreateArchetype(
                typeof(LocalTransform),
                typeof(LocalToWorld),
                typeof(SimpleMoveVelocity),
                typeof(MovementBenchmarkTag));

            using var entities = new NativeArray<Entity>(objectCount, Allocator.Temp);
            em.CreateEntity(archetype, entities);

            for (int i = 0; i < objectCount; i++)
            {
                float t = i * 0.137f;
                float3 pos = new float3(math.cos(t), 0f, math.sin(t)) * spawnRadius;
                float3 dir = new float3(math.sin(t * 1.7f), 0f, math.cos(t * 1.7f));
                dir = math.normalizesafe(dir) * moveSpeed;

                em.SetComponentData(entities[i], LocalTransform.FromPositionRotationScale(pos, quaternion.identity, 1f));
                em.SetComponentData(entities[i], new SimpleMoveVelocity { Value = dir });
            }
        }

        void SpawnMonoBatched()
        {
            EnsureMonoInfrastructure();
            var targets = new Transform[objectCount];
            var vels = new Vector3[objectCount];

            for (int i = 0; i < objectCount; i++)
            {
                float tf = i * 0.137f;
                var pos = new Vector3(math.cos(tf), 0f, math.sin(tf)) * spawnRadius;
                var dir = new Vector3(math.sin(tf * 1.7f), 0f, math.cos(tf * 1.7f));
                if (dir.sqrMagnitude > 1e-6f)
                    dir = dir.normalized * moveSpeed;
                else
                    dir = Vector3.forward * moveSpeed;

                var go = new GameObject($"Mover_{i}");
                go.transform.SetParent(_monoRoot, false);
                go.transform.position = pos;
                targets[i] = go.transform;
                vels[i] = dir;
            }

            _batchedMover.Targets = targets;
            _batchedMover.Velocities = vels;
        }

        void SpawnMonoPerObject()
        {
            EnsureMonoInfrastructure();
            for (int i = 0; i < objectCount; i++)
            {
                float tf = i * 0.137f;
                var pos = new Vector3(math.cos(tf), 0f, math.sin(tf)) * spawnRadius;
                var dir = new Vector3(math.sin(tf * 1.7f), 0f, math.cos(tf * 1.7f));
                if (dir.sqrMagnitude > 1e-6f)
                    dir = dir.normalized * moveSpeed;
                else
                    dir = Vector3.forward * moveSpeed;

                var go = new GameObject($"Mover_{i}");
                go.transform.SetParent(_monoRoot, false);
                go.transform.position = pos;
                var mover = go.AddComponent<MonoPerObjectMover>();
                mover.Velocity = dir;
            }

            _batchedMover.Targets = null;
            _batchedMover.Velocities = null;
        }

        void OnGUI()
        {
            if (!Application.isPlaying)
                return;

            const int w = 420;
            GUILayout.BeginArea(new Rect(10, 10, w, 220), GUI.skin.box);
            GUILayout.Label($"Movement Benchmark | Mode: {mode} | Count: {objectCount}");
            GUILayout.Label($"FPS: {1f / Mathf.Max(Time.unscaledDeltaTime, 1e-6f):0.0}");

            if (mode == MovementBenchmarkMode.MonoBatched && _batchedMover != null)
                GUILayout.Label($"Mono batched loop (last frame): {_batchedMover.LastLoopSeconds * 1000.0:0.###} ms");

            if (mode == MovementBenchmarkMode.Dots)
            {
                GUILayout.Label("DOTS: 打开 Window > Analysis > Profiler，在 Timeline 搜索 SimpleMoveJob");
                GUILayout.Label("或查看 Worker Thread 上 Burst 任务耗时。");
            }

            if (mode == MovementBenchmarkMode.MonoPerObject)
            {
                GUILayout.Label("Mono/每物体 Update: 主线程 Behaviour.Update 总耗");
                GUILayout.Label("请在 Profiler 对比 Scripts / BehaviourUpdate。");
            }

            GUILayout.EndArea();
        }
    }
}
