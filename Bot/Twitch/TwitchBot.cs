﻿using ApuDoingStuff.Commands.DiceGame;
using ApuDoingStuff.Database.Models;
using ApuDoingStuff.Messages;
using ApuDoingStuff.Properties;
using HLE.Emojis;
using HLE.Numbers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Enums;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Models;
using static ApuDoingStuff.Program;
using static HLE.Time.TimeHelper;


namespace ApuDoingStuff.Twitch
{
    public class TwitchBot
    {
        public TwitchClient TwitchClient { get; private set; }

        public ConnectionCredentials ConnectionCredentials { get; private set; }

        public ClientOptions ClientOptions { get; private set; }

        public WebSocketClient WebSocketClient { get; private set; }

        public TcpClient TcpClient { get; private set; }

        private static TwitchBot _apu;

        public DottedNumber CommandCount { get; set; } = 1;

        public static readonly List<Cooldown> Cooldowns = new();
        public static readonly List<MessageCooldown> MessageCooldowns = new();
        public static readonly Dictionary<string, string> DiceTimer = new();
        public static readonly List<FightAccept> FightAccepts = new();
        public const int MaxLenght = 500;




        public string Runtime => ConvertUnixTimeToTimeStamp(_runtime);

        private readonly long _runtime = Now();



        public TwitchBot()
        {
            ConnectionCredentials = new("ApuDoingStuff", Resources.Token);



            ClientOptions = new()
            {
                ClientType = ClientType.Chat,
                ReconnectionPolicy = new(10000, 30000, 1000),
                UseSsl = true
            };
            WebSocketClient = new(ClientOptions);
            TcpClient = new(ClientOptions);
            TwitchClient = new(TcpClient, ClientProtocol.TCP)
            {
                AutoReListenOnException = true
            };
            TwitchClient.Initialize(ConnectionCredentials, Config.GetChannels());
            TwitchClient.OnLog += Client_OnLog;
            TwitchClient.OnConnected += Client_OnConnected;
            TwitchClient.OnJoinedChannel += Client_OnJoinedChannel;
            TwitchClient.OnMessageReceived += Client_OnMessageReceived;
            TwitchClient.OnMessageSent += Client_OnMessageSent;
            TwitchClient.OnWhisperReceived += Client_OnWhisperReceived;
            TwitchClient.OnConnectionError += Client_OnConnectionError;
            TwitchClient.OnError += Client_OnError;
            TwitchClient.OnDisconnected += Client_OnDisconnect;
            TwitchClient.OnReconnected += Client_OnReconnected;

            TwitchClient.Connect();


        }



        public void SetBot()
        {
            _apu = this;
        }

        public void Send(string channel, string message)
        {
            TwitchClient.SendMessage(channel, message);
        }

        public void JoinChannel(string channel)
        {
        }

        #region SystemInfo

        public string GetSystemInfo()
        {
            return $"Uptime: {Runtime} ApuSpin Memory usage: {GetMemoryUsage()}MB ApuSpin";
        }

        public string GetRuntime()
        {
            return $"Uptime: [{Runtime}]";
        }

        private static double GetMemoryUsage()
        {
            return Math.Truncate(Process.GetCurrentProcess().PrivateMemorySize64 / Math.Pow(10, 6) * 100) / 100;
        }

        public string GetChannelInfo()
        {
            BotdbContext database = new();
            return $"The bot is currently in {database.Channels.Max(u => u.Id)} channels";

        }




        #endregion SystemInfo

        #region Bot_On

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            //ConsoleOut($"LOG>{e.Data}");
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            ConsoleOut("BOT>CONNECTED", ConsoleColor.Green);
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            ConsoleOut($"BOT>Joined channel: {e.Channel}", fontColor: ConsoleColor.Green);
            if (e.Channel == "apudoingstuff")
            {
                Send(channel: "ApuDoingStuff", message: "/me MrDestructoid BOT ONLINE");
            }
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            MessageHandler.Handle(_apu, e.ChatMessage);

            ConsoleOut($"#{e.ChatMessage.Channel}>{e.ChatMessage.Username}: {e.ChatMessage.Message}");
        }

        private void Client_OnMessageSent(object sender, OnMessageSentArgs e)
        {
            ConsoleOut($"#{e.SentMessage.Channel} > : {e.SentMessage.Message}", fontColor: ConsoleColor.Green);
        }

        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {

            ConsoleOut($"WHISPER>{e.WhisperMessage.Username}: {e.WhisperMessage.Message}");
        }

        private void Client_OnConnectionError(object sender, OnConnectionErrorArgs e)
        {
            ConsoleOut($"CONNECTION-ERROR>{e.Error.Message}", ConsoleColor.Red);

        }

        private void Client_OnError(object sender, OnErrorEventArgs e)
        {
            ConsoleOut($"ERROR>{e.Exception.Message}", ConsoleColor.Red);

        }

        private void Client_OnDisconnect(object sender, OnDisconnectedEventArgs e)
        {
            ConsoleOut($"BOT>DISCONNECTED", ConsoleColor.Red);

        }

        private void Client_OnReconnected(object sender, OnReconnectedEventArgs e)
        {
            ConsoleOut($"BOT>RECONNECTED", ConsoleColor.Red);
        }

        #endregion Bot_On

        public static void SendDicePing(TwitchBot twitchBot, string channel, string username)
        {
            BotdbContext database = new();

            if (database.Dicegamedbs.FirstOrDefault(d => d.UserName == username).PingMe == true)
            {
                twitchBot.Send(channel, $"/me APU / {Emoji.Bell} @{username} you can roll your next dice!");
            }
        }

        public static void SendBigDicePing(TwitchBot twitchBot, string channel, string username)
        {
            BotdbContext database = new();

            if (database.Dicegamedbs.FirstOrDefault(d => d.UserName == username).PingMe == true)
            {
                twitchBot.Send(channel, $"/me APU / {Emoji.Bell} @{username} you can roll a BIG dice! B)");
            }
        }

        public static void FightTimerExpired(TwitchBot twitchBot, string channel, string opponent, string challenger)
        {
            twitchBot.Send(channel, $"/me APU @{challenger}, your opponent ( @{opponent} ) didn't showed up to the fight :/");
        }
    }
}
