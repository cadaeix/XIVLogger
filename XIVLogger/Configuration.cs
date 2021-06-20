using Dalamud.Configuration;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.IO;

namespace XIVLogger
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public Dictionary<int, Boolean> EnabledChatTypes;

        public Dictionary<int, string> PossibleChatTypes;

        [NonSerialized]
        private DalamudPluginInterface pluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;

            PossibleChatTypes = new Dictionary<int, string>
                {
                    { (int) XivChatType.Say, "Say"},
                    { (int) XivChatType.Shout, "Shout" },
                    { (int) XivChatType.Yell, "Yell" },
                    { (int) XivChatType.Party, "Party" },
                    { (int) XivChatType.CrossParty, "Cross World Party" },
                    { (int) XivChatType.Alliance, "Alliance" },
                    { (int) XivChatType.TellIncoming, "Tell Incoming" },
                    { (int) XivChatType.TellOutgoing, "Tell Outgoing" },
                    { (int) XivChatType.CustomEmote, "Custom Emotes" },
                    { (int) XivChatType.StandardEmote, "Standard Emotes" },
                    { (int) XivChatType.CrossLinkShell1, "Cross Link Shell 1" },
                    { (int) XivChatType.CrossLinkShell2, "Cross Link Shell 2" },
                    { (int) XivChatType.CrossLinkShell3, "Cross Link Shell 3" },
                    { (int) XivChatType.CrossLinkShell4, "Cross Link Shell 4" },
                    { (int) XivChatType.CrossLinkShell5, "Cross Link Shell 5" },
                    { (int) XivChatType.CrossLinkShell6, "Cross Link Shell 6" },
                    { (int) XivChatType.CrossLinkShell7, "Cross Link Shell 7" },
                    { (int) XivChatType.CrossLinkShell8, "Cross Link Shell 8" },
                    { (int) XivChatType.Ls1, "Linkshell 1" },
                    { (int) XivChatType.Ls2, "Linkshell 2" },
                    { (int) XivChatType.Ls3, "Linkshell 3" },
                    { (int) XivChatType.Ls4, "Linkshell 4" },
                    { (int) XivChatType.Ls5, "Linkshell 5" },
                    { (int) XivChatType.Ls6, "Linkshell 6" },
                    { (int) XivChatType.Ls7, "Linkshell 7" },
                    { (int) XivChatType.Ls8, "Linkshell 8" },
                    { (int) XivChatType.PvPTeam, "PVP Team" },
                    { (int) XivChatType.NoviceNetwork, "Novice Network" },
                    { (int) XivChatType.FreeCompany, "Free Company" }
                };

            EnabledChatTypes = new Dictionary<int, bool>
                {
                    { (int) XivChatType.Say, true },
                    { (int) XivChatType.Shout, true },
                    { (int) XivChatType.Yell, true },
                    { (int) XivChatType.Party, true },
                    { (int) XivChatType.CrossParty, true },
                    { (int) XivChatType.Alliance, true },
                    { (int) XivChatType.TellIncoming, true },
                    { (int) XivChatType.TellOutgoing, true },
                    { (int) XivChatType.CustomEmote, true },
                    { (int) XivChatType.StandardEmote, true },
                    { (int) XivChatType.CrossLinkShell1, false },
                    { (int) XivChatType.CrossLinkShell2, false },
                    { (int) XivChatType.CrossLinkShell3, false },
                    { (int) XivChatType.CrossLinkShell4, false },
                    { (int) XivChatType.CrossLinkShell5, false },
                    { (int) XivChatType.CrossLinkShell6, false },
                    { (int) XivChatType.CrossLinkShell7, false },
                    { (int) XivChatType.CrossLinkShell8, false },
                    { (int) XivChatType.Ls1, false },
                    { (int) XivChatType.Ls2, false },
                    { (int) XivChatType.Ls3, false },
                    { (int) XivChatType.Ls4, false },
                    { (int) XivChatType.Ls5, false },
                    { (int) XivChatType.Ls6, false },
                    { (int) XivChatType.Ls7, false },
                    { (int) XivChatType.Ls8, false },
                    { (int) XivChatType.PvPTeam, false },
                    { (int) XivChatType.NoviceNetwork, false },
                    { (int) XivChatType.FreeCompany, false },
                };

        }

        public void Save()
        {
            this.pluginInterface.SavePluginConfig(this);
        }
    }


    public class ChatLog
    {
        private List<ChatMessage> log;
        private Dictionary<int, bool> chatConfig;

        public List<ChatMessage> Log { get => log; }
        public Dictionary<int, bool> ChatConfig { get => chatConfig; set => chatConfig = value; }

        public ChatLog(Dictionary<int, bool> chatConfig)
        {
            log = new List<ChatMessage>();
            ChatConfig = chatConfig;
        }

        public void addMessage(XivChatType type, string sender, string message)
        {
            Log.Add(new ChatMessage(type, sender, message));
        }

        private string getTimeStamp()
        {
            return DateTime.Now.ToString("dd-MM-yyyy_hh.mm.ss");
        }

        public string printLog()
        {
            string name = getTimeStamp();

            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + name + ".txt";

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true))
            {
                file.WriteLine(name + "\n");

                foreach (ChatMessage message in Log)
                {
                    if (ChatConfig.ContainsKey((int)message.Type) && ChatConfig[(int)message.Type])
                    {
                        string text = "";

                        switch (message.Type)
                        {
                            case XivChatType.CustomEmote:
                                text = message.Sender + message.Message;
                                break;
                            case XivChatType.StandardEmote:
                                text = message.Message;
                                break;
                            case XivChatType.TellIncoming:
                                text = message.Sender + " >> " + message.Message;
                                break;
                            case XivChatType.TellOutgoing:
                                text = ">>" + message.Sender + " : " + message.Message;
                                break;
                            case XivChatType.FreeCompany:
                                text = "[FC]" + message.Sender + ": " + message.Message;
                                break;
                            case XivChatType.NoviceNetwork:
                                text = "[NN]" + message.Sender + ": " + message.Message;
                                break;
                            case XivChatType.CrossLinkShell1:
                                text = "[CWLS1]" + message.Sender + ": " + message.Message;
                                break;
                            case XivChatType.CrossLinkShell2:
                                text = "[CWLS2]" + message.Sender + ": " + message.Message;
                                break;
                            case XivChatType.CrossLinkShell3:
                                text = "[CWLS3]" + message.Sender + ": " + message.Message;
                                break;
                            case XivChatType.CrossLinkShell4:
                                text = "[CWLS4]" + message.Sender + ": " + message.Message;
                                break;
                            case XivChatType.CrossLinkShell5:
                                text = "[CWLS5]" + message.Sender + ": " + message.Message;
                                break;
                            case XivChatType.CrossLinkShell6:
                                text = "[CWLS6]" + message.Sender + ": " + message.Message;
                                break;
                            case XivChatType.CrossLinkShell7:
                                text = "[CWLS7]" + message.Sender + ": " + message.Message;
                                break;
                            case XivChatType.CrossLinkShell8:
                                text = "[CWLS8]" + message.Sender + ": " + message.Message;
                                break;
                            case XivChatType.Ls1:
                                text = "[LS1]" + message.Sender + ": " + message.Message;
                                break;
                            case XivChatType.Ls2:
                                text = "[LS2]" + message.Sender + ": " + message.Message;
                                break;
                            case XivChatType.Ls3:
                                text = "[LS3]" + message.Sender + ": " + message.Message;
                                break;
                            case XivChatType.Ls4:
                                text = "[LS4]" + message.Sender + ": " + message.Message;
                                break;
                            case XivChatType.Ls5:
                                text = "[LS5]" + message.Sender + ": " + message.Message;
                                break;
                            case XivChatType.Ls6:
                                text = "[LS6]" + message.Sender + ": " + message.Message;
                                break;
                            case XivChatType.Ls7:
                                text = "[LS7]" + message.Sender + ": " + message.Message;
                                break;
                            case XivChatType.Ls8:
                                text = "[LS8]" + message.Sender + ": " + message.Message;
                                break;
                            case XivChatType.PvPTeam:
                                text = "[PvP]" + message.Sender + ": " + message.Message;
                                break;
                            default:
                                text = message.Sender + ": " + message.Message;
                                break;
                        }

                        file.WriteLine(text);
                    }
                }
            }

            return name;
        }

    }

    public class ChatMessage
    {
        private XivChatType type;
        private string message;
        private string sender;

        public string Message { get => message; set => message = value; }
        public XivChatType Type { get => type; set => type = value; }
        public string Sender { get => sender; set => sender = value; }

        public ChatMessage(XivChatType type, string sender, string message)
        {
            this.Sender = sender;
            this.Type = type;
            this.Message = message;
        }

    }


}



