using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using System.Text.Json;

namespace DownloaderException.Exceptions;

public class CloudWatchLogger
{
    private readonly IAmazonCloudWatchLogs _client;
    private readonly string _logGroupName = "/aws/lambda/CloudWatchLogsTest";
    private readonly string _logStreamName;

    public CloudWatchLogger(string logStreamName, IAmazonCloudWatchLogs client = null)
    {
        _client = client ?? new AmazonCloudWatchLogsClient();
        _logStreamName = logStreamName;
    }

    public async Task SendToCloudWatchAsync(string message)
    {
        var formattedMessage = FormatMessage(message);

        var logEvent = new InputLogEvent
        {
            Timestamp = DateTime.UtcNow,
            Message = formattedMessage
        };

        var request = new PutLogEventsRequest
        {
            LogGroupName = _logGroupName,
            LogStreamName = _logStreamName,
            LogEvents = new List<InputLogEvent> { logEvent }
        };

        await _client.PutLogEventsAsync(request);
    }

    private string FormatMessage(string message)
    {
        var exceptionData = new
        {
            Application = "LambdaSet",
            Message = message,
            Time = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
        };

        return JsonSerializer.Serialize(exceptionData);
    }
}