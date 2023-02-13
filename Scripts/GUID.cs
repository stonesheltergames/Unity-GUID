using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
#endif
using UnityEngine;

namespace StoneShelter{
    [AddComponentMenu("Stone Shelter/Core/GUID")]
    public class GUID : MonoBehaviour{
        // Static data
        /// <summary>
        /// Value to set in prefabs to auto generate the GUID in instances
        /// </summary>
        public const string autogenerateValue = "AUTO";
        protected static Dictionary<string, GUID> m_data = new Dictionary<string, GUID>();  // Active instances

        [RuntimeInitializeOnLoadMethod]
        protected static void RegisterInactiveObjects(){
            foreach(GUID guid in Resources.FindObjectsOfTypeAll<GUID>()) {
                m_data[guid.m_guid] = guid;
            }
        }

        /// <summary>
        /// Finds an object by its GUID
        /// </summary>
        /// <param name="guid">unique identifier</param>
        /// <returns>GUID if found, null otherwise</returns>
        public static GUID Find(string guid){
#if UNITY_EDITOR
			if(!EditorApplication.isPlaying){
                foreach(GUID gd in Resources.FindObjectsOfTypeAll<GUID>()) {
                    if(gd.m_guid == guid)
                        return gd;
                }
                return null;
            }
#endif

            if(m_data.ContainsKey(guid))
                return m_data[guid];
            return null;
		}


        // Configurable variables
        [Tooltip("Unique ID of the component, leave it empty to generate a new one")]
        [SerializeField]
        protected string m_guid = "";
        [Tooltip("If true the GUID will be inherited from the Prefab Asset")]
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
		protected void Awake(){
            m_data[m_guid] = this;
		}
		protected void OnDestroy(){
            if(m_data.ContainsKey(m_guid))
                m_data.Remove(m_guid);
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
            foreach(GUID guid in Resources.FindObjectsOfTypeAll<GUID>()) {
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

            foreach(GUID gd in Resources.FindObjectsOfTypeAll<GUID>()) {
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