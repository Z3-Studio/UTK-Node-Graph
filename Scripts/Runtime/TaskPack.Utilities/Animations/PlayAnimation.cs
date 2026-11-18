using System;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    public enum EAnimationPlayEnd
    {
        DontWait = 0,
        EnterEnd = 1,
        ClipEnd = 2,
        ExitStart = 3,
        ExitEnd = 4,
    }

    [NodeCategory(Categories.Animations)]
    [NodeDescription("Play an animator state by name")]
    public class PlayAnimation : ActionTask
    {
        [ParameterDefinition(AutoBindType.AnyWithSameType)]
        [SerializeField] private Parameter<Animator> animator;
        [SerializeField] private Parameter<string> stateName;
        [SerializeField] private Parameter<int> layer;
        [SerializeField] private Parameter<float> transition = 0.1f;
        [SerializeField] private Parameter<EAnimationPlayEnd> waitUntil;

        private readonly AnimationStateWatch watch = new();

        public override string Info
        {
            get
            {
                string layerText = AnimationStateWatch.LayerLabel(layer);
                if (!waitUntil.IsBinding && waitUntil.Value == EAnimationPlayEnd.DontWait)
                    return $"► Play {stateName}{layerText}";

                return $"► Play {stateName} until {waitUntil}{layerText}";
            }
        }

        protected override void StartAction()
        {
            Animator target = animator.Value;
            target.PlayOrResetState(stateName.Value, transition.Value, layer.Value);
            target.Update(0f); // Applies the play now, so this tick already reads the new state

            EAnimationPlayEnd end = waitUntil.Value;
            if (end == EAnimationPlayEnd.DontWait)
            {
                EndAction();
                return;
            }

            watch.Begin(target, stateName.Value, layer.Value, ToCheck(end), false);
        }

        protected override void UpdateAction()
        {
            if (watch.IsMet())
            {
                EndAction();
            }
        }

        private static EAnimationCheck ToCheck(EAnimationPlayEnd end) => end switch
        {
            EAnimationPlayEnd.EnterEnd => EAnimationCheck.EnterEnd,
            EAnimationPlayEnd.ClipEnd => EAnimationCheck.ClipEnd,
            EAnimationPlayEnd.ExitStart => EAnimationCheck.ExitStart,
            EAnimationPlayEnd.ExitEnd => EAnimationCheck.ExitEnd,
            _ => throw new ArgumentOutOfRangeException(nameof(end), end, null),
        };
    }
}
