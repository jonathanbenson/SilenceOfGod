
using System;
using System.Speech.Recognition;
using System.Threading;
using System.Windows;

namespace sog
{
    public abstract class ExitableWindow : Window
    {

        private SpeechRecognitionEngine Recognizer;

        private Thread ListenerThread;

        public ExitableWindow(SpeechRecognitionEngine recognizer)
        {
            DataContext = this;

            Recognizer = recognizer;

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

            Dispatcher.Invoke(() => Close());
        }
    }
}