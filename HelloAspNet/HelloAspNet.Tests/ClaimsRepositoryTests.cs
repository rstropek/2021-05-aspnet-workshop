using HelloAspNet.Data;
using System;
using System.Threading.Tasks;
using Xunit;

namespace HelloAspNet.Tests
{
    public class ClaimsRepositoryTests
    {
        [Fact]
        public async Task Verify_GetById_After_Add()
        {
            // Prepare
            var repo = new ClaimsRepository();

            // Act
            await repo.AddAsync(new Claim() { ID = 4711 });

            // Verify
            Assert.NotNull(await repo.GetByIdAsync(4711));
        }
    }
}
