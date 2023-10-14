using System;
using System.Collections.Generic;
using Contract;
using AlexDataAnalyser;
namespace CSharpBasic
{
    class Program
    {
        static void Main(string[] args)
        {
            List<IDataAnalyser> Analysers = new List<IDataAnalyser>();
            Analysers.Add(new AlexAnalyser(@"D:\Dataa"));

            Dictionary<string, int> results;
            foreach (IDataAnalyser analyser in Analysers)
            {
                Console.WriteLine($"Author is { analyser.Author} ");
                results = analyser.GetTopTenStrings(analyser.Path);
                foreach (var kvp in results)
                {
                    Console.WriteLine($"{kvp.Key}: {kvp.Value} occurrences");
                }
            }
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
