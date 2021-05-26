using HelloAspNet.Controllers;
using HelloAspNet.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HelloAspNet.Tests
{
    public class ClaimsControllerTests
    {
        [Fact]
        public async Task DeleteAsync_Success()
        {
            var repo = new Mock<IClaimsRepository>();
            repo.Setup(r => r.TryDeleteAsync(It.IsAny<int>())).ReturnsAsync(true);
            var controller = new ClaimsController(repo.Object, Mock.Of<IConfiguration>(), Mock.Of<ILogger<ClaimsController>>());

            var result = await controller.DeleteAsync(4711);

            repo.Verify(r => r.TryDeleteAsync(4711), Times.Once);
            // Or: repo.Verify(r => r.TryDeleteAsync(4711), Times.Exactly(2));
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_NotFound()
        {
            var repo = new Mock<IClaimsRepository>();
            repo.Setup(r => r.TryDeleteAsync(It.IsAny<int>())).ReturnsAsync(false);
            var controller = new ClaimsController(repo.Object, Mock.Of<IConfiguration>(), Mock.Of<ILogger<ClaimsController>>());

            var result = await controller.DeleteAsync(4711);

            repo.Verify(r => r.TryDeleteAsync(4711), Times.Once);
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(true, typeof(NoContentResult))]
        [InlineData(false, typeof(NotFoundResult))]
        public async Task DeleteAsync(bool deleteResult, Type expectedResult)
        {
            var repo = new Mock<IClaimsRepository>();
            repo.Setup(r => r.TryDeleteAsync(It.IsAny<int>())).ReturnsAsync(deleteResult);
            var controller = new ClaimsController(repo.Object, Mock.Of<IConfiguration>(), Mock.Of<ILogger<ClaimsController>>());

            var result = await controller.DeleteAsync(4711);

            repo.Verify(r => r.TryDeleteAsync(4711), Times.Once);
            Assert.IsType(expectedResult, result);
        }
    }
}
