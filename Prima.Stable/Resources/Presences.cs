﻿using Activity = System.Collections.Generic.KeyValuePair<string, Discord.ActivityType>;

namespace Prima.Stable.Resources
{
    public static class Presences
    {
        public static readonly Activity[] List = {
            // Playing
            new("FINAL FANTASY XVI", Discord.ActivityType.Playing),
            new("FINAL FANTASY XV", Discord.ActivityType.Playing),
            new("FINAL FANTASY XIV", Discord.ActivityType.Playing),
            new("FINAL FANTASY XIII", Discord.ActivityType.Playing),
            new("FINAL FANTASY XIII-2", Discord.ActivityType.Playing),
            new("FINAL FANTASY XI", Discord.ActivityType.Playing),
            new("FINAL FANTASY XXVI", Discord.ActivityType.Playing),
            new("PHANTASY STAR ONLINE 2", Discord.ActivityType.Playing),
            new("Fate/Extella", Discord.ActivityType.Playing),
            new("Arknights", Discord.ActivityType.Playing),
            new("Puzzle & Dragons", Discord.ActivityType.Playing),
            new("Granblue Fantasy", Discord.ActivityType.Playing),
            new("ラストイデア", Discord.ActivityType.Playing),
            new("ワールドフリッパー", Discord.ActivityType.Playing),
            new("Temtem", Discord.ActivityType.Playing),
            new("Tetra Master", Discord.ActivityType.Playing),
            new("PlayOnline Launcher", Discord.ActivityType.Playing),
            new("Pokémon Shield", Discord.ActivityType.Playing),
            new("Detroit: Become Human", Discord.ActivityType.Playing),
            new("NieR: Automata", Discord.ActivityType.Playing),
            new("Drakengard 3", Discord.ActivityType.Playing),
            new("Fire Emblem: Three Houses", Discord.ActivityType.Playing),
            new("The Baldesion Arsenal", Discord.ActivityType.Playing),
            new("MONSTER HUNTER: WORLD", Discord.ActivityType.Playing),
            new("Microsoft Visual Studio", Discord.ActivityType.Playing),
            new("League of Legends", Discord.ActivityType.Playing),
            new("Dragalia Lost", Discord.ActivityType.Playing),
            new("Dragalia Found", Discord.ActivityType.Playing),
            new("Pokémon Black 2", Discord.ActivityType.Playing),
            new("Rune Factory 4", Discord.ActivityType.Playing),
            new("Rune Factory 7", Discord.ActivityType.Playing),
            new("Cytus", Discord.ActivityType.Playing),
            new("Cytus 2", Discord.ActivityType.Playing),
            new("Groove Coaster 3", Discord.ActivityType.Playing),
            new("Groove Coaster 4", Discord.ActivityType.Playing),
            new("Groove Coaster 5", Discord.ActivityType.Playing),
            new("Dissidia Final Fantasy", Discord.ActivityType.Playing),
            new("太鼓の達人", Discord.ActivityType.Playing),
            new("Pokémon Tretta", Discord.ActivityType.Playing),
            new("Maimai", Discord.ActivityType.Playing),
            new("Destiny 2", Discord.ActivityType.Playing),
            new("Pokémon Café", Discord.ActivityType.Playing),
            new("NieR: Reincarnation", Discord.ActivityType.Playing),
            new("Cyberpunk 2078", Discord.ActivityType.Playing),
            new("CONTROL", Discord.ActivityType.Playing),
            new("Assassin's Creed: Black Flag", Discord.ActivityType.Playing),
            new("Minceraft", Discord.ActivityType.Playing),
            new("Portal", Discord.ActivityType.Playing),
            new("Portal 2", Discord.ActivityType.Playing),
            new("Half Life 3", Discord.ActivityType.Playing),
            new("Genshin Impact", Discord.ActivityType.Playing),
            new("スクスタ", Discord.ActivityType.Playing),
            new("Apex Legends", Discord.ActivityType.Playing),
            // Listening
            new("Vaporwave Furret 10 Hours", Discord.ActivityType.Listening),
            new("Super Touhou Eurobeat Mix", Discord.ActivityType.Listening),
            // Streaming gets turned into "Playing" if there's no actual stream.
            // Watching
            new("Live Vana'diel", Discord.ActivityType.Watching),
            new("you", Discord.ActivityType.Watching),
        };
    }
}
