namespace StudyBuddy.Shared.Utilities;

public static class ConversationIdHelper
{
    public static Guid GetGroupId(Guid senderGuid, Guid receiverGuid)
    {
        byte[] bytes1 = senderGuid.ToByteArray();
        byte[] bytes2 = receiverGuid.ToByteArray();

        for (int i = 0; i < bytes1.Length; i++)
        {
            bytes1[i] = (byte)(bytes1[i] ^ bytes2[i]);
        }

        return new Guid(bytes1);
    }
}
