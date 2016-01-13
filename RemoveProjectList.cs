using Microsoft.Win32;
using PlistCS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
public class RemoveProjectList : EditorWindow {
    private static Dictionary<string, string> projects = new Dictionary<string, string>();

    [MenuItem("Edit/Remove Projects From Wizard1")]
    public static void ShowWindow() {
        if (SystemInfo.operatingSystem.Contains("Windows")) {
            GetProjectsFromRegistry();
        }
        else {
            if (SystemInfo.operatingSystem.Contains("Mac")) {
                GetProjectsFromPList();
            }
        }
        EditorWindow.GetWindow(typeof(RemoveProjectList));
    }

    private void OnGUI() {
        GUILayout.Label("Previous Projects", EditorStyles.boldLabel, new GUILayoutOption[0]);
        List<string> list = new List<string>();
        foreach (string current in projects.Keys) {
            GUILayout.Label(projects[current], new GUILayoutOption[0]);
            if (GUILayout.Button("Remove Project From Wizard", new GUILayoutOption[0])) {
                EditorPrefs.DeleteKey(current);
                list.Add(current);
            }
        }
        foreach (string current2 in list) {
            projects.Remove(current2);
        }
    }
    private static void GetProjectsFromRegistry() {
        projects.Clear();
        string text = "Software\\Unity Technologies\\Unity Editor ";
        text += Application.unityVersion;
        text = text.Substring(0, text.IndexOf(".")) + ".x";
        RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(text, true);
        Debug.Log(registryKey.ToString());
        string[] valueNames = registryKey.GetValueNames();
        string[] array = valueNames;
        for (int i = 0; i < array.Length; i++) {
            string text2 = array[i];
            if (text2.Contains("RecentlyUsedProjectPaths")) {
                byte[] bytes = registryKey.GetValue(text2) as byte[];
                string @string = Encoding.Default.GetString(bytes);
                projects.Add(text2, @string);
            }
        }
    }

    private static void GetProjectsFromPList() {
        projects.Clear();
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library") + "/Preferences/com.unity3d.UnityEditor5.x.plist";
        Dictionary<string, object> dictionary = Plist.readPlist(path) as Dictionary<string, object>;
        foreach (KeyValuePair<string, object> current in dictionary) {
            if (current.Key.Contains("RecentlyUsedProjectPaths")) {
                projects.Add(current.Key, current.Value.ToString());
            }
        }
    }
}
