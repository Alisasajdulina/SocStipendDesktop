using SocStipendDesktop.Services;
using SocStipendDesktop.Views;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace SocStipendDesktop.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly LoginView _loginView;
        private string? _username;
        private string? _password;
        private string? _captchaInput;
        private BitmapImage? _captchaImage;
        private string? _captchaCode;

        public string? Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string? Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string? CaptchaInput
        {
            get => _captchaInput;
            set
            {
                _captchaInput = value;
                OnPropertyChanged(nameof(CaptchaInput));
            }
        }

        public BitmapImage? CaptchaImage
        {
            get => _captchaImage;
            set
            {
                _captchaImage = value;
                OnPropertyChanged(nameof(CaptchaImage));
            }
        }

        public string? CaptchaCode
        {
            get => _captchaCode;
            private set
            {
                _captchaCode = value;
                OnPropertyChanged(nameof(CaptchaCode));
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand RefreshCaptchaCommand { get; }

        public LoginViewModel(LoginView loginView)
        {
            _loginView = loginView;
            LoginCommand = new RelayCommand(Login);
            RefreshCaptchaCommand = new RelayCommand(RefreshCaptcha);
            GenerateNewCaptcha();
        }

        private void GenerateNewCaptcha()
        {
            try
            {
                var (code, image) = CaptchaService.GenerateCaptcha();
                CaptchaCode = code;
                CaptchaImage = image;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка генерации капчи");
            }
        }

      
       private void Login(object? parameter)
        {
            // Удаляем получение пароля из параметра
            if (string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Введите пароль!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка капчи (добавлена проверка на null)
            if (string.IsNullOrWhiteSpace(CaptchaInput) ||
                !CaptchaInput.Equals(CaptchaCode, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Неверная капча!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                GenerateNewCaptcha();
                return;
            }

            // Хеширование пароля
            string hashedPassword = AuthService.ComputeSha256Hash(Password);

            // Сравнение без учета регистра для логина
            if ((Username?.Equals("admin", StringComparison.OrdinalIgnoreCase) ?? false) &&
                hashedPassword == "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918")
            {
                _loginView.DialogResult = true;
            }
            else
            {
                MessageBox.Show("Неверные учетные данные!");
                GenerateNewCaptcha();
            }
        }


        private void RefreshCaptcha(object? parameter)
        {
            GenerateNewCaptcha();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}