using Dalamud.Game.Text;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

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

        //private bool rpAidVisible = false;
        //public bool RpAidVisible { get => rpAidVisible; set => rpAidVisible = value; }

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

        private int chatIndex = 0;

        public PluginUI(Configuration configuration)
        {
            this.configuration = configuration;
        }

        public void Dispose()
        {

        }

        public void Draw()
        {
            DrawSettingsWindow();
            drawNewConfig();
            //DrawRPAidWindow();
        }

        //public void DrawRPAidWindow()
        //{
        //    if (!RpAidVisible)
        //    {
        //        return;
        //    }

        //    ImGui.SetNextWindowSize(new Vector2(500, 400));
        //    if (ImGui.Begin("RP Aid", ref this.rpAidVisible))
        //    {
        //        if (ImGui.InputTextMultiline(string.Empty, ref configuration.RPAidLog, 1024 * 4196, new Vector2(400, 230)))
        //            { }

        //        ImGui.Text($"{configuration.RPAidLog.Length} / 500");

        //        ImGui.Text($"{showSelectedText()}");


        //        if (ImGui.Button("<"))
        //        {
        //            if (chatIndex > 0)
        //            {
        //                chatIndex--;
        //            }
        //        }

        //        ImGui.SameLine();

        //        if (ImGui.Button(">"))
        //        {
        //            if (chatIndex > Math.Ceiling((decimal)(configuration.RPAidLog.Length/500)))
        //            {
        //                chatIndex = (int)Math.Ceiling((decimal)(configuration.RPAidLog.Length / 500));
        //            }
        //            else
        //            {
        //                chatIndex++;
        //            }
        //        }


        //    }

        //}

        public string showSelectedText()
        {
            string text = configuration.RPAidLog.Replace(System.Environment.NewLine, " ");

            return text?[(chatIndex * 500)..Math.Min((500 * (chatIndex + 1)), text.Length)];
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
                    configuration.Save();
                }

                ImGui.SameLine();

                if (ImGui.Button("Copy Log To Clipboard"))
                {
                    string clip = log.printLog("", aClipboard: true);
                    ImGui.SetClipboardText(clip);
                    CopyConfirmMessage = true;
                    LogConfirmMessage = false;
                    configuration.Save();
                }

                // RP Aid button is coming soon
                //ImGui.SameLine();
                //if (ImGui.Button("Text Editor"))
                //{
                //    RpAidVisible = true;
                //}

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

                            ImGui.TableNextRow();

                            {
                                ImGui.TableNextColumn();
                                if (ImGui.Button((configuration.defaultConfig.IsActive ? "@##active_default" : "##active_default"), new Vector2(20, 20)))
                                {
                                    configuration.setActiveConfig(configuration.defaultConfig);

                                }
                            }

                            {
                                ImGui.TableNextColumn();
                                ImGui.Text($"{configuration.defaultConfig.Name}");
                            }

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

                            ImGui.TableNextRow();

                            ImGui.TableNextColumn();

                            if (ImGui.Button("+"))
                            {
                                configuration.addNewConfig("New Config");
                            }

                            ImGui.EndTable();

                        }

                        ImGui.EndTabItem();
                    }

                    ImGui.EndTabBar();
                }

                if (ImGui.Button("Save"))
                {
                    configuration.Save();
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

                ImGui.Text("Config Name:");
                ImGui.SameLine();
                ImGui.InputText("##confname", ref selectedConfig.name, 256);
                ImGui.Spacing();

                if (ImGui.BeginTabBar("##indie tabs"))
                {
                    if (ImGui.BeginTabItem("Chat Config"))
                    {
                        foreach (KeyValuePair<int, string> entry in configuration.PossibleChatTypes)
                        {
                            bool enabled = selectedConfig.TypeConfig[entry.Key];
                            if (ImGui.Checkbox($"{entry.Value}", ref enabled))
                            {
                                selectedConfig.TypeConfig[entry.Key] = enabled;
                                configuration.Save();
                            }

                        }
                        ImGui.EndTabItem();
                    }

                    // Currently unimplemented, name replacements is half working

                    //if (ImGui.BeginTabItem("Name Config"))
                    //{
                    //    nameTab();
                    //    ImGui.EndTabItem();
                    //}
                }
            }

        }

        private void nameTab()
        {
            ImGui.Spacing();

            if (ImGui.BeginTable("configlist", 4, ImGuiTableFlags.BordersInner))
            {

                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.TableNextColumn();
                ImGui.Text($"Name");
                ImGui.TableNextColumn();
                ImGui.Text($"Replacement");
                ImGui.TableNextColumn();

                foreach (KeyValuePair<string, string> entry in selectedConfig.NameReplacements)
                {
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.TableNextColumn();
                    ImGui.Text($"{entry.Key}");
                    ImGui.TableNextColumn();
                    ImGui.Text($"{entry.Value}");
                    ImGui.TableNextColumn();
                    if (ImGui.Button("Remove##" + entry.Key))
                    {
                        selectedConfig.removeNameReplacement(entry.Key);
                    }
                }

                ImGui.TableNextRow();


                ImGui.TableNextColumn();

                if (ImGui.Button("+"))
                {
                    if (!String.IsNullOrWhiteSpace(configuration.tempFirstName) && !String.IsNullOrWhiteSpace(configuration.tempSecondName))
                    {
                        selectedConfig.addNameReplacement(configuration.tempFirstName, configuration.tempSecondName);
                        configuration.tempFirstName = string.Empty;
                        configuration.tempSecondName = string.Empty;
                    }
                    
                }
                ImGui.TableNextColumn();
                ImGui.InputText("##nameone", ref configuration.tempFirstName, 256);
                ImGui.TableNextColumn();
                ImGui.InputText("##nametwo", ref configuration.tempSecondName, 256);
                ImGui.TableNextColumn();

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
                log.setupAutosave();
                log.autoSave();
                configuration.updateAutosaveTime();
                configuration.Save();
            }

            ImGui.Text("Every ");
            ImGui.SameLine();
            ImGui.InputInt("##autosavemin", ref configuration.fAutoMin, 64);
            ImGui.SameLine();
            ImGui.Text(" minutes");

            ImGui.Text("Autosave File Path:");
            ImGui.SameLine();
            ImGui.InputText("##autofilepath", ref configuration.autoFilePath, 256);

            if (ImGui.Checkbox("Autosave notification echoed in chat?", ref configuration.fAutosaveNotif))
            {
                configuration.Save();
            }

        }


    }
}
