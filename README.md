# FoliageTool: Procedural Foliage
Artist-friendly procedural foliage generation tool for the Unity Editor. Enables rapid prototyping using a non-destructive foliage workflow.

## Features

### Procedural Foliage
At it's core, FoliageTool functions as a procedural foliage painter, removing the need to configure and paint details on every terrain manually.

#### **Biomes**
Create variation in your terrain's foliage with **biomes**.

With biomes, you are able to create custom **procedural rule-sets** for terrain foliage.
- Include or exclude foliage on a specified terrain texture.
- Include foliage within a specified steepness / height range.
- Filter your foliage through highly configurable perlin noise.

Additionally, it is possible to add more variation by configuring **many foliage assets** in **one biome**.

#### **Tree Avoidance**
To prevent overlapping, FoliageTool optionally removes foliage around all trees on your terrain.
- Custom blend range to softly fade around trees.
- Custom padding size around trees.

### Brush Workflow
To enable rapid and iterative prototyping, **brushes** are implemented as a mechanism to replace manual painting.

#### Create any shape using curves!
FoliageTool integrates natively with [Unity Splines](https://docs.unity3d.com/Packages/com.unity.splines@2.4/manual/index.html) to allow you to create beautiful landscapes of foliage.

By utilizing curves, FoliageTool can easily create details such as paths, flowerbeds, or crop formations.

#### More Power to Create!!!
- **Brush Layering**
    - Supports **up to 64 brushes** on top of each other.
- **Brush Blending**
    - Creatively blend between brushes.
    - **Blend, Add, Subtract** are supported Blend Modes.
    - **Falloff and Alpha** properties to allow custom fading.

#### Non-Destructive
Brushes are non-destructive by design, persisting as GameObjects in the scene.
Even after clearing the details on a terrain, you will not lose your work.

You'll never need to re-paint your details again! ;)

### Native Terrain Integration
FoliageTool uses just one component to pair with Unity's default `Terrain` component, `FoliageTerrain`.

To keep it simple, FoliageTool utilizes the terrain's native detail system to paint foliage on the terrain.

FoliageTool automatically bakes the details into your terrain without additional work.
After it bakes, you can safely remove FoliageTool from your terrain without losing your work.

## More Info

### What FoliageTool is not...
- FoliageTool is ***not*** a real-time foliage generator.
    - It is only designed for use in the Unity Editor as a *step* in the level design process.
- FoliageTool is ***not*** a procedural tree generator, nor will it spawn trees for you.
    - Instead, it works with the existing trees that are painted on your terrain.
- FoliageTool does ***not*** manage foliage instancing.
    - This task is best left to Unity or other instancing plugins.

### Installing
For instructions on how to install FoliageTool as a package, [click here](Documentation/Installing.md).

### Getting Started
For instructions on how to begin creating with FoliageTool, [click here](Documentation/GettingStarted.md).

### Issues
For a list of FoliageTool's known and documented issues, [click here](Documentation/Issues.md).

### Performance
For more details about the performance of FoliageTool, [click here](Documentation/Performance.md).
