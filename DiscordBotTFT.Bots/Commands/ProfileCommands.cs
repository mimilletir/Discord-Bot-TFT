﻿using DiscordBotTFT.Core.Services.Profiles;
using DiscordBotTFT.Core.ViewModels;
using DiscordBotTFT.DAL.Models.Profiles;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace DiscordBotTFT.Bots.Commands
{
    public class ProfileCommands : BaseCommandModule
    {
        private readonly IProfileService _profileService;

        public ProfileCommands(IProfileService profileService, IExperienceService experienceService)
        {
            _profileService = profileService;
        }

        [Command("profile")]
        public async Task Profile(CommandContext ctx)
        {
            await GetProfileToDisplayAsync(ctx, ctx.Member.Id);
        }

        [Command("profile")]
        public async Task Profile(CommandContext ctx, DiscordMember member)
        {
            await GetProfileToDisplayAsync(ctx, member.Id);
        }

        private async Task GetProfileToDisplayAsync(CommandContext ctx, ulong memberId)
        {
            Profile profile = await _profileService.GetOrCreateProfileAsync(memberId, ctx.Guild.Id).ConfigureAwait(false);

            DiscordMember member = ctx.Guild.Members[profile.DiscordId];

            var profileEmbed = new DiscordEmbedBuilder
            {
                Title = $"{member.DisplayName}'s Profile",
            }.WithThumbnail(member.AvatarUrl);

            profileEmbed.AddField("Level", profile.Level.ToString());
            profileEmbed.AddField("Xp", profile.Xp.ToString());
            profileEmbed.AddField("Gold", profile.Gold.ToString());
            if (profile.Items.Count > 0)
            {
                profileEmbed.AddField("Items", string.Join(", ", profile.Items.Select(x => x.Item.Name)));
            }

            await ctx.Channel.SendMessageAsync(embed: profileEmbed).ConfigureAwait(false);
        }
    }
}
