using System;
using Amazon.DynamoDBv2.DataModel;

namespace Marketing.Api.Services
{
    [DynamoDBTable("Adverts")]
    public class AdvertDbModel
    {
        [DynamoDBHashKey]
        [DynamoDBProperty("Id")]
        public string Id { get; set; }

        [DynamoDBProperty("Title")] public string Title { get; set; }

        [DynamoDBProperty("Description")] public string Description { get; set; }

        [DynamoDBProperty("Price")] public double Price { get; set; }

        [DynamoDBProperty("CreationDateTime")] public DateTime CreationDateTime { get; set; }

        [DynamoDBProperty("Status")] public AdvertStatus Status { get; set; }

        [DynamoDBProperty("FilePath")]public string FilePath { get; set; }

        [DynamoDBProperty("UserName")] public string UserName { get; set; }

    }
}