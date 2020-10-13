using System;
using System.Collections.Generic;

namespace Weary
{
    public sealed class UuidManager
    {
        private ulong highestId;
        private Queue<ulong> freedIds = new Queue<ulong>();

        public UuidManager(ulong baseId = 0)
        {
            highestId = baseId;
        }

        public UuidManager(ulong? highest, Queue<ulong> freed)
        {
            highestId = highest == null ? 0 : highest.Value;
            freedIds = freed;
        }

        public (ulong highest, Queue<ulong> freed) GetInternals()
        {
            return (highestId, freedIds);
        }

        public ulong GenerateId()
        {
            ulong rtnId = highestId + 1;
            if (freedIds.Count > 0)
                rtnId = freedIds.Dequeue();
            else
                highestId++;
            return rtnId;
        }

        public void FreeId(ulong id)
        {
            if (id == highestId)
                highestId--;
            else
                freedIds.Enqueue(id);
        }
    }
}