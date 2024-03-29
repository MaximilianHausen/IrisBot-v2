﻿using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using IrisLoader.Modules;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AntiPing;

public class AntiPingModule : GlobalIrisModule
{
    public static AntiPingModule Instance { get; private set; }
    public AntiPingModule() => Instance = this;

    public override Task Loaded()
    {
        Connection.Client.MessageCreated += MessageCreated;
        Connection.Client.GuildEmojisUpdated += EmojisEdited;
        Connection.ReminderRecieved += ProcessReminder;
        Connection.RegisterCommands<AntiPingCommands>();
        return Task.CompletedTask;
    }

    public override Task Unloaded()
    {
        Connection.Client.MessageCreated -= MessageCreated;
        Connection.Client.GuildEmojisUpdated -= EmojisEdited;
        Connection.ReminderRecieved -= ProcessReminder;
        Connection.UnregisterCommands<AntiPingCommands>();
        return Task.CompletedTask;
    }

    public override Task Ready()
    {
        Connection.UpdateSettignsFromFile<AntiPingSettingsModel>();
        return Task.CompletedTask;
    }

    public async Task MessageCreated(DiscordClient client, MessageCreateEventArgs args)
    {
        if (args.Guild == null || args.Author.IsBot) return;

        AntiPingSettingsModel settings = Connection.GetSettings<AntiPingSettingsModel>(args.Guild);
        if (HasReplyPing(args.Message) && ((args.Message.Timestamp - args.Message.ReferencedMessage.Timestamp) < new TimeSpan(0, 30, 0)))
        {
            if (settings.AutoReact && settings.ReactionEmoji != null)
            {
                await args.Message.CreateReactionAsync(settings.ReactionEmoji);
            }
            if (settings.PingBack)
            {
                Connection.AddReminder(TimeSpan.FromMinutes(new Random().Next((int)settings.MinPingDelay, (int)settings.MaxPingDelay)), new string[] { args.Guild.Id.ToString(), args.Channel.Id.ToString(), args.Author.Id.ToString() });
            }
        }
    }

    public async Task ProcessReminder(BaseIrisModule module, ReminderEventArgs args)
    {
        DiscordClient client = Connection.Client.GetShard(ulong.Parse(args.Values[0]));
        try
        {
            DiscordChannel channel = await client.GetChannelAsync(ulong.Parse(args.Values[1]));
            DiscordMember member = await channel.Guild.GetMemberAsync(ulong.Parse(args.Values[2]));
            DiscordMessage pingMessage = await channel.SendMessageAsync(member.Mention);
            await pingMessage.DeleteAsync();
        }
        catch (Exception) { }
    }

    public Task EmojisEdited(DiscordClient client, GuildEmojisUpdateEventArgs args)
    {
        AntiPingSettingsModel settings = Connection.GetSettings<AntiPingSettingsModel>(args.Guild);
        if (!DiscordEmoji.IsValidUnicode(settings.ReactionEmoji) && !args.EmojisAfter.ContainsKey(settings.ReactionEmoji.Id))
        {
            settings.AutoReact = false;
            settings.ReactionEmoji = null;
            Connection.SetSettings(args.Guild, settings);
        }
        return Task.CompletedTask;
    }

    private bool HasReplyPing(DiscordMessage message) => message.ReferencedMessage != null && message.MentionedUsers.Contains(message.ReferencedMessage.Author);
}
