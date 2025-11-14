using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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

namespace ToursApp
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        private Hotel _currentHotel = new Hotel();
        public AddEditPage(Hotel selectedHotel)
        {
            InitializeComponent();

            if (selectedHotel != null)
                _currentHotel = selectedHotel;

            DataContext = _currentHotel;
            ComboCountries.ItemsSource = ToursBaseeEntities.GetContext().Countries.ToList();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentHotel.Name))
                errors.AppendLine("Укажите название отеля");
            if (_currentHotel.CountOfStars < 1 || _currentHotel.CountOfStars > 5)
                errors.AppendLine("Количество звёзд - число от 1 до 5");
            if (_currentHotel.Country == null)
                errors.AppendLine("Выберите страну");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            try
            {
                var context = ToursBaseeEntities.GetContext();

                // синхронизируем код страны с выбранным объектом
                _currentHotel.CountryCode = _currentHotel.Country?.Code;

                if (_currentHotel.Country != null)
                {
                    var existingCountry = context.Countries.Find(_currentHotel.Country.Code);
                    if (existingCountry != null)
                    {
                        _currentHotel.Country = existingCountry;
                    }
                }

                // Для нового отеля
                if (_currentHotel.id == 0)
                {
                    context.Hotels.Add(_currentHotel);
                }
                // Для существующего отеля
                else
                {
                    // убираем возможный дубликат из локального кеша
                    var trackedHotel = context.Hotels.Local.FirstOrDefault(h => h.id == _currentHotel.id);
                    if (trackedHotel != null && !ReferenceEquals(trackedHotel, _currentHotel))
                    {
                        context.Entry(trackedHotel).State = EntityState.Detached;
                    }

                    var entry = context.Entry(_currentHotel);
                    if (entry.State == EntityState.Detached)
                    {
                        context.Hotels.Attach(_currentHotel);
                        entry = context.Entry(_currentHotel);
                    }

                    entry.State = EntityState.Modified;
                }

                context.SaveChanges();
                MessageBox.Show("Информация сохранена!");
            }
            catch (Exception ex)
            {
                string errorMessage = "Ошибка: " + ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += "\nВнутренняя ошибка: " + ex.InnerException.Message;
                    if (ex.InnerException.InnerException != null)
                    {
                        errorMessage += "\nДетали: " + ex.InnerException.InnerException.Message;
                    }
                }
                MessageBox.Show(errorMessage);
            }
        }

        private void ComboCountries_Selected(object sender, RoutedEventArgs e)
        {

        }
    }
}
