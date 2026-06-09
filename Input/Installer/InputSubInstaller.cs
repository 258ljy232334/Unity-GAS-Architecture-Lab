using Gameplay;
using PlayerInput.Module;
using Zenject;

namespace PlayerInput.Installer
{
    public class InputSubInstaller : Installer<InputSubInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<InputModule>().AsSingle();
            Container.BindInterfacesAndSelfTo<SkillComponent>().FromComponentInHierarchy().AsSingle();
        }
    }
}