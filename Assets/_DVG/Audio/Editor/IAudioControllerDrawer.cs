#if !ODIN_INSPECTOR
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace DVG.Audio.Editor
{
    [CustomPropertyDrawer(typeof(IAudioController), true)]
    public class IAudioControllerDrawer : PropertyDrawer
    {
        private static readonly Type[] _types;
        private static readonly string[] _options;
        private static readonly Dictionary<Type, int> _typeToIndex;

        static IAudioControllerDrawer()
        {
            _types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t =>
                    typeof(IAudioController).IsAssignableFrom(t) &&
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
            Type currentType = GetCurrentType(property);

            int currentIndex = 0;
            if (currentType != null && _typeToIndex.TryGetValue(currentType, out int idx))
                currentIndex = idx;

            var popupRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            int newIndex = EditorGUI.Popup(popupRect, label.text, currentIndex, _options);

            if (newIndex != currentIndex)
            {
                property.managedReferenceValue = newIndex == 0
                    ? null
                    : Activator.CreateInstance(_types[newIndex - 1]);

                property.serializedObject.ApplyModifiedProperties();
            }

            if (property.managedReferenceValue == null)
                return;

            float y = popupRect.y + EditorGUIUtility.singleLineHeight + 2;

            var iterator = property.Copy();
            var end = iterator.GetEndProperty();

            EditorGUI.indentLevel++;

            if (iterator.NextVisible(true))
            {
                while (!SerializedProperty.EqualContents(iterator, end))
                {
                    float h = EditorGUI.GetPropertyHeight(iterator, true);

                    EditorGUI.PropertyField(
                        new Rect(position.x, y, position.width, h),
                        iterator,
                        true);

                    y += h + 2;

                    if (!iterator.NextVisible(false))
                        break;
                }
            }

            EditorGUI.indentLevel--;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;

            if (property.managedReferenceValue == null)
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

        private static Type GetCurrentType(SerializedProperty property)
        {
            if (string.IsNullOrEmpty(property.managedReferenceFullTypename))
                return null;

            var split = property.managedReferenceFullTypename.Split(' ');
            return Type.GetType($"{split[1]}, {split[0]}");
        }
    }
}
#endif