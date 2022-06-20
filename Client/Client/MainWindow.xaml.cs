using Nancy.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Client
{
    public class Card
    {
        public string Id { get; set; }
        public string Name { get; set; }
        private string Base64 { get; set; }
        public string Map
        {
            get => Base64;
            set
            {
                byte[] byteBuffer = Convert.FromBase64String(value);
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(byteBuffer);
                bitmapImage.EndInit();
                Image = bitmapImage;
                Base64 = value;
            }
        }
        public BitmapImage Image { get; set; }
    }

    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadList();
        }

        private void LoadList()
        {
            string address = "http://localhost:55067/cards";
            List<Card> list = new List<Card>();
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(address);
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                string json;
                using (StreamReader stream = new StreamReader(resp.GetResponseStream() ?? throw new InvalidOperationException(), Encoding.UTF8))
                {
                    json = stream.ReadToEnd();
                }
                try
                {
                    var serializer = new JavaScriptSerializer();
                    list = serializer.Deserialize<List<Card>>(json);
                }
                catch
                {
                    Label.Visibility = Visibility.Visible;
                    Label.Content = "Deserialize error.";
                }
            }
            catch
            {
                Label.Visibility = Visibility.Visible;
                Label.Content = "HTTP error.";
            }
            DeleteButton.IsEnabled = false;
            UpdateButton.IsEnabled = false;
            ListView.ItemsSource = list;
        }

        private void listView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ListView.SelectedItems.Count == 0)
            {
                DeleteButton.IsEnabled = false;
                UpdateButton.IsEnabled = false;
            }
            else
            {
                DeleteButton.IsEnabled = true;
                UpdateButton.IsEnabled = true;
            }
            if (ListView.SelectedItems.Count > 1)
                UpdateButton.IsEnabled = false;
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            Card card = new Card();
            AddOrUpdate add = new AddOrUpdate(card)
            {
                Owner = this
            };
            if (add.ShowDialog() == true)
            {
                using (var client = new HttpClient())
                {
                    try
                    {
                        Card sCard = new Card
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = add.CardName,
                            Map = add.Map,
                            Image = null
                        };

                        var response = client.PostAsync("http://localhost:55067/cards",
                            new StringContent(
                                new JavaScriptSerializer().Serialize(sCard), Encoding.UTF8, "application/json")).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            Console.Write(@"Update: Success.");
                            LoadList();
                        }
                        else
                            Console.Write(@"Server: Error.");
                    }
                    catch
                    {
                        Console.Write(@"Update: Error");
                    }
                }
            }
        }

        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            Card card = ListView.SelectedItem as Card;
            AddOrUpdate update = new AddOrUpdate(card)
            {
                Owner = this
            };
            if (update.ShowDialog() == true)
            {
                using (var client = new HttpClient())
                {
                    try
                    {
                        if (card == null) return;
                        Card sCard = new Card
                        {
                            Id = card.Id,
                            Name = update.CardName,
                            Map = update.Map,
                            Image = null
                        };

                        var response = client.PutAsync("http://localhost:55067/cards",
                            new StringContent(
                                new JavaScriptSerializer().Serialize(sCard), Encoding.UTF8, "application/json")).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            Console.Write(@"Update: Success.");
                            LoadList();
                        }
                        else
                            Console.Write(@"Server: Error.");
                    }
                    catch
                    {
                        Console.Write(@"Update: Error");
                    }
                }
            }
        }
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            int i = 1;
            foreach (Card card in ListView.SelectedItems)
            {
                using (var client = new HttpClient())
                {
                    var response = client.DeleteAsync("http://localhost:55067/cards/" + card.Id).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        Console.Write(@"Delete: Success");
                    }
                    else
                    {
                        Console.Write(@"Delete: Error.");
                        Console.Write($@"Error at {i} position.");
                    }
                    if (i == ListView.SelectedItems.Count)
                    {
                        LoadList();
                        break;
                    }
                    i++;
                }
            }
        }
    }
}
