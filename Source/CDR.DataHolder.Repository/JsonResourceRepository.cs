using AutoMapper;
using CDR.DataHolder.Domain.Entities;
using CDR.DataHolder.Domain.Repositories;
using CDR.DataHolder.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using JsonDB = JsonFlatFileDataStore.DataStore;

namespace CDR.DataHolder.Repository
{
    public class JsonResourceRepository : IResourceRepository, IDisposable
    {
        private readonly JsonDB _jsonDB;
        private readonly string _dataHolderId;
        private readonly IMapper _mapper;

        public JsonResourceRepository(string path, string dataHolderId, IMapper mapper)
        {
            _jsonDB = new JsonDB(path);
            _dataHolderId = dataHolderId;
            _mapper = mapper;
        }

        public async Task<Page<Account[]>> GetAllAccounts(AccountFilter filter, int page, int pageSize)
        {
            var result = new Page<Account[]>()
            {
                Data = Array.Empty<Account>(),
                CurrentPage = page,
                PageSize = pageSize,
            };

            // We always return accounts for the individual. We don't have a concept of joint or shared accounts at the moment
            // So, if asked from accounts which rent owned, just return empty result.
            if (filter.IsOwned.HasValue && !filter.IsOwned.Value)
            {
                return result;
            }

            // If none of the account ids are allowed, return empty list
            if (filter.AllowedAccountIds == null || !filter.AllowedAccountIds.Any())
            {
                return result;
            }

            IEnumerable<dynamic> dynamicCollection = string.IsNullOrEmpty(_dataHolderId) ?
                _jsonDB.GetCollection("holders").AsQueryable().FirstOrDefault().holder.authenticated.customers :
                _jsonDB.GetCollection("holders").AsQueryable().FirstOrDefault(holder => holder.holderId == _dataHolderId).holder.authenticated.customers;

            // EBA: conditional in the Where clause on the presence of field is required as those fields are dynamic
            // and not schematically defined in the test data, for purposes of using non-compliant test data fields.
            var accountsQuery = dynamicCollection.Where(c => c.customerId == filter.CustomerId)
                .SelectMany<dynamic, dynamic>(c => c.banking.accounts)
                .Where(a => ((IDictionary<string, object>)a.account).ContainsKey("accountId") && filter.AllowedAccountIds.Contains((string)a.account.accountId));

            // Apply filters.
            if (!string.IsNullOrEmpty(filter.OpenStatus))
            {
                accountsQuery = accountsQuery.Where(a => ((IDictionary<string, object>)a.account).ContainsKey("openStatus") && 
                    a.account.openStatus == filter.OpenStatus);
            }

            if (!string.IsNullOrEmpty(filter.ProductCategory))
            {
                accountsQuery = accountsQuery.Where(a => ((IDictionary<string, object>)a.account).ContainsKey("productCategory") && 
                    a.account.productCategory == filter.ProductCategory);
            }

            accountsQuery = accountsQuery.Select(a => a.account);

            // Apply ordering and pagination.
            accountsQuery = accountsQuery
                .OrderBy(ad => ((IDictionary<string, object>)ad).ContainsKey("displayName") ? ad.displayName : string.Empty)
                .ThenBy(ad => ((IDictionary<string, object>)ad).ContainsKey("accountId") ? ad.accountId : string.Empty)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var accounts = JsonConvert.DeserializeObject<Account[]>(JsonConvert.SerializeObject(accountsQuery));
            foreach (var account in accounts)
            {
                account.CustomerId = filter.CustomerId;
            }

            result.Data = accounts;
            result.TotalRecords = accounts.Count();

            return await Task.FromResult(result);
        }

