# Installing FoliageTool

## Install via UPM Registry (Recommended)

The recommended way to install this package is through Unity Package Manager (UPM).
Make sure you're on **Unity 2022.3** or higher for maximum compatibility.

Add the following registry to your `Packages/manifest.json` file:
```json
"scopedRegistries": [
    {
      "name": "Poni Team",
      "url": "http://upm.poniga.me/",
      "scopes": ["team.poni"]
    }
]
```

Once Unity finds the scoped registries, you may proceed onto installing the package.

### Automatic Installation through UPM (Recommended)
In Package Manager, you should find `Foliage Tool` in Scoped Packages. Click to view, and install.
Unity should then fetch the package and install it.

### Manual Installation through UPM
Back in your `Packages/manifest.json` file, add the following into the `dependencies` section:
```json
"team.poni.foliage-tool": "1.0.0-alpha",
```
**NOTE**: The version in this example is `1.0.0-alpha`. You can find the latest version [**on the UPM registry**](http://upm.poniga.me/team.poni.foliage-tool/latest).

After you're done, switch back to Unity. It should then download and install the dependencies, then set up the package.

If all goes well and Unity doesn't complain, that's all you need!

## Install via Git (for Development)

For more bleeding edge updates, or if you are working on a development copy, it might be preferable to clone this repository directly.

Clone the repository into your Unity project's `Packages` folder.
```bash
cd Packages/
git clone https://git.poniga.me/GeekBrony/FoliageTool.git
mv FoliageTool/ team.poni.foliage-tool/
```
The commands above clone the repository into a folder with the name of the package.

Once this process is done, Unity should recognize it as a valid package, and you should be ready to dive in!

