# Unity GUID
Globally Unique Identifier

An implementation of a GUID, it can be used standalone, with the **GUID component** provided, or inside any script which can derive from it.
Unlike Unity's `Object.GetInstanceID()` this implementation is persistent as it gets serialized inside the component itself.

#### Features
- **Simple** - a single component
- **Automated** - each GUID is automatically generated and can be edited, regenerated or reset
- **Persistent** - no more Unity randomly regenerating InstanceIDs, as the GUIDs are serialized directly into the gameobjects
- **Extensible** - add functionalities by inheriting from the GUID class, you can write your own Editor by inheriting from GUIDEditor too

#### Guides
- [Installation](#installation)
- [Usage (standalone)](#usage-standalone)
- [Usage (script inheritance)](#usage-script-inheritance)
- [Scripting](#scripting)
- [GUID generation and reset](#guid-generation-and-reset)
- [Prefab workflow](#prefab-workflow)

---

### Installation
#### Package Manager via git URL
1. Open Package Manager via Window > Package Manager
2. Add package from git URL...
3. Enter `https://github.com/stonesheltergames/Unity-GUID.git`
<img width="500" alt="package-manager-install" src="https://user-images.githubusercontent.com/5909516/155850620-c7e7d61c-d6c2-4bf3-b84e-ba4bcd439ace.png">

#### Manual install
1. Download this repository or one of the Released **.unitypackage**s at https://github.com/stonesheltergames/Unity-GUID/releases
2. Copy the files or extract the .unitypackage inside the root of your project (you can move the folder anywhere you need after that)

### Usage (standalone)
You can add the GUID component to any gameobject by clicking **Add Component > Stone Shelter > Core > GUID**  
![](https://www.stonesheltergames.com/wp-content/uploads/2020/12/AddComponent.png)

### Usage (script inheritance)
You can inherit from the GUID script and add your own functionalities on top, remember to include the correct namespace by adding on top of your file:  
`using StoneShelter;`

### Scripting
Inside your scripts you can retrieve the GUID from a gameobject by using `GetComponent<GUID>().guid` which will give you the current GUID as a string.  
You can also retrieve any object from any scene with the static method `GUID.Find(guid)` where you can pass a guid as a string.

### GUID generation and reset
When you add a GUID component (or a component derived from it) for the first time to a gameobject a new GUID is automatically generated:  
![](https://www.stonesheltergames.com/wp-content/uploads/2020/12/GUIDGenerated.png)

You can manually generate a new GUID or reset it (for example if you duplicated the gameobject) from the context menu by clicking on the three dots on top of the component and then clicking one of the options on the bottom:
![](https://www.stonesheltergames.com/wp-content/uploads/2020/12/ContextMenu.png)

### Prefab workflow
If you are working with prefabs there are some things that you need to know:
- **Prefab Instances** can inherit the GUID from the **Prefab Asset** (linked instances) or they can ignore it and set their own.  
If a gameobject is derived from a prefab you will see a toggle on the right of the GUID, if you leave it checked the Prefab Instance will **inherit the GUID** from the Prefab Asset, otherwise it will copy its current value and **won't be overwritten** by it.  
![](https://www.stonesheltergames.com/wp-content/uploads/2020/12/PrefabInherit.png)
- **Prefab Assets** can set a fixed GUID for all the **linked Prefab Instances** or generate a new one for each instance.
When editing a prefab you will see an **AUTO button** on the right of the GUID, if you press it you will generate a new unique GUID for each linked instance (see above) of the prefab (existing and future ones), if you change the GUID to anything other than "AUTO" all the existing linked instances will update their GUIDs to be the same.  
![](https://www.stonesheltergames.com/wp-content/uploads/2020/12/PrefabAsset.png)
