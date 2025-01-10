using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SendGrid;

namespace HealthCheckApp.Helpers;

public class SendGridHealthCheck : IHealthCheck
{
    private readonly ISendGridClient _sendGridClient;

    public SendGridHealthCheck(ISendGridClient sendGridClient)
    {
        _sendGridClient = sendGridClient;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Ping SendGrid by attempting a simple API request
            var response = await _sendGridClient.RequestAsync(
                method: SendGridClient.Method.GET,
                urlPath: "user/profile",
                cancellationToken: cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return HealthCheckResult.Healthy("SendGrid is reachable.");
            }

            return HealthCheckResult.Unhealthy("SendGrid responded with an error status code.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Error reaching SendGrid.", ex);
        }
    }
}

