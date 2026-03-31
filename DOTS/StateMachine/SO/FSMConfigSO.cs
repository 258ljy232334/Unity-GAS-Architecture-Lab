using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FSM.SO
{
    [CreateAssetMenu(fileName = "FSMConfigSO", 
        menuName = "SO/FSM/State")]
    public class FSMConfigSO : ScriptableObject
    {
        public List<StateNode> States;
    }
}
