# Flora Foliage

Artist-friendly procedural foliage generation tool, providing an iterative level design workflow in Unity.

## Features

### Procedural Foliage

Flora removes the need to configure and paint details on every terrain manually.

#### **Custom Biomes**
Create custom procedural rule-sets for terrain foliage.
- Terrain Texture (Terrain Layer) Rules
- Steepness and Height limits
- Perlin Noise

#### **Tree Filtering**
Flora optionally filters foliage around all trees on your terrain.
- Custom Blend Range
- Custom Padding

### Brush Workflow

To enable rapid and iterative prototyping, **brushes** are implemented as a mechanism to replace manual painting.

Because brushes are GameObjects that exist in a scene, they are non-destructive by design.
Brushes are able to persist even after clearing the details on a terrain.

#### Biome Brushes
Define a biome within a configurable spline area. 
- Create any shape using Splines.
    - Example: Paths, Flower Beds, Crop Formations
- Blends between other biome brushes, using `Draw Order` and `Blend Mode`
- Custom brush falloff and alpha


### Native Terrain Integration

Flora uses just one component to pair with Unity's default `Terrain` component, `FloraTerrain`.

To keep it simple, Flora utilizes the terrain's native detail system to paint foliage on the terrain.

Flora automatically bakes the details into your terrain without additional busywork. After it bakes, you can safely remove Flora from your terrain without losing your work.

---

## More Info

### What Flora is not...
- Flora is ***not*** a real-time foliage generator.
    - It is only designed for use in the Unity Editor as a *step* in the level design process.
- Flora is ***not*** a procedural tree generator.
    - Instead, it works with the existing trees on your terrain.
- Flora does ***not*** manage foliage instancing.
    - This task is best left to Unity or other instancing plugins.

### Known Issues
For a list of known and documented issues, [click here](Documentation/Issues.md).

## Getting Started
For instructions on how to get started, [click here](Documentation/GettingStarted.md).


---
## Performance

### Recommended Terrain Settings
For best results, ensure that the **Detail Resolution** and **Control Resolution** are set to the same resolution. This will prevent integer rounding artifacts.

The terrain settings used for testing are as follows:
> - Detail Resolution: 1024
> - Detail Resolution Per Patch: 64
> - Heightmap Resolution: 513 x 513
> - Control Resolution: 1024 x 1024

### Optimizing Refresh Performance
Using a typical setup with our testing, a full refresh of one terrain can take:
- Minimum of **7 seconds**
- Average of **30 seconds**
- Maximum of **2 minutes**

#### Performance Considerations
The performance of refreshing a terrain depends on the factors listed below.
> ##### Detail Resolution
> The bigger the detail resolution, the longer it will take to refresh.
> Consider adjusting the `Chunked Refresh Resolution` value.
> ##### Brush Count
> The more brushes you have on a terrain, the longer it will take to refresh. More complex layering will also slow down the refresh process.
> ##### Brush Size
> When editing a brush, consider the size of the spline.
> Larger brushes will refresh a larger part of the terrain, which will take a longer time.
> ##### Tree Count
> The more trees you have on a terrain, the longer it will take to refresh.
> ##### Biome and Foliage Complexity
> These typically shouldn't matter, but if you still aren't getting good performance:
> - Try simplifying your biomes to a max of 3 foliage assets.
> - Remove unnecessary texture rules where needed.

#### Debugging Performance
If you are running into performance issues, add the `FLORA_DEBUG` compiler definition to measure times and print a summary of each step.