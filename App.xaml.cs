using Otaku_Database.Models;
using Otaku_Database.Stores;
using Otaku_Database.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Otaku_Database
{
    public partial class App : Application
    {
        private readonly TagDataStore _tagDataStore;
        private readonly ItemDataStore _itemDataStore;
        private readonly ModalNavigationStore _modalNavigationStore;

   
        public App()
        {
            _tagDataStore = new TagDataStore();
            _itemDataStore = new ItemDataStore();
            _modalNavigationStore = new ModalNavigationStore();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            using(var db = new DatabaseContext())
            {
                db.Database.Migrate();
            }

            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(_tagDataStore,  _itemDataStore, _modalNavigationStore),
                
            };
            MainWindow.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {

        }

    }
}
