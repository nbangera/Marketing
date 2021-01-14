using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using AutoMapper;
using Marketing.Common.Models;

namespace Marketing.Api.Services
{
    public class DynamoDBAdvertStorage: IAdvertStorageService
    {
      private readonly IMapper _mapper;
      private readonly IDynamoDbContext<AdvertDbModel> _db;

        public DynamoDBAdvertStorage(IMapper mapper,IDynamoDbContext<AdvertDbModel> db)
        {
            _mapper = mapper;
            _db = db;
        }

        public async Task<string> AddAsync(AdvertModel model)
        {
            var dbModel = _mapper.Map<AdvertDbModel>(model);

            dbModel.Id = Guid.NewGuid().ToString();
            dbModel.CreationDateTime = DateTime.UtcNow;
            dbModel.Status = AdvertStatus.Pending;

             await _db.SaveAsync(dbModel);

//             var chain = new CredentialProfileStoreChain();
// AWSCredentials awsCredentials;
// if (chain.TryGetAWSCredentials("dynamodb-user", out awsCredentials))
// {
//     // use awsCredentials
//      using (var client = new AmazonDynamoDBClient(awsCredentials,RegionEndpoint.APSouth1))
//             {
//                 var table = await client.DescribeTableAsync("Adverts");

//                 using (var context = new DynamoDBContext(client))
//                 {
//                     await context.SaveAsync(dbModel);
//                 }
//             }
// }

           

            return dbModel.Id;
        }

        public async Task<bool> CheckHealthAsync()
        {
            Console.WriteLine("Health checking...");
            using (var client = new AmazonDynamoDBClient())
            {
                var tableData = await client.DescribeTableAsync("Adverts");
                return string.Compare(tableData.Table.TableStatus, "active", true) == 0;
            }
        }

        public async Task ConfirmAsync(ConfirmAdvertModel model)
        {
            using (var client = new AmazonDynamoDBClient())
            {
                using (var context = new DynamoDBContext(client))
                {
                    var record = await context.LoadAsync<AdvertDbModel>(model.Id);
                    if (record == null) throw new KeyNotFoundException($"A record with ID={model.Id} was not found.");
                    if (model.Status == AdvertStatus.Active)
                    {
                        record.FilePath = model.FilePath;
                        record.Status = AdvertStatus.Active;
                        await context.SaveAsync(record);
                    }
                    else
                    {
                        await context.DeleteAsync(record);
                    }
                }
            }
        }

        public async Task<List<AdvertModel>> GetAllAsync()
        {
            // using (var client = new AmazonDynamoDBClient())
            // {
            //     using (var context = new DynamoDBContext(client))
            //     {
            //         var scanResult =
            //             await context.ScanAsync<AdvertDbModel>(new List<ScanCondition>()).GetNextSetAsync();
            //         return scanResult.Select(item => _mapper.Map<AdvertModel>(item)).ToList();
            //     }

                
            // }

            var result = await _db.GetAllAsync();
            return result.Select(item=> _mapper.Map<AdvertModel>(item)).ToList();
        }

        public async Task<AdvertModel> GetByIdAsync(string id)
        {
           // using (var client = new AmazonDynamoDBClient())
            // {
            //     using (var context = new DynamoDBContext(client))
            //     {
            //         var dbModel = await context.LoadAsync<AdvertDbModel>(id);
            //         if (dbModel != null) return _mapper.Map<AdvertModel>(dbModel);
            //     }
            // }

            var dbModel = await _db.GetByIdAsync(id);
            if (dbModel != null) return _mapper.Map<AdvertModel>(dbModel);

            throw new KeyNotFoundException();
        }
    }
}