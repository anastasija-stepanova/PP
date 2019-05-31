using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace pp_lab3
{
    public class CriticalSection : ICriticalSection
    {
        private int SpinCount = 1;
        private int DefaultTimeout = 10;
        private AutoResetEvent Event = new AutoResetEvent(true);

        public void Enter()
        {
            Event.WaitOne();
        }

        public void Leave()
        {
            Event.Set();
        }

        public void SetSpinCount(int count)
        {
            SpinCount = count;
        }

        public bool TryEnter(int timeout)
        {
            var Time = DateTime.Now;
            do
            {
                for (int i = 0; i < SpinCount; ++i)
                {
                    if (Event.WaitOne(0))
                    {
                        return true;
                    }
                    if ((DateTime.Now - Time).TotalMilliseconds >= timeout)
                    {
                        return false;
                    }
                }

            } while ((DateTime.Now - Time).TotalMilliseconds < timeout);
            return false;
        }

        public void Dispose()
        {
            Event.Dispose();
        }
    }
}
