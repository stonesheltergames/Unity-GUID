using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

namespace StoneShelter{
    [CustomEditor(typeof(GUID), true)]
    public class GUIDEditor : Editor{
        protected const string lastGUIDProp = "m_guidPrefab";

        protected void DrawGUID() {
            EditorGUILayout.BeginHorizontal();
            string guid = serializedObject.FindProperty("m_guid").stringValue;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_guid"));
            if(PrefabUtility.IsPartOfNonAssetPrefabInstance(serializedObject.targetObject)) {
                bool prev = serializedObject.FindProperty("m_guidPrefab").boolValue;
                GUIStyle style = EditorStyles.label;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_guidPrefab"), new GUIContent("", serializedObject.FindProperty("m_guidPrefab").tooltip), GUILayout.Width(style.fontSize + EditorGUIUtility.standardVerticalSpacing));
                bool curr = serializedObject.FindProperty("m_guidPrefab").boolValue;
                if(curr != prev) {
                    if(!prev && curr) {
                        Debug.unityLogger.logEnabled = false;
                        ((GUID)serializedObject.targetObject).ResetGUID();
                        Debug.unityLogger.logEnabled = true;
                        serializedObject.FindProperty("m_guid").stringValue = ((GUID)serializedObject.targetObject).guid;

                        Debug.Log("<color=yellow>[" + ((GUID)serializedObject.targetObject).gameObject.name + "(" + this.GetType().Name + ")]</color> Linked GUID to prefab", ((GUID)serializedObject.targetObject).gameObject);
                    }
                    else {
                        serializedObject.FindProperty("m_guid").stringValue = guid + "$$$";
                        serializedObject.ApplyModifiedProperties();
                        serializedObject.FindProperty("m_guid").stringValue = guid;

                        Debug.Log("<color=yellow>[" + ((GUID)serializedObject.targetObject).gameObject.name + "(" + this.GetType().Name + ")]</color> Unlinked GUID from prefab", ((GUID)serializedObject.targetObject).gameObject);
                    }
                }
            }
            else if(PrefabStageUtility.GetCurrentPrefabStage() != null || PrefabUtility.IsPartOfPrefabAsset(serializedObject.targetObject)) {
                if(GUILayout.Button(GUID.autogenerateValue, GUILayout.Width(50))) {
                    // Set the GUID for the prefab
                    serializedObject.FindProperty("m_guid").stringValue = GUID.autogenerateValue;

                    Debug.Log("<color=yellow>[" + ((GUID)serializedObject.targetObject).gameObject.name + "(" + this.GetType().Name + ")]</color> Prefab instances' GUID set to autogenerate", ((GUID)serializedObject.targetObject).gameObject);
                }

                // If we have changed the GUID we update all the linked instances
                if(guid != serializedObject.FindProperty("m_guid").stringValue) {
                    ((GUID)serializedObject.targetObject).PrefabUpdateLinkedInstancesGUID();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        override public void OnInspectorGUI(){
            // Draw script
            using(new EditorGUI.DisabledScope(true)){
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            }

            // Draw GUID Field
            DrawGUID();

            // Draw default properties
            SerializedProperty prop = serializedObject.FindProperty(lastGUIDProp);
			while(prop.NextVisible(false)){
                EditorGUILayout.PropertyField(prop);
			}

            // Apply edits
            serializedObject.ApplyModifiedProperties();
        }
    }
}