﻿using DSharpPlus.Entities;
using System.IO;
using System.Text.Json;

namespace IrisLoader.IO
{
	public static class ModuleIO
	{
		/// <param name="relPath"> Has to begin with one slash </param>
		public static T ReadJson<T>(DiscordGuild guild, string moduleName, string relPath)
		{
			string filePath = GetModuleFileDirectory(guild, moduleName).FullName + relPath;
			if (!relPath.EndsWith(".json") || !File.Exists(filePath))
				return default;

			string jsonString = File.ReadAllText(filePath);
			T result = JsonSerializer.Deserialize<T>(jsonString);

			return result;
		}
		/// <param name="relPath"> Has to begin with one slash </param>
		public static void WriteJson<T>(DiscordGuild guild, string moduleName, string relPath, T mapObject)
		{
			string filePath = GetModuleFileDirectory(guild, moduleName).FullName + relPath;
			Directory.CreateDirectory(new FileInfo(filePath).DirectoryName);
			string jsonString = JsonSerializer.Serialize(mapObject);
			File.WriteAllText(filePath, jsonString);
		}

		public static DirectoryInfo GetModuleFileDirectory(DiscordGuild guild, string moduleName)
		{
			DirectoryInfo dir = new DirectoryInfo(GetGuildFileDirectory(guild).FullName + '/' + moduleName);
			Directory.CreateDirectory(dir.FullName);
			return dir;
		}
		public static DirectoryInfo GetGuildFileDirectory(DiscordGuild guild)
		{
			DirectoryInfo dir = new DirectoryInfo("./ModuleFiles/" + guild.Name + '~' + guild.Id);
			Directory.CreateDirectory(dir.FullName);
			return dir;
		}
	}
}