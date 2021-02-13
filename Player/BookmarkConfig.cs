using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Player
{
    public class BookmarkConfig
    {
        [JsonProperty("bookmarks")]
        public Bookmark[] Bookmarks { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; }
    }

    public class Bookmark
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
    }

}
