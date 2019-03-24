using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Rift.Configuration;

using IonicLib;
using IonicLib.Extensions;

namespace Rift.Services
{
    public class QuizService
    {
        public Boolean IsActive { get; private set; }

        readonly TimeSpan interval = TimeSpan.FromSeconds(10);
        readonly TimeSpan duration = TimeSpan.FromSeconds(20);
        
        readonly List<QuizEntry> entries = new List<QuizEntry>
        {
            new QuizEntry("Глава Демасии", "Джарван"),
            new QuizEntry("Глава Ноксуса", "Свейн"),
            new QuizEntry("Сотый чемпион?", "Джейс"),
            new QuizEntry("Какому персонажу принадлежит скилл \"Тёмный ветер\"?", "Фиддлстикс"),
            new QuizEntry("Какой персонаж носит нечто неразрушимое на спиной?", "Райз"),
        };
        Stack<QuizEntry> questions;

        Timer intervalTimer;
        Timer checkTimer;

        static QuizEntry currentEntry;

        public ConcurrentQueue<QuizGuess> Answers = new ConcurrentQueue<QuizGuess>();

        public async Task StartAsync()
        {
            currentEntry = GetNextEntry();

            await PostQuestion(currentEntry.Question);

            StartTimers();
        }

        QuizEntry GetNextEntry()
        {
            if (questions is null || questions.Count == 0)
            {
                questions = new Stack<QuizEntry>(entries.Shuffle());
            }

            return questions.Pop();
        }

        static async Task PostQuestion(String question)
        {
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var channel))
                return;

            await channel.SendMessageAsync(question);
        }

        void StartTimers()
        {
            IsActive = true;

            intervalTimer = new Timer(async delegate { await TimeIsUpAsync(); }, null, duration, TimeSpan.Zero);
            checkTimer = new Timer(async delegate { await CheckAnswersAsync(); }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }

        void StopTimers()
        {
            IsActive = false;
            //intervalTimer?.Dispose();
            checkTimer?.Dispose();
        }

        async Task TimeIsUpAsync()
        {
            StopTimers();

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var channel))
                return;

            await channel.SendMessageAsync($"Время вышло! Правильный ответ: `{currentEntry.Answer}`");
            
            await NextQuiz();
        }

        async Task CheckAnswersAsync()
        {
            if (!Answers.TryDequeue(out var guess))
                return;

            if (!currentEntry.IsRightAnswer(guess.Guess))
                return;

            StopTimers();

            await DeclareWinnerAsync(guess.UserId);
        }

        async Task DeclareWinnerAsync(UInt64 userId)
        {
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var channel))
                return;

            await channel.SendMessageAsync($"Правильный ответ дал(а): <@{userId.ToString()}> !");

            await NextQuiz();
        }

        async Task NextQuiz()
        {
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var channel))
                return;

            await channel.SendMessageAsync("Следующий вопрос через 10 секунд!");
            
            new Timer(async delegate { await StartAsync(); }, null, interval, TimeSpan.Zero);
        }
    }
    
    public class QuizGuess
    {
        public UInt64 UserId { get; }
        public String Guess { get; }

        public QuizGuess(UInt64 userId, String guess)
        {
            UserId = userId;
            Guess = guess;
        }
    }

    public class QuizEntry
    {
        public String Question { get; }
        public String Answer { get; }

        public QuizEntry(String question, String answer)
        {
            Question = question;
            Answer = answer;
        }

        public Boolean IsRightAnswer(String guess)
            => guess.Equals(Answer, StringComparison.InvariantCultureIgnoreCase);
    }
}
