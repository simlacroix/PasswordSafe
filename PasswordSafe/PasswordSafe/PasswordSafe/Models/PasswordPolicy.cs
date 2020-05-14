using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PasswordSafe.Models
{
    public class PasswordPolicy
    {
        public string Name { get; set; }
        public int Length { get; set; }
        public int numLowerCase { get; set; }
        public int numUpperCase { get; set; }
        public int numDigits { get; set; }
        public int numSymbols { get; set; }

        private List<char> symbols = 
            new List<char>() { '+','-','=','_','@','#','$','%','^','&','*','(',')','{','}',';',':','\'','?','!'};

        private Random random = new Random();

        public PasswordPolicy()
        {
            Name = "Default";
            Length = 15;
            numUpperCase = 4;
            numLowerCase = 4;
            numDigits = 4;
            numSymbols = 3;
        }

        public void AddCustomSymbols(char symbol)
        {
            symbols.Add(symbol);
        }

        // https://www.csharp-console-examples.com/loop/c-shuffle-list/
        public string GenerateRandomPassword() {
            StringBuilder builder = new StringBuilder();

            List<string> password = new List<string>();
            password.AddRange(RandomString(numLowerCase, true));
            password.AddRange(RandomString(numUpperCase, false));
            password.AddRange(RandomNumber());
            password.AddRange(RandomSymbol());

            var shuffledPassword = password.OrderBy(item => random.Next());
            foreach (var item in shuffledPassword)
            {
                builder.Append(item);
            }

            return builder.ToString();
        }

        // http://csharp.net-informations.com/string/random.htm
        public List<string> RandomSymbol() {
            List<string> symbolsCollection = new List<string>();
            for (int i = 0; i < numSymbols; i++)
            {
                char c = symbols[random.Next(0, symbols.Count)];
                symbolsCollection.Add(c.ToString());
            }
            return symbolsCollection;
        }

        public List<string> RandomNumber() {
            List<string> numbers = new List<string>();
            for (int i = 0; i < numDigits; i++) {
                numbers.Add(random.Next(0,10).ToString());
            }
            return numbers;
        }

        // Generate a random string with a given size  
        // https://www.c-sharpcorner.com/article/generating-random-number-and-string-in-C-Sharp/
        public List<string> RandomString(int size, bool lowerCase)
        {
            List<string> letters = new List<string>();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                string letter = ch.ToString();
                if (lowerCase)
                    letter = letter.ToLower();
                letters.Add(letter);
            }

            return letters;
        }
    }
}
