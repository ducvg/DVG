#if !ODIN_INSPECTOR
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Linq;
using System.Collections.Generic;

namespace DVG.UI.Editor
{
    [CustomPropertyDrawer(typeof(ITransition), true)]
    public sealed class ITransitionDrawer : PropertyDrawer
    {
        static readonly Type[] _types;
        static readonly string[] _options;
        static readonly Dictionary<Type, int> _typeToIndex;

        static ITransitionDrawer()
        {
            _types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch { return Type.EmptyTypes; }
                })
                .Where(t =>
                    typeof(ITransition).IsAssignableFrom(t) &&
                    !t.IsAbstract &&
                    !t.IsInterface &&
                    !t.IsGenericType)
                .ToArray();

            _options = new string[_types.Length + 1];
            _options[0] = "None";

            _typeToIndex = new Dictionary<Type, int>(_types.Length);

            for (int i = 0; i < _types.Length; i++)
            {
                _options[i + 1] = ObjectNames.NicifyVariableName(_types[i].Name);
                _typeToIndex[_types[i]] = i + 1;
            }
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();

            var header = CreateHeader(property, root);
            var content = CreateContent(property);

            root.Add(header);
            root.Add(content);

            return root;
        }
        
        private VisualElement CreateHeader(SerializedProperty property, VisualElement root)
        {
            var header = new VisualElement();
            header.style.flexDirection = FlexDirection.Row;

            // Foldout
            var foldout = new Foldout
            {
                text = property.displayName,
                value = property.isExpanded
            };
            foldout.style.flexGrow = 1;

            header.Add(foldout);

            // Popup
            var popup = CreateTypePopup(property, root);
            header.Add(popup);

            // Foldout behavior
            foldout.RegisterValueChangedCallback(evt =>
            {
                property.isExpanded = evt.newValue;
                
                var content = root.Q<VisualElement>("content");
                content.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
            });

            return header;
        }
        
        VisualElement CreateTypePopup(SerializedProperty property, VisualElement root)
        {
            var popup = new PopupField<string>(_options.ToList(), 0);
            popup.style.flexGrow = 5;

            Type currentType = GetCurrentType(property);

            int currentIndex = 0;
            if (currentType != null && _typeToIndex.TryGetValue(currentType, out int idx))
                currentIndex = idx;

            popup.index = currentIndex;

            popup.RegisterValueChangedCallback(_ =>
            {
                int newIndex = popup.index;
                if (newIndex == currentIndex) return;

                property.serializedObject.Update();

                property.managedReferenceValue = newIndex == 0
                    ? null
                    : Activator.CreateInstance(_types[newIndex - 1]);

                property.serializedObject.ApplyModifiedProperties();

                root.Clear();
                root.Add(CreatePropertyGUI(property));
            });

            return popup;
        }
        
        VisualElement CreateContent(SerializedProperty property)
        {
            var content = new VisualElement();
            content.name = "content";
            content.style.marginLeft = 16;
            content.style.display = property.isExpanded ? DisplayStyle.Flex : DisplayStyle.None;

            if (property.managedReferenceValue == null)
                return content;

            var iterator = property.Copy();
            var end = iterator.GetEndProperty();

            if (iterator.NextVisible(true))
            {
                while (!SerializedProperty.EqualContents(iterator, end))
                {
                    var field = new PropertyField(iterator.Copy());
                    field.Bind(property.serializedObject);
                    content.Add(field);

                    if (!iterator.NextVisible(false))
                        break;
                }
            }

            return content;
        }
        
        static Type GetCurrentType(SerializedProperty property)
        {
            if (string.IsNullOrEmpty(property.managedReferenceFullTypename))
                return null;

            var split = property.managedReferenceFullTypename.Split(' ');
            return Type.GetType($"{split[1]}, {split[0]}");
        }
    }
}
#endif