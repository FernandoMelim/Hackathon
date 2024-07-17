using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using HealthMed.Patient.Domain.Contracts.Authentication;

namespace HealthMed.Patient.Infrastructure.Cognito;

public class CognitoPatientAuthentication(AmazonCognitoIdentityProviderClient cognito) : IPatientAuthentication
{
    public async Task<string> AuthenticatePatient(string patientEmail, string patientPassword)
    {
        var userPoolId = Environment.GetEnvironmentVariable("AWS_PATIENT_POOL_ID");
        var clientId = Environment.GetEnvironmentVariable("AWS_PATIENT_CLIENT_ID_COGNITO");

        var authParameters = new Dictionary<string, string>
        {
            { "USERNAME", patientEmail },
            { "PASSWORD", patientPassword }
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
