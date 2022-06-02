# PrefabLoader

Prefab loader allows you to load unity `.prefab` files directly into SCP SL. However, it only supports primitive types (cube, cylinder, sphere, capsule, plane, and quad). This is a limitation of SL, and you can't load things like meshes without client mods.

This is a developer dependency, for the most part. This means regular server owners probably won't ever care about this, but it makes loading custom objects easy.

# What about MER?
[MER](https://github.com/Michal78900/MapEditorReborn/) is a bigger plugin for map editing (hence the name). It has more features, such as rigidbodies and animations. PrefabLoader is made to be as lightweight as possible and offer exactly what it says: loading prefabs. If you need more functionality, then use MapEditorReborn. If you just need to load a couple staticly positioned prefabs, then use this. It really isn't that hard to understand the differences between the two. What I don't want is for *people* to try and start some kinda war between this, it's not that big of a deal. They can both co-exist, PrefabLoader accomplishes a subsection of what MER does. However, if you just need that subsection (spawning prefabs which don't have animations or physics), your server will legitimately just be more performant because it doesn't have all the extra overhead.

# Features
- Primitive prefab loading
- Parenting/offsetting
- Origin positioning (setting the origin position will move/rotate all parts accordingly. Basically it's an empty object at 0, 0, 0 in the local space of the group of objects, like an actual prefab)
- Material (color only) loading

# How to use
In Unity:
1. Open up a unity project
2. Go to Edit->Project Settings->Editor. Find `Asset Serialization`. Set it to `Force Text`
3. Make your prefab
4. Get the location of the prefab (right click it in your asset browser then "Show in explorer")

From code (assuming it's installed and referenced):
1. Load materials first using `PrefabLoader.API.LoadMaterial(materialFileLocation, materialMetaDataLocation)` (metadata is needed to get the guid of the material)
2. Load the prefab using `PrefabLoader.API.LoadPrefab(prefabFileLocation)`. This will return a `Prefab` object which you can then use `Prefab.Spawn()` on
3. Use the `SpawnedPrefab` returned from `Prefab.Spawn()` to manipulate it (rotation, position)

# Important notes
- You can call `Prefab.Spawn()` as many times as you want and it will always return a unique `SpawnedPrefab`
- `SpawnedPrefab`s are destroyed on round restart. This means any references you have to a spawned prefab should be cleared on round restart unless you plan on repopulating the memory. If you don't, congrats you have a memory leak