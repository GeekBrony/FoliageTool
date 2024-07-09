# Installing FoliageTool in your Unity project

## Dependencies
To install FoliageTool, you must be using **Unity 2022** (or newer).

FoliageTool uses the following packages as dependencies. These should install automatically when you install FoliageTool.
```json
"com.unity.modules.terrain": "1.0.0",
"com.unity.mathematics": "1.2.6",
"com.unity.splines": "2.4.0", 
"com.unity.editorcoroutines": "1.0.0"
```

## Install via Git

Clone the repository into your Unity project's `Packages` folder.
```bash
cd Packages/
git clone https://github.com/GeekBrony/FoliageTool.git
mv FoliageTool/ team.poni.foliage-tool/
```
The commands above clone the repository into a folder with the name of the package.

Once this process is done, Unity should recognize it as a valid package, and you should be ready to [dive in](GettingStarted.md)!