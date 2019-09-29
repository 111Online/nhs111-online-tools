using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using NHS111.Online.Tools.Models.FeedbackViewModels;
using NHS111.Online.Tools.Web.Helpers;

namespace NHS111.Online.Tools.Web.Builders
{
    public class ListFeedbackViewModelBuilder : IListFeedbackViewModelBuilder
    {
        public async Task<IEnumerable<ListFeedbackViewModel>> Build(int pageNumber = 0, int pageSize = 1000)
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(CloudConfigurationManager.GetSetting("StorageTableReference"));

            var query = new TableQuery<ListFeedbackViewModel>();
            var results = await table.ExecuteQueryAsync(query);

            if (!results.Any()) return new List<ListFeedbackViewModel>();

            var orderedResults = results.OrderByDescending(f => f.DateAdded);
            var feedback = (pageNumber > 0) ? orderedResults.Skip((pageNumber - 1) * pageSize).Take(pageSize) : orderedResults.Take(pageSize);
            return feedback;
        }
    }

    public interface IListFeedbackViewModelBuilder
    {
        Task<IEnumerable<ListFeedbackViewModel>> Build(int pageNumber = 0, int pageSize = 1000);
    }
}
