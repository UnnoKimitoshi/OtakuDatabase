using Otaku_Database.Models;
using Otaku_Database.Stores;
using MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Otaku_Database.ViewModels
{
    public class ItemViewModel : ObservableObject
    {
        private ItemDataStore _itemDataStore;
        private TagDataStore _tagDataStore;

        private bool _isDisplay;

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private byte[] _imgBytes;
        public byte[]? ImgBytes 
        {
            get { return _imgBytes; }
            set { SetProperty(ref _imgBytes, value);}
        }

        private string _category;
        public string Category
        {
            get { return _category; }
            set { SetProperty(ref _category, value); }
        }

        public ObservableCollection<string> ItemTags { get; } = new ObservableCollection<string>();
        

        public ICommand RemoveItemCmd { get; }
        public RelayCommand<string> UpdateItemCmd { get; }
        public ICommand ItemDoubleClickCmd { get; }

        [DebuggerHidden()]
        public ItemViewModel(Item item, ItemDataStore itemDataStore, TagDataStore tagDataStore)
        {
            _itemDataStore = itemDataStore;
            _tagDataStore = tagDataStore;

            Load(item);

            RemoveItemCmd = new RelayCommand(RemoveItem);
            UpdateItemCmd = new RelayCommand<string>(UpdateItem);
            ItemDoubleClickCmd = new RelayCommand(OpenItem);
        }

        private void OpenItem()
        {
            _itemDataStore.OpenItem(Title);
        }

        private void RemoveItem()
        {
            _itemDataStore.RemoveItem(Title);
        }

        private void Load(Item sourceItem)
        {
            Title = sourceItem.Title;
            ImgBytes = sourceItem.ImgBytes;
            Category = sourceItem.Category;
            LoadItemTags(sourceItem);
        }

        private void LoadItemTags(Item sourecItem)
        {
            ItemTags.Clear();
            var tags = sourecItem.Tags?.Split(";").ToList();

            if (tags == null || tags.Count == 0) return;

            tags.RemoveAt(tags.Count - 1);
            tags.ForEach(t => ItemTags.Add(t));
        }

        private async void UpdateItem(string url)
        {
            var newItem = await ItemFactory.MakeFromUrlAsync(url);
            if(newItem == null) return;
            RemoveItemTags();
            _itemDataStore.UpdateItem(Title, newItem);
        }

        public void UpdateItemVM(Item updatedItem)
        {
            Load(updatedItem);
            AddItemTags();
        }

        public void AddItemTags()
        {
            if (!_isDisplay)
            {
                _tagDataStore.AddTags(ItemTags);
            }
            _isDisplay = true;
        }

        public void RemoveItemTags()
        {
            if (_isDisplay)
            {
                _tagDataStore.RemoveTags(ItemTags);
            }
            _isDisplay= false;
        }
    }
}
