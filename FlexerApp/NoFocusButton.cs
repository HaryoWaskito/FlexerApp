using System.Windows.Forms;

namespace FlexerApp
{
    public  class NoFocusButton : Button
    {
        public NoFocusButton() : base()
        {
            this.SetStyle(ControlStyles.Selectable, false);
        }
    }
}
