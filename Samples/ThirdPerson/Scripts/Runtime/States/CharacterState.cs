using UnityEngine;
using Z3.NodeGraph.Sample.ThirdPerson.Data;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.Sample.ThirdPerson.Character.States
{
    [NodeCategory(Categories.Samples + "/Third Person")]
    public abstract class CharacterCondition : ConditionTask<CharacterPawn>
    {
        protected CharacterPhysics Physics => Agent.Physics;
        protected PawnController Controller => Agent.Controller;
        protected CharacterData Data => Agent.Data;
    }

    [NodeCategory(Categories.Samples + "/Third Person")]
    public abstract class CharacterAction : ActionTask<CharacterPawn>
    {
        protected CharacterCamera Camera => Agent.Camera;
        protected CharacterPhysics Physics => Agent.Physics;
        protected Animator Animator => Agent.Animator;
        protected PawnController Controller => Agent.Controller;
        protected CharacterData Data => Agent.Data;
    }
}