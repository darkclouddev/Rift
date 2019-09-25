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
        public bool IsActive { get; private set; }

        readonly TimeSpan intervalBetween = TimeSpan.FromSeconds(10);
        readonly TimeSpan duration = TimeSpan.FromSeconds(20);

        readonly List<QuizEntry> entries = new List<QuizEntry>
        {
            new QuizEntry("Глава Демасии", "Джарван"),
            new QuizEntry("Глава Ноксуса", "Свейн"),
            new QuizEntry("Сотый чемпион?", "Джейс"),
            new QuizEntry("Какому персонажу принадлежит скилл \"Тёмный ветер\"?", "Фиддлстикс"),
            new QuizEntry("Какой персонаж носит нечто неразрушимое за спиной?", "Райз"),
        };

        Stack<QuizEntry> questions;

        Timer durationTimer;
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
            if (questions is null || questions.Count == 0) questions = new Stack<QuizEntry>(entries.Shuffle());

            return questions.Pop();
        }

        static async Task PostQuestion(string question)
        {
            if (!IonicHelper.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Commands, out var channel))
                return;

            await channel.SendMessageAsync(question);
        }

        void StartTimers()
        {
            IsActive = true;

            durationTimer = new Timer(async delegate { await TimeIsUpAsync(); }, null, duration, TimeSpan.Zero);
            checkTimer = new Timer(async delegate { await CheckAnswersAsync(); }, null, TimeSpan.FromSeconds(1),
                                   TimeSpan.FromSeconds(1));
        }

        void StopTimers()
        {
            IsActive = false;
            durationTimer?.Dispose();
            checkTimer?.Dispose();
        }

        async Task TimeIsUpAsync()
        {
            StopTimers();

            if (!IonicHelper.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Commands, out var channel))
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

        async Task DeclareWinnerAsync(ulong userId)
        {
            if (!IonicHelper.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Commands, out var channel))
                return;

            await channel.SendMessageAsync($"Правильный ответ дал(а): <@{userId.ToString()}> !");

            await NextQuiz();
        }

        async Task NextQuiz()
        {
            if (!IonicHelper.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Commands, out var channel))
                return;

            await channel.SendMessageAsync("Следующий вопрос через 10 секунд!");

            new Timer(async delegate { await StartAsync(); }, null, intervalBetween, TimeSpan.Zero);
        }
    }

    public class QuizGuess
    {
        public ulong UserId { get; }
        public string Guess { get; }

        public QuizGuess(ulong userId, string guess)
        {
            UserId = userId;
            Guess = guess;
        }
    }

    public class QuizEntry
    {
        public string Question { get; }
        public string Answer { get; }

        public QuizEntry(string question, string answer)
        {
            Question = question;
            Answer = answer;
        }

        public bool IsRightAnswer(string guess)
        {
            return guess.Equals(Answer, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
