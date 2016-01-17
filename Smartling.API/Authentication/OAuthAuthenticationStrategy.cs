﻿using System;

namespace Smartling.Api.Authentication
{
  public class OAuthAuthenticationStrategy : IAuthenticationStrategy
  {
    private DateTime LastSuccessfullUpdate = DateTime.MinValue;
    private AuthResponse LastResponse;
    private readonly AuthApiClient client;

    public OAuthAuthenticationStrategy(string userIdentifier, string userSecret)
    {
      client = new AuthApiClient(userIdentifier, userSecret);
    }

    public OAuthAuthenticationStrategy(AuthApiClient client)
    {
      this.client = client;
    }

    public string GetToken()
    {
      if (LastResponse != null && DateTime.UtcNow < LastSuccessfullUpdate.AddSeconds(LastResponse.data.expiresIn))
      {
        return LastResponse.data.accessToken;
      }

      if (LastResponse != null && DateTime.UtcNow < LastSuccessfullUpdate.AddSeconds(LastResponse.data.refreshExpiresIn))
      {
        LastResponse = client.Refresh(LastResponse.data.refreshToken);
      }
      else
      {
        LastResponse = client.Authenticate();
      }

      LastSuccessfullUpdate = DateTime.UtcNow;
      return LastResponse.data.accessToken;
    }
  }
}
