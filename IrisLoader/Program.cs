﻿using System.IO;
using System.Text.Json;

namespace IrisLoader
{
	public class Program
	{
		public static Loader ActiveLoader { get; private set; }

		static void Main(string[] args)
		{
			string configString = File.ReadAllText("./config.json");
			Config config = JsonSerializer.Deserialize<Config>(configString);

			if (config?.Token == null || config?.MySqlPassword == null) return;

			if (config.UseShardedLoader)
			{
				ActiveLoader = new ShardedLoader(config);
				ActiveLoader.MainAsync().GetAwaiter().GetResult();
			}
			else
			{
				ActiveLoader = new StandartLoader(config);
				ActiveLoader.MainAsync().GetAwaiter().GetResult();
			}
		}
	}
}
