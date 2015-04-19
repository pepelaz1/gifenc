using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.IO;

namespace gifenc
{
    public class SourceImage
    {
        private Stream _src;
        public Stream Src
        {
            get { return _src; }
            set { _src = value; }
        }

        private String _filename;
        public String Filename
        {
            get { return _filename; }
            set { _filename = value; }
        }

        private WriteableBitmap _wb;
        public WriteableBitmap Wb
        {
            get { return _wb; }
            set { _wb = value; }
        }
    }
}
