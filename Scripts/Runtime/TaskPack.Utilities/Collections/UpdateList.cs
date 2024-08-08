using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [System.Flags]
    public enum ItemListState
    {
        Null = 1,
        Disabled = 2,
        Enabled = 4
    }

    [NodeCategory(Categories.Collections)]
    [NodeDescription("Remove GameObjects disabled, null or enabled. Useful to remove object references that have returned to the object pool for example.")]
    public class UpdateList<T> : ActionTask where T : Component
    {
        [SerializeField] private Parameter<List<T>> list;
        [SerializeField] private Parameter<ItemListState> itemState = ItemListState.Disabled | ItemListState.Null;

        protected override void StartAction()
        {
            if (itemState.Value == ItemListState.Disabled)
            {
                list.Value.RemoveAll(i => !i.gameObject.activeSelf);
            }
            if (itemState.Value == ItemListState.Enabled)
            {
                list.Value.RemoveAll(i => i.gameObject.activeSelf);
            }
            if (itemState.Value == ItemListState.Null)
            {
                list.Value.RemoveAll(i => i == null);
            }

            EndAction();
        }
    }
}