using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

namespace StoneShelter{
    public class GUID : MonoBehaviour{
        // Static data
        public const string autogenerateValue = "AUTO";


        // Configurable variables
        [Tooltip("Unique ID of the component, leave it empty to generate a new one")]
        [SerializeField]
        protected string m_guid = "";
        [Tooltip("If true the GUID can be generated and reset by the Prefab Asset")]
        [SerializeField]
        protected bool m_guidPrefab = true;

        // Public access
        /// <summary>
        /// Globally Unique Identifier
        /// </summary>
        public string guid { get => m_guid; }


		protected void Reset(){
			#region prefab
#if UNITY_EDITOR
			if(PrefabStageUtility.GetCurrentPrefabStage() != null || PrefabUtility.IsPartOfPrefabAsset(this)){
                m_guid = autogenerateValue;
                return;
            }
#endif
			#endregion

            // Generate a new GUID
			GenerateGUID();
        }

		/// <summary>
		/// Generate a new GUID
		/// </summary>
		public void GenerateGUID(){
            // Generate a new GUID
            m_guid = Guid.NewGuid().ToString();

			#region prefab + editor
#if UNITY_EDITOR
			// If this is part of a prefab we want to override the property of the instance to save the GUID correctly
			if(PrefabUtility.IsPartOfNonAssetPrefabInstance(this))
                PrefabUtility.RecordPrefabInstancePropertyModifications(this);

            // Mark the object as edited
            EditorUtility.SetDirty(gameObject);

            Debug.Log("<color=yellow>[" + gameObject.name + "(" + this.GetType().Name + ")]</color> Generated new GUID", gameObject);
#endif
			#endregion
		}

        /// <summary>
        /// Delete a generated GUID
        /// </summary>
        public void ResetGUID(){
            // Reset the GUID
            m_guid = "";

			#region prefab + editor
#if UNITY_EDITOR
			// If this is part of a prefab we want to override the property of the instance to save the GUID correctly
			if(PrefabUtility.IsPartOfNonAssetPrefabInstance(this))
                PrefabUtility.RevertPropertyOverride(new SerializedObject(this).FindProperty("m_guid"), InteractionMode.AutomatedAction);

            // Mark the object as edited
            EditorUtility.SetDirty(gameObject);

            Debug.Log("<color=yellow>[" + gameObject.name + "(" + this.GetType().Name + ")]</color> GUID reset", gameObject);
#endif
			#endregion
		}


		#region context menu + editor
#if UNITY_EDITOR
		[MenuItem("CONTEXT/GUID/Generate GUID")]
        protected static void GenerateGUIDStatic(MenuCommand command){
            ((GUID)command.context).GenerateGUID();
        }

        [MenuItem("CONTEXT/GUID/Reset GUID")]
        protected static void ResetGUIDStatic(MenuCommand command){
            ((GUID)command.context).ResetGUID();
        }

        public void PrefabUpdateLinkedInstancesGUID(){
            string assetPath = "";
            if(PrefabStageUtility.GetCurrentPrefabStage() != null)
                assetPath = PrefabStageUtility.GetCurrentPrefabStage().assetPath;
            else if(PrefabUtility.IsPartOfPrefabAsset(this))
                assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(this);
            else {
                Debug.LogError("GameObject is not a Prefab Asset!", gameObject);
                return;
            }

            m_guid = autogenerateValue;
            EditorUtility.SetDirty(this);

            Debug.unityLogger.logEnabled = false;
            GUID[] objs = (GUID[])Resources.FindObjectsOfTypeAll(typeof(GUID));
            foreach(GUID guid in objs) {
                // Check if it is part of a prefab instance and if it doesn't already have a guid
                if(!guid.m_guidPrefab || !PrefabUtility.IsPartOfNonAssetPrefabInstance(guid) || PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(guid) != assetPath)
                    continue;
                UnityEngine.Object source = PrefabUtility.GetCorrespondingObjectFromSource(guid);
                if(source.name != name || source.GetType() != GetType())
                    continue;
                guid.ResetGUID();
            }
            Debug.unityLogger.logEnabled = true;
        }

        [InitializeOnLoadMethod]
        protected static void RegisterHierarchyChanges(){
            EditorApplication.hierarchyChanged += PrefabsInstanceGUIDInitialization;
        }
        protected static void PrefabsInstanceGUIDInitialization(){
            if(PrefabStageUtility.GetCurrentPrefabStage() != null)
                return;

            GUID[] objs = (GUID[])Resources.FindObjectsOfTypeAll(typeof(GUID));
            foreach(GUID gd in objs) {
                // Check if it is part of a prefab instance and if it doesn't already have a guid
                if(!gd.m_guidPrefab || gd.m_guid != autogenerateValue || !PrefabUtility.IsPartOfNonAssetPrefabInstance(gd))
                    continue;
                gd.GenerateGUID();
            }
        }
#endif
        #endregion
    }
}