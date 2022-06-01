# PrefabLoader

Prefab loader allows you to load unity `.prefab` files directly into SCP SL. However, it only supports primitive types (cube, cyllinder, sphere, capsule, plane, and quad). This is a limitation of SL, and you can't load things like meshes without client mods.

This is a developer dependency, for the most part. This means regular server owners probably won't ever care about this, but it makes loading custom objects easy.

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
4. Get the location of the prefab

From code (assuming it's installed and referenced):
1. Load materials first using `PrefabLoader.API.LoadMaterial(materialFileLocation, materialMetaDataLocation)` (metadata is needed to get the guid of the material)
2. Load the prefab using `PrefabLoader.API.LoadPrefab(prefabFileLocation)`. This will return a `Prefab` object which you can then use `Prefab.Spawn()` on
3. Use the `SpawnedPrefab` returned from `Prefab.Spawn()` to manipulate it (rotation, position)