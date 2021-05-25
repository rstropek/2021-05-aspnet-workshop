using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloAspNet.Data
{
    public interface IClaimsRepository
    {
        Task<Claim> AddAsync(Claim c);
        Task<IEnumerable<Claim>> GetAsync(Func<Claim, bool>? predicate = null);
        Task<Claim?> GetByIdAsync(int claimId);
        Task<bool> TryDeleteAsync(int claimId);
    }

    public class ClaimsRepository : IClaimsRepository
    {
        // NOTE: This class is NOT thread-safe. In practice, implement it thread-safe,
        //       because web servers will access the list concurrently!

        private readonly List<Claim> claims = new();

        public async Task<Claim> AddAsync(Claim c)
        {
            // Simulate DB access
            await Task.Delay(10);

            claims.Add(c);
            return c;
        }

        public async Task<bool> TryDeleteAsync(int claimId)
        {
            var claimToDelete = claims.FirstOrDefault(c => c.ID == claimId);
            if (claimToDelete != null)
            {
                // Simulate DB access
                await Task.Delay(10);

                claims.Remove(claimToDelete);
                return true;
            }

            return false;
        }

        public async Task<IEnumerable<Claim>> GetAsync(Func<Claim, bool>? predicate = null)
        {
            // Simulate DB access
            await Task.Delay(10);

            IEnumerable<Claim> result = claims;
            if (predicate != null)
            {
                result = result.Where(predicate);
            }

            return result.ToList();
        }

        public async Task<Claim?> GetByIdAsync(int claimId)
        {
            // Simulate DB access
            await Task.Delay(10);

            return claims.FirstOrDefault(c => c.ID == claimId);
        }
    }
}
