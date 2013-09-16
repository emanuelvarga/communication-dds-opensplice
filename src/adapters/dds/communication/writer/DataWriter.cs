using System;
using DDS;
using DDS.OpenSplice;

namespace communication.dds.opensplice.adapters.dds.communication.writer
{
    public class DataWriter<T> : DataWriter
    {
        public DataWriter(IntPtr gapiPtr)
            : base(gapiPtr)
        {
        }

        #region IGenericDataWriter<T>

        public ReturnCode Dispose(T instanceData, InstanceHandle instanceHandle)
        {
            return FooDataWriter.Dispose(this, instanceData, instanceHandle);
        }

        public ReturnCode DisposeWithTimestamp(T instanceData, InstanceHandle instanceHandle, Time sourceTimestamp)
        {
            return FooDataWriter.DisposeWithTimestamp(this, instanceData, instanceHandle, sourceTimestamp);
        }

        public ReturnCode GetKeyValue(ref T key, InstanceHandle instanceHandle)
        {
            object keyObj = key;
            ReturnCode result = FooDataWriter.GetKeyValue(this, ref keyObj, instanceHandle);
            if (!keyObj.Equals(key))
            {
                key = (T) keyObj;
            }

            return result;
        }

        public InstanceHandle LookupInstance(T instanceData)
        {
            InstanceHandle result = FooDataWriter.LookupInstance(this, instanceData);
            return result;
        }

        public InstanceHandle RegisterInstance(T instanceData)
        {
            return FooDataWriter.RegisterInstance(this, instanceData);
        }

        public InstanceHandle RegisterInstanceWithTimestamp(T instanceData, Time sourceTimestamp)
        {
            return FooDataWriter.RegisterInstanceWithTimestamp(this, instanceData, sourceTimestamp);
        }

        public ReturnCode UnregisterInstance(T instanceData, InstanceHandle instanceHandle)
        {
            return FooDataWriter.UnregisterInstance(this, instanceData, instanceHandle);
        }

        public ReturnCode UnregisterInstanceWithTimestamp(
            T instanceData, InstanceHandle instanceHandle, Time sourceTimestamp)
        {
            return FooDataWriter.UnregisterInstanceWithTimestamp(this, instanceData, instanceHandle, sourceTimestamp);
        }


        public ReturnCode Write(T instanceData)
        {
            return Write(instanceData, InstanceHandle.Nil);
        }

        public ReturnCode Write(T instanceData, InstanceHandle instanceHandle)
        {
            return FooDataWriter.Write(this, instanceData, instanceHandle);
        }

        public ReturnCode WriteDispose(T instanceData)
        {
            return WriteDispose(instanceData, InstanceHandle.Nil);
        }

        public ReturnCode WriteDispose(T instanceData, InstanceHandle instanceHandle)
        {
            return FooDataWriter.WriteDispose(this, instanceData, instanceHandle);
        }

        public ReturnCode WriteDisposeWithTimestamp(T instanceData, Time sourceTimestamp)
        {
            return WriteDisposeWithTimestamp(instanceData, InstanceHandle.Nil, sourceTimestamp);
        }

        public ReturnCode WriteDisposeWithTimestamp(T instanceData, InstanceHandle instanceHandle, Time sourceTimestamp)
        {
            return FooDataWriter.WriteDisposeWithTimestamp(this, instanceData, instanceHandle, sourceTimestamp);
        }

        public ReturnCode WriteWithTimestamp(T instanceData, Time sourceTimestamp)
        {
            return WriteWithTimestamp(instanceData, InstanceHandle.Nil, sourceTimestamp);
        }

        public ReturnCode WriteWithTimestamp(T instanceData, InstanceHandle instanceHandle, Time sourceTimestamp)
        {
            return FooDataWriter.WriteWithTimestamp(this, instanceData, instanceHandle, sourceTimestamp);
        }

        #endregion IGenericDataWriter<T>
    }
}