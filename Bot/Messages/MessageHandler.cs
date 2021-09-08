﻿using ApuDoingStuff.Commands;
using ApuDoingStuff.Twitch;
using TwitchLib.Client.Models;

namespace ApuDoingStuff.Messages
{
    public static class MessageHandler
    {
        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage)
        {
            CommandHandler.Handle(twitchBot, chatMessage);
            MessageCommands.Laurin(twitchBot, chatMessage);
            MessageCommands.ApuSquats(twitchBot, chatMessage);
            MessageCommands.Ronic(twitchBot, chatMessage);
            MessageCommands.Bowliy(twitchBot, chatMessage);
            MessageCommands.Jann(twitchBot, chatMessage);
            MessageCommands.HabibiTeaTime(twitchBot, chatMessage);
        }
    }
}
