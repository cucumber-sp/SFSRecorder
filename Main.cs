using System.Collections.Generic;
using ModLoader;
using ModLoader.Helpers;
using SFS.Cameras;
using SFS.IO;
using SFS.World;
using UnityEngine;

namespace SFSRecorder
{
    public class Main : Mod
    {

        public static Main main;
        public Main() => main = this;
        
        public override string ModNameID => nameof(SFSRecorder);
        public override string DisplayName => "SFS Recorder";
        public override string Author => "Cucumber Space";
        public override string MinimumGameVersionNecessary => "1.5.10";
        public override string ModVersion => "1.0.0";
        public override string Description => "Records the game to a video file";

        public override Dictionary<string, string> Dependencies => new()
        {
            { "UITools", "1.0" }
        };
        
        public new FolderPath ModFolder => new (base.ModFolder);

        public override void Early_Load()
        {
            Recorder.Setup();
            RecorderPatches.Patch();
            SceneHelper.OnSceneLoaded += () => { ActiveCamera.main.activeCamera.OnChange += Recorder.OnCameraChange; };
            SceneHelper.OnSceneUnloaded += Recorder.OnSceneUnload;
        }

        public override void Load()
        {
            Config.Setup();
            ConfigGUI.Setup();
        }
    }
}