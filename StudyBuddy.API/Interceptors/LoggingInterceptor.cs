namespace StudyBuddy.API.Interceptors;

using Castle.DynamicProxy;
using System;
using System.IO;
using System.Text;

public class LoggingInterceptor : IInterceptor
{
    private readonly string _logFilePath;

    public LoggingInterceptor(string logFilePath)
    {
        _logFilePath = logFilePath;
    }

    public void Intercept(IInvocation invocation)
    {
        var logEntry = new StringBuilder();
        logEntry.AppendLine($"Timestamp: {DateTime.UtcNow:o}");
        logEntry.AppendLine($"Method: {invocation.Method.Name}");

        if (invocation.Arguments.Length > 0)
        {
            logEntry.Append("Arguments: ");
            foreach (var arg in invocation.Arguments)
            {
                logEntry.Append($"{arg}, ");
            }
            logEntry.AppendLine();
        }

        try
        {
            invocation.Proceed();

            if (invocation.Method.ReturnType != typeof(void))
            {
                logEntry.AppendLine($"Return Value: {invocation.ReturnValue}");
            }
        }
        catch (Exception ex)
        {
            logEntry.AppendLine($"Exception: {ex.Message}");
            throw;
        }
        finally
        {
            File.AppendAllText(_logFilePath, logEntry.ToString());
        }
    }
}
