using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using AutoMapper;
using Marketing.Api.AddHealthChecks;
using Marketing.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Marketing.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
       
            services.AddAutoMapper(typeof(Startup));
            //services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
        //  var options  = new AWSOptions{
        //         Profile = "dynamodb-user"
        //     };
           // var client =  options.CreateServiceClient<IAmazonDynamoDB>();
            services.AddAWSService<IAmazonDynamoDB>(new AWSOptions{
                Profile = "dynamodb-user",
                Region = RegionEndpoint.APSouth1
            });
          //   var sp = services.BuildServiceProvider();

    // This will succeed.
    //var client = sp.GetService<IAmazonDynamoDB>();
           // var client = services.<IAmazonDynamoDB>();
//             var chain = new CredentialProfileStoreChain();
// AWSCredentials awsCredentials;
// if (!chain.TryGetAWSCredentials("dynamodb-user", out awsCredentials))
// {
//     throw new Exception("dynamodb-user configuration not found");
// }
    // use awsCredentials
            //services.AddScoped<IDynamoDbContext<AdvertDbModel>>(provider => new DynamoDbContext<AdvertDbModel>(client,"Adverts"));

            services.AddSingleton<IDynamoDbContext<AdvertDbModel>>(provider => new DynamoDbContext<AdvertDbModel>(provider.GetService<IAmazonDynamoDB>(),"Adverts"));

            services.AddTransient<IAdvertStorageService, DynamoDBAdvertStorage>();
            services.AddTransient<StorageHealthCheck>();

            services.AddHealthChecks().AddCheck<StorageHealthCheck>("Storage");


           services.AddControllers();
          
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Marketing.Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Marketing.Api v1"));
            }



           app.UseHealthChecks("/health");

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
