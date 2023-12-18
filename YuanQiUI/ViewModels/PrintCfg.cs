using Prism.Mvvm;
using System.ComponentModel;

namespace YuanQiUI.ViewModels
{
    public class PrintCfg : BindableBase
    {
        #region fields
        private double _X = 0;
        private double _Y = 0;
        private double _Width = 20;
        private double _Height = 20;
        private string _FontName = "宋体";
        private string _PrintBody = "";
        private bool _IsBold = false;
        private string _PrintHead = "";
        #endregion

        #region constructor
        public PrintCfg()
        {

        }
        public PrintCfg(double x, double y, double width, double height, bool bold, bool r180, string font, string body, string head = "")
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
            this.IsBold = bold;
            this.FontName = font;
            this.PrintBody = body;
            this.PrintHead = head;
        }
        #endregion

        #region properties
        public bool IsBold { get { return _IsBold; } set { SetProperty(ref _IsBold, value); } }
        public string PrintHead { get { return _PrintHead; } set { SetProperty(ref _PrintHead, value); } }
        public string PrintBody { get { return _PrintBody; } set { SetProperty(ref _PrintBody, value); } }
        public string FontName { get { return _FontName; } set { SetProperty(ref _FontName, value); } }
        public double Height { get { return _Height; } set { SetProperty(ref _Height, value); } }
        public double Width { get { return _Width; } set { SetProperty(ref _Width, value); } }
        public double Y { get { return _Y; } set { SetProperty(ref _Y, value); } }
        public double X { get { return _X; } set { SetProperty(ref _X, value); } }
        #endregion
    }
}
