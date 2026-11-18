using System;
using System.Collections.Generic;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Animations)]
    [NodeDescription("Play a random animator state from a list")]
    public class PlayRandomAnimation : ActionTask
    {
        [ParameterDefinition(AutoBindType.AnyWithSameType)]
        [SerializeField] private Parameter<Animator> animator;
        [SerializeField] private Parameter<List<string>> stateNames;
        [SerializeField] private Parameter<int> layer;
        [SerializeField] private Parameter<float> transition = 0.1f;
        [SerializeField] private Parameter<EAnimationPlayEnd> waitUntil;

        private readonly AnimationStateWatch watch = new();

        public override string Info
        {
            get
            {
                string names = stateNames.IsBinding ? stateNames.ToString() : $"<b>Count = {stateNames.Value?.Count ?? 0}</b>";
                string layerText = AnimationStateWatch.LayerLabel(layer);
                if (!waitUntil.IsBinding && waitUntil.Value == EAnimationPlayEnd.DontWait)
                    return $"► Play Random {names}{layerText}";

                return $"► Play Random {names} until {waitUntil}{layerText}";
            }
        }

        protected override void StartAction()
        {
            List<string> names = stateNames.Value;
            string stateName = names[UnityEngine.Random.Range(0, names.Count)];

            Animator target = animator.Value;
            target.PlayOrResetState(stateName, transition.Value, layer.Value);
            target.Update(0f);

            EAnimationPlayEnd end = waitUntil.Value;
            if (end == EAnimationPlayEnd.DontWait)
            {
                EndAction();
                return;
            }

            watch.Begin(target, stateName, layer.Value, ToCheck(end), false);
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