        public async Task<Account[]> GetAllAccountsByCustomerIdForConsent(string customerId)
        {
            IEnumerable<dynamic> dynamicCollection = string.IsNullOrEmpty(_dataHolderId) ?
                _jsonDB.GetCollection("holders").AsQueryable().FirstOrDefault().holder.authenticated.customers :
                _jsonDB.GetCollection("holders").AsQueryable().FirstOrDefault(holder => holder.holderId == _dataHolderId).holder.authenticated.customers;

            var accountsQuery = dynamicCollection.Where(c => c.customerId == customerId)
                .SelectMany<dynamic, dynamic>(c => c.banking.accounts)
                .Select(a => a.account);

            // Apply ordering.
            accountsQuery = accountsQuery
                .OrderBy(ad => ((IDictionary<string, object>)ad).ContainsKey("displayName") ? ad.displayName : string.Empty)
                .ThenBy(ad => ((IDictionary<string, object>)ad).ContainsKey("accountId") ? ad.accountId : string.Empty);

            var accounts = JsonConvert.DeserializeObject<Account[]>(JsonConvert.SerializeObject(accountsQuery));
            foreach (var account in accounts)
            {
                account.CustomerId = customerId;
            }

            return await Task.FromResult(accounts);
        }

        /// <summary>
        /// Get a list of all transactions for a given account.
        /// </summary>
        /// <param name="transactionsFilter">Query filter</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns></returns>
        public async Task<Page<AccountTransaction[]>> GetAccountTransactions(AccountTransactionsFilter filter, int page, int pageSize)
        {
            var result = new Page<AccountTransaction[]>()
            {
                Data = Array.Empty<AccountTransaction>(),
                CurrentPage = page,
                PageSize = pageSize,
            };

            if (!filter.NewestTime.HasValue)
            {
                filter.NewestTime = DateTime.UtcNow;
            }

            if (!filter.OldestTime.HasValue)
            {
                filter.OldestTime = filter.NewestTime.Value.AddDays(-90);
            }

            IEnumerable<dynamic> dynamicCollection = string.IsNullOrEmpty(_dataHolderId) ?
                _jsonDB.GetCollection("holders").AsQueryable().FirstOrDefault().holder.authenticated.customers :
                _jsonDB.GetCollection("holders").AsQueryable().FirstOrDefault(holder => holder.holderId == _dataHolderId).holder.authenticated.customers;

            // EBA: conditional in the Where clause on the presence of field is required as those fields are dynamic
            // and not schematically defined in the test data, for purposes of using non-compliant test data fields.
            var accountQuery = dynamicCollection.Where(c => c.customerId == filter.CustomerId)
                .SelectMany<dynamic, dynamic>(c => c.banking.accounts)
                .Where(a => ((IDictionary<string, object>)a.account).ContainsKey("accountId") && a.account.accountId == filter.AccountId);

            var transactionQuery = accountQuery.SelectMany<dynamic, dynamic>(a => a.transactions);
                
            // Apply filters.

            // Oldest / Newest Time.

            // Newest
            transactionQuery = transactionQuery.WhereIf(filter.NewestTime.HasValue, t =>  
                (((IDictionary<string, object>)t).ContainsKey("postingDateTime") && t.postingDateTime <= filter.NewestTime) ||
                (((IDictionary<string, object>)t).ContainsKey("executionDateTime") && t.executionDateTime <= filter.NewestTime));

            // Oldest
            transactionQuery = transactionQuery.WhereIf(filter.OldestTime.HasValue, t =>
                (((IDictionary<string, object>)t).ContainsKey("postingDateTime") && t.postingDateTime >= filter.OldestTime) ||
                (((IDictionary<string, object>)t).ContainsKey("executionDateTime") && t.executionDateTime >= filter.OldestTime));

            // Min / Max Amount.

            // Min.
            transactionQuery = transactionQuery.WhereIf(filter.MinAmount.HasValue, t =>
                ((IDictionary<string, object>)t).ContainsKey("amount") && t.amount >= filter.MinAmount.Value);

            // Max.
            transactionQuery = transactionQuery.WhereIf(filter.MaxAmount.HasValue, t =>
                ((IDictionary<string, object>)t).ContainsKey("amount") && t.amount <= filter.MaxAmount.Value);

            // Text.
            transactionQuery.WhereIf(!string.IsNullOrEmpty(filter.Text), t =>
                (((IDictionary<string, object>)t).ContainsKey("description") && EF.Functions.Like((string)t.description, $"%{filter.Text}%")) ||
                (((IDictionary<string, object>)t).ContainsKey("reference") && EF.Functions.Like((string)t.reference, $"%{filter.Text}%")));

            // Apply ordering and pagination
            transactionQuery = transactionQuery
                .OrderByDescending(t => ((IDictionary<string, object>)t).ContainsKey("postingDateTime") ? t.postingDateTime : string.Empty)
                .ThenByDescending(t => ((IDictionary<string, object>)t).ContainsKey("executionDateTime") ? t.executionDateTime : string.Empty)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var transactions = JsonConvert.DeserializeObject<AccountTransaction[]>(JsonConvert.SerializeObject(transactionQuery));
            
            result.Data = transactions;
            result.TotalRecords = transactions.Count();

            return await Task.FromResult(result);
        }

