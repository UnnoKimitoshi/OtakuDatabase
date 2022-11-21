using Otaku_Database.Stores;
using MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Otaku_Database.ViewModels
{
    public class MainViewModel
    {
        private TagDataStore _tagDataStore;
        private ItemDataStore _itemDataStore;
        private ModalNavigationStore _modalNavigationStore;

        private bool _isBusy;

        public TagListViewModel TagListVM { get; }
        public ItemListViewModel ItemListVM { get; }
        public SearchingTagListViewModel SearchingTagsVM { get; }

        public RelayCommand MoveWindowCmd { get; }
        public RelayCommand ShutdownWindowCmd{ get; }
        public RelayCommand MaximizeWindowCmd { get; }
        public RelayCommand MinimizeWindowCmd { get; }
        public RelayCommand ShowSettingDialog { get; }


        public MainViewModel(TagDataStore tagDataStore, ItemDataStore itemDataStore, ModalNavigationStore modalNavigationStore)
        {
            _tagDataStore = tagDataStore;
            _itemDataStore = itemDataStore;
            _itemDataStore.EditStart += OnEditStart;
            _itemDataStore.EditEnd += OnEditEnd;
            _modalNavigationStore = modalNavigationStore;

            TagListVM = new TagListViewModel(tagDataStore);
            SearchingTagsVM = new SearchingTagListViewModel(tagDataStore);
            ItemListVM = new ItemListViewModel(itemDataStore, tagDataStore);

            MoveWindowCmd = new RelayCommand(Application.Current.MainWindow.DragMove);
            ShutdownWindowCmd = new RelayCommand(async () => 
            {
                while (_isBusy)
                {
                    await Task.Delay(500);
                    if (!_isBusy)
                        break;
                }
                Application.Current.Shutdown();
            });
            MaximizeWindowCmd = new RelayCommand(() =>
            {
                if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
                    Application.Current.MainWindow.WindowState = WindowState.Normal;
                else
                    Application.Current.MainWindow.WindowState = WindowState.Maximized;
            });
            MinimizeWindowCmd = new RelayCommand(() =>
            {
                Application.Current.MainWindow.WindowState = WindowState.Minimized;
            });

            //ShowSettingDialog = new RelayCommand(() =>
            //{
            //    MessageBox.Show("Test");
            //});
        }

        private void OnEditStart()
        {
            _isBusy = true;
        }

        private void OnEditEnd()
        {
            _isBusy = false;
        }
    }
}
