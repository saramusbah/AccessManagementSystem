namespace AccessManagementSystem.API
{
    public class IdentityConfig
    {
        /// <summary>
        /// Gets or sets the security key used to sign JWT tokens.
        /// </summary>
        public string SecurityKey { get; set; }

        /// <summary>
        /// Gets or sets the issuer of tokens.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Gets or sets the audience for which the tokens are issued.
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// Gets or sets the token expiry period in minutes.
        /// </summary>
        public long TokenExpiryInMinutes { get; set; }
    }
}