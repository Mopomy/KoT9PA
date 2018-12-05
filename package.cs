namespace RBot
{
    class Commands
    {
        Thread RainbowThread = new Thread(new ParameterizedThreadStart(Recolor));
        static CommandContext cttx;
        static System.Timers.Timer timer = new System.Timers.Timer(interval: 0.1);
        static float prog = 0;
        static float dprog = 0.0255F;
        static DiscordColor defcol;

        [Command("reset")]
        public async Task reset(CommandContext ctx)
        {
            timer.Stop();
            RainbowThread.Abort();
            await ctx.Guild.UpdateRoleAsync(ctx.Member.Roles.FirstOrDefault(), color: defcol);
        }

        [Command("exit")]
        public async Task exit(CommandContext ctx)
        {
            await ctx.Client.DisconnectAsync();
            Environment.Exit(0);
        }

        [Command("rainbow")]
        public async Task rainbow(CommandContext ctx, float ddprog = -1)
        {
            if (ddprog != -1 && ddprog > 0 && ddprog < 0.3)
            {
                dprog = ddprog;
            }
            RainbowThread.Start(ctx);
        }

        public static void Recolor(object e)
        {
            cttx = (CommandContext)e;
            timer.Elapsed += Elapsed;
            timer.Start();
        }

        private static void Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            DiscordColor col = Rainbow(prog);
            prog += dprog;
            cttx.Guild.UpdateRoleAsync(cttx.Member.Roles.FirstOrDefault(), color: col);
        }

        public static DiscordColor Rainbow(float progress)
        {
            float div = (Math.Abs(progress % 1) * 6);
            int ascending = (int)((div % 1) * 255);
            int descending = 255 - ascending;
            DiscordColor col;
            switch ((int)div)
            {
                case 0:
                    col = new DiscordColor(255 / 255, (float)ascending / 255, 0);
                    break;
                case 1:
                    col = new DiscordColor((float)descending / 255, 255 / 255, 0);
                    break;
                case 2:
                    col = new DiscordColor(0, 255 / 255, (float)ascending / 255);
                    break;
                case 3:
                    col = new DiscordColor(0, (float)descending / 255, 255 / 255);
                    break;
                case 4:
                    col = new DiscordColor((float)ascending / 255, 0, 255 / 255);
                    break;
                default:
                    col = new DiscordColor(255 / 255, 0, (float)descending / 255);
                    break;
            }
            return col;
        }

        class Program
        {
            static DiscordClient discord;
            static CommandsNextModule cmd;

            static void Main(string[] args)
            {
                MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
            }

            static async Task MainAsync(string[] args)
            {
                discord = new DiscordClient(new DiscordConfiguration
                {
                    Token = "NDc1MjA5NzgzMTQyNjQ1Nzcw.Dkgf4w.8h5DoOh6jUNHdd2h2MHI2u7ZB-Y",
                    TokenType = TokenType.Bot,
                    UseInternalLogHandler = true,
                    LogLevel = LogLevel.Debug
                });

                cmd = discord.UseCommandsNext(new CommandsNextConfiguration
                {
                    StringPrefix = "+"
                });

                discord.SetWebSocketClient<WebSocket4NetCoreClient>();
                cmd.RegisterCommands<Commands>();

                await discord.ConnectAsync();
                await Task.Delay(-1);
            }
        }
    }
}
    
