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
