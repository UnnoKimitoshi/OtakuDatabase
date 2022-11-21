using Otaku_Database.Stores;
using MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Otaku_Database.ViewModels
{
    enum SortType { StringCode, MembersCount}
    public class TagListViewModel : ObservableObject
    {
        private TagDataStore _tagDataStore;

        public ObservableCollection<TagViewModel> DisplayTagVMs { get; set; } = new ObservableCollection<TagViewModel>();

        private SortType _currentSortType;
        private SortType CurrentSortType
        {
            get { return _currentSortType; }
            set
            {
                _currentSortType = value;
                TagsSort();
            }
        }

        private bool _stringCodeSort;
        public bool StringCodeSort
        {
            get { return _stringCodeSort; }
            set 
            {
                SetProperty(ref _stringCodeSort, value);
                if(value)
                    CurrentSortType = SortType.StringCode;
            }
        }

        private bool _membersCountSort;
        public bool MembersCountSort
        {
            get { return _membersCountSort; }
            set
            {
                SetProperty(ref _membersCountSort, value);
                if (value)
                    CurrentSortType = SortType.MembersCount;
            }
        }

        public TagListViewModel(TagDataStore tagDataStore)
        {
            _tagDataStore = tagDataStore;
            _tagDataStore.TagsChanged += UpdateDisplayTagVMs;
            MembersCountSort = true;
        }

        private void UpdateDisplayTagVMs()
        {
            DisplayTagVMs.Clear();
            foreach (var tag in _tagDataStore.AllTags)
            {
                var tagVM = new TagViewModel(tag, _tagDataStore);
                DisplayTagVMs.Add(tagVM);
            }
            TagsSort();
        }

        private void TagsSort()
        {
            switch (_currentSortType)
            {
                case SortType.StringCode:
                    DisplayTagVMs = new ObservableCollection<TagViewModel>(DisplayTagVMs.OrderBy(x => x.Name));
                    OnPropertyChanged(nameof(DisplayTagVMs));
                    break;
                case SortType.MembersCount:
                    DisplayTagVMs = new ObservableCollection<TagViewModel>(DisplayTagVMs.OrderByDescending(x => x.MembersCount));
                    OnPropertyChanged(nameof(DisplayTagVMs));
                    break;
               
            }
        }
    }
}
