using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Josing.ToolsEditor
{
    using Josing.Utils;
#if UNITY_EDITOR
    public class MD5CreateEditor : EditorWindow
    {

        static MD5CreateEditor md5Create;


        string key;
        string pwd;
        string md5str;
        string saveFileName;


        string dekey;
        string demd5str;

        [MenuItem("Tools/MD5 Tool")]
        static void InitWindow()
        {
            md5Create = GetWindow<MD5CreateEditor>("MD5生成工具", true);
            md5Create.Show();
            md5Create.Init();
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(0,0,500,500));
            GUILayout.BeginVertical();
            GUILayout.Box("");

            GUILayout.BeginHorizontal();
            GUILayout.Label("key", GUILayout.Width(100));
            GUILayout.TextField(key);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("pwd", GUILayout.Width(100));
            pwd = EditorGUILayout.DelayedTextField(pwd);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("md5str", GUILayout.Width(100));
            GUILayout.TextField(md5str);
            GUILayout.EndHorizontal();


            GUILayout.Space(30);

            if (GUILayout.Button("CopyKey"))
            {
                TextEditor te = new TextEditor();
                te.text = key;
                te.SelectAll();
                te.Copy();
            }


            GUILayout.BeginHorizontal();
            GUILayout.Label("saveFileName", GUILayout.Width(100));
            saveFileName = GUILayout.TextField(saveFileName);
            GUILayout.EndHorizontal();

            if (GUILayout.Button("CopyKeyAndWrite"))
            {
                StreamWriter fs = new StreamWriter(Application.dataPath + "/" + saveFileName + ".txt", false, System.Text.Encoding.ASCII);
                fs.Write(key);
                fs.Flush();
                fs.Close();
                fs.Dispose();
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("CopyMD5Str"))
            {
                TextEditor te = new TextEditor();
                te.text = md5str;
                te.SelectAll();
                te.Copy();
            }

            if (GUILayout.Button("Apply"))
            {
                md5str = MD5Tools.MD5Encrypt(pwd, key);
            }


            GUILayout.Space(30);

            GUILayout.BeginHorizontal();
            GUILayout.Label("dekey", GUILayout.Width(100));
            dekey = GUILayout.TextField(dekey);
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Paste"))
            {
                dekey = key;
            }

            if (GUILayout.Button("Apply"))
            {
                demd5str = MD5Tools.MD5Decrypt(md5str, dekey);
                Debug.Log(demd5str);
            }
            ShowNotification(new GUIContent("1234"));

            //EditorUtility.OpenFolderPanel()
            GUILayout.EndVertical();

            GUILayout.EndArea();
        }

        public void Init()
        {
            key = MD5Tools.GenerateKey();
        }
    }
#endif
}

