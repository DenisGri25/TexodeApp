using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Client
{
    public partial class AddOrUpdate
    {
        public string CardName { get; private set; }
        public string Map { get; private set; }
        private BitmapImage Image { get; set; }

        public AddOrUpdate(Card card)
        {
            InitializeComponent();
            TextBox.Text = card.Name;
            if (card.Image != null)
            {
                Image = card.Image;
                Img.Source = Image;
            }

            else
            {
                Image = new BitmapImage(
                    new Uri("Assets/no_image.png", UriKind.Relative))
                {
                    CreateOptions = BitmapCreateOptions.IgnoreImageCache
                };
                Img.Source = Image;
            }
        }

        private void chooseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "Image files (*.png;*.jpg)|*.png;*.jpg"
            };
            if (openFileDialog.ShowDialog() != true) return;
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(openFileDialog.FileName);
                bitmap.DecodePixelHeight = 200;
                bitmap.EndInit();
                Image = bitmap;
                Img.Source = bitmap;
            }
            catch
            {
                Image = new BitmapImage(
                    new Uri("Assets/no_image.png", UriKind.Relative))
                {
                    CreateOptions = BitmapCreateOptions.IgnoreImageCache
                };
                Img.Source = Image;
                Console.WriteLine(@"Encoding error.");
            }
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TextBox.Text))
            {
                TextBox.BorderBrush = Brushes.Green;
                CardName = TextBox.Text;
                try
                {
                    MemoryStream ms = new MemoryStream();
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(Image));
                    encoder.Save(ms);
                    byte[] bitMapData = ms.ToArray();
                    Map = Convert.ToBase64String(bitMapData);
                }
                catch
                {
                    MessageBox.Show("Encoding error.");
                }

                DialogResult = true;
            }
            else
                TextBox.BorderBrush = Brushes.Red;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}