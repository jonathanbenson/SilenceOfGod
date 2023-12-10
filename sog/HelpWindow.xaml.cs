
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Speech.Recognition;
using System.Threading;
using System.Windows;
using sog.src.model;

namespace sog
{
    public partial class HelpWindow : Window
    {

        private SpeechRecognitionEngine Recognizer;
        bool DoExitListenerThread = false;

        private Thread ListenerThread;

        public HelpWindow(SpeechRecognitionEngine recognizer)
        {
            InitializeComponent();

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