using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageToText.Controll
{
    /// <summary>
    /// Логика взаимодействия для imageBorder.xaml
    /// </summary>
    public partial class imageBorder : UserControl
    {
        public imageBorder()
        {
            InitializeComponent();
        }

        public imageBorder(Grid gr, Image image)
        {
            InitializeComponent();
            border.Height = 100;
            border.Width = 200;
            this.image = image;
            this.Width = gr.Width;
            this.Height = gr.Height;
            grImage = gr;
        }
        Grid grImage;
        Image image;
        Point _positionInBlock;
        bool flagResize = false;
        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            
            if (this.IsMouseCaptured && !flagResize)
            {
                var container = VisualTreeHelper.GetParent(this) as UIElement;

                if (container == null)
                    return;

                var mousePosition = e.GetPosition(container);
                this.RenderTransform = new TranslateTransform(mousePosition.X - _positionInBlock.X, mousePosition.Y - _positionInBlock.Y);
            }
        }
        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _positionInBlock = Mouse.GetPosition(this);
            this.CaptureMouse();
        }
        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.ReleaseMouseCapture();
        }
        private void border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            flagResize = true;
        }
        private void border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            flagResize = false;
        }
        private void border_MouseMove(object sender, MouseEventArgs e)
        {
            if (flagResize)
            {
                var mousePosition = e.GetPosition(border);
                border.Width = mousePosition.X+20;
                border.Height = mousePosition.Y+20;
            }
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            grImage.Children.Remove(this);
        }
    }
}
