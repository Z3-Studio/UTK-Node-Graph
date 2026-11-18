using System;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Animations)]
    [NodeDescription("Play an animator state by name in all layers")]
    public class PlayAnimationAllLayers : ActionTask
    {
        [ParameterDefinition(AutoBindType.AnyWithSameType)]
        [SerializeField] private Parameter<Animator> animator;
        [SerializeField] private Parameter<string> stateName;
        [SerializeField] private Parameter<float> transition = 0.1f;
        [SerializeField] private Parameter<EAnimationPlayEnd> waitUntil;
        [SerializeField] private Parameter<int> waitLayer;

        private readonly AnimationStateWatch watch = new();

        public override string Info
        {
            get
            {
                if (!waitUntil.IsBinding && waitUntil.Value == EAnimationPlayEnd.DontWait)
                    return $"► Play All {stateName}";

                return $"► Play All {stateName} until {waitUntil}{AnimationStateWatch.LayerLabel(waitLayer)}";
            }
        }

        protected override void StartAction()
        {
            Animator target = animator.Value;
            for (int i = 0; i < target.layerCount; i++)
            {
                target.PlayOrResetState(stateName.Value, transition.Value, i);
            }
            target.Update(0f);

            EAnimationPlayEnd end = waitUntil.Value;
            if (end == EAnimationPlayEnd.DontWait)
            {
                EndAction();
                return;
            }

            watch.Begin(target, stateName.Value, waitLayer.Value, ToCheck(end), false);
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