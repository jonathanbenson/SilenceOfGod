
using System.Threading;
using System.Speech.Recognition;

namespace sog.src;

enum ListenerState
{
    NotAcceptingCommands,
    AcceptingCommands,
    Listening
}

public class Listener
{
    const int ErrorMessageWait = 1000;

    private ListenerState state = ListenerState.NotAcceptingCommands;
    private string _Message = "";
    public string Message
    {
        get
        {
            if (state == ListenerState.NotAcceptingCommands)
                return "Say 'start' for the program to start listening to commands";
            else if (state == ListenerState.AcceptingCommands)
                return "Speak a command, or say 'stop' for the program to stop listening to commands";
            else
                return _Message;
        }
    }

    private CommandDispatcher CommandDispatcher;
    private Thread ListenThread;

    public Listener(CommandDispatcher dispatcher)
    {
        CommandDispatcher = dispatcher;
        ListenThread = new Thread(Listen);
    }

    ~Listener()
    {
        TurnOff();
    }

    public void TurnOn()
    {
        ListenThread.Start();
        state = ListenerState.NotAcceptingCommands;
    }

    public void TurnOff()
    {
        ListenThread.Join();
    }

    private void Listen()
    {
        while (true)
        {
            SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine();
            Grammar dictationGrammar = new DictationGrammar();
            recognizer.LoadGrammar(dictationGrammar);
            
            string text = "Invalid command. Say 'help' for a list of available commands";

            try
            {
                recognizer.SetInputToDefaultAudioDevice();
                RecognitionResult result = recognizer.Recognize();

                text = result.Text.ToLower();

                if (state == ListenerState.AcceptingCommands)
                    CommandDispatcher.Dispatch(text);
                else if (state == ListenerState.NotAcceptingCommands && text == "start")
                    state = ListenerState.AcceptingCommands;

            }
            catch (ArgumentException e)
            {
                _Message = $"Invalid command: '{text}'. Say 'help' for a list of available commands";
                Thread.Sleep(ErrorMessageWait);
            }
            catch (InvalidOperationException e)
            {
                _Message = "Could not recognize input from default aduio device. Try changing your microphone and restart the application.";
                Thread.Sleep(ErrorMessageWait);
            }
            finally
            {
                recognizer.UnloadAllGrammars();
            }
        }
    }

}




