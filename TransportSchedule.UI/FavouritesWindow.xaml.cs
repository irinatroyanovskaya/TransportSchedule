using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using TransportScheduleClasses;

namespace TransportSchedule.UI
{
    /// <summary>
    /// Логика взаимодействия для FavouritesWindow.xaml
    /// </summary>
    public partial class FavouritesWindow : Window
    {
        DBRepository _repo;
        User _user;
        public event Action<int> StationSelected;

        public FavouritesWindow(DBRepository repo, User user)
        {
            InitializeComponent();
            _repo = repo;
            _user = user;
            ListBoxFavourites.ItemsSource = _repo.Context.Favourites.Where(f=>f.User.Id ==_user.Id).ToList();
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxFavourites.SelectedItem != null)
            {
                StationSelected?.Invoke(((Favourite)ListBoxFavourites.SelectedItem).Station.Id);
                DialogResult = true;
            }
            else
                MessageBox.Show("No station selected!");
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddFavouritesWindow addFavouritesWindow = new AddFavouritesWindow(_repo, _user);
            if (addFavouritesWindow.ShowDialog()==true)
                UpdateFavouritesList();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxFavourites.SelectedItem==null)
            {
                MessageBox.Show("No selected station to delete!");
                return;
            }
                _repo.DeleteFavourite(_user, _user.FavouriteStations.First(f => f.Station.Id == ((Favourite)ListBoxFavourites.SelectedItem).Station.Id));
                UpdateFavouritesList();
        }

        public void UpdateFavouritesList()
        {
            ListBoxFavourites.ItemsSource = null;
            ListBoxFavourites.ItemsSource = _repo.Context.Favourites.Where(f => f.User.Id == _user.Id).ToList();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxFavourites.SelectedItem==null)
            {
                MessageBox.Show("No selected station to edit!");
                return;
            }
            EditFavouritesWindow editFavouritesWindow = new EditFavouritesWindow(_repo, (Favourite)ListBoxFavourites.SelectedItem);
            if (editFavouritesWindow.ShowDialog() == true)
                UpdateFavouritesList();
        }

        private void ListBoxFavourites_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((Favourite)ListBoxFavourites.SelectedItem)?.Description == null)
            {
                DescriptionTextBlock.Text = "";
                return;
            }
            DescriptionTextBlock.Text = ((Favourite)ListBoxFavourites.SelectedItem).Description;
        }
    }
}