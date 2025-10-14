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
using System.Xml.Serialization;

namespace ToursApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new HotelsPage());
            Manager.MainFrame = MainFrame;

            ImportTours();
        }

        private void ImportTours()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var dataDir = System.IO.Path.Combine(baseDir, "данные");
            var toursFilePath = System.IO.Path.Combine(dataDir, "Туры1.txt");
            var imagesDir = System.IO.Path.Combine(dataDir, "туры фото");

            if (!File.Exists(toursFilePath))
            {
                MessageBox.Show($"Не найден файл с турами: {toursFilePath}\nСоздайте папку 'данные' рядом с exe и поместите туда 'Туры1.txt'.", "Файл не найден", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!Directory.Exists(imagesDir))
            {
                MessageBox.Show($"Не найдена папка с изображениями: {imagesDir}\nСоздайте папку 'данные/туры фото' рядом с exe и добавьте изображения.", "Папка не найдена", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            var fileData = File.ReadAllLines(toursFilePath);
            var images = Directory.Exists(imagesDir) ? Directory.GetFiles(imagesDir) : Array.Empty<string>();

            foreach (var line in fileData)
            {
                var data = line.Split('\t');

                var tempTour = new Tour
                {
                    Name = data[0].Replace("\"", ""),
                    Description = data[1], 
                    TicketsCount = int.Parse(data[2]),
                    Price = decimal.Parse(data[3]),
                    IsActual = (data[4] == "0") ? false : true
                };

                foreach (var tourType in data[5].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var currentType = ToursBaseeEntities.GetContext().Types.ToList().FirstOrDefault(p => p.Name == tourType);
                    if (currentType != null)
                    {
                        tempTour.Types.Add(currentType);
                    }
                }
                try
                {
                    ToursBaseeEntities.GetContext().Tours.Add(tempTour);
                    ToursBaseeEntities.GetContext().SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении тура: {ex.Message}");
                }
            }
        }


        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.GoBack();
        }

        private void MainFrame_ContentRendered(object sender, EventArgs e)
        {
            if (MainFrame.CanGoBack)
            {
                BtnBack.Visibility = Visibility.Visible;
            }
            else
            {
                BtnBack.Visibility = Visibility.Hidden;
            }
        }
    }
}
