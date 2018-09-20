// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Utils;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor.ShortcutManagement
{
    interface IShortcutProfileStore
    {
        bool ValidateProfileId(string id);
        bool ProfileExists(string id);
        void SaveShortcutProfileJson(string id, string json);
        string LoadShortcutProfileJson(string id);
        void DeleteShortcutProfile(string id);
        IEnumerable<string> GetAllProfileIds();
    }

    class ShortcutProfileStore : IShortcutProfileStore
    {
        public bool ValidateProfileId(string id)
        {
            // TODO: This could be problematic since Path.GetInvalidFileNameChars is platform dependent
            return !string.IsNullOrEmpty(id) && id.IndexOfAny(Path.GetInvalidFileNameChars()) == -1;
        }

        public bool ProfileExists(string id)
        {
            return File.Exists(GetPathForProfile(id));
        }

        public void SaveShortcutProfileJson(string id, string json)
        {
            var path = GetPathForProfile(id);
            System.IO.Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, json);
        }

        public string LoadShortcutProfileJson(string id)
        {
            var path = GetPathForProfile(id);
            return File.ReadAllText(path);
        }

        public string[] LoadAllShortcutProfilesJsonFromDisk()
        {
            string[] profilePaths = GetAllShortcutProfilePaths().ToArray();
            string[] filesJson = new string[profilePaths.Length];
            for (int i = 0; i < profilePaths.Length; ++i)
            {
                filesJson[i] = File.ReadAllText(profilePaths[i]);
            }

            return filesJson;
        }

        public IEnumerable<string> GetAllProfileIds()
        {
            var profilePaths = GetAllShortcutProfilePaths();
            var profileIds = new List<string>(profilePaths.Count());
            foreach (var profilePath in profilePaths)
            {
                profileIds.Add(Path.GetFileNameWithoutExtension(profilePath));
            }
            return profileIds;
        }

        public void DeleteShortcutProfile(string id)
        {
            File.Delete(Path.Combine(GetShortcutFolderPath(), id + ".shortcut"));
        }

        public static string GetShortcutFolderPath()
        {
            return Paths.Combine(InternalEditorUtility.unityPreferencesFolder, "shortcuts");
        }

        static string GetPathForProfile(string id)
        {
            return Paths.Combine(GetShortcutFolderPath(), id + ".shortcut");
        }

        static IEnumerable<string> GetAllShortcutProfilePaths()
        {
            var shortcutsFolderPath = GetShortcutFolderPath();
            if (!System.IO.Directory.Exists(shortcutsFolderPath))
                return Enumerable.Empty<string>();

            return System.IO.Directory.GetFiles(shortcutsFolderPath, "*.shortcut", SearchOption.TopDirectoryOnly);
        }
    }
}
