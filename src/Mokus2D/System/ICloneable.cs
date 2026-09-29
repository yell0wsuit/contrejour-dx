namespace System;

public interface ICloneable<out T>
{
    T Clone();
}
