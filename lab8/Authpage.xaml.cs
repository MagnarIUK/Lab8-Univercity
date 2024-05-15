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
using System.Windows.Shapes;

namespace lab8
{
    /// <summary>
    /// Interaction logic for Authpage.xaml
    /// </summary>
    public partial class Authpage : Page
    {

        public Authpage()
        {
            InitializeComponent();
        }

        private void login_button_Click(object sender, RoutedEventArgs e)
        {

            string pas;
            if(password.Visibility == Visibility.Visible)
            {
                pas = password.Password;
            }
            else
            {
                pas = password_show.Text;
            }

            if (login.Text.Length > 0 && pas.Length > 0)
            {
                try
                {
                    Customer c = new Customer(login.Text);
                    if (c.login(pas))
                    {
                        this.NavigationService.Navigate(new MainPage(c));
                    }
                    else
                    {
                        Tools.CreateErrorBox("Невірний пароль");
                    }
                }
                catch (UserNotFoundException)
                {
                    Tools.CreateErrorBox("Користувача не знайдено");
                }

            }
            else
            {
                Tools.CreateErrorBox("Заповніть всі поля");
            }
        }

        private void sighnup_button_Click(object sender, RoutedEventArgs e)
        {
            string pas;
            if (password.Visibility == Visibility.Visible)
            {
                pas = password.Password;
            }
            else
            {
                pas = password_show.Text;
            }

            if (login.Text.Length > 0 && password.Password.Length > 0)
            {
                Customer c = new Customer(login.Text, pas);

                
                
                try
                {
                    c.register();
                    if (c.login(pas))
                    {
                        this.NavigationService.Navigate(new MainPage(c));
                    } 
                } catch (UserFoundException)
                {
                    Tools.CreateErrorBox("Користувач з таким логіном вже існує");
                }
            }


        }
        private void ShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            password.Visibility = Visibility.Hidden;
            password_show.Visibility = Visibility.Visible;
            password_show.Text = password.Password;
        }

        private void ShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            password.Visibility = Visibility.Visible;
            password_show.Visibility = Visibility.Hidden;
            password.Password = password_show.Text;
        }


    }
}
