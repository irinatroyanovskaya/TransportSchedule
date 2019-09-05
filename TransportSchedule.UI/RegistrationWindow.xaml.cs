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
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        DBRepository _repo;
        public RegistrationWindow(DBRepository repo)
        {
            InitializeComponent();
            _repo = repo;
            NameTextBox.Focus();
        }

        private void RegisterButtonRegistrationWindow_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                MessageBox.Show("Email is not entered!");
                EmailTextBox.Focus();
            }
            else if (!_repo.IsEmailUnique(EmailTextBox.Text))
            {
                MessageBox.Show("User with such email already exists. \nEmail should be unique!");
                return;
            }
            else if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Full name is not entered!");
                NameTextBox.Focus();
            }
            else if (string.IsNullOrWhiteSpace(PasswordTextBox.Password))
            {
                MessageBox.Show("Password is not entered");
                PasswordTextBox.Focus();
            }
            else
            {
                _repo.CreateNewUser(NameTextBox.Text, EmailTextBox.Text, PasswordTextBox.Password);
                DialogResult = true;
            }   
        }

        private void CancelButtonRegistrationWindow_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
