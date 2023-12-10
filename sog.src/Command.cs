
using System.Security.Principal;
using System.Windows;

namespace sog.src;

public class CommandDispatcher
{
    /*

    A class to route commands and their parameters to callbacks provided by the UI

    */

    private ICollection<CommandExecuter> CommandExecuters = new List<CommandExecuter>();

    public void Dispatch(string text)
    {
        // Split the command text into words
        string[] words = text.Split();

        // The command must have at least one word (the command itself)
        if (words.Length == 0)
            throw new ArgumentException("Invalid command");

        foreach (CommandExecuter commandExecuter in CommandExecuters)
        {
            // Execute the command if there is a CommandExecuter associated with the command name
            if (commandExecuter.Name == words[0])
            {
                // A command with no arguments provided
                if (words.Length == 1)
                    commandExecuter.Execute(new string[] {});
                // A command with arguments
                else
                    commandExecuter.Execute(words.Skip(1).ToArray());
                
                return;
            }
        }

        // At this point, no CommandExecuters have been found to be associated with the provided command name
        // so throw an error
        throw new ArgumentException("Invalid command");
    }

    public void RouteCommand(string commandName, Action<string[]> uiCallback)
    {
        // Associate a command name with a callback provided by the UI
        CommandExecuters.Add(new CommandExecuter(commandName, uiCallback));
    }
}

class CommandExecuter
{
    // The command name. For example, in "search Genesis 1", "search" would be the command name
    public string Name { get; }

    // A callback provided by the UI
    protected Action<string[]> UiCallback { get; }

    public CommandExecuter(string name, Action<string[]> uiCallback)
    {
        Name = name;
        UiCallback = uiCallback;
    }

    public void Execute(string[] args)
    {
        // Replace commonly mis-recognized words by the speech recognizer with the right words
        for (int i = 0; i < args.Length; i++)
            foreach (Tuple<string, string> replacement in ArgReplacements)
                args[i] = args[i].Replace(replacement.Item1, replacement.Item2);

        // Execute the command with the provided arguments
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
