using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using NGif;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media.Imaging;
using System.Threading;



namespace gifenc
{
    public partial class MainPage : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        void FirePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        private int _frame_rate;
        public int FrameRate
        {
            get { return _frame_rate; }
            set { _frame_rate = value; }
        }

        private int _width;
        public int OutputWidth
        {
            get { return _width; }
            set { _width = value; }
        }

        private int _height;
        public int OutputHeight
        {
            get { return _height; }
            set { _height = value; }
        }

        private bool _loop;
        public bool Loop
        {
            get { return _loop; }
            set { _loop = value; }
        }

        private String _transparent_color;
        public String TransparentColor
        {
            get { return _transparent_color; }
            set { _transparent_color = value; }
        }

        private int _effect_number;
        public int EffectNumber
        {
            get { return _effect_number; }
            set { _effect_number = value; }
        }

        private int _repeat;
        public int Repeat
        {
            get { return _repeat; }
            set { _repeat = value; }
        }

        private ObservableCollection<SourceImage> _srcimages = new ObservableCollection<SourceImage>();
        public ObservableCollection<SourceImage> SrcImages
        {
            get { return _srcimages; }
         }

        public MainPage()
        {
            InitializeComponent();
            DataContext = this;
            FrameRate = 5;
            OutputWidth = 160;
            OutputHeight = 120;
            Loop = true;
            TransparentColor = "FFFFFF";
            EffectNumber = 1;
            Repeat = 1;
            FirePropertyChanged("OutputWidth");
            FirePropertyChanged("OutputHeight");
            FirePropertyChanged("FrameRate");
            FirePropertyChanged("Loop");
            FirePropertyChanged("TransparentColor");
            FirePropertyChanged("EffectNumber");
            FirePropertyChanged("Repeat");
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "PNG files (*.png)|*.png";
            ofd.FilterIndex = 1;
            ofd.Multiselect = true;

            if ((bool)ofd.ShowDialog())
            {
                foreach(FileInfo fi in ofd.Files)
                {
                    using (Stream stream = fi.OpenRead())
                    {
                        SourceImage si = new SourceImage();
                        si.Src = fi.OpenRead();
                        si.Filename = fi.Name;


                        BitmapImage bitmap = new BitmapImage();
                        bitmap.SetSource(stream);
             
                       // WriteableBitmap wbitmap = new WriteableBitmap(bitmap);
                       // si.Wb = wbitmap.Resize(OutputWidth, OutputHeight, WriteableBitmapExtensions.Interpolation.Bilinear);
                        si.Wb = new WriteableBitmap(bitmap);
                        SrcImages.Add(si);
                    }
                }
                FirePropertyChanged("SrcImages");
            }
        }

        private Color HexColor(String hex)
        {
            //remove the # at the front
            hex = hex.Replace("#", "");

            byte a = 0;
            byte r = 255;
            byte g = 255;
            byte b = 255;

            int start = 0;

            //handle ARGB strings (8 characters long)
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                start = 2;
            }

            //convert RGB characters to bytes
            r = byte.Parse(hex.Substring(start, 2), System.Globalization.NumberStyles.HexNumber);
            g = byte.Parse(hex.Substring(start + 2, 2), System.Globalization.NumberStyles.HexNumber);
            b = byte.Parse(hex.Substring(start + 4, 2), System.Globalization.NumberStyles.HexNumber);

            return Color.FromArgb(a, r, g, b);
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = ".gif";
            sfd.Filter = "GIF files (*.gif)|*.gif";
            sfd.FilterIndex = 1;
            if (sfd.ShowDialog() == true)
            {
                try
                {
                    AnimatedGifEncoder enc = new AnimatedGifEncoder();
                    Stream strm = sfd.OpenFile();
                    enc.Start(strm);
                    Color c= HexColor(TransparentColor);
                    enc.SetTransparent(HexColor(TransparentColor));
                    enc.SetRepeat( Loop ? 0 : 1);
                    enc.SetDelay(1000 / FrameRate);   // 1 frame per sec
                    enc.SetSize(OutputWidth, OutputHeight);
                    enc.SetEffect(EffectNumber);
                    foreach (SourceImage si in SrcImages)
                    {
                        for ( int i = 0; i < Repeat; i++)
                            enc.AddFrame(si.Wb);
                    }                    
                    enc.Finish();

                    byte[] buffer = new byte[strm.Length];
                    strm.Position = 0;
                    strm.Read(buffer, 0, (int)strm.Length);
                    MemoryStream ms = new MemoryStream(buffer, 0, buffer.Length);
               
                    strm.Close();
                    MessageBox.Show("Ok");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        } 
    }
}
