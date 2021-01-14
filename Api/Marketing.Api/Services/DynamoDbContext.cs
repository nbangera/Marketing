using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace Marketing.Api.Services
{
    public class DynamoDbContext<T> : DynamoDBContext, IDynamoDbContext<T> where T : class
    {
        private DynamoDBOperationConfig _config;

        public DynamoDbContext(IAmazonDynamoDB client, string tableName)
            : base(client)
        {
            _config = new DynamoDBOperationConfig()
            {
                OverrideTableName = tableName
            };
        }

        public async Task<T> GetByIdAsync(string id)
        {
            return await base.LoadAsync<T>(id, _config);
        }

        public async Task SaveAsync(T item)
        {
            await base.SaveAsync(item, _config);
        }

        public async Task DeleteByIdAsync(T item)
        {
            await base.DeleteAsync(item, _config);
        }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken)
    {
      var resultList = new List<T>();
     
        var scanConditions = new List<ScanCondition>();
        var search = base.ScanAsync<T>(scanConditions);

        while (!search.IsDone)
        {
          var entities = await search.GetNextSetAsync(cancellationToken);
          resultList.AddRange(entities);
        }

      return resultList;
    }

    public async Task<IEnumerable<T>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
      var resultList = new List<T>();
     
        var scanCondition = new ScanCondition("Id", ScanOperator.Equal, id);
        var search = base.ScanAsync<T>(new[] { scanCondition });

        while (!search.IsDone)
        {
          var entities = await search.GetNextSetAsync(cancellationToken);
          resultList.AddRange(entities);
        }

      return resultList;
    }


    }
}