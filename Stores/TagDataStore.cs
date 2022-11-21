using Otaku_Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otaku_Database.Stores
{
    public class TagDataStore
    {
        private List<Tag> _tags = new List<Tag>();
        public IEnumerable<Tag> AllTags => _tags;

        public IEnumerable<Tag> SearchingTags
        {
            get
            {
                var searchingTags = new List<Tag>();
                foreach (var tag in _tags)
                {
                    if (tag.IsChecked)
                    {
                        searchingTags.Add(tag);
                    }
                }
                return searchingTags;
            }
        }

        public event Action<IEnumerable<string>>? TagsAdedd;
        public event Action<IEnumerable<string>>? TagsRemoved;
        public event Action? TagsChanged;
        public event Action<Tag>? TagUpdated;
        public event Action<Tag>? SearchingTagAdded;
        public event Action<Tag>? SearchingTagRemoved;
        public event Action? SearchingTagChanged;

        public void AddTags(IEnumerable<string> tagNames)
        {
            var addedTags = new List<string>();
            foreach (var tagName in tagNames)
            {
                var targetTag = FindSame(tagName);
                if(targetTag != null)
                {
                    targetTag.MembersCount++;
                }
                else
                {
                    _tags.Add(new Tag(tagName));
                    addedTags.Add(tagName);
                }
            }
            TagsChanged?.Invoke();

        }

        public void RemoveTags(IEnumerable<string> tagNames)
        {
            foreach (var tagName in tagNames)
            {
                var targetTag = FindSame(tagName);
                targetTag.MembersCount--;
                if(targetTag.MembersCount > 0)
                {
                }
                else
                {
                    if (targetTag.IsChecked)
                    {
                        RemoveSearchingTag(targetTag);
                    }
                    _tags.Remove(targetTag);
                }

            }
            TagsChanged?.Invoke();

        }


        private Tag? FindSame(string tagName)
        {
            return _tags.FirstOrDefault(t => t.Name == tagName);
        }

        public void ChangeTagCheck(string tagName, bool isChecked)
        {
            var targetTag = FindSame(tagName);
            targetTag.IsChecked = isChecked;
            
            SearchingTagChanged?.Invoke();
        }

        public void RemoveSearchingTag(Tag tag)
        {
            tag.IsChecked = false;
            SearchingTagChanged?.Invoke();
        }
    }
}
