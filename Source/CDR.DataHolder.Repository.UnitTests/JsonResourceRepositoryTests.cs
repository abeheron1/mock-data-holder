using AutoMapper;
using CDR.DataHolder.Domain.ValueObjects;
using CDR.DataHolder.Repository.Entities.Json;
using CDR.DataHolder.Repository.Infrastructure;

namespace CDR.DataHolder.Repository.UnitTests
{
    public class JsonResourceRepositoryTests
    {
        private const string jsonPath = "banking-seed.json";

        [Fact]
        public async Task JsonResourceRepositoryCanLoadTestData()
        {
            JsonFlatFileDataStore.DataStore ds = new JsonFlatFileDataStore.DataStore(jsonPath);

            // Typed Accounts.
            var holders = ds.GetCollection<Holders>().AsQueryable();
            Assert.True(holders.Count() > 0);

            var defaultDataHolder = holders.FirstOrDefault()?.holder;
            Assert.NotNull(defaultDataHolder);

            // Typed ClientCache.
            var clientCache = ds.GetCollection<ClientCache>();
            Assert.True(clientCache.Count > 0);

            // Typed RegisterCache.
            var registerCache = ds.GetCollection<RegisterCache>();
            Assert.True(registerCache.Count > 0);

            // Dynamic query.
            var dynamicHolder = ds.GetCollection("holders").AsQueryable().FirstOrDefault(p => p.holderId == "700992444");

            JsonResourceRepository repository = new JsonResourceRepository(jsonPath, string.Empty, null);

            Assert.True(true);
        }

        [Fact]
        public async Task Given_GetAllAccounts_When_InvalidCustomer_Then_ReturnEmpty()
        {            
            var automapConfig = new MapperConfiguration(cfg => {
                cfg.AddProfile<MappingProfile>();
            });

            var mapper = new Mapper(automapConfig);

            var repository = new JsonResourceRepository(jsonPath, string.Empty, mapper);

            var allowedAccountIds = new[] { "283467960" };
            var accountFilter = new AccountFilter(allowedAccountIds)
            {
                CustomerId = "0",
                OpenStatus = null,
                ProductCategory = null,
            };

            var accounts = await repository.GetAllAccounts(accountFilter, 1, 10);

            Assert.True(accounts.TotalRecords == 0);
        }

        [Fact]
        public async Task Given_GetAllAccounts_When_ValidCustomer_Then_ReturnAccounts()
        {
            var repository = new JsonResourceRepository(jsonPath, string.Empty, null);

            var allowedAccountIds = new[] { "283467960", "383467960" };
            var accountFilter = new AccountFilter(allowedAccountIds)
            {
                CustomerId = "100451449",
                OpenStatus = null,
                ProductCategory = null,
            };

            var accounts = await repository.GetAllAccounts(accountFilter, 1, 10);

            Assert.True(accounts.TotalRecords > 0);
        }

        [Fact]
        public async Task Given_GetAllAccountsByCustomerIdForConsent_When_ValidCustomer_Then_ReturnAccounts()
        {
            var repository = new JsonResourceRepository(jsonPath, string.Empty, null);

            var accounts = await repository.GetAllAccountsByCustomerIdForConsent("100451449");

            Assert.True(accounts.Count() > 0);
        }

        [Fact]
        public async Task Given_CanAccessAccount_When_CustomerAccountMatch_Then_ReturnTrue()
        {
            var repository = new JsonResourceRepository(jsonPath, string.Empty, null);

            var canAccess = await repository.CanAccessAccount("383467960", "100451449");

            Assert.True(canAccess);
        }

        [Fact]
        public async Task Given_CanAccessAccount_When_CustomerAccountMismatch_Then_ReturnFalse()
        {
            var repository = new JsonResourceRepository(jsonPath, string.Empty, null);

            var canAccess = await repository.CanAccessAccount("111", "100451449");

            Assert.False(canAccess);
        }

        [Fact]
        public async Task Given_GetCustomer_When_ValidCustomer_Then_ReturnCustomer()
        {
            var repository = new JsonResourceRepository(jsonPath, string.Empty, null);

            var customer = await repository.GetCustomer("100451449");

            Assert.NotNull(customer);
        }

        [Fact]
        public async Task Given_GetCustomer_When_InvalidCustomer_Then_ReturnNull()
        {
            var repository = new JsonResourceRepository(jsonPath, string.Empty, null);

            var customer = await repository.GetCustomer("111");

            Assert.Null(customer);
        }

        [Fact]
        public async Task Given_GetCustomerByLoginId_When_ValidLoginId_Then_ReturnCustomer()
        {
            var repository = new JsonResourceRepository(jsonPath, string.Empty, null);

            var customer = await repository.GetCustomerByLoginId("jojo");

            Assert.NotNull(customer);
        }

        [Fact]
        public async Task Given_GetCustomerByLoginId_When_InvalidLoginId_Then_ReturnNull()
        {
            var repository = new JsonResourceRepository(jsonPath, string.Empty, null);

            var customer = await repository.GetCustomerByLoginId("111");

            Assert.Null(customer);
        }

        [Fact]
        public async Task Given_GetAccountTransactions_When_ValidCustomerAndAccount_Then_ReturnTransactions()
        {
            var repository = new JsonResourceRepository(jsonPath, string.Empty, null);

            var transactionFilter = new AccountTransactionsFilter
            {
                CustomerId = "100451449",
                AccountId = "283467960",
                OldestTime = DateTime.MinValue,
                NewestTime = DateTime.MaxValue,
            };

            var transactions = await repository.GetAccountTransactions(transactionFilter, 1, 10);

            Assert.True(transactions.TotalRecords > 0);
        }

        [Fact]
        public async Task Given_GetAccountTransactions_When_ValidCustomerAndInvalidAccount_Then_ReturnEmpty()
        {
            var repository = new JsonResourceRepository(jsonPath, string.Empty, null);

            var transactionFilter = new AccountTransactionsFilter
            {
                CustomerId = "100451449",
                AccountId = "111",
                OldestTime = DateTime.MinValue,
                NewestTime = DateTime.MaxValue,
            };

            var transactions = await repository.GetAccountTransactions(transactionFilter, 1, 10);

            Assert.True(transactions.TotalRecords == 0);
        }
    }
}