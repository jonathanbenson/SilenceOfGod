
using System.Speech.Recognition;

namespace sog
{
    public partial class HelpWindow : ExitableWindow
    {
        public HelpWindow(SpeechRecognitionEngine recognizer) : base(recognizer)
            => InitializeComponent();
    }
}