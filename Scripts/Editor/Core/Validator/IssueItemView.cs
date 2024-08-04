using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Core;

namespace Z3.NodeGraph.Editor
{
    public class IssueItemView : Label, IBindElement<IssueDetail>
    {
        public void Bind(IssueDetail issue, int index)
        {
            style.paddingLeft = 8;
            style.paddingTop = 2;

            text = $"{index + 1}. {issue}";
        }
    }
}