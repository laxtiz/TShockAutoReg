using System;

namespace PiQiu.AutoReg
{
    public class RandomPasswordSettings
    {
        public int PasswordLength { get; set; } = 4;
        public bool IncludeDigits { get; set; } = true;
        public bool IncludeLowerLetters { get; set; } = false;
        public bool IncludeUpperLetters { get; set; } = false;
        public bool ExcludeSimilarCharacters { get; set; } = true;
    }
}
