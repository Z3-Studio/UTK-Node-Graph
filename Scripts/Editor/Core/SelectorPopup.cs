using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Linq;
using UnityEditor.UIElements;
using Z3.UIBuilder.ExtensionMethods;
using Z3.UIBuilder.Editor;
using Z3.UIBuilder.Core;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.Editor
{
    public class SelectorPopup<T> : UIPopup
    {
        [UIElement("title")] private Label titleLabel;
        [UIElement] private ToolbarSearchField searchField;
        [UIElement] private VisualElement buttonsContainer;
        [UIElement] private ToolbarButton backButton;

        protected List<(string key, T value)> Items { get; set; } = new();

        private readonly List<string> pathHistory = new();
        private readonly Action<string, T> onSelectType;
        private readonly string title;
        private readonly bool autoOrder;

        protected SelectorPopup(Action<string, T> onSelectType, string title, bool autoOrder = true)
        {
            this.onSelectType = onSelectType;
            this.title = title;
            this.autoOrder = autoOrder;
        }

        protected SelectorPopup(Action<string, T> onSelectType, string title, List<(string, T)> source, bool autoOrder = true) : this(onSelectType, title, autoOrder)
        {
            Items = source;
        }

        public static void OpenWindow(Action<string, T> onSelectType)
        {
            SelectorPopup<T> instance = new(onSelectType, string.Empty);
            instance.OpenGenericPopup();
        }

        public static void OpenWindow(string title, List<(string, T)> source, Action<string, T> onSelectType, Vector2 windowPosition)
        {
            SelectorPopup<T> instance = new(onSelectType, title, source);
            instance.OpenGenericPopup(windowPosition);
        }

        public static void OpenWindow(string title, List<(string, T)> source, Action<string, T> onSelectType, bool autoOrder = true)
        {
            SelectorPopup<T> instance = new(onSelectType, title, source, autoOrder);
            instance.OpenGenericPopup();
        }

        public override void OnOpen()
        {
            VisualElement root = editorWindow.rootVisualElement;
            NodeGraphResources.PopupVT.CloneTree(root);
            root.BindUIElements(this);

            // Set window size
            VisualElement first = root.Children().First();
            Size = new Vector2(first.style.width.value.value, first.style.height.value.value);
            Size = new Vector2(400, 400); // The style is not working

            backButton.clicked += GoBack;
            backButton.visible = false;

            // Setup the window
            titleLabel.text = title;

            if (autoOrder)
            {
                Items = Items.OrderBy(i => i.key).ToList();
            }

            CreateButtons(Items);

            searchField.RegisterCallback<ChangeEvent<string>>(OnSearchFieldChanged);
        }

        protected virtual void CreateButtons(List<(string, T)> items)
        {
            buttonsContainer.Clear();

            Dictionary<string, SelectorPopupItemView<T>>  paths = new();


            foreach ((string key, T value) in items)
            {
                int index = key.IndexOf("/");
                if (index == -1)
                {
                    SelectorPopupItemView<T> button = new SelectorPopupItemView<T>(key, value);
                    button.OnSelectItem += OnSelect;
                    buttonsContainer.Add(button);
                    continue;
                }

                string part1 = key.Substring(0, index); // Before the first '/'
                string part2 = key.Substring(index + 1); // After the first '/'

                if (!paths.ContainsKey(part1))
                {
                    SelectorPopupItemView<T> button = new SelectorPopupItemView<T>(part1, part2, value);
                    button.OnOpenNextPath += (subItems) =>
                    {
                        backButton.visible = true;
                        pathHistory.Add(part1);
                        CreateButtons(subItems);
                    };
                    buttonsContainer.Add(button);

                    paths.Add(part1, button);
                }
                else
                {
                    paths[part1].AddSubItem(part2, value);
                }
            }
        }

        private void GoBack()
        {
            int count = pathHistory.Count;

            if (count > 1)
            {
                CreateButtons(GetItemsForPath());
            }
            else
            {
                backButton.visible = false;
                CreateButtons(Items);
            }

            pathHistory.RemoveAt(count - 1);
        }

        private List<(string, T)> GetItemsForPath()
        {
            string path = string.Join("/", pathHistory);

            // Get the last segment of the path
            string lastSegment = pathHistory.LastOrDefault();

            // Get all segments before the last one
            int lastSeparatorIndex = path.LastIndexOf("/") + 1;
            string prevSegments = path.Substring(0, lastSeparatorIndex);

            // Filter items that start with the previous segments and update the string 
            return Items.Where(i => i.key.StartsWith(prevSegments))
                        .Select(i => (i.key.Remove(0, lastSeparatorIndex), i.value))
                        .ToList();
        }

        protected virtual void OnSelect(string key, T value)
        {
            onSelectType.Invoke(key, value);
            editorWindow.Close();
        }


        private void OnSearchFieldChanged(ChangeEvent<string> evt)
        {
            // Get the text typed
            string searchText = evt.newValue;

            if (string.IsNullOrEmpty(searchText))
            {
                CreateButtons(Items);
                return;
            }

            pathHistory.Clear();
            backButton.visible = false;
            buttonsContainer.Clear();

            // Update the exibition list to only show content matching the search
            foreach ((string key, T value) in Items)
            {
                if (key.SearchMatch(searchText) || (value != null && value.ToString().SearchMatch(searchText)))
                {
                    string[] elements = key.Split('/');
                    string finalName = elements.Last();

                    SelectorPopupItemView<T> button = new SelectorPopupItemView<T>(finalName, value);
                    button.OnSelectItem += OnSelect;
                    buttonsContainer.Add(button);
                }
            }
        }
    }
}