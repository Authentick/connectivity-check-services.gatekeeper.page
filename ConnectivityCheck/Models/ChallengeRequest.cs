using System;

namespace ConnectivityCheck.Models
{
    public class ChallengeRequest
    {
        public Guid Challenge { get; set; }
        public string PublicHostname { get; set; }
    }
}
