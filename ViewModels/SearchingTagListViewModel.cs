using Otaku_Database.Stores;
using MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Otaku_Database.ViewModels
{
    public class SearchingTagListViewModel :ObservableObject
    {
        private TagDataStore _tagDataStore;

        public ObservableCollection<SearchingTagViewModel> SearchingTagVMs { get; }= new ObservableCollection<SearchingTagViewModel>();
      

        public SearchingTagListViewModel(TagDataStore tagDataStore)
        {
            _tagDataStore = tagDataStore;
            _tagDataStore.SearchingTagChanged += LoadSearcingTags;
        }

        public void LoadSearcingTags()
        {
            SearchingTagVMs.Clear();
            foreach (var searchigTag in _tagDataStore.SearchingTags)
            {
                SearchingTagVMs.Add(new SearchingTagViewModel(searchigTag, _tagDataStore));
            }
        }
    }
}
