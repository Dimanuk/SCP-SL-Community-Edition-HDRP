public class AuthenticatedMessage
{
    public readonly string Message;

    public readonly bool Administrator;

    public AuthenticatedMessage(string m, bool a)
    {
        Message = m;
        Administrator = a;
    }

    public static string GenerateAuthenticatedMessage(string message, long timestamp, string password)
    {
        if (message.Contains(":[:BR:]:"))
        {
            throw new MessageUnallowedCharsException("Message can't contain :[:BR:]:");
        }
        string text = message + ":[:BR:]:" + global::System.Convert.ToString(timestamp);
        return text + ":[:BR:]:" + global::Cryptography.Sha.HashToString(global::Cryptography.Sha.Sha512Hmac(global::Cryptography.Sha.Sha512(password), Utf8.GetBytes(text)));
    }

    public static string GenerateNonAuthenticatedMessage(string message)
    {
        if (message.Contains(":[:BR:]:"))
        {
            throw new MessageUnallowedCharsException("Message can't contain :[:BR:]:");
        }
        return message + ":[:BR:]:Guest";
    }

    public static AuthenticatedMessage AuthenticateMessage(string message, long timestamp, string password)
    {
        if (!message.Contains(":[:BR:]:"))
        {
            throw new MessageAuthenticationFailureException("Malformed message.");
        }
        string[] array = message.Split(new string[1] { ":[:BR:]:" }, global::System.StringSplitOptions.None);
        if (array.Length < 2 || array.Length > 3)
        {
            throw new MessageAuthenticationFailureException("Malformed message.");
        }
        if (array[1] == "Guest")
        {
            return new AuthenticatedMessage(array[0], a: false);
        }
        try
        {
            if (!TimeBehaviour.ValidateTimestamp(timestamp, global::System.Convert.ToInt64(array[1]), 1200000L))
            {
                throw new MessageExpiredException();
            }
        }
        catch (MessageExpiredException)
        {
            throw new MessageAuthenticationFailureException();
        }
        catch
        {
            throw new MessageAuthenticationFailureException("Malformed message - timestamp can't be converted to long.");
        }
        if (global::Cryptography.Sha.HashToString(global::Cryptography.Sha.Sha512Hmac(global::Cryptography.Sha.Sha512(password), Utf8.GetBytes(array[0] + ":[:BR:]:" + array[1]))) != array[2])
        {
            throw new MessageAuthenticationFailureException("Invalid authentication code.");
        }
        if (!string.IsNullOrEmpty(password) && !(password == "none"))
        {
            return new AuthenticatedMessage(array[0], a: true);
        }
        return new AuthenticatedMessage(array[0], a: false);
    }

    public static byte[] Encode(byte[] data)
    {
        byte[] array = new byte[data.Length + 4];
        byte[] bytes = global::System.BitConverter.GetBytes(data.Length);
        global::System.Array.Reverse((global::System.Array)bytes);
        global::System.Array.Copy(bytes, 0, array, 0, bytes.Length);
        global::System.Array.Copy(data, 0, array, 4, data.Length);
        return array;
    }

    public static global::System.Collections.Generic.List<byte[]> Decode(byte[] data)
    {
        global::System.Collections.Generic.List<byte[]> list = new global::System.Collections.Generic.List<byte[]>();
        while (data.Length != 0)
        {
            byte[] obj = new byte[4]
            {
                data[0],
                data[1],
                data[2],
                data[3]
            };
            global::System.Array.Reverse((global::System.Array)obj);
            short num = global::System.BitConverter.ToInt16(obj, 0);
            if (num == 0)
            {
                break;
            }
            byte[] array = new byte[num];
            global::System.Array.Copy(data, 4, array, 0, num);
            list.Add(array);
            array = new byte[data.Length - num - 4];
            global::System.Array.Copy(data, num + 4, array, 0, data.Length - num - 4);
            data = array;
        }
        return list;
    }
}
