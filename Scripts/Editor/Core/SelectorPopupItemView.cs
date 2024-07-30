using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using Z3.UIBuilder.Editor;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.Editor
{
    public class SelectorPopupItemView<T> : VisualElement
    {
        public event Action<string, T> OnSelectItem;
        public event Action<List<(string, T)>> OnOpenNextPath;

        private List<(string, T)> SubItems { get; }

        private Label toolbarButton;
        private VisualElement icon;

        public string Key { get; }
        public T Value { get; }

        private Clickable clickable;

        private readonly Color hoverColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        private readonly Color defaultColor = new Color(1f, 1f, 1f, 0f);

        private SelectorPopupItemView(string name)
        {
            style.flexDirection = FlexDirection.Row;

            icon = new();
            icon.style.minWidth = 16;
            icon.style.minHeight = 16;
            icon.style.SetPadding(4);
            icon.style.marginLeft = 4;
            icon.style.marginRight = 4;
            icon.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);

            toolbarButton = new();
            toolbarButton.style.fontSize = 14;
            toolbarButton.style.SetPadding(4);
            toolbarButton.style.width = new Length(100, LengthUnit.Percent);
            toolbarButton.text = name;

            Add(icon);
            Add(toolbarButton);

            RegisterCallback<MouseEnterEvent>(OnMouseEnter);
            RegisterCallback<MouseLeaveEvent>(OnMouseLeave);

        }

        /// <summary> Final Item </summary>
        public SelectorPopupItemView(string name, T value1) : this(name)
        {
            clickable = new Clickable(OnSelect);
            this.AddManipulator(clickable);

            Key = name;
            Value = value1;

            icon.style.backgroundImage = EditorIcons.GetTypeIcon(value1 as Type);
        }

        /// <summary> Next Path </summary>
        public SelectorPopupItemView(string part1, string part2, T value) : this(part1)
        {
            clickable = new Clickable(OnNextPath);
            this.AddManipulator(clickable);

            SubItems = new();
            AddSubItem(part2, value);

            Add(new Label(">"));
        }

        public void AddSubItem(string part2, T value)
        {
            SubItems.Add((part2, value));
        }

        private void OnSelect() => OnSelectItem.Invoke(Key, Value);

        private void OnNextPath() => OnOpenNextPath.Invoke(SubItems);

        private void OnMouseEnter(MouseEnterEvent evt) => style.backgroundColor = hoverColor;

        private void OnMouseLeave(MouseLeaveEvent evt) => style.backgroundColor = defaultColor;
    }
}