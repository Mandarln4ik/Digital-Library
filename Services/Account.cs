using Digital_Library.Model;
using Digital_Library.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Services
{
    class Account : INotifyPropertyChanged
    {
        private static Account Instance;
        private User _currentUser;
        public User currentUser
        {
            get {  return _currentUser; }
            set { _currentUser = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public async Task<(bool, string)> TryLogIn(string login, string passwordHash)
        {
            DBservice db = new DBservice();
            User user = db.GetUserByEmailOrNickname(login);

            if (user != null && user.PasswordHash == passwordHash)
            {
                currentUser = user;
                return (true, " ");
            }
            return (false, "Не удалось автоматически войти в аккаунт");
        }

        public static Account GetInstance()
        {
            if (Instance == null)
            {
                Instance = new Account();
            }
            return Instance;
        }

        public async Task<bool> LogIn(string login, string password)
        {
            DBservice db = new DBservice();
            User user = db.GetUserByEmailOrNickname(login);
            if (user == null)
            {
                return false;
            }
            else
            {
                string hashedPass = GetHash(password);
                if (user.PasswordHash == hashedPass)
                {
                    currentUser = user;
                    Settings.Default.Login = login;
                    Settings.Default.PasswordHash = hashedPass;
                    Settings.Default.IsLoggedOn = true;
                    Settings.Default.Save();
                    return true;
                }
            }
            return false;
        }

        public async Task<(bool, string)> SignIn(string username, string email, string password)
        {
            DBservice db = new DBservice();
            User userByName = db.GetUserByEmailOrNickname(username);
            User userByMail = db.GetUserByEmailOrNickname(username);

            if (userByName != null && userByName.Username == username)
            {
                return (false, "Аккаунт с таким логином уже существует!");
            }
            else if (userByMail != null && userByMail.Email == email)
            {
                return (false, "Аккаунт с таким email уже существует!");
            }

            db.AddNewUser(username,email,GetHash(password));
            return (true, "Успешная регистрация");
        }

        public string GetHash(string value)
        {
            byte[] Bytes = Encoding.UTF8.GetBytes(value);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Bytes);
                string hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                return hashString;
            }
        }

        public async Task SaveProfile(User user)
        {
            user.Id = currentUser.Id;
            DBservice db = new DBservice();
            if (await db.EditUser(user))
            {
                _currentUser.Username = user.Username;
                _currentUser.Email = user.Email;
                _currentUser.FirstName = user.FirstName;
                _currentUser.LastName = user.LastName;

                Settings.Default.Login = user.Username;
                Settings.Default.Save();
            }
        }

        public async Task LogOut()
        {
            if (_currentUser != null)
            {
                _currentUser = null;
            }
            Settings.Default.Reset();
            return;
        }
    }
}
