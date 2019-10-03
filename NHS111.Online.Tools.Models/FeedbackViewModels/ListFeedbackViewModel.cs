using System;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace NHS111.Online.Tools.Models.FeedbackViewModels
{
    public class ListFeedbackViewModel : TableEntity
    {
        public ListFeedbackViewModel()
        {
            var now = DateTime.UtcNow;
            PartitionKey = $"{now:yyyy-MM}";
            RowKey = $"{now:dd HH-mm-ss-fff}-{Guid.NewGuid()}";
        }
        
        [JsonProperty(PropertyName = "data")]
        public dynamic Data
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject(PageId);
                }
                catch
                {
                    return null; // Old (2017) feedback has a URL string in PageId instead of object.
                }
            }
        }

        [JsonProperty(PropertyName = "dateAdded")]
        public DateTime DateAdded { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "pageId")]
        public string PageId { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }
    }
}
