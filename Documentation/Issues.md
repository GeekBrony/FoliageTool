# Known Issues

## Brush: On Modify
### All details removed outside of the brush
> Currently, this is a limitation primarily at fault by design.
>
> The brush triggers a *refresh* of all the details, but only within the bounds of the brush that was modified.
> However, when a brush uses a foliage asset that is not yet on the terrain, the terrain clears all details from the terrain before refreshing.
>
>**Workaround:** Refresh the affected terrain through the `FoliageTerrain` component.
