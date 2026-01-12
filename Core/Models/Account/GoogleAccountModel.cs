using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Models.Account
{
	public class GoogleAccountModel
	{
		[JsonPropertyName("id")]
		public string GogoleId { get; set; } = string.Empty;
		[JsonPropertyName("email")]
		public string Email { get; set; } = string.Empty;

		[JsonPropertyName("given_name")]
		public string FirstName { get; set; } = string.Empty;
		[JsonPropertyName("family_name")]
		public string LastName { get; set; } = string.Empty;
		[JsonPropertyName("picture")]
		public string Picture { get; set; } = string.Empty;
	}
}
