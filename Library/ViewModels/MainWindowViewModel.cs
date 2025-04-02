using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Threading;
using Library.Models;
using ReactiveUI;
using System.Reactive;

namespace Library.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<LibraryBook> _books;
        private LibraryBook? _selectedBook;
        private BookStatus _selectedStatus;
        private string _newInventoryCode = string.Empty;

        public ObservableCollection<LibraryBook> Books
        {
            get => _books;
            set => this.RaiseAndSetIfChanged(ref _books, value);
        }

        public LibraryBook? SelectedBook
        {
            get => _selectedBook;
            set => this.RaiseAndSetIfChanged(ref _selectedBook, value);
        }

        public BookStatus SelectedStatus
        {
            get => _selectedStatus;
            set => this.RaiseAndSetIfChanged(ref _selectedStatus, value);
        }

        public string NewInventoryCode
        {
            get => _newInventoryCode;
            set => this.RaiseAndSetIfChanged(ref _newInventoryCode, value);
        }

        public ReactiveCommand<Unit, Unit> ChangeStatusCommand { get; }
        public ReactiveCommand<Unit, Unit> UpdateCodeCommand { get; }

        public MainWindowViewModel()
        {
            Console.WriteLine("Initializing MainWindowViewModel");

            _books = new ObservableCollection<LibraryBook>
            {
                new LibraryBook("LIB-001", "Война и мир", new[] { "Лев Толстой" }, 1225, "Русская классика"),
                new LibraryBook("LIB-002", "1984", new[] { "Джордж Оруэлл" }, 328, "Антиутопия", BookStatus.CheckedOut),
                new LibraryBook("LIB-003", "Гарри Поттер", new[] { "Джоан Роулинг" }, 432, "Фэнтези")
            };

            SelectedBook = Books.FirstOrDefault();
            SelectedStatus = SelectedBook?.CurrentStatus ?? BookStatus.InStock;

            Console.WriteLine($"Initialized with {Books.Count} books");

            ChangeStatusCommand = ReactiveCommand.Create(ChangeBookStatus);
            UpdateCodeCommand = ReactiveCommand.Create(UpdateBookInventoryCode);
        }

        private void ChangeBookStatus()
{
    if (SelectedBook == null) return;
    
    SelectedBook.ChangeStatus(SelectedStatus);
    // Обновляем список книг, чтобы изменения отобразились в ListBox
    var index = Books.IndexOf(SelectedBook);
    if (index >= 0)
    {
        Books[index] = SelectedBook;
    }
    Console.WriteLine($"Status changed to {SelectedStatus}");
}


        private void UpdateBookInventoryCode()
{
    if (SelectedBook == null || string.IsNullOrWhiteSpace(NewInventoryCode)) return;
    
    SelectedBook.UpdateInventoryCode(NewInventoryCode);
    NewInventoryCode = string.Empty;
    // Обновляем список книг
    var index = Books.IndexOf(SelectedBook);
    if (index >= 0)
    {
        Books[index] = SelectedBook;
    }
    Console.WriteLine("Inventory code updated");
}
    }
}