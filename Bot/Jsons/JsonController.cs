﻿using ApuDoingStuff.Properties;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ApuDoingStuff.Jsons
{
    public static class JsonController
    {
        public static CommandList CommandList { get; private set; }
        public static List<Rank> Ranks { get; private set; }
        public static void LoadData()
        {
            CommandList = JsonSerializer.Deserialize<CommandList>(File.ReadAllText(Resources.CommandsJsonPath));
            Ranks = JsonSerializer.Deserialize<List<Rank>>(File.ReadAllText(Resources.RanksJsonPath));
        }

    }
}
