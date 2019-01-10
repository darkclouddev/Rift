using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Rift.Services;
using Rift.Preconditions;

using IonicLib.Util;

using Discord;
using Discord.Commands;

namespace Rift.Modules
{
    public class UtilModule : RiftModuleBase
    {
        readonly MessageService messages;

        public UtilModule(MessageService messageService)
        {
            messages = messageService;
        }

        [Command("id")]
        [RequireContext(ContextType.Guild)]
        public async Task Id() =>
            await Context.User.SendEmbedAsync(new EmbedBuilder()
                                              .WithAuthor("Ваш ID")
                                              .WithDescription(Context.User.Id.ToString()));

        [Command("cleanup")]
        [RequireModerator]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        public async Task Clean([Summary("The optional number of messages to delete; defaults to 10")]
                                int count = 10,
                                [Summary("The type of messages to delete - Self, Bot, or All")]
                                DeleteType deleteType = DeleteType.Self,
                                [Summary("The strategy to delete messages - BulkDelete or Manual")]
                                DeleteStrategy deleteStrategy = DeleteStrategy.BulkDelete)
        {
            int index = 0;
            var deleteMessages = new List<IMessage>(count);
            var messages = Context.Channel.GetMessagesAsync();

            await messages.ForEachAsync(async x =>
            {
                IEnumerable<IMessage> delete = null;
                if (deleteType == DeleteType.Self)
                    delete = x.Where(msg => msg.Author.Id == Context.Client.CurrentUser.Id);
                else if (deleteType == DeleteType.Bot)
                    delete = x.Where(msg => msg.Author.IsBot);
                else if (deleteType == DeleteType.All)
                    delete = x;

                foreach (var msg in delete.OrderByDescending(msg => msg.Timestamp))
                {
                    if (index >= count)
                    {
                        await EndClean(deleteMessages, deleteStrategy);
                        return;
                    }

                    deleteMessages.Add(msg);
                    index++;
                }
            });
        }

        internal async Task EndClean(IEnumerable<IMessage> messages, DeleteStrategy strategy)
        {
            if (strategy == DeleteStrategy.BulkDelete)
                await ((ITextChannel) (Context.Channel)).DeleteMessagesAsync(messages);
            else if (strategy == DeleteStrategy.Manual)
            {
                foreach (var msg in messages.Cast<IUserMessage>())
                {
                    await msg.DeleteAsync();
                }
            }
        }
    }

    public enum DeleteType
    {
        Self = 0,
        Bot = 1,
        All = 2
    }

    public enum DeleteStrategy
    {
        BulkDelete = 0,
        Manual = 1,
    }
}
