using Microsoft.Azure.EventHubs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace EHGenerator
{
    public class EHBlobProducer
    {
        private EventHubClient _eventHubClient;
        private CloudBlobClient _cloudBlobClient;

        private const int DELAY_MS = 5;

        public EHBlobProducer(string ehConnectionString, string storageConnectionString)
        {
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(ehConnectionString);
            _eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            CloudStorageAccount storageAccount;
            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                _cloudBlobClient = storageAccount.CreateCloudBlobClient();
            }
            else
            {
                throw new StorageException("A connection string has not been defined in the system environment variables. " +
                    "Add a environment variable named 'storageconnectionstring' with your storage " +
                    "connection string as a value.");
            }
        }

        public async Task sendFile(string blobContainer, string blobFilePath, IDictionary<string, object> properties = null)
        {
            foreach (var line in ReadBlobStorageFile(blobContainer, blobFilePath))
            {
                var edata = new EventData(Encoding.UTF8.GetBytes(line));
                if (properties != null)
                {
                    foreach (KeyValuePair<string, object> entry in properties)
                    {
                        edata.Properties.Add(entry.Key, entry.Value);
                    }
                }
                await _eventHubClient.SendAsync(edata);
                await Task.Delay(DELAY_MS);
            }
        }


        private IEnumerable<string> ReadBlobStorageFile(string container, string filePath)
        {
            CloudBlobContainer blobContainer = _cloudBlobClient.GetContainerReference(container);

            // List the blobs in the container.
            CloudBlob blob = blobContainer.GetBlobReference(filePath);
            using (var stream = blob.OpenReadAsync())
            {
                using (StreamReader reader = new StreamReader(stream.Result))
                {
                    while (!reader.EndOfStream)
                    {
                        yield return reader.ReadLine();
                    }
                }
            }
        }
    }
}
