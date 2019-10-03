using System;
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

        public async Task<IEnumerable<ListFeedbackViewModel>> Build(int pageNumber, int pageSize, string startDate, string endDate, string searchQuery)
        {
            var storageAccount = CloudStorageAccount.Parse(_configuration.GetConnectionString("AzureConnection"));
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(_configuration["AzureSettings:TableName"]);

            string dateStartFilter = string.IsNullOrEmpty(startDate) ? "" : "DateAdded ge datetime'" + startDate + "'";
            string dateEndFilter = string.IsNullOrEmpty(endDate) ? "" : "DateAdded le datetime'" + endDate + "'";

            var query = new TableQuery<ListFeedbackViewModel>();
            query.Where(buildFilters(dateStartFilter, dateEndFilter));

            var results = await table.ExecuteQueryAsync(query);

            if (!results.Any()) return new List<ListFeedbackViewModel>();
            
            var filteredResults = results; // Fallback if no searchQuery to be all results

            if (!string.IsNullOrEmpty(searchQuery))
            {
                filteredResults = results.Where(m =>
                {
                    return (!string.IsNullOrEmpty(m.Text) && m.Text.Contains(searchQuery, StringComparison.CurrentCultureIgnoreCase)) || 
                           (!string.IsNullOrEmpty(m.UserId) && m.UserId.Contains(searchQuery, StringComparison.CurrentCultureIgnoreCase));
                });
            }
            var orderedResults = filteredResults.OrderByDescending(f => f.DateAdded);

            var feedback = (pageNumber > 0) ? orderedResults.Skip((pageNumber - 1) * pageSize).Take(pageSize) : orderedResults.Take(pageSize);
            return feedback;
        }

        private string buildFilters (params string[] filters) {
            // Query cannot take empty filters, so this takes any filters and builds it up. Currently AND only.
            string filter = "";
            for (int i = 0; i < filters.Length; i++)
            {
                if (filters[i] != "")
                {
                    if (filter != "") filter += " and ";
                    filter += "(" + filters[i] + ")";
                }
            }
            return filter;
        }
    }

    public interface IListFeedbackViewModelBuilder
    {
        Task<IEnumerable<ListFeedbackViewModel>> Build(int pageNumber, int pageSize, string startDate, string endDate, string query);
    }
}
