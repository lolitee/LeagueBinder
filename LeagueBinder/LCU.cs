﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Diagnostics;

/// <summary>
///  3rd party libraries
/// </summary>
using LCUSharp; // https://github.com/bryanhitc/lcu-sharp
using LCUSharp.Websocket;

using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LeagueBinder
{

    public class LCU
    {

        public event EventHandler<LeagueEvent> ChampionSelected;

        /// <summary>
        /// Gets the current status/version of the client 
        /// </summary>
        public static Boolean Status { get; set; }
        public static String PatchVersion { get; set; }

        //current champion
        public static String CurrentChampion { get; set; }

        public static LeagueClientApi League;

        HttpClient Client = new HttpClient();
        DDragon DDragon = new DDragon();

        public async void Init()
        {
            var League = await LeagueClientApi.ConnectAsync();
            //Connected!
            Console.WriteLine("Connected to League Client!");
            Status = true;

            League.Disconnected += async (e, a) =>
            {
                Console.WriteLine("Disconnected! Attempting to reconnect...");
                await League.ReconnectAsync();
                Console.WriteLine("Reconnected!");
            };

            GetVersion(League);

            //Saves into PatchVersion
            PatchVersion = GetVersion(League);
            Console.WriteLine(PatchVersion);

            ChampionSelected += OnChampionSelected;
            League.EventHandler.Subscribe("/lol-champ-select/v1/current-champion", ChampionSelected);

        }

        private void OnChampionSelected(object sender, LeagueEvent e)
        {
            // /lol-patch/v1/game-version

            if (e.Data.ToString() == "0") return;

            Console.WriteLine(e.Data.ToString());
            
            var RAWDATACHAMPIONS = JObject.Parse(DDragon.GetChampions());
            foreach (JProperty item in RAWDATACHAMPIONS["data"])
            {
                Console.WriteLine(item);
            }
        }

        public string GetVersion(LeagueClientApi League)
        {
            //system/v1/builds
            var json = League.RequestHandler.GetJsonResponseAsync(HttpMethod.Get, "system/v1/builds");
            return json.Result;
        }

        public LCU()
        {
            Status = false;
            Init();
        }

    }
}
