
using System.Security.Principal;
using System.Windows;

namespace sog.src;

public class CommandDispatcher
{
    private ICollection<CommandExecuter> CommandExecuters = new List<CommandExecuter>();

    public void Dispatch(string text)
    {
        string[] words = text.Split();

        if (words.Length == 0)
            throw new ArgumentException("Invalid command");

        foreach (CommandExecuter commandExecuter in CommandExecuters)
        {
            if (commandExecuter.Name == words[0])
            {
                if (words.Length == 1)
                    commandExecuter.Execute(new string[] {});
                else
                    commandExecuter.Execute(words.Skip(1).ToArray());
                
                return;
            }
        }

        throw new ArgumentException("Invalid command");
    }

    public void RouteCommand(string commandName, Action<string[]> uiCallback)
    {
        CommandExecuters.Add(new CommandExecuter(commandName, uiCallback));
    }
}

class CommandExecuter
{
    public string Name { get; }

    protected Action<string[]> UiCallback { get; }

    public CommandExecuter(string name, Action<string[]> uiCallback)
    {
        Name = name;
        UiCallback = uiCallback;
    }

    public void Execute(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
            foreach (Tuple<string, string> replacement in ArgReplacements)
                args[i] = args[i].Replace(replacement.Item1, replacement.Item2);

        UiCallback(args);
    }

    private static IEnumerable<Tuple<string, string>> ArgReplacements = new List<Tuple<string, string>>
    {
        new Tuple<string, string>("first", "1"),
        new Tuple<string, string>("second", "2"),
        new Tuple<string, string>("third", "3"),
        new Tuple<string, string>("one", "1"),
        new Tuple<string, string>("two", "2"),
        new Tuple<string, string>("three", "3"),
        new Tuple<string, string>("four", "4"),
        new Tuple<string, string>("five", "5"),
        new Tuple<string, string>("six", "6"),
        new Tuple<string, string>("seven", "7"),
        new Tuple<string, string>("eight", "8"),
        new Tuple<string, string>("nine", "9"),
        new Tuple<string, string>("ten", "10"),
    };

}
