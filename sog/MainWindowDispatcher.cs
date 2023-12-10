using System;
using System.Windows;
using System.ComponentModel;
using sog.src;

namespace sog;

public partial class MainWindow : Window, INotifyPropertyChanged
{

    private void RouteCommands()
    {
        CommandDispatcher.RouteCommand("exit", HandleExit);
        CommandDispatcher.RouteCommand("help", HandleHelp);
        CommandDispatcher.RouteCommand("contents", HandleContents);
        CommandDispatcher.RouteCommand("search", HandleSearch);
        CommandDispatcher.RouteCommand("next", HandleNextPage);
        CommandDispatcher.RouteCommand("back", HandleBackPage);
    }

    private void HandleExit(string[] args)
    {
        Close();
    }

    private void HandleHelp(string[] args)
    {
        HelpWindow window = new HelpWindow();
        window.ShowDialog();
    }

    private void HandleContents(string[] args)
    {
        ContentsWindow window = new ContentsWindow(Bible);
        window.ShowDialog();
    }

    private void HandleSearch(string[] args)
    {
        if (CurrentPageKey is not null)
            CurrentPageKey = new PageKey(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]), Convert.ToInt32(args[2]));

        LoadPage(CurrentPageKey);
    }

    private void HandleNextPage(string[] args)
    {
        int n = 1;

        if (args.Length == 1)
            n = Convert.ToInt32(args[0]);

        if (CurrentPageKey is not null && PageKeyIndexLookup[CurrentPageKey.ToString()] < PageKeys.Count - n)
            CurrentPageKey = PageKeys[PageKeyIndexLookup[CurrentPageKey.ToString()] + n];

        LoadPage(CurrentPageKey);
    }

    private void HandleBackPage(string[] args)
    {
        int n = 1;

        if (args.Length == 1)
            n = Convert.ToInt32(args[0]);

        if (CurrentPageKey is not null && PageKeyIndexLookup[CurrentPageKey.ToString()] >= n)
            CurrentPageKey = PageKeys[PageKeyIndexLookup[CurrentPageKey.ToString()] - n];

        LoadPage(CurrentPageKey);
    }

}