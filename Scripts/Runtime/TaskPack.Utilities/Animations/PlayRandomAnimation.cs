using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Animations)]
    [NodeDescription("Play animation by random state name")]
    public class PlayRandomAnimation : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Animator> animator;
        [SerializeField] private Parameter<bool> waitUntilFinish;
        [SerializeField] private Parameter<List<string>> stateNames;

        public override string Info
        {
            get
            {
                if (stateNames.Value is null)
                    return base.Info;

                string info = waitUntilFinish.Value ? 
                    $"► Playing Random " :
                    $"► Play Random ";

                if (stateNames.IsBinding)                
                    info += stateNames;                
                else                
                    info += $"<b>Count = {stateNames.Value.Count}</b>";
                
                return info;
            }
        }

        private AnimatorStateInfo stateInfo;
        private bool played;
        private string selectedStateName;

        protected override void StartAction()
        {
            played = false; 
            
            int index = Random.Range(0, stateNames.Value.Count);
            selectedStateName = stateNames.Value[index];
            animator.Value.Play(selectedStateName);

            if (!waitUntilFinish.Value)
            {
                EndAction();
            }
        }

        protected override void UpdateAction()
        {
            stateInfo = animator.Value.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName(selectedStateName))
            {
                played = true;
                if (NodeRunningTime >= (stateInfo.length / animator.Value.speed))
                {
                    EndAction();
                }
            }
            else if (played)
            {
                EndAction();
            }
        }
    }
}