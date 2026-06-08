using UnityEngine;

namespace Gameplay.Ability
{
    [CreateAssetMenu(fileName ="JumpFunc",
        menuName ="SO/Gameplay/AbilityFunc/Jump")]
    public class JumpFunc : AbilityExtraFuncSO
    {
        [SerializeField]
        private float _jumpPower;
        [SerializeField]
        private Vector3 _jumpDirection;
        public override void ExecuteFunc(AbilityInstance instance)
        {
            var player = instance.ActorInfo.Owner;
            player.GetComponent<Rigidbody>().AddForce(_jumpDirection.normalized* _jumpPower, ForceMode.Impulse);
        }
    }
}