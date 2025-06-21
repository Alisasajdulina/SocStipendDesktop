using SocStipendDesktop.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SocStipendDesktop.Views
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
            DataContext = new LoginViewModel(this);
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                // Передаем пароль напрямую во ViewModel
                viewModel.Password = ((PasswordBox)sender).Password;
            }
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                viewModel.RefreshCaptchaCommand.Execute(null);
            }
        }
    }
}
