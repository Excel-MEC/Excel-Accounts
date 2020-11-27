using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using API.Services.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace API.Services
{
    public class GoogleCloudStorage : ICloudStorage
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;

        public GoogleCloudStorage(IEnvironmentService env)
        {
            var googleCredential = GoogleCredential.FromJson(env.GoogleCredential);
            _storageClient = StorageClient.Create(googleCredential);
            _bucketName = env.GoogleCloudStorageBucket;
        }

        public async Task<string> UploadFileAsync(IFormFile imageFile, string fileNameForStorage)
        {
            using (var memoryStream = new MemoryStream())
            {
                await imageFile.CopyToAsync(memoryStream);
                var dataObject = await _storageClient.UploadObjectAsync(_bucketName, fileNameForStorage, null, memoryStream);
                dataObject.Acl ??= new List<ObjectAccessControl>();
                dataObject.CacheControl = "no-cache, max-age=0";
                _storageClient.UpdateObject(dataObject, new UpdateObjectOptions
                {
                    PredefinedAcl = PredefinedObjectAcl.PublicRead
                });
                Console.WriteLine(dataObject.CacheControl);
                return dataObject.MediaLink;
            }
        }

        public async Task DeleteFileAsync(string fileNameForStorage)
        {
            await _storageClient.DeleteObjectAsync(_bucketName, fileNameForStorage);
        }
    }
}