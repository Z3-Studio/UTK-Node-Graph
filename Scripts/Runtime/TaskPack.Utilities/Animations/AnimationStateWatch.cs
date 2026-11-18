using System;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    public enum EAnimationCheck
    {
        /// <summary> Current or next </summary>
        Playing = 0,

        /// <summary> Current, no transition </summary>
        PlayingNoTransition = 1,

        /// <summary> Enter transition ended </summary>
        EnterEnd = 2,

        /// <summary> Clip reached the end. Looping: first loop </summary>
        ClipEnd = 3,

        /// <summary> Exit transition started </summary>
        ExitStart = 4,

        /// <summary> Exit transition ended </summary>
        ExitEnd = 5,
    }

    /// <summary> Reads one state on one layer, as the Animator is now </summary>
    public sealed class AnimationStateWatch
    {
        private enum Phase 
        {
            None, 
            Entering, 
            Inside, 
            Exiting 
        }

        private Animator animator;
        private int stateHash;
        private int layer;
        private EAnimationCheck check;
        private bool requireEnter;
        private bool entered;

        public void Begin(Animator animator, string stateName, int layer, EAnimationCheck check, bool requireEnter)
        {
            this.animator = animator;
            stateHash = Animator.StringToHash(stateName);
            this.layer = layer;
            this.check = check;
            this.requireEnter = requireEnter;
            entered = false;
        }

        /// <summary> With requireEnter, false until the state is seen playing since Begin </summary>
        public bool IsMet()
        {
            Phase phase = Read(out float time);
            entered |= phase != Phase.None;

            if (requireEnter && !entered)
                return false;

            return check switch
            {
                EAnimationCheck.Playing => phase != Phase.None,
                EAnimationCheck.PlayingNoTransition => phase == Phase.Inside,
                EAnimationCheck.EnterEnd => phase != Phase.Entering,
                EAnimationCheck.ClipEnd => phase == Phase.None || time >= 1f,
                EAnimationCheck.ExitStart => phase is Phase.Exiting or Phase.None,
                EAnimationCheck.ExitEnd => phase == Phase.None,
                _ => throw new ArgumentOutOfRangeException(nameof(check), check, null),
            };
        }

        private Phase Read(out float time)
        {
            AnimatorStateInfo current = animator.GetCurrentAnimatorStateInfo(layer);
            bool isCurrent = Matches(current);

            if (animator.IsInTransition(layer))
            {
                AnimatorStateInfo next = animator.GetNextAnimatorStateInfo(layer);
                if (Matches(next)) // Also a transition to itself: the new instance
                {
                    time = next.normalizedTime;
                    return Phase.Entering;
                }

                time = current.normalizedTime;
                return isCurrent ? Phase.Exiting : Phase.None;
            }

            time = current.normalizedTime;
            return isCurrent ? Phase.Inside : Phase.None;
        }

        private bool Matches(AnimatorStateInfo info)
        {
            return info.shortNameHash == stateHash || info.fullPathHash == stateHash;
        }

        public static string LayerLabel(Parameter<int> layer)
        {
            return layer.IsBinding || layer.Value != 0 ? $" [L {layer.ToStringBold()}]" : string.Empty;
        }
    }
}
