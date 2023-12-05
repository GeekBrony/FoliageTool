# Flora: Procedural Foliage

Artist-friendly procedural foliage generation tool, providing an iterative terrain workflow in the Unity Editor.

## Features

### Procedural Foliage

Flora Studio functions as a procedural foliage painter, removing the need to configure and paint details on every terrain manually.

#### **Biomes**
Create variation in your terrain's foliage with **biomes**.

With biomes, you are able to create custom **procedural rule-sets** for terrain foliage.
- Include or exclude foliage from a specified terrain texture.
- Include foliage within a specified steepness / height range.
- Filter your foliage through highly configurable perlin noise.

Additionally, it is possible to add more variation by configuring **many foliage assets** in **one biome**.

#### **Tree Filtering**
Flora Studio optionally filters foliage around all trees on your terrain.
- Custom Blend Range
- Custom Padding

### Brush Workflow
To enable rapid and iterative prototyping, **brushes** are implemented as a mechanism to replace manual painting.

#### Create any shape using curves!
Flora Studio integrates natively with [Unity Splines](https://docs.unity3d.com/Packages/com.unity.splines@2.4/manual/index.html) to allow you to create beautiful landscapes of foliage with ease.
Create details such as paths, flowerbeds, or crop formations!

#### More Power To Create!
- **Brush Layering**
    - Supports **up to 64 brushes** on top of each other.
- **Brush Blending**
    - Creatively blend between brushes.
    - **Blend, Add, Subtract** are supported Blend Modes.
    - **Falloff and Alpha** properties to allow custom fading.

#### Non-Destructive 
Because brushes are GameObjects that exist in a scene, they are non-destructive by design.
Brushes are able to persist even after clearing the details on a terrain.

### Native Terrain Integration

Flora Studio uses just one component to pair with Unity's default `Terrain` component, `Flora StudioTerrain`.

To keep it simple, Flora Studio utilizes the terrain's native detail system to paint foliage on the terrain.

Flora Studio automatically bakes the details into your terrain without additional busywork.
After it bakes, you can safely remove Flora Studio from your terrain without losing your work.

---

## More Info

### What Flora Studio is not...
- Flora Studio is ***not*** a real-time foliage generator.
    - It is only designed for use in the Unity Editor as a *step* in the level design process.
- Flora Studio is ***not*** a procedural tree generator.
    - Instead, it works with the existing trees on your terrain.
- Flora Studio does ***not*** manage foliage instancing.
    - This task is best left to Unity or other instancing plugins.

### Installing
For instructions on how to install Flora Studio as a package, [click here](Documentation/Installing.md).

### Getting Started
For instructions on how to begin creating with Flora Studio, [click here](Documentation/GettingStarted.md).

### Issues
For a list of Flora Studio's known and documented issues, [click here](Documentation/Issues.md).

### Performance
For more details about the performance of Flora Studio, [click here](Documentation/Performance.md).
