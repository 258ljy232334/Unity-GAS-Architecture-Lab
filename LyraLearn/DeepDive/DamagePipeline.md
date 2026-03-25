# Lyra 伤害管线 (Damage Pipeline) 技术文档

> **声明**：本方案参考 Epic Games 官方项目 `Lyra Starter Game` 源码实现，遵循 GAS (Gameplay Ability System) 工业化解耦标准。

## 1. 核心流程概述
Lyra 的伤害管线遵循 “计算与数值分离、数值与表现分离” 的工业级设计原则，主要分为以下四个阶段：

1. **触发阶段** (Initiation)：通过 GameplayAbility (GA) 应用 GameplayEffect (GE)。

2. **计算阶段** (Calculation)：在 GE 的 Execution 中配置 ULyraDamageExecution，在 C++ 中计算最终伤害。

3. **结算阶段** (Settlement)：ULyraHealthSet 在 PostGameplayEffectExecute 中处理属性扣除与数值钳制。

4. **分发阶段** (Dispatch)：通过 GameplayMessageSubsystem 广播消息，实现表现层（UI/特效）解耦。

## 2. 关键代码实现
A. 逻辑计算层：Damage Execution
负责捕获属性（攻击、防御）并结合上下文（距离、部位）算出最终 Magnitude。

```
/**
 * ULyraDamageExecution: 执行复杂的伤害逻辑计算
 */
void ULyraDamageExecution::Execute_Implementation(const FGameplayEffectCustomExecutionParameters& ExecutionParams, FGameplayEffectCustomExecutionOutput& OutExecutionOutput) const
{
#if WITH_SERVER_CODE
    const FGameplayEffectSpec& Spec = ExecutionParams.GetOwningSpec();
    
    // 1. 提取 EffectContext (包含 HitResult, 物理材质, 攻击来源等)
    FLyraGameplayEffectContext* TypedContext = FLyraGameplayEffectContext::ExtractEffectContext(Spec.GetContext());
    check(TypedContext);

    // 2. 属性捕获 (Attribute Capture)
    float BaseDamage = 0.0f;
    ExecutionParams.AttemptCalculateCapturedAttributeMagnitude(DamageStatics().BaseDamageDef, FAggregatorEvaluateParameters(), BaseDamage);

    // 3. 团队过滤与逻辑审计 (Team Subsystem)
    float DamageMultiplier = 0.0f;
    if (AActor* TargetActor = ExecutionParams.GetTargetAbilitySystemComponent()->GetAvatarActor())
    {
        if (ULyraTeamSubsystem* TeamSubsystem = TargetActor->GetWorld()->GetSubsystem<ULyraTeamSubsystem>())
        {
            // 验证是否允许伤害（如队友免伤）
            DamageMultiplier = TeamSubsystem->CanCauseDamage(TypedContext->GetEffectCauser(), TargetActor) ? 1.0f : 0.0f;
        }
    }

    // 4. 距离衰减计算 (Distance Attenuation)
    float DistanceAttenuation = 1.0f;
    if (const ILyraAbilitySourceInterface* AbilitySource = TypedContext->GetAbilitySource())
    {
        // 从 Context 获取物理位置并调用接口获取衰减值
        double Distance = FVector::Dist(TypedContext->GetOrigin(), TypedContext->GetHitResult()->ImpactPoint);
        DistanceAttenuation = AbilitySource->GetDistanceAttenuation(Distance, nullptr, nullptr);
    }

    // 5. 输出计算结果到 Meta Attribute: Damage
    const float FinalDamage = FMath::Max(BaseDamage * DistanceAttenuation * DamageMultiplier, 0.0f);
    if (FinalDamage > 0.0f)
    {
        // 核心：将计算结果以 Additive 方式传给 HealthSet 的 Damage 属性
        OutExecutionOutput.AddOutputModifier(FGameplayModifierEvaluatedData(ULyraHealthSet::GetDamageAttribute(), EGameplayModOp::Additive, FinalDamage));
    }
#endif
}
```
B. 数值落地层：Attribute Set
负责记账，处理 GodMode、数值钳制并分发消息。

```
/**
 * ULyraHealthSet: 属性变动后的回调处理
 */
void ULyraHealthSet::PostGameplayEffectExecute(const FGameplayEffectModCallbackData& Data)
{
    Super::PostGameplayEffectExecute(Data);

    // 1. 处理伤害元属性 (Meta Attribute)
    if (Data.EvaluatedData.Attribute == GetDamageAttribute())
    {
        const float LocalDamageDone = GetDamage();
        SetDamage(0.0f); // 重置临时伤害属性

        if (LocalDamageDone > 0.0f)
        {
            // 2. 表现解耦：通过消息总线广播伤害 Verb Message
            FLyraVerbMessage Message;
            Message.Verb = TAG_Lyra_Damage_Message; // 标签定义：Ability.Verb.Damage
            Message.Instigator = Data.EffectSpec.GetEffectContext().GetEffectCauser();
            Message.Target = GetOwningActor();
            Message.Magnitude = LocalDamageDone;

            UGameplayMessageSubsystem::Get(GetWorld()).BroadcastMessage(Message.Verb, Message);

            // 3. 应用扣血逻辑并进行 Clamp (数值钳制)
            const float NewHealth = FMath::Clamp(GetHealth() - LocalDamageDone, 0.0f, GetMaxHealth());
            SetHealth(NewHealth);
        }
    }
    
    // 4. 判定死亡状态并触发回调
    if (GetHealth() <= 0.0f && !bOutOfHealth)
    {
        bOutOfHealth = true;
        OnOutOfHealth.Broadcast(Data.EffectSpec.GetEffectContext().GetOriginalInstigator(), Data.EffectSpec.GetEffectContext().GetEffectCauser(), &Data.EffectSpec, Data.EvaluatedData.Magnitude, 0.0f, 0.0f);
    }
}
```
## 3. 设计要点 (Architectural Highlights)
* **Meta Attribute (元属性)**：伤害不直接操作 Health，而是通过 Damage 属性中转。这为 PostExecute 阶段提供了统一的数值审计点（如：GodMode 检查、护盾抵扣、受击逻辑触发）。

* **Decoupling (表现解耦)**：AttributeSet 只负责修改数值和发出“信号”。UI 模块通过订阅消息总线来决定是否弹出伤害数字，实现了逻辑与表现的彻底分离。

* **Interface Driven (多态计算)**：通过 ILyraAbilitySourceInterface 抹平了不同武器（狙击枪、步枪、技能）的计算差异，使得 Execution 逻辑通用化。
