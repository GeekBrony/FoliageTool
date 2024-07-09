# Known Issues
## Performance

### Wait times
At this time, FoliageTool runs on the main thread. This unfortunately blocks the Unity Editor for several seconds when evaluating brushes.

While there is currently no fix available, there are plans to optimize this process in the future, using the following techniques:
- Compute shaders for fast GPU evaluation
- Unity's C# Jobs + Burst compiler
