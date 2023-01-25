using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Xml.Linq;
using Tesseract;
using Spire.Pdf;
using static System.Net.WebRequestMethods;
using Point = System.Windows.Point;

namespace ImageToText
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        TesseractEngine ocrengine = new TesseractEngine(@".\tessdata", "rus+eng", EngineMode.Default);

        /// <summary>
        /// Добавить рамку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            grBorder.Children.Add(new Controll.imageBorder(grBorder, imageControl)) ;
        }
        //Распознать
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            tbResult.Text = "";

            if (!Directory.Exists("Images"))
            {
                Directory.CreateDirectory("Images");
            }

            System.IO.DirectoryInfo di = new DirectoryInfo("Images");

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            int index = 0;
            foreach (Controll.imageBorder item in grBorder.Children)
            {
                Point relativePoint = item.border.TransformToAncestor(grBorder)
                              .Transform(new Point(0, 0));
                var fullBitmap = imageControl.Source as BitmapImage;
                BitmapImage source = new BitmapImage();
                source.BeginInit();
                source.UriSource = fullBitmap.UriSource;
                source.DecodePixelHeight = (int)grBorder.ActualHeight;
                source.DecodePixelWidth = (int)grBorder.ActualWidth;
                source.EndInit();
                var result = new CroppedBitmap(source, new Int32Rect((int)relativePoint.X, (int)relativePoint.Y, (int)item.border.Width, (int)item.border.Height));
                PngBitmapEncoder pngImage = new PngBitmapEncoder();
                pngImage.Frames.Add(BitmapFrame.Create(result));           
                using (Stream fileStream = System.IO.File.Create(@"Images\" + index+".png"))
                {
                    pngImage.Save(fileStream);
                }
                index++;
            }

            foreach (FileInfo file in di.GetFiles())
            {
                var result = GetText(file.FullName);
                tbResult.Text += $"{result}\n";
            }
        }
        /// <summary>
        /// Возвращает строку из куска фото
        /// </summary>
        /// <param name="urlImage"></param>
        /// <returns></returns>
        private string GetText(string urlImage)
        {
            var img = Pix.LoadFromFile(urlImage);
            using (var page = ocrengine.Process(img, PageSegMode.Auto))
            {
                return page.GetText();
            }
        }
        /// <summary>
        /// Выбор файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbFile_MouseUp(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                tbFile.Text = openFileDialog.FileName; 
                imageControl.Source = new BitmapImage(new Uri(openFileDialog.FileName));

            }
        }

        /// <summary>
        /// PDF Convert
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ///Выбираем файл PDF
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Pdf Files|*.pdf";
            if (openFileDialog.ShowDialog() == true)
            {
                PdfDocument pdf = new PdfDocument();
                pdf.LoadFromFile(openFileDialog.FileName);
                for (int i = 0; i < pdf.Pages.Count; i++)
                {
                    var source = pdf.SaveAsImage(i);
                    source.Save($"{Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)}\\PdfConvert{i}.png", System.Drawing.Imaging.ImageFormat.Png);
                }

                MessageBox.Show("Все изображения сохранены на рабочий стол.", "Information");
            }
        }
    }
}
