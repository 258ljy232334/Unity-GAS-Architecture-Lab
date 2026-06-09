using Zenject;
namespace ObjectPool
{
    public class ObjectPoolSubInstaller : Installer<ObjectPoolSubInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<Pool>().AsSingle();
        }
    }
}