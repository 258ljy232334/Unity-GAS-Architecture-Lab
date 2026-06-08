namespace Gameplay.Effect
{
    public enum StackType
    {
        Unique,     //唯一，不能叠层
        Refresh,    //刷新，新的效果会刷新旧的效果持续时间
        CanStack,    //能叠层，新的效果会叠加在旧的效果上
    }
}