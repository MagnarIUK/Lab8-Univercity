using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace lab8
{
    public class Customer
    {
        private string _login;
        private string Login { get { return _login; } set { _login = value; } }
        private bool _registered;

        private string _password;

        private double _money;
        public double Money { get { return _money; } set { _money = value; } }

        public Customer(string login, string password)
        {
            _registered = false;
            Login = login;
            _password = hashPassword(password);
        }

        public bool login(string pass)
        {
            var data = Tools.Read<Dictionary<string, Dictionary<string, string>>>("customers_db.json");
            if (data[_login] != null)
            {
                if (data[_login]["password"] == pass)
                {
                    return true;
                }
            }

            return false;
        }

        public Customer(string login)
        {
            var data = Tools.Read<Dictionary<string, Dictionary<string, string>>>("customers_db.json");
            if (data[login] != null)
            {
                _registered = true;
                string password = data[login]["password"];
                Login = login;
                _password = password;
                Money = double.Parse(data[login]["money"]);
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
                if (data[_login] == null)
                {
                    Dictionary<string, Dictionary<string, string>> user_data = new Dictionary<string, Dictionary<string, string>>() 
                    {
                        {_login, new Dictionary<string, string>() 
                            {
                                {"password", _password },
                                {"money", "0.0" }
                            } 
                        }
                    };
                   
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


    internal class Payment
    {
        public class Good
        {
            private int _id;
            public int Id { get { return _id; } set { _id = value; } }
            private string _name;
            public string Name { get { return _name; } set { _name = value; } }

            public Good(int id)
            {
                Id = id;
                Name = Tools.Read<Dictionary<string, string>>("db.json")[id.ToString()];
            }

        }

        private List<Good> _goods;
        public List<Good> Goods { get { return _goods; } }





    }


}
