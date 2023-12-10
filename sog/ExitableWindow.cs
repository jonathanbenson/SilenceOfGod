
using System;
using System.Speech.Recognition;
using System.Threading;
using System.Windows;

namespace sog
{
    public abstract class ExitableWindow : Window
    {
        /*

        This is a base class for windows that can be exited by saying 'exit'

        */

        private SpeechRecognitionEngine Recognizer;

        private Thread ListenerThread;

        public ExitableWindow(SpeechRecognitionEngine recognizer)
        {
            DataContext = this;

            Recognizer = recognizer;

            // Listen in the background
            ListenerThread = new Thread(Listen)
            {
                IsBackground = true
            };
            ListenerThread.Start();
        }

        private void Listen()
        {
            while (true)
            {
                Recognizer = new SpeechRecognitionEngine();
                Grammar dictationGrammar = new DictationGrammar();
                Recognizer.LoadGrammar(dictationGrammar);

                try
                {
                    Recognizer.SetInputToDefaultAudioDevice();
                    RecognitionResult result = Recognizer.Recognize();

                    string text = result.Text.ToLower();

                    // If the user said 'exit', then break out of the loop so the window can be closed
                    if (text == "exit")
                        break;
                }
                catch (Exception)
                {
                    continue;
                }
                finally
                {
                    Recognizer.UnloadAllGrammars();
                }
            }

            // Close the window
            Dispatcher.Invoke(() => Close());
        }
    }
}