#if !ODIN_INSPECTOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DVG.Audio.Editor
{
    [CustomEditor(typeof(AudioManager))]
    public sealed class AudioManagerEditor : UnityEditor.Editor
    {
        ReorderableList _list;

        void OnEnable()
        {
            var prop = serializedObject.FindProperty("_controllers");

            _list = new ReorderableList(serializedObject, prop, true, true, true, true);

            _list.drawHeaderCallback = rect =>
            {
                rect.x -= 6;
                prop.isExpanded = EditorGUI.Foldout(rect, true, "Audio Controllers", true);
            };

            _list.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = prop.GetArrayElementAtIndex(index);

                rect.y += 2;

                string label = GetNiceName(element);

                EditorGUI.PropertyField(
                    rect,
                    element,
                    GetCachedLabel(label),
                    true
                );
            };

            _list.elementHeightCallback = index =>
            {
                var element = prop.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(element, true) + 4;
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject, "_controllers");

            var prop = serializedObject.FindProperty("_controllers");
            

            if (prop.isExpanded)
            {
                _list.DoLayoutList();
            } else {
                prop.isExpanded = EditorGUILayout.Foldout(false, "Audio Controllers", true);
            }

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }

        string GetNiceName(SerializedProperty prop)
        {
            if (prop.managedReferenceValue == null)
                return "Null";

            return ObjectNames.NicifyVariableName(prop.managedReferenceValue.GetType().Name);
        }

        static readonly System.Collections.Generic.Dictionary<string, GUIContent> _labelCache = new();
        GUIContent GetCachedLabel(string text)
        {
            if (!_labelCache.TryGetValue(text, out var content))
            {
                content = new GUIContent(text);
                _labelCache[text] = content;
            }

            return content;
        }
    }
}
#endif