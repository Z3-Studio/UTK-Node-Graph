using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Core;
using Z3.UIBuilder.Editor;
using Z3.UIBuilder.ExtensionMethods;

namespace Z3.NodeGraph.Editor
{
    public class WelcomeWindow : Z3EditorWindow
    {
        [UIElement] private Toggle dontDisplayAgain;

        private static readonly Vector2 windowSize = new Vector2(800f, 600f);

        private const string DocumentationUrl = "https://z3-studio.com/docs/node-graph";
        private const string WindowName = "Welcome";

        [MenuItem(GraphPath.MenuPath + WindowName)]
        public static void OpenWindow()
        {
            WelcomeWindow window = GetWindow<WelcomeWindow>(WindowName);
            window.position = new Rect(window.position.x, window.position.y, 1000f, 600f);
        }

        public static void Init()
        {
            if (!UserPreferences.OpenWelcome)
                return;

            float openWindowStartTime = Time.realtimeSinceStartup;
            EditorApplication.update += DelayToOpen;

            void DelayToOpen()
            {
                if (Time.realtimeSinceStartup - openWindowStartTime > 1f)
                {
                    EditorApplication.update -= DelayToOpen;
                    OpenWindow();
                }
            }
        }

        protected override void CreateGUI()
        {
            NodeGraphResources.WelcomeVT.CloneTree(rootVisualElement);
            rootVisualElement.BindUIElements(this);

            dontDisplayAgain.value = !UserPreferences.OpenWelcome;
            dontDisplayAgain.RegisterValueChangedCallback(e =>
            {
                UserPreferences.OpenWelcome = !e.newValue;
            });
        }

        [UIElement("close-button")]
        private void OnClose()
        {
            Close();
        }

        [UIElement("open-documentation-button")]
        private void OnOpenDocumentation()
        {
            Application.OpenURL(DocumentationUrl);
        }
    }
}