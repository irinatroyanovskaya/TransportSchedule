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
    /// Логика взаимодействия для AddFavouritesWindow.xaml
    /// </summary>
    public partial class AddFavouritesWindow : Window
    {
        DBRepository _repo;
        User _user;
        public AddFavouritesWindow(DBRepository repo, User user)
        {
            InitializeComponent();
            _repo = repo;
            _user = user;
            StationsList.ItemsSource = _repo.Context.Stations.ToList() ;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (StationsList.SelectedItem == null)
            {
                MessageBox.Show("Any station was not chosen!");
                return;
            }
            if (!_repo.IsFavouriteStationUnique(_user, (Station)StationsList.SelectedItem))
            {
                MessageBox.Show("Such station is already in your favourites list!");
                return;
            }
            _repo.AddFavourite(_user, (Station)StationsList.SelectedItem, DescriptionTexBox.Text);
            DialogResult = true;       
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
