using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FoliageTool.Core
{
    public class FoliageToolSceneProcessor : IProcessSceneWithReport
    {
        public int callbackOrder { get; }
        public void OnProcessScene(Scene scene, BuildReport report)
        {
            // strip FoliageTool from build
            foreach (var gameObject in scene.GetRootGameObjects())
            {
                ProcessGameObject(gameObject);
            }
        }
    
        private void ProcessGameObject(GameObject gameObject)
        {
            var terrains = gameObject.GetComponentsInChildren<FoliageTerrain>();
            var brushes = gameObject.GetComponentsInChildren<Brush>();
        
            // "bake" terrain foliage by deleting FoliageTerrain
            for (var index = 0; index < terrains.Length; index++)
            {
                var terrain = terrains[index];
                Object.DestroyImmediate(terrain);
            }

            // Delete brushes
            for (var index = 0; index < brushes.Length; index++)
            {
                var brush = brushes[index];
                Object.DestroyImmediate(brush);
            }
        }


    }
}
