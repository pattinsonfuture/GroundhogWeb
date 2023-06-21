using System;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;

namespace microsoft.mongrate
{
    class Program
    {
        static int Main(string[] args)
        {
            var app = new CommandLineApplication();

            app.Name = "mongrate";
            app.HelpOption("-?|-h|--help");

            app.VersionOption("-v|--version", () =>
            {
                var versionString = Assembly.GetEntryAssembly()?
                                            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                                            .InformationalVersion;
                return $"版本 {versionString}";
            });

            app.Command("migrate", command =>
            {
                command.Description = "執行MongoDB創建資料表功能";
                command.HelpOption("-?|-h|--help");

                command.OnExecute(() =>
                {
                    Console.WriteLine("正在執行MongoDB創建資料表功能...");
                    // 在這裡撰寫你的資料表創建邏輯
                    return 0;
                });
            });

            app.Command("seeder", command =>
            {
                command.Description = "執行MongoDB新增資料到資料表";
                command.HelpOption("-?|-h|--help");

                command.OnExecute(() =>
                {
                    Console.WriteLine("正在執行MongoDB新增資料到資料表...");
                    // 在這裡撰寫你的資料添加邏輯
                    return 0;
                });
            });

            return app.Execute(args);
        }
    }
}
