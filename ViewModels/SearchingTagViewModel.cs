using Otaku_Database.Models;
using Otaku_Database.Stores;
using MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Otaku_Database.ViewModels
{
    public class SearchingTagViewModel
    {
        private Tag _tag;
        private TagDataStore _tagDataStore;

        public string Name { get; }

        public ICommand RemoveSearchingTagCmd { get; }

        public SearchingTagViewModel(Tag tag, TagDataStore tagDataStore)
        {
            _tag = tag;
            _tagDataStore = tagDataStore;
            Name = tag.Name;
            RemoveSearchingTagCmd = new RelayCommand(RemoveSearchingTag);
        }

        private void RemoveSearchingTag()
        {
            _tagDataStore.RemoveSearchingTag(_tag);
        }
    }
}
