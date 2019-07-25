using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Terraria.Localization;

namespace PiQiu.AutoReg
{
    public class Message
    {
        public string SuccessRegistered { get; set; } =
        "Log in for the first time and have automatically registered for you.";

        public string PasswordTip { get; set; } = "Your initial password is {0}";
        public string LogFormat { get; set; } = "User {0} is automatically registered, the password is {1}.";

        public string PasswordLengthTooShortWarning { get; set; } =
        "The random password length must not be less than TShock specified minimum password length.";

        public string NonCharsWarning { get; set; } =
        "Random password characters must contain at least one of the Digits, LowerLetters, UpperLetters.";

        public string UnavailableCharacterWarning { get; set; } = "Role names can only use letters and digits.";

        #region LoadFromResource

        [JsonIgnore] public static Message Instance { get; set; }

        public static Message Load()
        {
            if (Instance != null) return Instance;

            var culture = Language.ActiveCulture;
            var assembly = Assembly.GetExecutingAssembly();
            try
            {
                var res = $"PiQiu.AutoReg.Localization.{culture.Name}.json";
                using(var stream = assembly.GetManifestResourceStream(res))
                {
                    using(var sr = new StreamReader(stream))
                    {
                        var json = sr.ReadToEnd();
                        Instance = JsonConvert.DeserializeObject<Message>(json);
                        return Instance;
                    }
                }
            }
            catch
            {
                Instance = new Message();
                return Instance;
            }
        }

        #endregion
    }
}
