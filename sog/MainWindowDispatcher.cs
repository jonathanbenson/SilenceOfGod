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
        CommandDispatcher.RouteCommand("search", HandleSearch);
        CommandDispatcher.RouteCommand("next", HandleNextPage);
        CommandDispatcher.RouteCommand("last", HandleLastPage);
    }

    private void HandleExit(string[] args)
    {
        Close();
    }

    private void HandleHelp(string[] args)
    {
        MessageBox.Show("help is on the way");
    }

    private void HandleSearch(string[] args)
    {
        if (CurrentPageKey is not null)
            CurrentPageKey = new PageKey(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]), Convert.ToInt32(args[2]));

        LoadPage(CurrentPageKey);
    }

    private void HandleNextPage(string[] args)
    {
        if (CurrentPageKey is not null && PageKeyIndexLookup[CurrentPageKey.ToString()] < PageKeys.Count - 1)
            CurrentPageKey = PageKeys[PageKeyIndexLookup[CurrentPageKey.ToString()] + 1];

        LoadPage(CurrentPageKey);
    }

    private void HandleLastPage(string[] args)
    {
        if (CurrentPageKey is not null && PageKeyIndexLookup[CurrentPageKey.ToString()] > 0)
            CurrentPageKey = PageKeys[PageKeyIndexLookup[CurrentPageKey.ToString()] - 1];

        LoadPage(CurrentPageKey);
    }

}