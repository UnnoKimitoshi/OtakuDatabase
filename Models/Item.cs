using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otaku_Database.Models
{
    public class Item
    {
        [Key]
        public string Title { get; set; }
        public string ItemPath { get; set; }
        public byte[]? ImgBytes { get; set; }
        public string Category { get; set; }
        public string? Tags { get; set; }
    }
}
