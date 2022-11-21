using Otaku_Database.Models;
using Otaku_Database.Stores;
using MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Otaku_Database.ViewModels
{
    public class ItemListViewModel : ObservableObject
    {
        private ItemDataStore _itemDataStore;
        private TagDataStore _tagDataStore;

        private List<ItemViewModel> _allItemVMs = new List<ItemViewModel>();

        public ObservableCollection<ItemViewModel> DisplayItemVMs { get; } = new ObservableCollection<ItemViewModel>();
       

        public RelayCommand<string[]> AddItemCmd { get; }

        public ItemListViewModel(ItemDataStore itemDataStore, TagDataStore tagDataStore)
        {
            _itemDataStore = itemDataStore;
            _itemDataStore.ItemsAdded += OnItemsAdded;
            _itemDataStore.ItemRemoved += OnItemRemoved;
            _itemDataStore.ItemUpdated += OnItemUpdated;
            _tagDataStore = tagDataStore;
            _tagDataStore.SearchingTagChanged += LoadDisplayItemVMs;

            AddItemCmd = new RelayCommand<string[]>(AddItem);

            ItemsLoad();
        }

        public void LoadDisplayItemVMs()
        {
            DisplayItemVMs.Clear();
            foreach (var itemVM in _allItemVMs)
            {
                bool canDisplay = true;
                foreach (var searchingTag in _tagDataStore.SearchingTags)
                {
                    if (!itemVM.ItemTags.Contains(searchingTag.Name))
                    {
                        canDisplay = false;
                        break;
                    }
                }
                if (canDisplay)
                {
                    DisplayItemVMs.Add(itemVM);
                    itemVM.AddItemTags();
                }
                else
                {
                    itemVM.RemoveItemTags();
                }
            }
        }

        private void ItemsLoad()
        {
            foreach (var item in _itemDataStore.AllItems)
            {
                var itemVM = new ItemViewModel(item, _itemDataStore, _tagDataStore);
                _allItemVMs.Add(itemVM);
            }
            LoadDisplayItemVMs();
        }

        private async void AddItem(string[] paths)
        {
              await _itemDataStore.AddItemsAsync(paths);
        }


        private void OnItemsAdded(IEnumerable<Item> addedItems)
        {
            foreach(var item in addedItems)
            {
                _allItemVMs.Add(new ItemViewModel(item, _itemDataStore, _tagDataStore));
            }
            LoadDisplayItemVMs();
        }

        private void OnItemRemoved(Item removedItem)
        {
            var removeItemVM = _allItemVMs.First(i => i.Title == removedItem.Title);
            _allItemVMs.Remove(removeItemVM);
            DisplayItemVMs.Remove(removeItemVM);
            removeItemVM.RemoveItemTags();
        }

        private void OnItemUpdated(Item updatedItem)
        {
            var updateItemVM = _allItemVMs.First(i => i.Title == updatedItem.Title);
            updateItemVM.UpdateItemVM(updatedItem);
        }
    }
}
