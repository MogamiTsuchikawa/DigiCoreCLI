using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DigiCoreCLI
{
    class Program
    {
        static CancellationTokenSource cancelTokenSource =
            new CancellationTokenSource();
        public static void Main(string[] args)
        {
            CancellationToken cToken = cancelTokenSource.Token;
            Task task = CLILoop();
            task.Wait();
        }
        static async Task CLILoop()
        {
            var digiCore = new DigiCore("https://core.digicre.net");
            while (true)
            {
                Console.Write("DigiCoreCLI >");
                string[] cmd = Console.ReadLine().Split(' ');
                switch (cmd[0])
                {
                    case "login":
                        await OnLogin(digiCore);
                        break;
                    case "exit":
                        cancelTokenSource.Cancel();
                        return;
                    case "show":
                        await OnShow(digiCore, cmd);
                        break;
                }
            }
            Console.WriteLine("CLI END");
            cancelTokenSource.Cancel();
            cancelTokenSource.Dispose();
        }
        static async Task<bool> OnShow(DigiCore digiCore, string[] cmd)
        {
            switch (cmd[1])
            {
                case "username":
                    Account account = new(digiCore);
                    var userInfoResponse = await account.Get<Account.UserInfo>(Account.USER_INFO_PATH);
                    if (userInfoResponse.data == null)
                    {
                        Console.WriteLine("GET NULL!");
                        break;
                    }
                    Console.WriteLine(userInfoResponse.data.UserName);
                    break;
                case "ver":
                    Console.WriteLine("version 0.0.1");
                    break;
            }
            return true;
        }

        static async Task<bool> OnLogin(DigiCore digiCore)
        {
            Console.Write("email >");
            string email = Console.ReadLine();
            string password = ReadPassword("password >");
            return await digiCore.Auth.Login(email!, password!);
        }

        public static string? ReadPassword(string prompt)
        {
            Console.Write(prompt);
            var password = new StringBuilder();

            while(true)
            {
                var keyinfo = Console.ReadKey(true);
                switch (keyinfo.Key)
                {
                    case ConsoleKey.Escape:
                        Console.WriteLine();
                        return null;
                    case ConsoleKey.Enter:
                        Console.WriteLine();
                        return password.ToString();
                    case ConsoleKey.Backspace:
                        if (0 < password.Length)
                            password.Length -= 1;
                        else
                            Console.Beep();
                        break;

                    default:
                        if (Char.IsLetter(keyinfo.KeyChar))
                        {
                            if ((keyinfo.Modifiers & ConsoleModifiers.Shift) == 0)
                            {
                                password.Append(keyinfo.KeyChar);
                            }
                            else
                            {
                                if (Console.CapsLock)
                                    password.Append(Char.ToLower(keyinfo.KeyChar));
                                else
                                    password.Append(Char.ToUpper(keyinfo.KeyChar));
                            }
                        }
                        else if (!Char.IsControl(keyinfo.KeyChar))
                        {
                            password.Append(keyinfo.KeyChar);
                        }
                        else
                        {
                            Console.Beep();
                        }
                        break;
                }
            }
        }

    }
    
}

