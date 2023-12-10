
using System.Speech.Recognition;

namespace sog
{
    public partial class HelpWindow : ExitableWindow
    {
        /*

        A window to display instructions on how to use the application

        */

        public HelpWindow(SpeechRecognitionEngine recognizer) : base(recognizer)
            => InitializeComponent();
    }
}