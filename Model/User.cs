using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Model
{
    public class User : INotifyPropertyChanged
    {
        private int _Id { get; set; }
        public int Id
        {
            get { return _Id; }
            set { _Id = value; OnPropertyChanged(); }
        }
        private string _Username { get; set; }
        public string Username
        {
            get { return _Username; }
            set { _Username = value; OnPropertyChanged(); }
        }

        private string _Email { get; set; }
        public string Email
        {
            get { return _Email; }
            set { _Email = value; OnPropertyChanged(); }
        }
        private string _PasswordHash { get; set; }
        public string PasswordHash
        {
            get { return _PasswordHash; }
            set { _PasswordHash = value; OnPropertyChanged(); }
        }
        private string _FirstName { get; set; }
        public string FirstName
        {
            get { return _FirstName; }
            set { _FirstName = value; OnPropertyChanged(); }
        }
        private string _LastName { get; set; }
        public string LastName
        {
            get { return _LastName; }
            set { _LastName = value; OnPropertyChanged(); }
        }
        private bool _isAdmin { get; set; }
        public bool isAdmin
        {
            get { return _isAdmin; }
            set { _isAdmin = value; OnPropertyChanged(); }
        }
        private DateTime _RegistrationDate { get; set; }
        public DateTime RegistrationDate
        {
            get { return _RegistrationDate; }
            set { _RegistrationDate = value; OnPropertyChanged(); }
        }
        private DateTime _LastLogin { get; set; }
        public DateTime LastLogin
        {
            get { return _LastLogin; }
            set { _LastLogin = value; OnPropertyChanged(); }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
