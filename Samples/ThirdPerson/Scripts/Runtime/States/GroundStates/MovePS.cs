﻿using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.Sample.ThirdPerson.Character.States
{
    public class MovePS : CharacterAction 
    {
        public Parameter<float> moveSpeed;

        protected override void UpdateAction()
        {
            Physics.Move(moveSpeed.Value);
        }
    }
}