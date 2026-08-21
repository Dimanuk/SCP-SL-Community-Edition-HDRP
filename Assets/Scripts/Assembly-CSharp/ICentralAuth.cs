public interface ICentralAuth
{
	void TokenGenerated(string token);

	void RequestBadge(string token);

	void RequestPublicPart(string token);

	void Fail();

	ReferenceHub GetHub();

	void Ok(string userId, string userId2, string ban, string server, bool bypass, bool bypassWl, bool DNT, string serial, string vacSession, string rqIp, string Asn, bool BypassIpCheck);

	void FailToken(string reason);
}
