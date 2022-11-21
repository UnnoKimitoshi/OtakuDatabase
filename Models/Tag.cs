using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otaku_Database.Models
{
    public class Tag
    {
        public string Name { get; set; }
        public int MembersCount { get; set; }
        public bool IsChecked { get; set; }

        public Tag(string name)
        {
            Name = name;
            MembersCount = 1;
            IsChecked = false;
        }
    }
}
