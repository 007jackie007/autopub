﻿// <directives>
using Azure.Core;
using System;
using Microsoft.Identity.Client;
using System.Threading;
using System.Threading.Tasks;
// <directives>

public class MyClientAssertionCredential : TokenCredential
{
    private readonly IConfidentialClientApplication _confidentialClientApp;

    public MyClientAssertionCredential()
    {
        // <authentication>
        // Azure AD Workload Identity webhook will inject the following env vars
        // 	AZURE_CLIENT_ID with the clientID set in the service account annotation
        // 	AZURE_TENANT_ID with the tenantID set in the service account annotation. If not defined, then
        // 		the tenantID provided via azure-wi-webhook-config for the webhook will be used.
        // 	AZURE_FEDERATED_TOKEN_FILE is the service account token path
        //var clientID = "be012b00-08d1-43bb-8cbd-42a5cb2ee4da";
        var clientID = "cd49ccb2-90b7-4e03-b52a-550eae87c50f";
        var tokenPath = "/var/run/secrets/azure/tokens/azure-identity-token";
        var tenantID = "4665a88d-987e-4633-9eaf-2d672f22c04d";

        _confidentialClientApp = ConfidentialClientApplicationBuilder.Create(clientID)
                .WithClientAssertion(ReadJWTFromFS(tokenPath))
                .WithTenantId(tenantID).Build();
    }

    public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
    {
        return GetTokenAsync(requestContext, cancellationToken).GetAwaiter().GetResult();
    }

    public override async ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
    {
        AuthenticationResult result = null;
        try
        {
            result = await _confidentialClientApp
                        .AcquireTokenForClient(requestContext.Scopes)
                        .ExecuteAsync();
        }
        catch (MsalUiRequiredException ex)
        {
            // The application doesn't have sufficient permissions.
            // - Did you declare enough app permissions during app creation?
            // - Did the tenant admin grant permissions to the application?
        }
        catch (MsalServiceException ex) when (ex.Message.Contains("AADSTS70011"))
        {
            // Invalid scope. The scope has to be in the form "https://resourceurl/.default"
            // Mitigation: Change the scope to be as expected.
        }
        return new AccessToken(result.AccessToken, result.ExpiresOn);
    }

    public string ReadJWTFromFS(string tokenPath)
    {
        string text = System.IO.File.ReadAllText(tokenPath);
        return text;
    }
}