        public async Task<Customer> GetCustomer(string customerId)
        {
            IEnumerable<dynamic> dynamicCollection = string.IsNullOrEmpty(_dataHolderId) ?
                _jsonDB.GetCollection("holders").AsQueryable().FirstOrDefault().holder.authenticated.customers :
                _jsonDB.GetCollection("holders").AsQueryable().FirstOrDefault(holder => holder.holderId == _dataHolderId).holder.authenticated.customers;

            var customerQuery = dynamicCollection.Where(c => c.customerId == customerId)
                .Select(cd => cd.customer)
                .FirstOrDefault();

            Customer customer = null;
            if (customerQuery != null)
            {
                string customerType = customerQuery.customerUType;
                switch (customerType)
                {
                    case "organisation":
                        customer = JsonConvert.DeserializeObject<Organisation>(JsonConvert.SerializeObject(customerQuery.organisation));
                        customer.CustomerId = customerId;
                        break;

                    case "person":
                        customer = JsonConvert.DeserializeObject<Person>(JsonConvert.SerializeObject(customerQuery.person));
                        customer.CustomerId = customerId;
                        break;

                    default:
                        customer = null;
                        break;
                }
            }

            return await Task.FromResult(customer);
        }

        public async Task<Customer> GetCustomerByLoginId(string loginId)
        {
            IEnumerable<dynamic> dynamicCollection = string.IsNullOrEmpty(_dataHolderId) ?
                _jsonDB.GetCollection("holders").AsQueryable().FirstOrDefault().holder.authenticated.customers :
                _jsonDB.GetCollection("holders").AsQueryable().FirstOrDefault(holder => holder.holderId == _dataHolderId).holder.authenticated.customers;

            var customerQuery = dynamicCollection.FirstOrDefault(c => ((IDictionary<string, object>)c).ContainsKey("loginId") && c.loginId == loginId);
                
            Customer customer = null;
            if (customerQuery != null)
            {
                customer = JsonConvert.DeserializeObject<Customer>(JsonConvert.SerializeObject(customerQuery));
            }

            return await Task.FromResult(customer);
        }

        /// <summary>
        /// Check that the customer can access the given accounts.
        /// </summary>
        /// <param name="accountId">Account ID</param>
        /// <param name="customerId">Customer ID</param>
        /// <returns>True if the customer can access the account, otherwise false.</returns>
        public async Task<bool> CanAccessAccount(string accountId, string customerId)
        {
            IEnumerable<dynamic> dynamicCollection = string.IsNullOrEmpty(_dataHolderId) ?
                _jsonDB.GetCollection("holders").AsQueryable().FirstOrDefault().holder.authenticated.customers :
                _jsonDB.GetCollection("holders").AsQueryable().FirstOrDefault(holder => holder.holderId == _dataHolderId).holder.authenticated.customers;

            var canAccess = dynamicCollection.Where(c => c.customerId == customerId)
                .SelectMany<dynamic, dynamic>(c => c.banking.accounts)
                .Any(a => ((IDictionary<string, object>)a.account).ContainsKey("accountId") && a.account.accountId == accountId);

            return await Task.FromResult(canAccess);
        }

        public void Dispose()
        {
            _jsonDB.Dispose();
        }
    }
}
