using System;
using System.Collections.Generic;

public class AuthenticationValues
{
	private CustomAuthenticationType authType = CustomAuthenticationType.None;

	public CustomAuthenticationType AuthType
	{
		get
		{
			return authType;
		}
		set
		{
			authType = value;
		}
	}

	public string AuthGetParameters { get; set; }

	public object AuthPostData { get; private set; }

	public string Token { get; set; }

	public string UserId { get; set; }

	public AuthenticationValues()
	{
	}

	public AuthenticationValues(string userId)
	{
		UserId = userId;
	}

	public virtual void SetAuthPostData(string stringData)
	{
		AuthPostData = ((!string.IsNullOrEmpty(stringData)) ? stringData : null);
	}

	public virtual void SetAuthPostData(byte[] byteData)
	{
		AuthPostData = byteData;
	}

	public virtual void SetAuthPostData(Dictionary<string, object> dictData)
	{
		AuthPostData = dictData;
	}

	public virtual void AddAuthParameter(string key, string value)
	{
		string text = ((!string.IsNullOrEmpty(AuthGetParameters)) ? "&" : string.Empty);
		AuthGetParameters = $"{AuthGetParameters}{text}{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}";
	}

	public override string ToString()
	{
		return $"AuthenticationValues UserId: {UserId}, GetParameters: {AuthGetParameters} Token available: {Token != null}";
	}
}
