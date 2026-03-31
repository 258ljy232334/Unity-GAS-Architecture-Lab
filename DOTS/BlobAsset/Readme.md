# 🛠️ Unity DOTS: 静态数据脱水架构 (BlobAsset)
## 📖 简介
本项目展示了如何在 Unity Entities (DOTS) 中使用 BlobAsset 构建一个高性能、内存紧凑且线程安全的配置系统。通过将传统的 ScriptableObject 数据“脱水”为连续的二进制内存块，实现了逻辑与配置的彻底解耦。

## 🌟 核心特性
* **极速寻址**：数据以连续内存布局存储，极大提高了 CPU 缓存命中率 (Cache Locality)。

* **逻辑数据化** (FunctionPointer)：利用 BurstCompiler 将静态函数指针直接存储在 BlobAsset 中，支持在 Burst 编译的 Job 内调用动态逻辑，而无需传统的虚函数或接口。

* **嵌套结构支持**：支持 BlobArray 嵌套扁平结构体 (SubDataBlob)，满足复杂技能配置或 AI 状态机的需求。

* **零 GC 开销**：数据在 Baker 阶段一次性生成，运行时为只读引用，完全避开了托管堆的垃圾回收。

## 🏗️ 架构组成
1. **数据定义** (The Schema)
使用 BlobAsset 必须定义严格的内存布局。
```
public struct TestConfigBlob
{
    public int TestInt;
    public BlobString TestString;
    public BlobArray<float> FloatList;
    public FunctionPointer<TestDelegate> LogicFunc; // 关键：函数指针实现行为配置
    public BlobArray<SubDataBlob> SubDataList;    // 关键：嵌套数据支持
}
```
2. **烘焙系统** (The Baker)
负责将托管层 (ScriptableObject) 的数据转换为非托管的 BlobAssetReference。

* 使用 BlobBuilder 手动分配内存。

* 通过 BurstCompiler.CompileFunctionPointer 实现策略模式。

3. **运行系统** (The System)
在 ISystem 中通过单例模式高效访问配置。

```
// 运行示例：
var config = SystemAPI.GetSingleton<TestConfigComponent>();
ref var blob = ref config.BlobRef.Value;

// 调用存储在 Blob 中的函数指针
bool result = blob.LogicFunc.Invoke(current, target);
```
## 🚀 性能优势
1. **Memory Layout**: 传统的 List<T> 在内存中是离散的指针映射，而 BlobArray<T> 是物理连续的字节流。

2. **Burst Friendly**: 文档中展示的 TestFuncLibrary 使用了 [BurstCompile]，配合函数指针，使得配置驱动的逻辑依然能享受最高级别的机器码优化。

3. **Scalability**: 这套模式是构建大型战斗系统（如 GAS）的基础。你可以通过修改 SO 上的 IsEqual 开关，在不重启游戏的情况下，通过重新烘焙直接改变上万个实体的运行逻辑。
