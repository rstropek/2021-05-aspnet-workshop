using HelloAspNet.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
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

        public ClaimsController(IClaimsRepository claimsRepository)
        {
            this.claimsRepository = claimsRepository;
        }

        // GET https://localhost:5001/api/claims -> List of all existing claim
        [HttpGet]
        public async Task<IEnumerable<GetClaimDto>> GetAllAsync()
        {
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
