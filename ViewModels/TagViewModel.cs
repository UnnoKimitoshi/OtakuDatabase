using Otaku_Database.Models;
using Otaku_Database.Stores;
using MVVM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Otaku_Database.ViewModels
{
    public class TagViewModel : ObservableObject
    {
        private TagDataStore _tagDataStore;

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private int _membersCount;
        public int MembersCount
        {
            get { return _membersCount; }
            set { SetProperty(ref _membersCount, value); }
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set { SetProperty(ref _isChecked, value); }
        }

        public ICommand CheckChangeCmd { get; }

        [DebuggerHidden()]
        public TagViewModel(Tag tag, TagDataStore tagDataStore)
        {
            _tagDataStore = tagDataStore;
            Name = tag.Name;
            MembersCount = tag.MembersCount;
            IsChecked = tag.IsChecked;

            CheckChangeCmd = new RelayCommand(ChangeTagCheck);
        }

        private void ChangeTagCheck()
        {
            _tagDataStore.ChangeTagCheck(Name, IsChecked);
        }
    }
}
