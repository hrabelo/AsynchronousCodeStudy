using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsynchronousCodeStudy
{
    class Program
    {
        private static readonly string sentence = "Hello, World. I am studying the behaviour of asynchronous code while dealing with string reversal";
        private static List<Task<string>> tasks = new List<Task<string>>();

        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string result = ProcessStringReversalSync();
            stopwatch.Stop();
            Console.WriteLine($"ProcessStringReversalSync took {stopwatch.ElapsedMilliseconds} ms");

            stopwatch.Restart();
            
            var task = Task<string[]>.Factory.StartNew(() => Map(sentence))
                .ContinueWith(t => Process(t.Result))
                    .ContinueWith(t => Reduce(t.Result));

            stopwatch.Stop();

            Console.Write($"ProcessStringReversalASync took {stopwatch.ElapsedMilliseconds} ms");
        }


        #region Async

        public static string[] Map(string sentence) => sentence.Split(' ');

        public static string[] Process(string[] words)
        {
            for (int i = 0; i < words.Length; i++)
            {
                int index = i;
                Task.Factory.StartNew(() =>
                {
                    words[index] = ReverseString(words[index]);
                    
                },
                TaskCreationOptions.AttachedToParent | TaskCreationOptions.LongRunning);
            }
            return words;
        }

        public static string Reduce(string[] words)
        {
            var stringBuilder = new StringBuilder();
            foreach(var word in words)
            {
                stringBuilder.Append(word);
                stringBuilder.Append(' ');
            }
            return stringBuilder.ToString();
        }

        public static void ProcessStringReversalAsync()
        {
            string[] words = sentence.Split(' ');
            foreach (var word in words)
                tasks.Add(Task<string>.Factory.StartNew(() =>
                    ReverseString(word) + " ",
                    TaskCreationOptions.AttachedToParent | TaskCreationOptions.LongRunning));

        }
        #endregion

        #region Sync
        public static string ProcessStringReversalSync()
        {
            List<string> result = new List<string>();

            string[] words = sentence.Split(' ');
            foreach (var word in words)
                result.Add(ReverseString(word));

            return string.Join(" ", result);
        }

        #endregion

        public static string ReverseString(string word)
        {
            Thread.Sleep(1000);
            var stringBuilder = new StringBuilder(word.Length);
            for (int i = word.Length - 1; i >= 0; i--)
                stringBuilder.Append(word[i]);

            return stringBuilder.ToString();
        }
    }
}
