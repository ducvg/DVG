#if !ODIN_INSPECTOR
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace DVG.UI.Editor
{
    [CustomPropertyDrawer(typeof(ITimeout), true)]
    public class TimeoutDrawer : PropertyDrawer
    {
        static readonly Type[] _types;
        static readonly string[] _options;
        static readonly Dictionary<Type, int> _typeToIndex;

        static TimeoutDrawer()
        {
            _types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch { return Type.EmptyTypes; }
                })
                .Where(t =>
                    typeof(ITimeout).IsAssignableFrom(t) &&
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

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float line = EditorGUIUtility.singleLineHeight;

            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, line);
            property.isExpanded = EditorGUI.Foldout(labelRect, property.isExpanded, label, true);

            Rect popupRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, line);

            Type currentType = GetCurrentType(property);

            int currentIndex = 0;
            if (currentType != null && _typeToIndex.TryGetValue(currentType, out int idx))
                currentIndex = idx;

            int newIndex = EditorGUI.Popup(popupRect, currentIndex, _options);

            if (newIndex != currentIndex)
            {
                property.managedReferenceValue = newIndex == 0
                    ? null
                    : Activator.CreateInstance(_types[newIndex - 1]);
            }

            if (!property.isExpanded || property.managedReferenceValue == null) return;

            EditorGUI.indentLevel++;

            float y = popupRect.y + line + 2;
            DrawSerializableClass(position, property, y);

            EditorGUI.indentLevel--;
        }

        private static void DrawSerializableClass(Rect position, SerializedProperty property, float y)
        {
            var iterator = property.Copy();
            var end = iterator.GetEndProperty();

            if (iterator.NextVisible(true))
            {
                while (!SerializedProperty.EqualContents(iterator, end))
                {
                    float h = EditorGUI.GetPropertyHeight(iterator, true);

                    EditorGUI.PropertyField(
                        new Rect(position.x, y, position.width, h),
                        iterator,
                        true
                    );

                    y += h + 2;

                    if (!iterator.NextVisible(false))
                        break;
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;

            if (!property.isExpanded || property.managedReferenceValue == null)
                return height;

            var iterator = property.Copy();
            var end = iterator.GetEndProperty();

            height += 2;

            if (iterator.NextVisible(true))
            {
                while (!SerializedProperty.EqualContents(iterator, end))
                {
                    height += EditorGUI.GetPropertyHeight(iterator, true) + 2;

                    if (!iterator.NextVisible(false))
                        break;
                }
            }

            return height;
        }

        static GUIContent GetHeaderLabel(SerializedProperty property, GUIContent label)
        {
            if (property.managedReferenceValue == null)
                return label;

            return new GUIContent(label.text);
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