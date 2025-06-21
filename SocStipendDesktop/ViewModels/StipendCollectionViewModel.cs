
using System.Collections.ObjectModel;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using SocStipendDesktop.Models;
using SocStipendDesktop.Services;
using SocStipendDesktop.Views;
using System.Collections.Generic;

namespace SocStipendDesktop.ViewModels
{
    public class StipendCollectionViewModel : INotifyPropertyChanged
    {
        public StipendCollectionViewModel()
        {
            // Инициализация команд
            CreateNewStudentClickCommand = new RelayCommand(CreateNewStudentClick);
            SelectedStipendClickCommand = new RelayCommand(SelectedStipendClick);
            RefreshClickCommand = new RelayCommand(RefreshClick);

            // Загрузка данных
            RefreshData();
        }

        private void RefreshData()
        {
            try
            {
                App.Context.Student.Load();
                App.Context.Stipend
                    .Include(s => s.Student)
                    .Load();

                var stipends = App.Context.Stipend.Local.ToList();
                int orderNum = 1;

                foreach (var stipend in stipends)
                {
                    stipend.OrderNum = orderNum++;
                    stipend.StudentName = stipend.Student?.StudentName ?? "Unknown";
                    stipend.StudentGroup = stipend.Student?.StudentGroup ?? "Unknown";
                    stipend.Status = stipend.Student?.Status ?? "Unknown";
                }

                StipendCollection = new ObservableCollection<Stipend>(stipends);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void CreateNewStudentClick(object? parameter)
        {
            var studentView = new StudentView(new Student());
            studentView.ShowDialog();
            RefreshData();
        }

        private void SelectedStipendClick(object? parameter)
        {
            if (SelectedStipend != null && SelectedStipend.Student != null)
            {
                var studentView = new StudentView(SelectedStipend.Student);
                studentView.ShowDialog();
                RefreshData();
            }
        }

        private void RefreshClick(object? parameter)
        {
            RefreshData();
        }

        // Команды
        public RelayCommand CreateNewStudentClickCommand { get; }
        public RelayCommand SelectedStipendClickCommand { get; }
        public RelayCommand RefreshClickCommand { get; }

        // Свойства
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
        private void SearchStudent(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                RefreshData();
                return;
            }

            var filteredStudent = App.Context.Student
                .Where(s => s.StudentName != null && s.StudentName.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                .Include(s => s.Stipend)
                .ToList();

            var stipend = filteredStudent
                .SelectMany(s => s.Stipend)
                .OrderBy(s => s.StudentName)
                .ToList();

            UpdateStipendCollection(stipend);
        }

        private void GroupByStudentGroup()
        {
            var groupedData = App.Context.Stipend
                .Include(s => s.Student)
                .AsEnumerable()
                .GroupBy(s => s.Student?.StudentGroup ?? "Unknown")
                .Select(g => new {
                    GroupName = g.Key,
                    Count = g.Count(),
                    TotalTravelCards = g.Count(s => s.HasTravelCard == true)
                })
                .OrderBy(g => g.GroupName)
                .ToList();

            // Можно использовать для отображения статистики
            foreach (var group in groupedData)
            {
                Console.WriteLine($"Группа: {group.GroupName}, Справок: {group.Count}, С проездным: {group.TotalTravelCards}");
            }
        }
        private void UpdateStipendCollection(List<Stipend> stipends)
        {
            int orderNum = 1;
            foreach (var stipend in stipends)
            {
                stipend.OrderNum = orderNum++;
                stipend.StudentName = stipend.Student?.StudentName ?? "Unknown";
                stipend.StudentGroup = stipend.Student?.StudentGroup ?? "Unknown";
                stipend.Status = stipend.Student?.Status ?? "Unknown";
            }

            StipendCollection = new ObservableCollection<Stipend>(stipends);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}