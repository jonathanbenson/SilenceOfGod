using System;
using System.Windows;
using System.ComponentModel;
using sog.src;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace sog;

public partial class MainWindow : Window, INotifyPropertyChanged
{

    private void RouteCommands()
    {
        CommandDispatcher.RouteCommand("exit", HandleExit);
        CommandDispatcher.RouteCommand("help", HandleHelp);
        CommandDispatcher.RouteCommand("contents", HandleContents);
        CommandDispatcher.RouteCommand("search", HandleVoiceSearch);
        CommandDispatcher.RouteCommand("next", HandleNextPage);
        CommandDispatcher.RouteCommand("back", HandleBackPage);
    }

    private void HandleExit(string[] args)
    {
        Close();
    }

    private void HandleHelp(string[] args)
    {
        bool oldDoListen = DoListen;

        DoListen = false;
        HelpWindow window = new HelpWindow(Recognizer);
        window.ShowDialog();
        DoListen = oldDoListen;
    }

    private void HandleContents(string[] args)
    {
        ContentsWindow window = new ContentsWindow(Recognizer, Bible);
        window.ShowDialog();
    }

    private void HandleVoiceSearch(string[] args)
    {
        List<string> bookNames = Bible.books.Select(book => book.book.ToLower().Replace(" ", "")).ToList();

        if (CurrentPageKey is not null)
        {
            List<string> newArgs = args.ToList();

            if (args.Length > 1 && Regex.IsMatch(args[0], @"^\d+$"))
            {
                newArgs = new List<string>
                {
                    args[0] + args[1]
                };

                if (args.Length > 2)
                    for (int i = 2; i < args.Length; i++)
                        newArgs.Add(args[i]);
            }

            if (newArgs.Count == 1)
                CurrentPageKey = new PageKey(bookNames.FindIndex(e => e == newArgs[0]), 0, 0);
            else if (newArgs.Count == 2)
                CurrentPageKey = new PageKey(bookNames.FindIndex(e => e == newArgs[0]), Convert.ToInt32(newArgs[1]) - 1, 0);
            else if (newArgs.Count == 4)
                CurrentPageKey = new PageKey(bookNames.FindIndex(e => e == newArgs[0]), Convert.ToInt32(newArgs[1]) - 1, Convert.ToInt32(newArgs[3]) - 1);
        }

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