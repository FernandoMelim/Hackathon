using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using HealthMed.Doctor.Domain.Contracts.Authentication;

namespace HealthMed.Doctor.Infrastructure.Cognito;

public class CognitoDoctorAuthentication(AmazonCognitoIdentityProviderClient cognito) : IDoctorAuthentication
{
    public async Task<string> AuthenticateDoctor(string doctorEmail, string doctorPassword)
    {
        var userPoolId = Environment.GetEnvironmentVariable("AWS_DOCTOR_POOL_ID");
        var clientId = Environment.GetEnvironmentVariable("AWS_DOCTOR_CLIENT_ID_COGNITO");

        var authParameters = new Dictionary<string, string>
        {
            { "USERNAME", doctorEmail },
            { "PASSWORD", doctorPassword }
        };

        var request = new AdminInitiateAuthRequest()
        {
            AuthParameters = authParameters,
            ClientId = clientId,
            AuthFlow = "ADMIN_USER_PASSWORD_AUTH",
            UserPoolId = userPoolId
        };

        var response = await cognito.AdminInitiateAuthAsync(request);

        return response.AuthenticationResult.IdToken;
    }
}
