
using System;

[Serializable]
public class ServerResponse<T>
{
    public int code;
    public string message;
    public T data;
}
