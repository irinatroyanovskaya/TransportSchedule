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
using System.Windows.Navigation;
using TransportScheduleClasses;

namespace TransportSchedule.UI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DBRepository _repo;
        User _user;

        public MainWindow(DBRepository repo, User user)
        {
            InitializeComponent();
            _repo = repo;
            _user = user;
            UserTextBox.Text = "Current user: \n"+_user.Name;
            SelectLineComboBox.ItemsSource = _repo.ShowOnlyDirectLines(_repo.Context.Lines.ToList());
            SelectStationComboBox.ItemsSource = _repo.Context.Stations.ToList();
            ScheduleDataGrid.ItemsSource = new List<TableMainWindow>();
        }

        private void SelectLineComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectStationComboBox.ItemsSource = _repo.ShowStationsInLine((Line)SelectLineComboBox.SelectedItem);
        }

        private void SelectStationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ScheduleDataGrid.ItemsSource = _repo.CalculateTime((Station)SelectStationComboBox.SelectedItem);
        }

        private void FavouritesButton_Click(object sender, RoutedEventArgs e)
        {
            FavouritesWindow favouritesWindow = new FavouritesWindow(_repo, _user);
            favouritesWindow.StationSelected += ChangeStationSelected;
            if (favouritesWindow.ShowDialog() == true) 
                SelectStationComboBox.ItemsSource = _repo.Context.Stations.ToList();
        }

        public void ChangeStationSelected(int Id)
        {
            SelectStationComboBox.SelectedItem = _repo.Context.Stations.First(st => st.Id == Id);
        }
    }
}
