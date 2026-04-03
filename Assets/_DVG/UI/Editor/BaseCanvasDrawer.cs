using UnityEditor;

namespace DVG.UI.Editor
{
    [CustomEditor(typeof(BaseCanvas), true)]
    public class BaseCanvasEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            SerializedProperty timeoutProp = serializedObject.FindProperty("_timeout");
            DrawPropertiesExcluding(serializedObject, "_timeout");
            
            // rect.y += 2;
            string label = GetNiceName(timeoutProp);

            EditorGUI.PropertyField(
                EditorGUILayout.PropertyField(timeoutProp),
                timeoutProp,
                GetCachedLabel(label),
                true
            );
        }

        private string GetNiceName(SerializedProperty prop)
        {
            if (prop.managedReferenceValue == null) return "Null";

            return ObjectNames.NicifyVariableName(prop.managedReferenceValue.GetType().Name);
        }
    }
}
