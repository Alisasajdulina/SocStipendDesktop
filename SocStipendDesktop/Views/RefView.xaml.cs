using System.Windows;
using SocStipendDesktop.Models;
using SocStipendDesktop.ViewModels;

namespace SocStipendDesktop.Views
{
    public partial class RefView : Window
    {
        public RefView(Stipend stipend)
        {
            InitializeComponent();
            DataContext = new RefViewModel(stipend);

            var viewModel = (RefViewModel)DataContext;
            viewModel.ClosingRequest += (sender, e) => Close();
        }
    }
}
   