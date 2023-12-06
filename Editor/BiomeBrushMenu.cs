using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

namespace FoliageTool.Core
{
    public static class BiomeBrushMenu
    {
        [MenuItem("GameObject/Foliage/Biome Brush/Circle", false, 10)]
        static void NewBrushCircle(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Biome Brush", typeof(SplineContainer), typeof(BiomeBrush));
            
            var container = go.GetComponent<SplineContainer>();
            container.Spline = SplineFactory.CreateCircle(10);

            GameObject context = menuCommand.context as GameObject;
            if (context)
            {
                GameObjectUtility.SetParentAndAlign(go, context);
            }
            else
            {
                SceneView sceneView = SceneView.lastActiveSceneView;
                Camera camera = sceneView.camera;
                Vector3 viewDir = camera.transform.forward;
                Ray r = new Ray(camera.transform.position, viewDir);
                Vector3 pos =  camera.transform.position;
                if (Physics.Raycast(r, out var hit, 100))
                    pos = hit.point;

                go.transform.position = pos;
            }


            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
        
        [MenuItem("GameObject/Foliage/Biome Brush/Square", false, 10)]
        static void NewBrushSquare(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Biome Brush", typeof(SplineContainer), typeof(BiomeBrush));
            
            var container = go.GetComponent<SplineContainer>();
            container.Spline = SplineFactory.CreateSquare(20);
            
            GameObject context = menuCommand.context as GameObject;
            if (context)
            {
                GameObjectUtility.SetParentAndAlign(go, context);
            }
            else
            {
                SceneView sceneView = SceneView.lastActiveSceneView;
                Camera camera = sceneView.camera;
                Vector3 viewDir = camera.transform.forward;
                Ray r = new Ray(camera.transform.position, viewDir);
                Vector3 pos =  camera.transform.position;
                if (Physics.Raycast(r, out var hit, 100))
                    pos = hit.point;

                go.transform.position = pos;
            }

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
        
        [MenuItem("GameObject/Foliage/Biome Brush/Square (Rounded)", false, 10)]
        static void NewBrushSquareRounded(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Biome Brush", typeof(SplineContainer), typeof(BiomeBrush));
            
            var container = go.GetComponent<SplineContainer>();
            container.Spline = SplineFactory.CreateRoundedSquare(20, 2.5f);
            
            GameObject context = menuCommand.context as GameObject;
            if (context)
            {
                GameObjectUtility.SetParentAndAlign(go, context);
            }
            else
            {
                SceneView sceneView = SceneView.lastActiveSceneView;
                Camera camera = sceneView.camera;
                Vector3 viewDir = camera.transform.forward;
                Ray r = new Ray(camera.transform.position, viewDir);
                Vector3 pos =  camera.transform.position;
                if (Physics.Raycast(r, out var hit, 100))
                    pos = hit.point;

                go.transform.position = pos;
            }
            
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
    }
}