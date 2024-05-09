using System;
using SFS.IO;
using SFS.UI.ModGUI;
using SFS.Variables;
using TMPro;
using UITools;
using UnityEngine;
using Button = SFS.UI.ModGUI.Button;
using Type = SFS.UI.ModGUI.Type;

namespace SFSRecorder
{
    public class Config : ModSettings<Config.ConfigData>
    {
        public static Config main;

        public static void Setup()
        {
            main = new Config();
            main.Initialize();
        }
        
        public class ConfigData
        {
            public Vector2_Local resolution = new () { Value = new Vector2(1920, 1080) };
            public String_Local saveFolder = new () { Value = "./Videos" };
        }

        protected override void RegisterOnVariableChange(Action onChange)
        {
            settings.resolution.OnChange += onChange;
            settings.saveFolder.OnChange += onChange;
        }

        protected override FilePath SettingsFile => Main.main.ModFolder.ExtendToFile("config.txt");
    }

    public static class ConfigGUI
    {
        public static void Setup()
        {
            ConfigurationMenu.Add(null, new (string, Func<Transform, GameObject>)[] { ("Recorder", CreateConfigGUI) });
        }

        static GameObject CreateConfigGUI(Transform parent)
        {
            Vector2Int size = ConfigurationMenu.ContentSize;

            Box box = Builder.CreateBox(parent, size.x, size.y);
            box.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperCenter, padding: new RectOffset(5, 5, 5, 5));
            
            Builder.CreateLabel(box, size.x - 50, 40, text: "Recording Settings");
            
            Box resolutionBox = Builder.CreateBox(box, size.x - 20, 130);
            resolutionBox.CreateLayoutGroup(Type.Vertical, padding: new RectOffset(5, 5, 5, 5), spacing: 5, disableChildSizeControl: true);
            
            Builder.CreateLabel(resolutionBox, size.x - 60, 30, text: "Resolution").TextAlignment = TextAlignmentOptions.BaselineLeft;
            
            Container resolutionContainer = Builder.CreateContainer(resolutionBox);
            resolutionContainer.CreateLayoutGroup(Type.Horizontal, spacing: 10);
            
            Button previousResolutionButton = Builder.CreateButton(resolutionContainer, 40, 40, text: "<");
            TextInput resolutionInput = Builder.CreateTextInput(resolutionContainer, size.x - 150, 50);
            resolutionInput.field.readOnly = true;
            Button nextResolutionButton = Builder.CreateButton(resolutionContainer, 40, 40, text: ">");
            
            return box.gameObject;
        }
    }
}