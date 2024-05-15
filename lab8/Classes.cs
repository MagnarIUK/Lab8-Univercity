using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;

namespace lab8
{
    public class Customer
    {
        private string _login;
        public string Login { get { return _login; } set { _login = value; } }
        private bool _registered;

        private string _password;

        private double _money;
        public double Money { get { return _money; } }

        public Customer(string login, string password)
        {
            _registered = false;
            Login = login;
            _password = hashPassword(password);
        }

        public bool login(string pass)
        {
            var data = Tools.Read<Dictionary<string, Dictionary<string, string>>>("customers_db.json");
            if (data.TryGetValue(_login, out Dictionary<string, string> user))
            {
                if (data[_login]["password"] == hashPassword(pass))
                {
                    return true;
                }
            }
            else
            {
                throw new UserNotFoundException();
            }

            return false;
        }

        public Customer(string login)
        {
            var data = Tools.Read<Dictionary<string, Dictionary<string, string>>>("customers_db.json");
            if (data.TryGetValue(login, out Dictionary<string, string> user))
            {
                _registered = true;
                string password = data[login]["password"];
                Login = login;
                _password = password;
                _money = double.Parse(data[login]["money"]);
            }
            else
            {
                throw new UserNotFoundException();
            }
        }

        

        public void register()
        {
            var data = Tools.Read<Dictionary<string, Dictionary<string, string>>>("customers_db.json");
            if (!_registered)
            {
                if (!data.TryGetValue(_login, out Dictionary<string, string> user))
                {
                    Dictionary<string, string> user_data = new Dictionary<string, string>()
                    {
                        {"password", _password },
                        {"money", "0" }
                    };
                    data.Add(_login, user_data);
                    Tools._Write<Dictionary<string, Dictionary<string, string>>>("customers_db.json", data).Wait();
                }
                else
                {
                    throw new UserFoundException();
                }
            }
            else
            {
                throw new UserFoundException();
            }
        }

        public static string hashPassword(string Pass)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(Pass);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                string hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                return hash;
            }
        }
    }

    public class UserNotFoundException : Exception
    {
        public UserNotFoundException() :base("UserNotFound") { }
    }

    public class UserFoundException : Exception
    {
        public UserFoundException() : base("UserFound") { }
    }

    public class GoodNotFoundException : Exception
    {
        public GoodNotFoundException() : base("GoodNotFound") { }
    }

    public class NotEnoughMoneyException : Exception
    {
        public NotEnoughMoneyException() : base("NotEnoughMoney") { }
    }

    public class Payment
    {
        public class Good
        {
            private string _id;
            public string Id { get { return _id; } set { _id = value; } }
            private string _name;
            public string Name { get { return _name; } set { _name = value; } }

            private double _price;
            public double Price { get { return _price; } set { _price = value; } }

            public Good(string id, string name, double price)
            {
                Id = id;
                Name = name;
                Price = price;
            }

            public string toString()
            {
                return $"{Name}: {Price}";
            }

        }

        

        private List<Good> _goods;
        public List<Good> Goods { get { return _goods; } }

        private Customer _customer;
        public Customer Customer { get { return _customer; } set { _customer = value; } }

        public Payment(Customer customer)
        {
            _customer = customer;
            _goods = new List<Good>();
        }

        public double getTotal()
        {
            double total = 0;
            for (int i = 0; i < _goods.Count(); i++)
            {
                total += _goods[i].Price;
            }
            return total;
        }

        public void buy(Customer c)
        {
            var data = Tools.Read<Dictionary<string, Dictionary<string, string>>>("customers_db.json");
            double total = getTotal();

            if (c.Money >= total)
            {
                data[c.Login]["money"] = (c.Money - total).ToString();
                Tools._Write<Dictionary<string, Dictionary<string, string>>>("customers_db.json", data).Wait();
                _customer = new Customer(c.Login);
            }
            else
            {
                throw new NotEnoughMoneyException();
            }


        }

        public void add_good(string id)
        {

            var data = Tools.Read<Dictionary<string, Dictionary<string, string>>>("db.json");
            if (data.TryGetValue(id, out Dictionary<string, string> user))
            {
                _goods.Add(new Good(id, data[id]["name"], double.Parse(data[id]["price"])));
            }
            else
            {
                throw new GoodNotFoundException();
            }
        }


        public string toString()
        {
            string g = "";
            for (int i = 0; i < _goods.Count(); i++)
            {
                g += $"{_goods[i].toString()}\n";
            }
            return g;
        }
    }


}
