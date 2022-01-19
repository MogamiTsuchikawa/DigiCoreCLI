using DigiCoreLib;
class Program
{
    static CancellationTokenSource cancelTokenSource = 
        new CancellationTokenSource();
    public static void Main(string[] args)
    {
        CancellationToken cToken = cancelTokenSource.Token;
        Task.Run(CLILoop, cToken);
        while(!cToken.IsCancellationRequested);
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
        Console.Write("password >");
        string password = Console.ReadLine();
        return await digiCore.Auth.Login(email!, password!);
    }
    
}






