using Dalamud.Game.Text;
using Dalamud.Plugin;
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

        private bool copyConfirmMessage = false;
        public bool CopyConfirmMessage
        {
            get { return this.copyConfirmMessage; }
            set { this.copyConfirmMessage = value; }
        }

        private bool showIndividualConfig = false;
        public bool ShowIndividualConfig { get => showIndividualConfig; set => showIndividualConfig = value; }
        public ChatConfig SelectedConfig { get => selectedConfig; set => selectedConfig = value; }

        private ChatConfig selectedConfig;


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

            DrawSettingsWindow();
            drawNewConfig();
        }

        public void DrawSettingsWindow()
        {
            if (!SettingsVisible)
            {
                return;
            }

            ImGui.SetNextWindowSize(new Vector2(500, 330));

            if (ImGui.Begin("XIV Logger Configuration", ref this.settingsVisible))
            {

                if (ImGui.Button("Save Log"))
                {
                    latestLogTime = log.printLog("");
                    LogConfirmMessage = true;
                    CopyConfirmMessage = false;
                }

                ImGui.SameLine();

                if (ImGui.Button("Copy Log To Clipboard"))
                {
                    string clip = log.printLog("", aClipboard: true);
                    ImGui.SetClipboardText(clip);
                    CopyConfirmMessage = true;
                    LogConfirmMessage = false;
                }

                if (LogConfirmMessage)
                {
                    ImGui.Text($"Log saved at {latestLogTime}");
                }
                else if (CopyConfirmMessage)
                {
                    ImGui.Text($"Log copied to clipboard");
                }
                else
                {
                    ImGui.Spacing();
                }

                ImGui.Separator();

                if (ImGui.BeginTabBar("##logger tabs"))
                {

                    if (ImGui.BeginTabItem("Config"))
                    {
                        firstTab();
                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem("Chat Types"))
                    {

                        ImGui.Text("Default configuration settings");

                        if (ImGui.BeginTable("configlist", 3, ImGuiTableFlags.BordersInnerH))
                        {

                            foreach (KeyValuePair<int, string> entry in configuration.PossibleChatTypes)
                            {
                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();

                                bool enabled = configuration.defaultConfig.TypeConfig[entry.Key];
                                if (ImGui.Checkbox($"{entry.Value}", ref enabled))
                                {
                                    configuration.defaultConfig.TypeConfig[entry.Key] = enabled;
                                    this.configuration.Save();
                                }

                            }

                            ImGui.EndTable();
                        }

                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem("Advanced Settings"))
                    {

                        ImGui.Text("Set up additional configurations with different combinations of chat types here.");

                        ImGui.Spacing();

                        if (ImGui.BeginTable("configlist", 4, ImGuiTableFlags.BordersInner))
                        {
                            ImGui.TableNextRow();

                            ImGui.TableNextColumn();

                            ImGui.Text("Active?");

                            ImGui.TableNextColumn();

                            ImGui.Text("Configuration Name");

                            ImGui.TableNextColumn();

                            ImGui.Text("Edit");

                            ImGui.TableNextColumn();

                            ImGui.Text("Remove");


                            // list
                            var index = 0;

                            foreach (ChatConfig config in configuration.configList.ToArray())
                            {
                                index++;

                                string id = config.Name + index; 

                                ImGui.TableNextRow();

                                {
                                    ImGui.TableNextColumn();
                                    if (ImGui.Button((config.IsActive ? "@##active_" + id : "##active_" + id), new Vector2(20, 20)))
                                    {
                                        configuration.setActiveConfig(config);

                                    }
                                }

                                {
                                    ImGui.TableNextColumn();
                                    ImGui.Text($"{config.Name}");
                                }

                                if (config != configuration.defaultConfig)
                                {

                                    {
                                        ImGui.TableNextColumn();
                                        if (ImGui.Button("Edit##" + id))
                                        {
                                            selectedConfig = config;
                                            showIndividualConfig = true;
                                        }
                                    }

                                    {
                                        ImGui.TableNextColumn();
                                        if (ImGui.Button("Remove##" + id))
                                        {
                                            configuration.removeConfig(config);
                                            showIndividualConfig = false;
                                        }
                                    }

                                }
                            }

                            ImGui.TableNextRow();

                            ImGui.TableNextColumn();

                            if (ImGui.Button("+"))
                            {
                                configuration.addNewConfig(DateTime.Now.ToString("ss"));
                            }

                            ImGui.EndTable();

                        }

                        ImGui.EndTabItem();
                    }

                    ImGui.EndTabBar();
                }

            }
            ImGui.End();
        }

        private void drawNewConfig()
        {
            if (!showIndividualConfig)
            {
                return;
            }

            ImGui.SetNextWindowSize(new Vector2(500, 330));

            if (ImGui.Begin("Edit Individual Config", ref this.showIndividualConfig))
            {
                if (ImGui.Button("Set as Active Chat Configuration"))
                {
                    configuration.setActiveConfig(selectedConfig);
                }

                foreach (KeyValuePair<int, string> entry in configuration.PossibleChatTypes)
                {
                    bool enabled = selectedConfig.TypeConfig[entry.Key];
                    if (ImGui.Checkbox($"{entry.Value}", ref enabled))
                    {
                        selectedConfig.TypeConfig[entry.Key] = enabled;
                        configuration.Save();
                    }

                }
            }

        }

        private void nameTab()
        {
            ImGui.Spacing();

            if (ImGui.BeginTable("configlist", 4, ImGuiTableFlags.BordersInner))
                {

                    ImGui.TableNextRow();
        

                    foreach (KeyValuePair<int, string> entry in selectedConfig.nameReplacements)
                    {
                        ImGui.TableNextColumn();
                        ImGui.Text($"{config.Name}");
                            
                    }

                    ImGui.EndTable();
                }
        }

        private void firstTab()
        {

            ImGui.Spacing();

            ImGui.Text("File Name:");
            ImGui.SameLine();
            ImGui.InputText("##filename", ref configuration.fileName, 256);
            ImGui.Spacing();

            ImGui.Text("File Path:");
            ImGui.SameLine();
            ImGui.InputText("##filepath", ref configuration.filePath, 256);
            ImGui.Text("Default: Documents folder");
            ImGui.Spacing();

            if (ImGui.Checkbox("Include Timestamp", ref configuration.fTimestamp))
            {
                this.configuration.Save();
            }

            if (ImGui.Checkbox("Autosave", ref configuration.fAutosave))
            {
                log.autoSave();
                configuration.updateAutosaveTime();
                configuration.Save();
            }

            ImGui.Text("Every ");
            ImGui.SameLine();
            ImGui.InputFloat("##autosavemin", ref configuration.fAutosaveMin, 64);
            ImGui.SameLine();
            ImGui.Text(" minutes");

            ImGui.Text("Autosave File Path:");

        }


    }
}
