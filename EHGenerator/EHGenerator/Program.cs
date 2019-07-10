using Microsoft.Azure.EventHubs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EHGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            // Setup EventHub
            string ehConnectionString = Environment.GetEnvironmentVariable("EVENTHUB_CONNECTION_STRING");

            // Setup storage account
            string storageConnectionString = Environment.GetEnvironmentVariable("STORAGE_CONTAINER_STRING");
            string storageContainer = Environment.GetEnvironmentVariable("STORAGE_CONTAINER");
            string filePath = Environment.GetEnvironmentVariable("STORAGE_FILEPATH");

            while (true)
            {
                Console.WriteLine("Start sending data to Event Hubs");
                try
                {
                    var producer = new EHBlobProducer(ehConnectionString, storageConnectionString);
                    await producer.sendFile(storageContainer, filePath, new Dictionary<string, object> { { "owner", "lace" } });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.ToString()}");
                }

                // Delay
                Console.WriteLine("Sleeping for one minute...");
                await Task.Delay(60000); //One minute
            }
        }

    }
}
