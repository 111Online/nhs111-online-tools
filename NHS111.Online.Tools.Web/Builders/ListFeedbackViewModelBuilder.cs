using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using NHS111.Online.Tools.Models.FeedbackViewModels;
using NHS111.Online.Tools.Web.Helpers;

namespace NHS111.Online.Tools.Web.Builders
{
    public class ListFeedbackViewModelBuilder : IListFeedbackViewModelBuilder
    {
        private readonly IConfiguration _configuration;

        public ListFeedbackViewModelBuilder(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<ListFeedbackViewModel>> Build(int pageNumber, int pageSize)
        {
            var storageAccount = CloudStorageAccount.Parse(_configuration.GetConnectionString("AzureConnection"));
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(_configuration["AzureSettings:TableName"]);

            var query = new TableQuery<ListFeedbackViewModel>();

            if (pageNumber == 0)
            {
                // Azure Tables do not support pagination queries
                // So on the first page it is quicker to just take the pageSize
                // while other pages need the full set of results
                query.Take(pageSize);
            }

            var results = await table.ExecuteQueryAsync(query);

            if (!results.Any()) return new List<ListFeedbackViewModel>();

            var orderedResults = results.OrderByDescending(f => f.DateAdded);
            var feedback = (pageNumber > 0) ? orderedResults.Skip((pageNumber - 1) * pageSize).Take(pageSize) : orderedResults.Take(pageSize);
            return feedback;
        }
    }

    public interface IListFeedbackViewModelBuilder
    {
        Task<IEnumerable<ListFeedbackViewModel>> Build(int pageNumber, int pageSize);
    }
}
