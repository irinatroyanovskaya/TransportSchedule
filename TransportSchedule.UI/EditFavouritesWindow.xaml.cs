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
    /// Логика взаимодействия для EditFavouritesWindow.xaml
    /// </summary>
    public partial class EditFavouritesWindow : Window
    {
        DBRepository _repo;
        Favourite _favourite;

        public EditFavouritesWindow(DBRepository repo, Favourite favourite)
        {
            InitializeComponent();
            _favourite = favourite;
            _repo = repo;
            StationTextBlock.Text ="Station: " + favourite.Station.Name;
            DescriptionTextBlock.Text = favourite.Description;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            _repo.EditFavourite(_favourite, DescriptionTextBlock.Text, _favourite.User);
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
