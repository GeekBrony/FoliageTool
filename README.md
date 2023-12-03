# Flora Foliage

Artist-friendly procedural foliage generation tool, providing an iterative level design workflow in Unity.

## Features

### Procedural Foliage
- Flora implements **biomes**, which are assets that function as procedural rule-sets for the foliage on your terrain.
- Flora also uses assets to reference configurations of foliage, removing the need to configure details on every terrain manually.

### Brush Workflow
To enable rapid and iterative prototyping, **brushes** are implemented as a mechanism to replace manual painting.

Because brushes are GameObjects that exist in a scene, they are non-destructive by design.
Brushes are able to persist even after clearing the details on a terrain.

#### Biome Brushes
Define a biome within a configurable spline area. 
- Blends between other biome brushes, using `Draw Order` and `Blend Mode`
- Custom brush falloff and alpha


### Native Terrain Integration
Most foliage tools make no sense to anyone who isn't a tinkerer. If you are working with these tools, you may be navigating more menus than you need to be!

Flora uses just one component to pair with Unity's default `Terrain` component.
- It utilizes the terrain's detail system to automagically paint foliage on the terrain.

You can safely remove Flora from your terrain without losing your work.

---

## More Info

### What Flora is not...
- Flora is *not* a runtime foliage generator. It is designed for use in Unity as a *step* in the level design process.
- Flora is *not* a procedural tree generator. Instead, it works with the existing trees on your terrain.
- Flora does *not* manage foliage instancing. This task is best left to Unity or other instancing plugins.

### Known Issues
For a list of known issues, refer to our documentation. Specifically [this page](Documentation/KnownIssues.md).

---

## Getting Started

### 1. Create your first Foliage
// TODO

### 2. Create your first Biome
// TODO

### 3. Add Flora to your Terrain
// TODO

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
> The bigger the detail resolution, the longer it will take to refresh. Consider adjusting the `Chunked Refresh Resolution` value.
>
> ##### Brush Count
> The more brushes you have on a terrain, the longer it will take to refresh. More complex layering will also slow down the refresh process.
>
> ##### Brush Size
> When editing a brush, consider the size of the spline. Larger brushes will refresh a larger part of the terrain, which will take a longer time.
>
> ##### Tree Count
> The more trees you have on a terrain, the longer it will take to refresh.
>
> ##### Biome and Foliage Complexity
> - Try simplifying your biomes to a max of 3 foliage assets.
> - Remove unnecessary texture rules where needed.

#### Debugging Performance
If you are running into performance issues, add the `FLORA_DEBUG` compiler definition to measure times and print a summary of each step.

Once added, this will log something like:
```
Base refresh time: 459 ms

BIOMES...
mask evaluation time: 54 ms
biome evaluation time: 48 ms

TREES...
mask evaluation time: 0 ms
tree evaluation time: 10 ms

Region total time: 571 ms
```