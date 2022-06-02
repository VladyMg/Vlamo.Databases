using AutoMapper;
using Mongo.Domain.Interfaces;
using Mongo.Domain.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Mongo.Service.Interfaces
{
    public interface IMongoService<TDocument> where TDocument : IDocument
    {
        public IMongoCollection<TDocument> collection { get; set; }

        /// <summary>
        /// Function to get single document by filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<TDocument> FindByFilter(FilterDefinition<TDocument> filter, CancellationToken cancellationToken = default);

        /// <summary>
        /// Functino to get single document by id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<TDocument> FindById(string Id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Function to update and replace a document
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        Task UpdateReplaceDocument(TDocument document, CancellationToken cancellationToken = default);

        /// <summary>
        /// Function to insert one document document 
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        Task InsertDocument(TDocument document, CancellationToken cancellationToken = default);

        /// <summary>
        /// Function to delete document by id 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task DeleteById(string Id, CancellationToken cancellationToken = default);


        /// <summary>
        /// Function to get Pagination Info of collection
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<PaginationEntity<TDocument>> GetPagingInfo(PaginationEntity<TDocument> pagination, FilterDefinition<TDocument> filter);

        /// <summary>
        /// /// Function to get a list of documents using pagination
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="filter"></param>
        /// <param name="order"></param>
        /// <param name="sortField"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<PaginationEntity<TDocument>> ListByPagination(PaginationEntity<TDocument> pagination, FilterDefinition<TDocument> filter, OrderQuery order = OrderQuery.None, string sortField = default, CancellationToken cancellationToken = default);

        /// <summary>
        /// Function to get list of documents using pagination mapping new object
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="mapper">Dependeci to map source object to out object</param>
        /// <param name="pagination"></param>
        /// <param name="filter"></param>
        /// <param name="order"></param>
        /// <param name="sortField"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<PaginationEntity<TResponse>> ListByPagination<TResponse>(IMapper mapper, PaginationEntity<TResponse> pagination, FilterDefinition<TDocument> filter, OrderQuery order = OrderQuery.None, string sortField = default, CancellationToken cancellationToken = default);

        /// <summary>
        /// /// Function to get all documents in a collection
        /// </summary>
        /// <param name="order"></param>
        /// <param name="sortField"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<TDocument>> List(OrderQuery order = OrderQuery.None, string sortField = default, CancellationToken cancellationToken = default);

        /// <summary>
        /// Function to get all documents using filter
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="order"></param>
        /// <param name="sortField"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<TDocument>> List(FilterDefinition<TDocument> filter, OrderQuery order = OrderQuery.None, string sortField = default, CancellationToken cancellationToken = default);

        /// <summary>
        /// Function to get all documents in collection mapping items
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="mapper">Dependeci to map source object to out object</param>
        /// <param name="filter"></param>
        /// <param name="order"></param>
        /// <param name="sortField"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<TResponse>> List<TResponse>(IMapper mapper, FilterDefinition<TDocument> filter, OrderQuery order = OrderQuery.None, string sortField = default, CancellationToken cancellationToken = default);


        /// <summary>
        /// Function to get last document and increment counter of documents manual
        /// </summary>
        /// <param name="sortField"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> NextNumDocument(string sortField, CancellationToken cancellationToken = default);

    }
}
