using HarmonyLib;
using SFS.Builds;
using SFS.Cameras;
using SFS.Input;
using SFS.UI.ModGUI;
using UnityEngine;

namespace SFSRecorder
{
    public static class Recorder
    {
        public static RecorderBehaviour recorderBehaviour;
        public static RenderTexture renderTexture;
        
        public static void Setup()
        {
            renderTexture = new RenderTexture(7680, 4320, 24);
            var uiholder = Builder.CreateHolder(Builder.SceneToAttach.BaseScene, "RecorderUI");
            var button = Builder.CreateButton(uiholder.transform, 100, 50, 0, 0, SaveFrame, text: "Save Frame");
        }
        
        public static void OnSceneUnload()
        {
            if (recorderBehaviour == null)
                return;
            recorderBehaviour.UnregisterCamera();
            Object.DestroyImmediate(recorderBehaviour);
            recorderBehaviour = null;
        }
        
        public static void OnCameraChange(CameraManager camera)
        {
            if (recorderBehaviour != null)
                recorderBehaviour.UnregisterCamera();
            Object.DestroyImmediate(recorderBehaviour);
            recorderBehaviour = camera.camera.gameObject.AddComponent<RecorderBehaviour>();
            recorderBehaviour.Initialize(camera);
        }
        
        public static void SaveFrame()
        {
            // Save the current frame to a file
            Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture.Apply();
            RenderTexture.active = null;
            byte[] bytes = texture.EncodeToPNG();
            // Destroy the texture
            Object.Destroy(texture);
            // Save the bytes to a file in separate thread
            System.IO.File.WriteAllBytes(FileLocations.BaseFolder.Extend("Pictures").CreateFolder().ExtendToFile("frame.png"), bytes);
        }
    }
    
    public class RecorderBehaviour : MonoBehaviour
    {
        CameraManager currentCamera;
        
        public void UnregisterCamera()
        {
            if (currentCamera == null)
                return;
            currentCamera.camera.targetTexture = null;
            currentCamera.camera.forceIntoRenderTexture = false;
        }
        
        public void Initialize(CameraManager camera)
        {
            currentCamera = camera;
            currentCamera.camera.targetTexture = Recorder.renderTexture;
            currentCamera.camera.forceIntoRenderTexture = true;
            if (PickGridPositioner.main is null)
                return;
        }

        void OnPostRender()
        {
            // Blit the render texture to a screen
            Graphics.Blit(Recorder.renderTexture, null as RenderTexture, new Vector2(4, 1), Vector2.zero);
        }
    }

    public static class RecorderPatches
    {
        public static void Patch()
        {
            Harmony harmony = new Harmony("recorder.patch");
            harmony.PatchAll();
        }
        
        [HarmonyPatch(typeof(PickGridPositioner), "Update")]
        public static class PickGridPositionerUpdatePatch
        {
            public static void Postfix()
            {
                PickGridPositioner.main.worldSpaceCanvas.scaleFactor *= 4;
            }
        }
        
        [HarmonyPatch(typeof(TouchPosition), nameof(TouchPosition.World))]
        public static class TouchPositionWorldPatch
        {
            [HarmonyPrefix]
            public static bool Prefix(ref TouchPosition __instance, ref Vector2 __result, float positionZ)
            {
                Debug.Log("TouchPosition.World");
                float num = 0f - (ActiveCamera.Camera.transform.position.z - positionZ);
                __result = ActiveCamera.Camera.camera.ScreenToWorldPoint((Vector3)(__instance.pixel * 4) + Vector3.forward * num);
                return true;
            }
        }
    }
}