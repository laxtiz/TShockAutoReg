using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PiQiu.AutoReg
{
    public class RandomPassword
    {
        private const string Digits = "0123456789";
        private const string LowerLetters = "abcdefghijklmnopqrstuvwxyz";
        private const string UpperLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private readonly Random _random;

        public int PasswordLength { get; private set; }
        public string Chars { get; private set; }

        public RandomPassword() : this(new RandomPasswordSettings()) { }

        public RandomPassword(RandomPasswordSettings settings)
        {
            _random = new Random();
            Configure(settings);
        }

        public void Configure(RandomPasswordSettings settings)
        {
            PasswordLength = settings.PasswordLength;

            var sb = new StringBuilder();
            if (settings.IncludeDigits) sb.Append(Digits);
            if (settings.IncludeLowerLetters) sb.Append(LowerLetters);
            if (settings.IncludeUpperLetters) sb.Append(UpperLetters);

            IEnumerable<char> chars = sb.ToString();

            if (settings.ExcludeSimilarCharacters)
            {
                if (settings.IncludeDigits && settings.IncludeLowerLetters)
                {
                    chars = chars.Except("0o1l");
                }
                if (settings.IncludeDigits && settings.IncludeUpperLetters)
                {
                    chars = chars.Except("0O1I");
                }
                if (settings.IncludeLowerLetters && settings.IncludeUpperLetters)
                {
                    chars = chars.Except("oOlI");
                }
            }

            Chars = new string(chars.ToArray());
        }

        public string NewPassword()
        {
            return NewPassword(PasswordLength);
        }

        public string NewPassword(int length)
        {
            var pass = new char[length];
            for (var i = 0; i < length; i++)
            {
                pass[i] = Chars[_random.Next(Chars.Length)];
            }

            return new string(pass);
        }
    }
}
