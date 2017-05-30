using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json.Linq;

namespace Library1
{
    public class Class1
    {
        string DocumentDbDatabaseNameConfig = "your-database";
        string collectionName = "your-collection";
        string id = "your-id";

        public string Get()
        {
            // run immediately
            ensureDocumentDbAsync(DocumentDbDatabaseNameConfig).ConfigureAwait(false);
            ensureDocumentCollectionExistsAsync(DocumentDbDatabaseNameConfig, collectionName).ConfigureAwait(false);

            Uri uri = UriFactory.CreateDocumentUri(DocumentDbDatabaseNameConfig, collectionName, id);
            var result = _documentDbclient.ReadDocumentAsync(uri).Result;
            JObject j = (dynamic)result.Resource;
            return j.ToString();
        }


        public class StorableParameterGroup
        {
            /// <summary>
            /// DocuementDB Id.
            /// </summary>
            public Guid Id { get; set; }
            /// <summary>
            /// 이미지 원본의 Uri
            /// </summary>
            public string Source { get; set; }
            public List<IStorable> Parameters { get; set; }
        }
        public interface IStorable
        {
            Guid ReferenceId { get; set; }
            Type ReferenceType { get; }
            string ParameterName { get; set; }
            object ParameterData { get; set; }
        }
        DocumentClient _documentDbclient;

        // 환경 파일에서 읽는 값
        static readonly string DocumentDbKeyConfig = "3dzS7V1Mrdd7ZZpVKi5W5g==";
        static readonly string DocumentDbEndPointConfig = "https://your-docdb.documents.azure.com:443/";

        public async Task ensureDocumentDbAsync(string databaseName)
        {
            // Create a new instance of the DocumentClient
            _documentDbclient = new DocumentClient(
                new Uri(DocumentDbEndPointConfig), DocumentDbKeyConfig);
            // Check if the database FamilyDB does not exist

            try
            {
                await _documentDbclient.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(databaseName));
            }
            catch (DocumentClientException de)
            {
                // If the database does not exist, create a new database
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    Trace.WriteLine($"{databaseName} not exist");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
            }
        }



        private async Task ensureDocumentCollectionExistsAsync(string databaseName, string collectionName)
        {
            try
            {
                await
                    _documentDbclient.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName));
            }
            catch (DocumentClientException de)
            {
                throw;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
            }
            Trace.WriteLine($"collection {collectionName} exists.");
        }
    }
}
