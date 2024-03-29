﻿namespace ApuDoingStuff.Commands.DiceGame
{
    public class FightAccept
    {
        public bool Accepted { get; set; } = false;
        public string Opponent { get; set; }
        public string Channel { get; set; }
        public int Points { get; set; }
        public string Challenger { get; set; }

        public FightAccept(string opponent, string channel, int points, string challenger)
        {
            Opponent = opponent;
            Channel = channel;
            Points = points;
            Challenger = challenger;
        }
    }

}
