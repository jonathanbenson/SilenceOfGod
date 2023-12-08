
using System.Security.Principal;
using System.Windows;

namespace sog.src;

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
        UiCallback(args);
    }

}

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
