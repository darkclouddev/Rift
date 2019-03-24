using System.Threading.Tasks;

using Rift.Preconditions;
using Rift.Services;

using Discord.Commands;

namespace Rift.Modules
{
    public class QuizModule : RiftModuleBase
    {
        readonly QuizService quizService;

        public QuizModule(QuizService quizService)
        {
            this.quizService = quizService;
        }

        [Command("quiz")]
        [RequireDeveloper]
        public async Task StartAsync()
        {
            await quizService.StartAsync();
        }
    }
}
