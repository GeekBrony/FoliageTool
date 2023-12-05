# Performance

## Recommended Terrain Settings
For best results, ensure that the **Detail Resolution** and **Control Resolution** are set to the same resolution. This will prevent integer rounding artifacts.

The terrain settings used for testing are as follows:
> - Detail Resolution: 1024
> - Detail Resolution Per Patch: 64
> - Heightmap Resolution: 513 x 513
> - Control Resolution: 1024 x 1024

## Optimizing Refresh Performance
Using a typical setup with our testing, a full refresh of one terrain can take:
- Minimum of **7 seconds**
- Average of **30 seconds**
- Maximum of **2 minutes**

### Performance Considerations
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
> - Try simplifying your biomes. Use a max of 2 to 4 foliage assets.
> - Remove unnecessary texture rules where needed.

### Debugging Performance
If you are running into performance issues, add the `FLORA_DEBUG` compiler definition to measure times and print a summary of each step.