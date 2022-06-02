using AutoMapper;
using Microsoft.Extensions.Options;
using Mongo.Domain.Interfaces;
using Mongo.Domain.Models;
using Mongo.Service.Context;
using Mongo.Service.Interfaces;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mongo.Service
{
    public class MongoService<TDocument> : IMongoService<TDocument> where TDocument : IDocument
    {
        public IMongoCollection<TDocument> collection { get; set; }

        private protected static string GetCollectionName(Type documentType)
        {
            return ((BsonCollectionAttribute)documentType.GetCustomAttributes(typeof(BsonCollectionAttribute), true).FirstOrDefault()).CollectionName;
        }

        /// <summary>
        /// Constructor para inicializar el servicio a través del startup
        /// </summary>
        /// <param name="options"></param>
        public MongoService(IOptions<MongoContext> options)
        {
            var camelCaseConvention = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("CamelCase", camelCaseConvention, type => true);

            var client = new MongoClient(options.Value.ConnectionString);

            var db = client.GetDatabase(options.Value.Database);

            var collectionName = GetCollectionName(typeof(TDocument));

            collection = db.GetCollection<TDocument>(collectionName);
        }

        public async Task<TDocument> FindByFilter(FilterDefinition<TDocument> filter, CancellationToken cancellationToken = default)
        {
            return await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<TDocument> FindById(string Id, CancellationToken cancellationToken = default)
        {
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, Id);

            return await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task UpdateReplaceDocument(TDocument document, CancellationToken cancellationToken = default)
        {
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
            document.LastUpdate = DateTime.UtcNow;
            await collection.FindOneAndReplaceAsync(filter, document, cancellationToken: cancellationToken);
        }

        public async Task InsertDocument(TDocument document, CancellationToken cancellationToken = default)
        {
            await collection.InsertOneAsync(document, cancellationToken: cancellationToken);
        }

        public async Task DeleteById(string Id, CancellationToken cancellationToken = default)
        {
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, Id);
            await collection.FindOneAndDeleteAsync(filter, cancellationToken: cancellationToken);
        }

        public async Task<PaginationEntity<TDocument>> GetPagingInfo(PaginationEntity<TDocument> pagination, FilterDefinition<TDocument> filter)
        {

            var totalDocuments = await collection.CountDocumentsAsync(filter);

            var rounded = Math.Ceiling(totalDocuments / Convert.ToDecimal(pagination.pageSize));

            var totalPages = Convert.ToInt32(rounded);

            pagination.totalPages = totalPages;
            pagination.totalRows = Convert.ToInt32(totalDocuments);

            return pagination;
        }

        public async Task<PaginationEntity<TDocument>> ListByPagination(PaginationEntity<TDocument> pagination, FilterDefinition<TDocument> filter, OrderQuery order = OrderQuery.None, string sortField = default, CancellationToken cancellationToken = default)
        {

            var respTask = order switch
            {
                OrderQuery.Asc => collection
                .Find(filter)
                .Sort(Builders<TDocument>.Sort.Ascending(sortField))
                .Skip((pagination.page - 1) * pagination.pageSize)
                .Limit(pagination.pageSize)
                .ToListAsync(cancellationToken: cancellationToken),

                OrderQuery.Desc => collection
                .Find(filter)
                .Sort(Builders<TDocument>.Sort.Descending(sortField))
                .Skip((pagination.page - 1) * pagination.pageSize)
                .Limit(pagination.pageSize)
                .ToListAsync(cancellationToken: cancellationToken),

                OrderQuery.None => collection
                .Find(filter)
                .Skip((pagination.page - 1) * pagination.pageSize)
                .Limit(pagination.pageSize)
                .ToListAsync(cancellationToken: cancellationToken),

                _ => throw new NotImplementedException("Error, no se encontró el tipo de orden para la consulta."),
            };

            var totalDocumentsTask = collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

            await Task.WhenAll(respTask, totalDocumentsTask);

            long totalDocuments = totalDocumentsTask.Result;

            var rounded = Math.Ceiling(totalDocuments / Convert.ToDecimal(pagination.pageSize));

            var totalPages = Convert.ToInt32(rounded);

            pagination.totalPages = totalPages;
            pagination.totalRows = Convert.ToInt32(totalDocuments);
            pagination.data = respTask.Result;

            return pagination;
        }

        public async Task<PaginationEntity<TResponse>> ListByPagination<TResponse>(IMapper mapper, PaginationEntity<TResponse> pagination, FilterDefinition<TDocument> filter, OrderQuery order = OrderQuery.None, string sortField = default, CancellationToken cancellationToken = default)
        {

            var respInTask = order switch
            {
                OrderQuery.Asc => collection
                .Find(filter)
                .Sort(Builders<TDocument>.Sort.Ascending(sortField))
                .Skip((pagination.page - 1) * pagination.pageSize)
                .Limit(pagination.pageSize)
                .ToListAsync(cancellationToken: cancellationToken),

                OrderQuery.Desc => collection
                .Find(filter)
                .Sort(Builders<TDocument>.Sort.Descending(sortField))
                .Skip((pagination.page - 1) * pagination.pageSize)
                .Limit(pagination.pageSize)
                .ToListAsync(cancellationToken: cancellationToken),

                OrderQuery.None => collection
                .Find(filter)
                .Skip((pagination.page - 1) * pagination.pageSize)
                .Limit(pagination.pageSize)
                .ToListAsync(cancellationToken: cancellationToken),

                _ => throw new NotImplementedException("Error, no se encontró el tipo de orden para la consulta."),
            };

            var totalDocumentsTask = collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

            await Task.WhenAll(respInTask, totalDocumentsTask);

            long totalDocuments = totalDocumentsTask.Result;

            var respOut = mapper.Map<List<TDocument>, List<TResponse>>(respInTask.Result);

            var rounded = Math.Ceiling(totalDocuments / Convert.ToDecimal(pagination.pageSize));

            var totalPages = Convert.ToInt32(rounded);

            pagination.totalPages = totalPages;
            pagination.totalRows = Convert.ToInt32(totalDocuments);
            pagination.data = respOut;

            return pagination;
        }

        public async Task<List<TDocument>> List(OrderQuery order = OrderQuery.None, string sortField = default, CancellationToken cancellationToken = default)
        {
            return order switch
            {
                OrderQuery.Asc => await collection
                .Find(Builders<TDocument>.Filter.Empty)
                .Sort(Builders<TDocument>.Sort.Ascending(sortField))
                .ToListAsync(cancellationToken: cancellationToken),

                OrderQuery.Desc => await collection
                .Find(Builders<TDocument>.Filter.Empty)
                .Sort(Builders<TDocument>.Sort.Descending(sortField))
                .ToListAsync(cancellationToken: cancellationToken),

                OrderQuery.None => await collection
                .Find(Builders<TDocument>.Filter.Empty)
                .ToListAsync(cancellationToken: cancellationToken),

                _ => throw new NotImplementedException("Error, no se encontró el tipo de orden para la consulta."),
            };
        }

        public async Task<List<TDocument>> List(FilterDefinition<TDocument> filter, OrderQuery order = OrderQuery.None, string sortField = null, CancellationToken cancellationToken = default)
        {
            return order switch
            {
                OrderQuery.Asc => await collection
                .Find(filter)
                .Sort(Builders<TDocument>.Sort.Ascending(sortField))
                .ToListAsync(cancellationToken: cancellationToken),

                OrderQuery.Desc => await collection
                .Find(filter)
                .Sort(Builders<TDocument>.Sort.Descending(sortField))
                .ToListAsync(cancellationToken: cancellationToken),

                OrderQuery.None => await collection
                .Find(filter)
                .ToListAsync(cancellationToken: cancellationToken),

                _ => throw new NotImplementedException("Error, no se encontró el tipo de orden para la consulta."),
            };
        }

        public async Task<List<TResponse>> List<TResponse>(IMapper mapper, FilterDefinition<TDocument> filter, OrderQuery order = OrderQuery.None, string sortField = default, CancellationToken cancellationToken = default)
        {
            var resp = order switch
            {
                OrderQuery.Asc => await collection
                .Find(filter)
                .Sort(Builders<TDocument>.Sort.Ascending(sortField))
                .ToListAsync(cancellationToken: cancellationToken),

                OrderQuery.Desc => await collection
                .Find(filter)
                .Sort(Builders<TDocument>.Sort.Descending(sortField))
                .ToListAsync(cancellationToken: cancellationToken),

                OrderQuery.None => await collection
                .Find(filter)
                .ToListAsync(cancellationToken: cancellationToken),

                _ => throw new NotImplementedException("Error, no se encontró el tipo de orden para la consulta."),
            };

            return mapper.Map<List<TDocument>, List<TResponse>>(resp); ;
        }

        public async Task<int> NextNumDocument(string sortField, CancellationToken cancellationToken = default)
        {
            int lastNum = 0;

            var lastDocument = await collection
                .Find(x => true)
                .Sort(Builders<TDocument>.Sort.Descending(sortField))
                .Project<TDocument>(Builders<TDocument>.Projection
                .Include(sortField))
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (lastDocument != null)
                foreach (var item in lastDocument.GetType().GetProperties())
                    if (item.Name == sortField)
                        if (int.TryParse(item.GetValue(lastDocument).ToString(), out int num))
                        {
                            lastNum = num;
                            break;
                        }

            return lastDocument != null ? lastNum + 1 : 1;
        }
    }
}
