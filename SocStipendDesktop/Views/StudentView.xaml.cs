using System.Windows;
using SocStipendDesktop.Models;
using SocStipendDesktop.ViewModels;

namespace SocStipendDesktop.Views
{
    public partial class StudentView : Window
    {
        public StudentView(Student student)
        {
            InitializeComponent();
            DataContext = new StudentViewModel(student);

            var viewModel = (StudentViewModel)DataContext;
            viewModel.ClosingRequest += (sender, e) => Close();
        }
    }
}