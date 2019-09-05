using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        DBRepository _repo = new DBRepository();

        public LoginWindow()
        {
            InitializeComponent();
            EmailTextBox.Focus();
        }

        private void LoginButtonLoginWindow_Click(object sender, RoutedEventArgs e)
        {
            if (!_repo.AreThereAnyUsers())
            {
                MessageBox.Show("No any user in system! \nPlease, register");
                return;
            }
            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                MessageBox.Show("Please, enter email!");
                EmailTextBox.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(PasswordTextBox.Password))
            {
                MessageBox.Show("Please, enter password!");
                PasswordTextBox.Focus();
                return;
            }
            if (_repo.AutentificateUser(EmailTextBox.Text, User.GetHash(PasswordTextBox.Password)) == null)
                MessageBox.Show("No such user! \nCheck your credentials or register in the system");
            else
            {
                MainWindow mainWindow = new MainWindow(_repo, _repo.AutentificateUser(EmailTextBox.Text, User.GetHash(PasswordTextBox.Password)));
                mainWindow.Show();
                this.Close();
            }
        }

        private void RegisterButtonLoginWindow_Click(object sender, RoutedEventArgs e) 
        {
            RegistrationWindow registrationWindow = new RegistrationWindow(_repo);
            if (registrationWindow.ShowDialog() == true)
                return;
        }
    }
}
