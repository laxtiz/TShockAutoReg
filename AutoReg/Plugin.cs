using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using PiQiu.AutoReg.Extensions;
using Terraria;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;
using Utils = TShockAPI.Utils;

namespace PiQiu.AutoReg
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        public override string Name => "AutoReg";
        public override string Author => "laxtiz";
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        private static string ConfigPath => Path.Combine(TShock.SavePath, "autoreg.json");
        public static Config Config { get; private set; }

        private static string LogPath => Path.Combine(TShock.Config.LogPath, "autoreg.log");
        public static ILog Log { get; private set; }

        public static Message Message { get; private set; }

        private readonly RandomPassword _random;

        private const string DataKey = "TempPassword";

        public Plugin(Main game) : base(game)
        {
            if (Message == null)
                Message = Message.Load();

            _random = new RandomPassword();
        }

        public override void Initialize()
        {
            if (Log == null)
                Log = new TextLog(LogPath, false);

            GeneralHooks.ReloadEvent += OnReload;

            SetupConfig();
            if (!Config.Enabled)
                return;

            ServerApi.Hooks.ServerJoin.Register(this, OnServerJoin);
            ServerApi.Hooks.NetGreetPlayer.Register(this, OnGreetPlayer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.ServerJoin.Deregister(this, OnServerJoin);
                ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnGreetPlayer);
                GeneralHooks.ReloadEvent -= OnReload;
                Log.Dispose();
            }

            base.Dispose(disposing);
        }

        private void SetupConfig()
        {
            Config = Config.Load(ConfigPath);
            var settings = Config.Settings;

            if (settings.PasswordLength < TShock.Config.MinimumPasswordLength)
            {
                settings.PasswordLength = TShock.Config.MinimumPasswordLength;
                Log.ConsoleError(Message.PasswordLengthTooShortWarning);
            }

            if (!(settings.IncludeDigits || settings.IncludeLowerLetters || settings.IncludeUpperLetters))
            {
                settings.IncludeDigits = true;
                Log.ConsoleError(Message.NonCharsWarning);
            }

            _random.Configure(settings);
            Config.Save(ConfigPath);
        }

        private void OnReload(ReloadEventArgs args)
        {
            ServerApi.Hooks.ServerJoin.Deregister(this, OnServerJoin);
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnGreetPlayer);

            SetupConfig();
            if (!Config.Enabled)
                return;

            ServerApi.Hooks.ServerJoin.Register(this, OnServerJoin);
            ServerApi.Hooks.NetGreetPlayer.Register(this, OnGreetPlayer);
        }

        private static void OnGreetPlayer(GreetPlayerEventArgs args)
        {
            var player = TShock.Players[args.Who];
            var password = player.GetData<string>(DataKey);

            if (string.IsNullOrEmpty(password))
                return;

            player.SendSuccessMessage(Message.SuccessRegistered);
            player.SendInfoMessage(Message.PasswordTip, password.ColorTag(Color.Pink));
            player.RemoveData(DataKey);
        }

        private void OnServerJoin(JoinEventArgs args)
        {
            var player = TShock.Players[args.Who];
            var user = TShock.Users.GetUserByName(player.Name);

            if (user != null)
                return;

            if (Config.NameOnlyLetterOrDigit && !player.Name.All(c => char.IsLower(c) || char.IsUpper(c) || char.IsDigit(c)))
            {
                TShock.Utils.ForceKick(player, Message.UnavailableCharacterWarning);
                return;
            }

            user = new User
            {
                Name = player.Name,
                UUID = player.UUID,
                Group = TShock.Config.DefaultRegistrationGroupName
            };
            var password = _random.NewPassword();
            user.CreateBCryptHash(password);
            TShock.Users.AddUser(user);

            player.SetData(DataKey, password);
            Log.ConsoleInfo(Message.LogFormat, user.Name, password);
        }
    }
}
