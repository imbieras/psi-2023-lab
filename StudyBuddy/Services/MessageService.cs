using StudyBuddy.Models;

namespace StudyBuddy.Services;

public class MessageService
{
    private readonly Dictionary<string, List<Message>> messages = new Dictionary<string, List<Message>>();

    public void AddMessage(string groupName, Message message)
    {
        if (!messages.ContainsKey(groupName))
        {
            messages[groupName] = new List<Message>();
        }

        messages[groupName].Add(message);
    }

    public List<Message> GetMessages(string groupName)
    {
        if (messages.ContainsKey(groupName))
        {
            return messages[groupName];
        }

        return new List<Message>();
    }
}
