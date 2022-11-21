using Otaku_Database.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otaku_Database.Stores
{
    public class ItemDataStore
    {
        public event Action? ItemsLoaded;
        public event Action<IEnumerable<Item>>? ItemsAdded;
        public event Action<Item>? ItemRemoved;
        public event Action<Item>? ItemUpdated;
        public event Action? EditStart;
        public event Action? EditEnd;

        public IEnumerable<Item> AllItems
        {
            get
            {
                var ret = new List<Item>();
                using (var db = new DatabaseContext())
                {
                    foreach (var item in db.Items)
                    {
                        ret.Add(item);
                    }
                }
                return ret;
            }
        }

        public async Task AddItemsAsync(string[] paths)
        {
            EditStart?.Invoke();
            var newItems = new List<Item>();
            await Task.Run(() =>
            {
                foreach (string path in paths)
                {
                    if (FindSame(Path.GetFileName(path)) == null)
                    {
                        var newItem = ItemFactory.MakeFromPath(path);
                        if (newItem == null) continue;

                        using (var db = new DatabaseContext())
                        {
                            db.Items.Add(newItem);
                            db.SaveChanges();
                        }
                        newItems.Add(newItem);
                    }
                }
            });
            EditEnd?.Invoke();
            ItemsAdded?.Invoke(newItems);
        }

        internal void OpenItem(string title)
        {
            var item = FindSame(title);
            var  path = item.ItemPath;
            Process ps = new Process();
            ps.StartInfo.UseShellExecute = true;
            ps.StartInfo.FileName = path;
            ps.Start();
        }

        public void RemoveItem(string title)
        {
            EditStart?.Invoke();
            var removeItem = FindSame(title);
            if (removeItem == null) return;

            using (var db = new DatabaseContext())
            {
                db.Items.Remove(removeItem);
                db.SaveChanges();
            }
            EditEnd?.Invoke();
            ItemRemoved?.Invoke(removeItem);
        }

        public void UpdateItem(string title, Item newItem)
        {
            EditStart?.Invoke();
            var targetItem = FindSame(title);
            if(targetItem== null) return;
            if(newItem.ItemPath != null) targetItem.ItemPath = newItem.ItemPath;
            if(targetItem.ImgBytes == null && newItem.ImgBytes != null) targetItem.ImgBytes = newItem.ImgBytes;
            if(newItem.Category != null) targetItem.Category = newItem.Category;
            if(newItem.Tags != null) targetItem.Tags = newItem.Tags;
            using (var db = new DatabaseContext())
            {
                db.Items.Update(targetItem);
                db.SaveChanges();
            }
            EditEnd?.Invoke();
            ItemUpdated?.Invoke(targetItem);
        }

        private Item? FindSame(string itemName)
        {
            using(var db = new DatabaseContext())
            {
                return db.Items.FirstOrDefault(i => i.Title == itemName);
            }
        }
    }
}
