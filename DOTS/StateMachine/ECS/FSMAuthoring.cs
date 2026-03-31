using FSM.Blob;
using FSM.Enum;
using FSM.FuncLibrary;
using FSM.SO;
using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
namespace FSM.ECS
{
    public class FSMAuthoring : MonoBehaviour
    {
        public FSMConfigSO Config;
        private class Baker : Baker<FSMAuthoring>
        {
            public override void Bake(FSMAuthoring authoring)
            {
                if(authoring.Config==null)
                {
                    throw new ArgumentNullException(nameof(authoring.Config), "FSMConfigSO cannot be null");
                }
                var entity = GetEntity(TransformUsageFlags.None);
                using var builder=new BlobBuilder(Allocator.Temp);
                ref var root = ref builder.ConstructRoot<FSMRuntimeBlob>();
                var stateBuilder=builder.Allocate(ref root.AllStates, authoring.Config.States.Count);
                for (int i = 0; i < authoring.Config.States.Count; i++)
                {
                    var src = authoring.Config.States[i];

                    stateBuilder[i].StateID = src.StateID;
                    stateBuilder[i].ExecuteFunc = src.Behavior switch
                    {
                        Enum.FSMBehaviorType.Idle => BurstCompiler.CompileFunctionPointer<FSMActionDelegate>(FSMBehaviors.DoIdle),
                        Enum.FSMBehaviorType.Move => BurstCompiler.CompileFunctionPointer<FSMActionDelegate>(FSMBehaviors.DoMove),
                        _ => throw new ArgumentOutOfRangeException($"Unsupported FSMBehaviorType: {src.Behavior}")
                    };
                    var transBuilder = builder.Allocate(ref stateBuilder[i].Translations, src.Translations.Count);
                    var idToIndexMap = new Dictionary<int, int>();
                    for(int j = 0; j < authoring.Config.States.Count; j++)
                    {
                        idToIndexMap[authoring.Config.States[j].StateID] = j;
                    }
                    for (int j = 0; j < src.Translations.Count; j++)
                    {
                        var transSrc=src.Translations[j];
                        transBuilder[j]=new StateTranslationBlob
                        {
                            TargetStateId= idToIndexMap[transSrc.TargetStateID],
                            ConditionFunc=transSrc.Condition switch
                            {
                                ConditionType.CheckInput=> BurstCompiler.CompileFunctionPointer<TransitionConditionDelegate>(FSMConditions.CheckInput),
                                ConditionType.CheckDistance=> BurstCompiler.CompileFunctionPointer<TransitionConditionDelegate>(FSMConditions.CheckDistance),
                                _=>throw new ArgumentOutOfRangeException($"Unsupported ConditionType: {transSrc.Condition}")
                            }
                        };
                    }
                }
                var blobRef=builder.CreateBlobAssetReference<FSMRuntimeBlob>(Allocator.Persistent);
                AddBlobAsset(ref blobRef, out _);
                AddComponent(entity, new FSMComponent { BlobRef = blobRef, CurrentStateIndex = 0 });
            }
        }
    }
}