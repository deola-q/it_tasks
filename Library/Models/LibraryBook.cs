using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Library.Models
{
    public enum BookStatus
    {
        InStock,
        CheckedOut,
        InRestoration
    }

    public class LibraryBook : INotifyPropertyChanged
    {
        
        public event PropertyChangedEventHandler? PropertyChanged;
        private string _inventoryCode = string.Empty;
        private BookStatus _currentStatus;

    public string InventoryCode 
    { 
        get => _inventoryCode;
        private set
        {
            if (_inventoryCode != value)
            {
                _inventoryCode = value ?? throw new ArgumentNullException(nameof(value));
                OnPropertyChanged();
            }
        }
    }
        public string Title { get; }
        public IReadOnlyList<string> Authors { get; }
        public int PageCount { get; }
        public string SubjectCategory { get; }
    public BookStatus CurrentStatus 
    { 
        get => _currentStatus;
        private set
        {
            if (_currentStatus != value)
            {
                _currentStatus = value;
                OnPropertyChanged();
            }
        }
    }
protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
        public LibraryBook(
            string inventoryCode,
            string title,
            IEnumerable<string> authors,
            int pageCount,
            string subjectCategory,
            BookStatus initialStatus = BookStatus.InStock)
        {
            InventoryCode = inventoryCode ?? throw new ArgumentNullException(nameof(inventoryCode));
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Authors = new List<string>(authors ?? throw new ArgumentNullException(nameof(authors))).AsReadOnly();
            PageCount = pageCount > 0 ? pageCount : throw new ArgumentOutOfRangeException(nameof(pageCount));
            SubjectCategory = subjectCategory ?? throw new ArgumentNullException(nameof(subjectCategory));
            CurrentStatus = initialStatus;
        }

        public void ChangeStatus(BookStatus newStatus)
        {
            CurrentStatus = newStatus;
        }

        public void UpdateInventoryCode(string newCode)
        {
            if (string.IsNullOrWhiteSpace(newCode))
            {
                throw new ArgumentException("Inventory code cannot be null or whitespace", nameof(newCode));
            }
            
            InventoryCode = newCode;
        }

        public override string ToString()
        {
            return $"{Title} by {string.Join(", ", Authors)} ({InventoryCode}) - {CurrentStatus}";
        }
    }
}