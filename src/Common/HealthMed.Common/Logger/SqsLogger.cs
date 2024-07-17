using Amazon.SQS;
using Amazon.SQS.Model;

namespace HealthMed.Common.Logger;

public class SqsLogger
{
    private readonly AmazonSQSClient _sqsClient;
    private readonly string _awsSqs;
    private readonly string _awsSqsGroupId;

    public SqsLogger(AmazonSQSClient sqsClient)
    {
        _sqsClient = sqsClient;
        _awsSqs = Environment.GetEnvironmentVariable("AWS_SQS_LOG");
        _awsSqsGroupId = Environment.GetEnvironmentVariable("AWS_SQS_GROUP_ID_LOG");
    }

    public async Task Log(string stackTrace, string message, string exception)
    {
        Dictionary<string, MessageAttributeValue> messageAttributes = new Dictionary<string, MessageAttributeValue>
        {
            { "Service",   new MessageAttributeValue { DataType = "String", StringValue = "HealthMed" } },
            { "StackTrace",   new MessageAttributeValue { DataType = "String", StringValue = stackTrace } },
            { "ExceptionMessage",  new MessageAttributeValue { DataType = "String", StringValue = message } },
            { "Ex", new MessageAttributeValue { DataType = "String", StringValue = exception } },
            { "Time", new MessageAttributeValue { DataType = "String", StringValue = DateTime.Now.ToString() } }
        };

        var sendMessageRequest = new SendMessageRequest
        {
            QueueUrl = _awsSqs,
            MessageBody = message,
            MessageGroupId = _awsSqsGroupId,
            MessageAttributes = messageAttributes,
            MessageDeduplicationId = Guid.NewGuid().ToString()
        };

        var sendMessageResponse = await _sqsClient.SendMessageAsync(sendMessageRequest);
    }
}
