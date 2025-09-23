

using System;

[AttributeUsage(AttributeTargets.Class)]
public class RedPointAttribute : Attribute {
    public int ID;

    public RedPointAttribute(int configID) {
        ID = configID;
    }
}
