using Contract;
using System;
using System.Collections.Generic;

using System.IO;
using System.Linq;

namespace AlexDataAnalyser
{
    public class AlexAnalyser : IDataAnalyser
    {
        /* ----- CONFIGURATION ----- */
        const int LIMIT_ELEMENT = 10;
        const int SIZE_DATA = 49999900;

        public string Author => "Sophia Tran";

        public string Path
        {
            get;
            private set;
        }

        public AlexAnalyser(string path)
        {
            this.Path = path;
        }

        public Dictionary<string, int> GetTopTenStrings(string path)
        {
            // init variables
            Dictionary<long, int> topTenLongs = new Dictionary<long, int>();
            Dictionary<string, int> results = new Dictionary<string, int>();
            long[] hashCodes = new long[SIZE_DATA];
            int indexHashCode = 0;

            try
            {
                // gen HashCode and add to array
                ReadFile(path, hashCodes, ref indexHashCode);

                // sort arr
                Array.Sort(hashCodes);

                // extract hashcodes has top value
                CalculateTopValues(topTenLongs, hashCodes, LIMIT_ELEMENT);
                // convert hashcodes to string
                FindStringByHashCode(path, topTenLongs, results);
                // sort result
                results = results.OrderByDescending(data => data.Value)
                                              .ToDictionary(data => data.Key, data => data.Value);
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
            }

            return results;
        }


        public void ReadFile(string path, long[] hashCodes, ref int indexHashCodes)
        {
            // extract all file in folder data to arr
            string[] files = Directory.GetFiles(path);
            // loop sequence file in files
            foreach (string filename in files)
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                using (StreamReader sd = new StreamReader(fs))
                {
                    string line;
                    string[] strings;
                    while ((line = sd.ReadLine()) != null)
                    {
                        strings = line.Split(';');
                        foreach (string item in strings)
                        {
                            if (item != String.Empty)
                            {
                                // add hashcode to arr
                                hashCodes[indexHashCodes++] = item.ToLower().GetHashCode();
                            }
                        }
                    }
                }
            }
        }

        public void CalculateTopValues(Dictionary<long, int> topValues, long[] hashCodes, int limit)
        {
            int i = 0;
            int value = 1;

            while (i < hashCodes.Length - 1)
            {
                // if element pre = next
                if (hashCodes[i] == hashCodes[i + 1])
                {
                    value++;
                }
                // if e pre != next
                else
                {
                    if (topValues.Count < limit || value > topValues.Values.Min())
                    {
                        if (topValues.ContainsKey(hashCodes[i]))
                        {
                            topValues[hashCodes[i]] += value;
                        }
                        else
                        {
                            topValues.Add(hashCodes[i], value);
                        }

                        if (topValues.Count > limit)
                        {
                            // find key smallest and remove it
                            var keyToRemove = topValues.OrderBy(kvp => kvp.Value).First().Key;
                            topValues.Remove(keyToRemove);
                        }
                    }
                    // reset value
                    value = 1;
                }
                i++;
            }
        }

        public void FindStringByHashCode(string path, Dictionary<long, int> topValues, Dictionary<string, int> topStrings)
        {
            string[] files = Directory.GetFiles(path);
            foreach (string filename in files)
            {

                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                using (StreamReader sd = new StreamReader(fs))
                {
                    string line;
                    string[] strings;
                    while ((line = sd.ReadLine()) != null)
                    {
                        strings = line.Split(';');
                        foreach (string item in strings)
                        {
                            if (item != String.Empty)
                            {
                                long hashCode = item.ToLower().GetHashCode();
                                if (topValues.ContainsKey(hashCode))
                                {
                                    // add string to result and remove from dictionary hashcode
                                    topStrings.Add(item.ToLower(), topValues[hashCode]);
                                    topValues.Remove(hashCode);

                                    if (topValues.Count == 0 && topStrings.Count == LIMIT_ELEMENT)
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

}
