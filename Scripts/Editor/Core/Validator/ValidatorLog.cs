using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Core;
using Z3.UIBuilder.ExtensionMethods;

namespace Z3.NodeGraph.Editor
{
    public class ValidatorLog : VisualElement
    {
        [UIElement] private Label contextLabel;
        [UIElement] private Button revealButton;
        [UIElement] private VisualElement errorsContainer;
        [UIElement] private Foldout foldout;

        private GraphDataAnalyzer logger;

        // Link error color #3D89D4

        public ValidatorLog(GraphDataAnalyzer logger)
        {
            NodeGraphResources.ValidatorLogVT.CloneTree(this);
            this.BindUIElements();

            contextLabel.text = logger.GraphData.name;
            this.logger = logger;

            revealButton.clicked += OnReveal;

            foldout.text = $"Errors: {logger.Errors.Count}";

            foreach (var error in logger.Errors)
            {
                Label newError = new Label(error.Value);
                newError.style.marginBottom = 8;
                newError.style.paddingLeft = 16;

                newError.RegisterCallback<MouseEnterEvent>(e =>
                {
                    newError.style.backgroundColor = new Color(0.172549f, 0.3647059f, 0.5294118f);
                });

                newError.RegisterCallback<MouseLeaveEvent>(e =>
                {
                    newError.style.backgroundColor = Color.clear;
                });

                errorsContainer.Add(newError);
            }
        }

        private void OnReveal()
        {
            EditorGUIUtility.PingObject(logger.GraphData);
        }
    }
}