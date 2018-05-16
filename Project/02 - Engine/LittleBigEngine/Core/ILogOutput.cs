using System;

namespace LBE.Log
{
    public interface ILogOutput
    {
        void Write(String msg);
        void Error(String msg);
    }
}