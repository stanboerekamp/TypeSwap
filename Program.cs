using System;
using System.Runtime.InteropServices;

namespace TypeSwap
{
    class Program
    {
        static void Main()
        {
            var obj = Cast.ForceCast<MyCode, SomeCode>(new MyCode());
            DoServer(obj);
            obj.SomeMethod();                       // TypeSwap.MyCode
            Console.ReadKey();
        }

        static void DoServer(SomeCode s)
        {
            Console.WriteLine(s.SomeProperty);      // 20
            Console.WriteLine(s.GetType());         // TypeSwap.MyCode
        }
    }

    class MyCode
    {
        public int MyProperty = 20;
    }

    class SomeCode
    {
        public readonly int SomeProperty = 10;

        public void SomeMethod()
        {
            Console.WriteLine(GetType());           
        }
    }

    class Cast
    {
        public static unsafe TOut ForceCast<Tin, TOut>(Tin input) where Tin : class where TOut : class
        {
            var p = new PinObj<Tin>() { TObj = input };

            var handle = __makeref(input);
            TOut obj = default;
            fixed (void* pin = &p.Pin)
            {
                TypedReference tr = __makeref(obj);
                *(IntPtr*)&tr = *(IntPtr*)&handle;
                return __refvalue(tr, TOut);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public class PinObj<T>
    {
        public bool Pin;
        public T TObj;
    }
}
