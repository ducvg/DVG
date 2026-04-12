using UnityEditor;
using UnityEngine;

namespace DVG.Editor
{
    public static class FileSetup
    {
        // _ for topmost
        // ~ for bottommost

        [MenuItem("File/Setup Folders")]
        private static void CreateFolders()
        {
            string[] Folders =
            {
                "Assets/_Projects",

                "Assets/_Projects/_Graphics",
                "Assets/_Projects/_Graphics/Models",
                "Assets/_Projects/_Graphics/MaterialTexture",
                "Assets/_Projects/_Graphics/Sprites",
                "Assets/_Projects/_Graphics/Shader",
                "Assets/_Projects/_Graphics/Animation",
                "Assets/_Projects/_Graphics/~Font",
                "Assets/_Projects/_Graphics/~Cursor",

                "Assets/_Projects/_Scripts",

                "Assets/_Projects/Audio",
                "Assets/_Projects/Audio/Background",
                "Assets/_Projects/Audio/SFX",

                "Assets/_Projects/DataObject",
                "Assets/_Projects/Prefabs",

                "Assets/_Projects/~Scenes",
            };
            
            foreach (var folder in Folders)
            {
                CreateFolderRecursive(folder);
            }

            AssetDatabase.Refresh();
            Debug.Log("Folder structure created!");
        }

        private static void CreateFolderRecursive(string fullPath)
        {
            var parts = fullPath.Split('/');
            string current = parts[0];

            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];

                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }

                current = next;
            }
        }
    }
}
