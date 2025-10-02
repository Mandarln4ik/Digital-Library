using Digital_Library.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

namespace Digital_Library
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        bool IsLogin = true;
        
        public Login()
        {
            InitializeComponent();
            ErrorLabel.Visibility = Visibility.Collapsed;
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IsLogin = !IsLogin;
            Tag = IsLogin;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(LoginTextBox.Text) || String.IsNullOrEmpty(PasswordTextBox.Text))
            {
                //Ошибка
                ShowError("Поля для ввода не могут быть пусты");
                return;
            }


            Account acc = Account.GetInstance();
            if (IsLogin)
            {
                bool result = await acc.LogIn(LoginTextBox.Text, PasswordTextBox.Text);
                if (!result)
                {
                    ShowError("Неправильная почта или пароль");
                    return;
                }
                //успешный вход

                DialogResult = true;
                Close();
            }
            else
            {
                if (String.IsNullOrEmpty(EmailTextBox.Text))
                {
                    //Ошибка
                    ShowError("Поля для ввода не могут быть пусты");
                    return;
                }
                else
                {
                    string email = EmailTextBox.Text;
                    string[] str;
                    if (email.Contains('@'))
                    {
                        str = email.Split('@');
                        if (str[1].Contains('.'))
                        {
                            (bool, string) status = await acc.SignIn(LoginTextBox.Text, EmailTextBox.Text, PasswordTextBox.Text);
                            if (status.Item1 != true)
                            {
                                //вывод ошибки
                                ShowError($"{status.Item2}");
                                return;
                            }
                            else
                            {
                                IsLogin = !IsLogin;
                                Tag = IsLogin;
                                bool result = await acc.LogIn(LoginTextBox.Text, PasswordTextBox.Text);

                                if (result != false)
                                {
                                    DialogResult = true;
                                    Close();
                                }
                            }
                        }
                        else
                        {
                            //недействительная почта
                            ShowError("Недействительная почта");
                            return;
                        }
                    }
                    else
                    {
                        //недействительная почта
                        ShowError("Недействительная почта");
                    }

                }
            }
        }

        private void ShowError(string message)
        {
            ErrorLabel.Content = message;
            ErrorLabel.Visibility = Visibility.Visible;
        }
    }
}
