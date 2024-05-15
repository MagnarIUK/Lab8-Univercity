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
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private Customer _customer;
        private Payment pay;
        public MainPage(Customer customer)
        {
            InitializeComponent();
            _customer = customer;
            pay = new Payment(_customer);
            login_holder.Text = _customer.Login;
            money_holder.Text = _customer.Money.ToString();
        }

        private void add_good_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                pay.add_good(barcode_reader.Text);
                goods.Text = pay.toString();
                total.Text = "Усього: " + pay.getTotal();

            } catch(GoodNotFoundException)
            {
                Tools.CreateErrorBox("Товар не знайдено");
            }
            
        }

        private void signout_button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Authpage());
        }

        private void buy_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Customer old = _customer;
                pay.buy(_customer);
                _customer = new Customer(old.Login);
                pay = new Payment(_customer);
                goods.Text = pay.toString();
                total.Text = "Усього: " + pay.getTotal();
                money_holder.Text = _customer.Money.ToString();
            }
            catch (NotEnoughMoneyException)
            {
                Tools.CreateErrorBox("Недостатньо грошей");

            }

        }
    }
}
