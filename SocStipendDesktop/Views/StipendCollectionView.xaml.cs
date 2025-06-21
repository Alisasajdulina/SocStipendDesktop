using System.Windows;
using SocStipendDesktop.ViewModels;

namespace SocStipendDesktop.Views
{
    public partial class StipendCollectionView : Window
    {
        public StipendCollectionView()
        {
            InitializeComponent();
            DataContext = new StipendCollectionViewModel();
            Closed += (sender, e) => Application.Current.Shutdown();
        }
    }

}