using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Org.BouncyCastle.Crypto;

namespace LocalCentralServer
{
    /// <summary>
    /// Builds the two token kinds the game expects, signed with the central private key.
    /// Formats are dictated by the game's verifiers:
    ///   - preauth: CustomLiteNetLib4MirrorTransport.ProcessConnectionRequest
    ///              VerifyBytes($"{UserId};{Flags};{Country};{Expiration}", sig, ServerConsole.PublicKey)
    ///   - auth:    CentralAuth._ValidateToken — "Key: Value&lt;br&gt;...&lt;br&gt;Signature: &lt;base64&gt;"
    /// </summary>
    internal static class Tokens
    {
        private const string IssuedBy = "LocalCentral";

        /// <summary>base64 ECDSA signature for the transport preauth token. Expiration is a Unix timestamp (seconds).</summary>
        public static string BuildPreauthSignature(string userId, byte flags, string country, long expiration, AsymmetricKeyParameter centralPriv)
        {
            string data = $"{userId};{flags};{country};{expiration}";
            return Crypto.Sign(data, centralPriv);
        }

        /// <summary>
        /// Full authentication token (raw text, NOT base64). The caller base64-encodes it into the
        /// requestsignature.php "auth" field (the game base64-decodes it back in RequestSignatureResponse).
        /// </summary>
        /// <param name="publicKeyHashBase64">
        /// Base64(UTF8(hexSha256(clientSessionPublicKeyPem))). The game (ServerRoles.CmdServerSignatureComplete)
        /// compares the token's "Public key" field against Base64Encode(HashToString(Sha256(clientPubKey))),
        /// so without it the post-auth challenge is rejected and the player never gets a UserId.
        /// </param>
        public static string BuildAuthToken(string userId, string nicknameBase64, string publicKeyHashBase64, AsymmetricKeyParameter centralPriv)
        {
            DateTime now = DateTime.UtcNow;
            string issuance = now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            string expiration = now.AddHours(6).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            // Order is irrelevant (the game parses into a dictionary); we just must not break "Key: Value".
            List<string> parts = new List<string>
            {
                "Nickname: " + nicknameBase64,
                "User ID: " + userId,
                "Public key: " + (publicKeyHashBase64 ?? string.Empty),
                "Usage: Authentication",
                "Test signature: NO",
                "Auth Version: 2",
                "Issuance time: " + issuance,
                "Expiration time: " + expiration,
                "Issued by: " + IssuedBy,
                "Global ban: NO",
                "Bypass bans: NO",
                "Bypass WL: NO",
                "Do Not Track: NO",
                "Serial: " + Guid.NewGuid().ToString("N").Substring(0, 12).ToUpperInvariant(),
                "VAC session: N/A",
                "Request IP: N/A",
                "ASN: N/A",
                "Skip IP Check: YES"
            };

            string text = string.Join("<br>", parts);
            string signature = Crypto.Sign(text, centralPriv);
            return text + "<br>Signature: " + signature;
        }

        public static string EncodeNickname(string nickname)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(nickname ?? "Player"));
        }
    }
}
