using Quest.Blackboard;
using Quest.Interface;
using Quest.Manager;
using Quest.Signal;
using Zenject;
namespace Quest.Installer
{
    public class QuestSubInstaller : Installer<QuestSubInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<QuestManager>().AsSingle();
            Container.Bind<IQuestBlackboard>().To<QuestBlackboard>().AsSingle();

            Container.DeclareSignal<OnBlackboardValueChangedSignal>().OptionalSubscriber();
            Container.DeclareSignal<OnTaskFinishedSignal>().OptionalSubscriber();
        }
    }
}