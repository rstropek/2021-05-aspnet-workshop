using System;
using System.ComponentModel.DataAnnotations;

namespace HelloAspNet.Data
{
    // Datenstruktur, wie sie im "Backend" (=DB) abgebildet ist.

    public class Claim
    {
        public int ID { get; set; }

        [Required]
        [MinLength(5)]
        public string? Contract { get; set; }

        public decimal ClaimAmount { get; set; }

        public DateTime ClaimTimestamp { get; set; }
    }
}
