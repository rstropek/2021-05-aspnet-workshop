using HelloAspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HelloAspNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimsController : ControllerBase
    {
        private readonly IClaimsRepository claimsRepository;
        private readonly IConfiguration configuration;
        private readonly ILogger<ClaimsController> logger;

        public ClaimsController(IClaimsRepository claimsRepository, IConfiguration configuration,
            ILogger<ClaimsController> logger)
        {
            this.claimsRepository = claimsRepository;
            this.configuration = configuration;
            this.logger = logger;

            // NEVER DO THAT IN REAL LIFE! Just for demonstrating settings.
            //var rand = new Random();
            //var initialClaims = int.Parse(configuration["InitialClaims"]);
            //for (var i = 0; i < initialClaims; i++)
            //{
            //    claimsRepository.AddAsync(new Claim()
            //    {
            //        ID = rand.Next(),
            //        Contract = $"Contract {i + 1}",
            //        ClaimAmount = 42m,
            //        ClaimTimestamp = DateTime.Today
            //    }).Wait();
            //}
        }

        // GET https://localhost:5001/api/claims -> List of all existing claim
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IEnumerable<GetClaimDto>> GetAllAsync()
        {
            // Authorization code
            if (User.Claims.First(c => c.Type == System.Security.Claims.ClaimTypes.GivenName).Value == "Max")
            {
                // ... (filter database according to identity)
            }

            return (await claimsRepository.GetAsync())
                .Select(claim => new GetClaimDto(claim.ID, claim.Contract, claim.ClaimAmount, claim.ClaimTimestamp));
        }

        public record GetClaimDto(
            int ID,
            string? Contract,
            decimal ClaimAmount,
            DateTime ClaimTimestamp);

        // GET https://localhost:5001/api/claims/4711
        // GET https://localhost:5001/api/contracts/4711/claims -> List of all existing claim for contract 4711
        // GET https://localhost:5001/api/claims?year=2008
        [HttpGet("{claimId}", Name = nameof(GetByIdAsync))]
        public async Task<ActionResult<GetClaimDto>> GetByIdAsync(int claimId)
        {
            var claim = await claimsRepository.GetByIdAsync(claimId);

            if (claim != null) return Ok(new GetClaimDto(claim.ID, claim.Contract, claim.ClaimAmount, claim.ClaimTimestamp));

            // Log not found error
            logger.LogWarning($"Claim with id {claimId} not found");
            return NotFound();
        }

        public record AddClaimDto(
            [Required][MinLength(5)] string? Contract,
            [Required] decimal? ClaimAmount,
            [Required] DateTime? ClaimTimestamp);

        [HttpPost]
        public async Task<ActionResult<Claim>> AddAsync([FromBody] AddClaimDto c)
        {
            // DTO -> BO
            var newClaim = new Claim()
            {
                ID = new Random().Next(100000),
                Contract = c.Contract,
                ClaimAmount = c.ClaimAmount!.Value,
                ClaimTimestamp = c.ClaimTimestamp!.Value
            };

            // In practice, use AutoMapper for that (https://automapper.org/)

            return CreatedAtRoute(
                nameof(GetByIdAsync),
                new { claimId = newClaim.ID },
                await claimsRepository.AddAsync(newClaim));
        }

        [HttpDelete("{claimId}")]
        public async Task<ActionResult> DeleteAsync(int claimId)
        {
            if (await claimsRepository.TryDeleteAsync(claimId)) return NoContent();
            return NotFound();
        }

        public record PatchClaimDto(
            string? Contract,
            decimal? ClaimAmount,
            DateTime? ClaimTimestamp);

        [HttpPatch("{claimId}")]
        public async Task<ActionResult<GetClaimDto>> PatchAsync(int claimId, [FromBody] PatchClaimDto patch)
        {
            var c = await claimsRepository.GetByIdAsync(claimId);
            if (c == null) return NotFound();

            if (patch.Contract != null) c.Contract = patch.Contract;
            if (patch.ClaimAmount != null)
            {
                if (patch.ClaimAmount < 0) return BadRequest("Claim amount must not be negative");
                c.ClaimAmount = patch.ClaimAmount.Value;
            }

            if (patch.ClaimTimestamp != null) c.ClaimTimestamp = patch.ClaimTimestamp.Value;

            return new GetClaimDto(c.ID, c.Contract, c.ClaimAmount, c.ClaimTimestamp);
        }
    }
}
