using System;

//反射标记，这里只适用于字段
[AttributeUsage(AttributeTargets.Field)]
public class DataListAttribute : Attribute
{
    
}
