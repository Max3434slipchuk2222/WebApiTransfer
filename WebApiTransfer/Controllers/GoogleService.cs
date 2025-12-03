using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Services;

public class GoogleService
{
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly IConfiguration _config;

	public GoogleService(IHttpClientFactory httpClientFactory, IConfiguration config)
	{
		_httpClientFactory = httpClientFactory;
		_config = config;
	}

	public async Task<GoogleTokenInfo?> VerifyGoogleToken(string idToken)
	{
		var client = _httpClientFactory.CreateClient();
		var response = await client.GetAsync($"https://oauth2.googleapis.com/tokeninfo?id_token={idToken}");

		if (!response.IsSuccessStatusCode) return null;

		var json = await response.Content.ReadAsStringAsync();
		var info = JsonSerializer.Deserialize<GoogleTokenInfo>(json);
		if (info?.Aud != _config["Google:ClientId"])
			throw new Exception("Invalid Client ID");

		return info;
	}
}
public class GoogleTokenInfo
{
	[JsonPropertyName("email")] public string Email { get; set; } = null!;
	[JsonPropertyName("given_name")] public string GivenName { get; set; } = null!;
	[JsonPropertyName("family_name")] public string FamilyName { get; set; } = null!;
	[JsonPropertyName("picture")] public string Picture { get; set; } = null!;
	[JsonPropertyName("aud")] public string Aud { get; set; } = null!;
	[JsonPropertyName("sub")]
	public string Subject { get; set; } = null!;
}
