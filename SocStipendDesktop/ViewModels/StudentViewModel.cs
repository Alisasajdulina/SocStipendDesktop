using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using SocStipendDesktop.Models;
using SocStipendDesktop.Services;
using SocStipendDesktop.Views;

namespace SocStipendDesktop.ViewModels
{
    public class StudentViewModel : INotifyPropertyChanged
    {
        public StudentViewModel(Student student)
        {
            CurrentStudent = student ?? throw new ArgumentNullException(nameof(student));

            // Инициализация команд
            SaveChangesClickCommand = new RelayCommand(SaveChangesClick);
            StudentDeleteClickCommand = new RelayCommand(StudentDeleteClick);
            StipendCreateClickCommand = new RelayCommand(StipendCreateClick);
            StipendUpdateClickCommand = new RelayCommand(StipendUpdateClick);
            StipendDeleteClickCommand = new RelayCommand(StipendDeleteClick);

            // Загрузка справок студента
            LoadStipends();
        }

        private void LoadStipends()
        {
            try
            {
                if (CurrentStudent.Id != 0)
                {
                    // Отслеживаем изменения для существующего студента
                    App.Context.Entry(CurrentStudent).State = EntityState.Unchanged;

                    var stipends = App.Context.Stipend
                        .Where(s => s.StudentId == CurrentStudent.Id)
                        .ToList();

                    StipendCollection = new ObservableCollection<Stipend>(stipends);
                    StipendsEnabled = true;
                }
                else
                {
                    StipendCollection = new ObservableCollection<Stipend>();
                    StipendsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки справок: {ex.Message}");
            }
        }

        private void SaveChangesClick(object? parameter)
        {
            if (string.IsNullOrWhiteSpace(CurrentStudent.StudentName))
            {
                MessageBox.Show("Введите ФИО студента!");
                return;
            }

            if (string.IsNullOrWhiteSpace(CurrentStudent.StudentGroup))
            {
                MessageBox.Show("Введите группу студента!");
                return;
            }

            try
            {
                // Явно указываем состояние сущности
                if (CurrentStudent.Id == 0)
                {
                    App.Context.Student.Add(CurrentStudent);
                }
                else
                {
                    App.Context.Entry(CurrentStudent).State = EntityState.Modified;
                }

                App.Context.SaveChanges();
                MessageBox.Show("Данные сохранены!");
                StipendsEnabled = true;
                LoadStipends();
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка сохранения в БД: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void StudentDeleteClick(object? parameter)
        {
            if (MessageBox.Show("Удалить студента и все его справки?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    // Удаляем все справки студента
                    var stipends = App.Context.Stipend
                        .Where(s => s.StudentId == CurrentStudent.Id)
                        .ToList();

                    App.Context.Stipend.RemoveRange(stipends);

                    // Удаляем студента
                    App.Context.Student.Remove(CurrentStudent);
                    App.Context.SaveChanges();

                    // Закрываем окно
                    OnClosingRequest();
                    MessageBox.Show("Студент удален!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}");
                }
            }
        }

        private void StipendCreateClick(object? parameter)
        {
            try
            {
                var newStipend = new Stipend
                {
                    StudentId = CurrentStudent.Id,
                    DtRef = DateTime.Today,
                    DtAssign = DateTime.Today
                };

                var refView = new RefView(newStipend);
                refView.ShowDialog();
                LoadStipends();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания справки: {ex.Message}");
            }
        }

        private void StipendUpdateClick(object? parameter)
        {
            if (SelectedStipend == null)
            {
                MessageBox.Show("Выберите справку!");
                return;
            }

            try
            {
                var refView = new RefView(SelectedStipend);
                refView.ShowDialog();
                LoadStipends();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка редактирования справки: {ex.Message}");
            }
        }

        private void StipendDeleteClick(object? parameter)
        {
            if (SelectedStipend == null)
            {
                MessageBox.Show("Выберите справку!");
                return;
            }

            if (MessageBox.Show("Удалить справку?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    App.Context.Stipend.Remove(SelectedStipend);
                    App.Context.SaveChanges();
                    LoadStipends();
                    MessageBox.Show("Справка удалена!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}");
                }
            }
        }

        // Свойства
        public Student CurrentStudent { get; }

        private ObservableCollection<Stipend> _stipendCollection;
        public ObservableCollection<Stipend> StipendCollection
        {
            get => _stipendCollection;
            set
            {
                _stipendCollection = value;
                OnPropertyChanged();
            }
        }

        private Stipend _selectedStipend;
        public Stipend SelectedStipend
        {
            get => _selectedStipend;
            set
            {
                _selectedStipend = value;
                OnPropertyChanged();
            }
        }

        private bool _stipendsEnabled;
        public bool StipendsEnabled
        {
            get => _stipendsEnabled;
            set
            {
                _stipendsEnabled = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Stipend> GetActiveStipend()
        {
            var today = DateTime.Today;
            return new ObservableCollection<Stipend>(
                App.Context.Stipend
                    .Where(s => s.StudentId == CurrentStudent.Id &&
                               s.DtAssign <= today &&
                               (s.DtEnd == null || s.DtEnd >= today) &&
                               (s.DtStop == null || s.DtStop >= today))
                    .OrderByDescending(s => s.DtAssign)
                    .ToList());
        }
        public Stipend? GetLatestStipend()
        {
            return App.Context.Stipend
                .Where(s => s.StudentId == CurrentStudent.Id)
                .OrderByDescending(s => s.DtAssign)
                .ThenByDescending(s => s.DtRef)
                .FirstOrDefault();
        }

        // Команды
        public ICommand SaveChangesClickCommand { get; }
        public ICommand StudentDeleteClickCommand { get; }
        public ICommand StipendCreateClickCommand { get; }
        public ICommand StipendUpdateClickCommand { get; }
        public ICommand StipendDeleteClickCommand { get; }

        // Закрытие окна
        public event EventHandler ClosingRequest;
        protected void OnClosingRequest() => ClosingRequest?.Invoke(this, EventArgs.Empty);

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}