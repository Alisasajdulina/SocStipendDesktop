using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;
using SocStipendDesktop.Models;
using SocStipendDesktop.Services;
using System.Linq;

namespace SocStipendDesktop.ViewModels
{
    public class RefViewModel : INotifyPropertyChanged
    {
        public RefViewModel(Stipend stipend)
        {
            CurrentStipend = stipend;
            SaveChangesClickCommand = new RelayCommand(SaveChangesClick);
        }

        private void SaveChangesClick(object? parameter)
        {
            if (CurrentStipend.DtAssign == null)
            {
                MessageBox.Show("Укажите дату назначения стипендии!");
                return;
            }

            try
            {
                if (CurrentStipend.Id == 0)
                {
                    App.Context.Stipend.Add(CurrentStipend);
                }
                else
                {
                    App.Context.Entry(CurrentStipend).State = EntityState.Modified;
                }

                App.Context.SaveChanges();
                OnClosingRequest();
                MessageBox.Show("Справка сохранена!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        // Свойства
        public Stipend CurrentStipend { get; set; }

        // Команды
        public ICommand SaveChangesClickCommand { get; }

        // Закрытие окна
        public event EventHandler ClosingRequest;
        protected void OnClosingRequest() => ClosingRequest?.Invoke(this, EventArgs.Empty);

        public event PropertyChangedEventHandler PropertyChanged;
        private bool CheckDuplicateStipend()
        {
            return App.Context.Stipend
                .Any(s => s.StudentId == CurrentStipend.StudentId &&
                         s.Id != CurrentStipend.Id &&
                         s.DtRef == CurrentStipend.DtRef &&
                         s.DtAssign == CurrentStipend.DtAssign);
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}