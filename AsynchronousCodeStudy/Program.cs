using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            Console.WriteLine($"ProcessStringReversalSync took {stopwatch.ElapsedMilliseconds} ms: {result}");
            stopwatch.Restart();
            Task.Factory.StartNew(() => ProcessStringReversalAsync()).Wait();
            stopwatch.Stop();
            Console.Write($"ProcessStringReversalASync took {stopwatch.ElapsedMilliseconds} ms: ");
            foreach (var t in tasks)
                Console.Write(t.Result);
        }


        public static void ProcessStringReversalAsync()
        {
            string[] words = sentence.Split(' ');
            foreach (var word in words)
                tasks.Add(Task<string>.Factory.StartNew(() =>
                    ReverseString(word) + " ",
                    TaskCreationOptions.AttachedToParent | TaskCreationOptions.LongRunning));

        }

        public static string ProcessStringReversalSync()
        {
            List<string> result = new List<string>();

            string[] words = sentence.Split(' ');
            foreach (var word in words)
                result.Add(ReverseString(word));

            return string.Join(" ", result);
        }

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
