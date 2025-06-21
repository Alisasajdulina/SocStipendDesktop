using Microsoft.EntityFrameworkCore;
using SocStipendDesktop.Models;
using SocStipendDesktop.Views;
using System;
using System.Windows;

namespace SocStipendDesktop
{
    public partial class App : Application
    {
        public static StipendDbContext Context { get; } = new StipendDbContext();

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                // Применить миграции БД
                Context.Database.Migrate();
            }
            catch
            {
                // Создать БД если не существует
                Context.Database.EnsureCreated();
            }

            base.OnStartup(e);

            var loginView = new LoginView();
            if (loginView.ShowDialog() == true)
            {
                new StipendCollectionView().Show();
            }
            else
            {
                Current.Shutdown();
            }
        }
    }
    
}