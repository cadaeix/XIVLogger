using Dalamud.Game.Text;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace XIVLogger
{
    // It is good to have this be disposable in general, in case you ever need it
    // to do any cleanup
    class PluginUI : IDisposable
    {
        private Configuration configuration;

        // this extra bool exists for ImGui, since you can't ref a property
        private bool visible = false;
        public bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
        }

        private bool settingsVisible = false;
        public bool SettingsVisible
        {
            get { return this.settingsVisible; }
            set { this.settingsVisible = value; }
        }

        private bool logConfirmMessage = false;
        public bool LogConfirmMessage
        {
            get { return this.logConfirmMessage; }
            set { this.logConfirmMessage = value; }
        }

        public string latestLogTime = "";

        public ChatLog log;

        public PluginUI(Configuration configuration)
        {
            this.configuration = configuration;
        }

        public void Dispose()
        {
        }

        public void Draw()
        {
            // This is our only draw handler attached to UIBuilder, so it needs to be
            // able to draw any windows we might have open.
            // Each method checks its own visibility/state to ensure it only draws when
            // it actually makes sense.
            // There are other ways to do this, but it is generally best to keep the number of
            // draw delegates as low as possible.

            DrawMainWindow();
            DrawSettingsWindow();
        }

        public void DrawMainWindow()
        {
            if (!Visible)
            {
                return;
            }

            ImGui.SetNextWindowSize(new Vector2(375, 330), ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowSizeConstraints(new Vector2(375, 330), new Vector2(float.MaxValue, float.MaxValue));
            if (ImGui.Begin("My Amazing Window", ref this.visible, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
            {
          
                if (ImGui.Button("Show Settings"))
                {
                    SettingsVisible = true;
                }

                if (ImGui.Button("Print Log"))
                {
                    latestLogTime = log.printLog();
                    LogConfirmMessage = true;
                }

                if (LogConfirmMessage)
                {
                    ImGui.Text($"Log saved at {latestLogTime}");
                }

            }
            ImGui.End();
        }

        public void DrawSettingsWindow()
        {
            if (!SettingsVisible)
            {
                return;
            }

            ImGui.SetNextWindowSize(new Vector2(375, 330));
            if (ImGui.Begin("XIV Logger Configuration", ref this.settingsVisible))
            {
                if (ImGui.Button("Print Log"))
                {
                    latestLogTime = log.printLog();
                    LogConfirmMessage = true;
                }

                if (LogConfirmMessage)
                {
                    ImGui.Text($"Log saved at {latestLogTime}");
                }
                else
                {
                    ImGui.Spacing();
                }

                ImGui.Spacing();

                ImGui.Text("File Path:");
                ImGui.SameLine();
                ImGui.InputText("##filename", ref configuration.fileName, 32);
                ImGui.Text("Default: Documents folder");
                ImGui.Spacing();

                foreach (KeyValuePair<int, string> entry in configuration.PossibleChatTypes)
                {
                    bool enabled = configuration.EnabledChatTypes[entry.Key];
                    if (ImGui.Checkbox($"{entry.Value}", ref enabled))
                    {
                        configuration.EnabledChatTypes[entry.Key] = enabled;
                        this.configuration.Save();
                    }

                }

            }
            ImGui.End();
        }

    }
}
