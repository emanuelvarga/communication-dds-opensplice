using System;
using DDS;
using DDS.OpenSplice;

namespace communication.dds.opensplice.adapters.dds.communication.reader
{
    public class DataReader<T> : DataReader
    {
        public DataReader(IntPtr gapiPtr)
            : base(gapiPtr)
        {
        }

        #region IGenericDataReader<T>

        public ReturnCode GetKeyValue(ref T key, InstanceHandle handle)
        {
            object keyObj = key;
            ReturnCode result = FooDataReader.GetKeyValue(this, ref keyObj, handle);
            if (!keyObj.Equals(key))
            {
                key = (T) keyObj;
            }

            return result;
        }

        public InstanceHandle LookupInstance(T instance)
        {
            return FooDataReader.LookupInstance(this, instance);
        }

        public ReturnCode Read(ref T[] dataValues, ref SampleInfo[] sampleInfos)
        {
            return Read(ref dataValues, ref sampleInfos, Length.Unlimited);
        }

        public ReturnCode Read(ref T[] dataValues, ref SampleInfo[] sampleInfos, int maxSamples)
        {
            return Read(
                ref dataValues,
                ref sampleInfos,
                maxSamples,
                SampleStateKind.Any,
                ViewStateKind.Any,
                InstanceStateKind.Any);
        }

        public ReturnCode Read(
            ref T[] dataValues,
            ref SampleInfo[] sampleInfos,
            SampleStateKind sampleStates,
            ViewStateKind viewStates,
            InstanceStateKind instanceStates)
        {
            return Read(
                ref dataValues, ref sampleInfos, Length.Unlimited, sampleStates, viewStates, instanceStates);
        }

        public ReturnCode Read(
            ref T[] dataValues,
            ref SampleInfo[] sampleInfos,
            int maxSamples,
            SampleStateKind sampleStates,
            ViewStateKind viewStates,
            InstanceStateKind instanceStates)
        {
            // dataValues.Cast<object>().ToArray()
            var objectValues = (object[]) ((object) dataValues);
            ReturnCode result = FooDataReader.Read(
                this, ref objectValues, ref sampleInfos, maxSamples, sampleStates, viewStates, instanceStates);

            dataValues = (T[]) ((object) objectValues);
            return result;
        }

        public ReturnCode ReadInstance(ref T[] dataValues, ref SampleInfo[] sampleInfos, InstanceHandle instanceHandle)
        {
            return ReadInstance(ref dataValues, ref sampleInfos, Length.Unlimited, instanceHandle);
        }

        public ReturnCode ReadInstance(
            ref T[] dataValues, ref SampleInfo[] sampleInfos, int maxSamples, InstanceHandle instanceHandle)
        {
            return ReadInstance(
                ref dataValues,
                ref sampleInfos,
                maxSamples,
                instanceHandle,
                SampleStateKind.Any,
                ViewStateKind.Any,
                InstanceStateKind.Any);
        }

        public ReturnCode ReadInstance(
            ref T[] dataValues,
            ref SampleInfo[] sampleInfos,
            int maxSamples,
            InstanceHandle instanceHandle,
            SampleStateKind sampleStates,
            ViewStateKind viewStates,
            InstanceStateKind instanceStates)
        {
            var objectValues = (object[]) ((object) dataValues);
            ReturnCode result = FooDataReader.ReadInstance(
                this,
                ref objectValues,
                ref sampleInfos,
                maxSamples,
                instanceHandle,
                sampleStates,
                viewStates,
                instanceStates);
            dataValues = (T[]) ((object) objectValues);
            return result;
        }

        public ReturnCode ReadNextInstance(
            ref T[] dataValues, ref SampleInfo[] sampleInfos, InstanceHandle instanceHandle)
        {
            return ReadNextInstance(ref dataValues, ref sampleInfos, Length.Unlimited, instanceHandle);
        }

        public ReturnCode ReadNextInstance(
            ref T[] dataValues, ref SampleInfo[] sampleInfos, int maxSamples, InstanceHandle instanceHandle)
        {
            return ReadNextInstance(
                ref dataValues,
                ref sampleInfos,
                maxSamples,
                instanceHandle,
                SampleStateKind.Any,
                ViewStateKind.Any,
                InstanceStateKind.Any);
        }

        public ReturnCode ReadNextInstance(
            ref T[] dataValues,
            ref SampleInfo[] sampleInfos,
            int maxSamples,
            InstanceHandle instanceHandle,
            SampleStateKind sampleStates,
            ViewStateKind viewStates,
            InstanceStateKind instanceStates)
        {
            var objectValues = (object[]) ((object) dataValues);
            ReturnCode result = FooDataReader.ReadNextInstance(
                this,
                ref objectValues,
                ref sampleInfos,
                maxSamples,
                instanceHandle,
                sampleStates,
                viewStates,
                instanceStates);
            dataValues = (T[]) ((object) objectValues);
            return result;
        }

        public ReturnCode ReadNextInstanceWithCondition(
            ref T[] dataValues,
            ref SampleInfo[] sampleInfos,
            InstanceHandle instanceHandle,
            IReadCondition readCondition)
        {
            return ReadNextInstanceWithCondition(
                ref dataValues, ref sampleInfos, Length.Unlimited, instanceHandle, readCondition);
        }

        public ReturnCode ReadNextInstanceWithCondition(
            ref T[] dataValues,
            ref SampleInfo[] sampleInfos,
            int maxSamples,
            InstanceHandle instanceHandle,
            IReadCondition readCondition)
        {
            var objectValues = (object[]) ((object) dataValues);
            ReturnCode result = FooDataReader.ReadNextInstanceWithCondition(
                this, ref objectValues, ref sampleInfos, maxSamples, instanceHandle, readCondition);
            dataValues = (T[]) ((object) objectValues);
            return result;
        }

        public ReturnCode ReadNextSample(T dataValue, SampleInfo sampleInfo)
        {
            object objectValues = dataValue;
            ReturnCode result = FooDataReader.ReadNextSample(this, ref objectValues, ref sampleInfo);
            dataValue = (T) objectValues;
            return result;
        }

        public ReturnCode ReadWithCondition(
            ref T[] dataValues, ref SampleInfo[] sampleInfos, IReadCondition readCondition)
        {
            return ReadWithCondition(ref dataValues, ref sampleInfos, Length.Unlimited, readCondition);
        }

        public ReturnCode ReadWithCondition(
            ref T[] dataValues, ref SampleInfo[] sampleInfos, int maxSamples, IReadCondition readCondition)
        {
            var objectValues = (object[]) ((object) dataValues);
            ReturnCode result = FooDataReader.ReadWithCondition(
                this, ref objectValues, ref sampleInfos, maxSamples, readCondition);
            dataValues = (T[]) ((object) objectValues);
            return result;
        }

        public ReturnCode ReturnLoan(ref T[] dataValues, ref SampleInfo[] sampleInfos)
        {
            ReturnCode result;

            if (dataValues != null && sampleInfos != null)
            {
                if (dataValues != null && sampleInfos != null)
                {
                    if (dataValues.Length == sampleInfos.Length)
                    {
                        dataValues = null;
                        sampleInfos = null;
                        result = ReturnCode.Ok;
                    }
                    else
                    {
                        result = ReturnCode.PreconditionNotMet;
                    }
                }
                else
                {
                    if ((dataValues == null) && (sampleInfos == null))
                    {
                        result = ReturnCode.Ok;
                    }
                    else
                    {
                        result = ReturnCode.PreconditionNotMet;
                    }
                }
            }
            else
            {
                result = ReturnCode.BadParameter;
            }

            return result;
        }

        public ReturnCode Take(ref T[] dataValues, ref SampleInfo[] sampleInfos)
        {
            return Take(ref dataValues, ref sampleInfos, Length.Unlimited);
        }

        public ReturnCode Take(ref T[] dataValues, ref SampleInfo[] sampleInfos, int maxSamples)
        {
            return Take(
                ref dataValues,
                ref sampleInfos,
                maxSamples,
                SampleStateKind.Any,
                ViewStateKind.Any,
                InstanceStateKind.Any);
        }

        public ReturnCode Take(
            ref T[] dataValues,
            ref SampleInfo[] sampleInfos,
            SampleStateKind sampleStates,
            ViewStateKind viewStates,
            InstanceStateKind instanceStates)
        {
            return Take(
                ref dataValues, ref sampleInfos, Length.Unlimited, sampleStates, viewStates, instanceStates);
        }

        public ReturnCode Take(
            ref T[] dataValues,
            ref SampleInfo[] sampleInfos,
            int maxSamples,
            SampleStateKind sampleStates,
            ViewStateKind viewStates,
            InstanceStateKind instanceStates)
        {
            var objectValues = (object[]) ((object) dataValues);
            ReturnCode result = FooDataReader.Take(
                this, ref objectValues, ref sampleInfos, maxSamples, sampleStates, viewStates, instanceStates);
            dataValues = (T[]) ((object) objectValues);
            return result;
        }

        public ReturnCode TakeInstance(ref T[] dataValues, ref SampleInfo[] sampleInfos, InstanceHandle instanceHandle)
        {
            return TakeInstance(ref dataValues, ref sampleInfos, Length.Unlimited, instanceHandle);
        }

        public ReturnCode TakeInstance(
            ref T[] dataValues, ref SampleInfo[] sampleInfos, int maxSamples, InstanceHandle instanceHandle)
        {
            return TakeInstance(
                ref dataValues,
                ref sampleInfos,
                maxSamples,
                instanceHandle,
                SampleStateKind.Any,
                ViewStateKind.Any,
                InstanceStateKind.Any);
        }

        public ReturnCode TakeInstance(
            ref T[] dataValues,
            ref SampleInfo[] sampleInfos,
            int maxSamples,
            InstanceHandle instanceHandle,
            SampleStateKind sampleStates,
            ViewStateKind viewStates,
            InstanceStateKind instanceStates)
        {
            var objectValues = (object[]) ((object) dataValues);
            ReturnCode result = FooDataReader.TakeInstance(
                this,
                ref objectValues,
                ref sampleInfos,
                maxSamples,
                instanceHandle,
                sampleStates,
                viewStates,
                instanceStates);
            dataValues = (T[]) ((object) objectValues);
            return result;
        }

        public ReturnCode TakeNextInstance(
            ref T[] dataValues, ref SampleInfo[] sampleInfos, InstanceHandle instanceHandle)
        {
            return TakeNextInstance(ref dataValues, ref sampleInfos, Length.Unlimited, instanceHandle);
        }

        public ReturnCode TakeNextInstance(
            ref T[] dataValues, ref SampleInfo[] sampleInfos, int maxSamples, InstanceHandle instanceHandle)
        {
            return TakeNextInstance(
                ref dataValues,
                ref sampleInfos,
                maxSamples,
                instanceHandle,
                SampleStateKind.Any,
                ViewStateKind.Any,
                InstanceStateKind.Any);
        }

        public ReturnCode TakeNextInstance(
            ref T[] dataValues,
            ref SampleInfo[] sampleInfos,
            int maxSamples,
            InstanceHandle instanceHandle,
            SampleStateKind sampleStates,
            ViewStateKind viewStates,
            InstanceStateKind instanceStates)
        {
            var objectValues = (object[]) ((object) dataValues);
            ReturnCode result = FooDataReader.TakeNextInstance(
                this,
                ref objectValues,
                ref sampleInfos,
                maxSamples,
                instanceHandle,
                sampleStates,
                viewStates,
                instanceStates);
            dataValues = (T[]) ((object) objectValues);
            return result;
        }

        public ReturnCode TakeNextInstanceWithCondition(
            ref T[] dataValues,
            ref SampleInfo[] sampleInfos,
            InstanceHandle instanceHandle,
            IReadCondition readCondition)
        {
            return TakeNextInstanceWithCondition(
                ref dataValues, ref sampleInfos, Length.Unlimited, instanceHandle, readCondition);
        }

        public ReturnCode TakeNextInstanceWithCondition(
            ref T[] dataValues,
            ref SampleInfo[] sampleInfos,
            int maxSamples,
            InstanceHandle instanceHandle,
            IReadCondition readCondition)
        {
            var objectValues = (object[]) ((object) dataValues);
            ReturnCode result = FooDataReader.TakeNextInstanceWithCondition(
                this, ref objectValues, ref sampleInfos, maxSamples, instanceHandle, readCondition);

            dataValues = (T[]) ((object) objectValues);
            return result;
        }

        public ReturnCode TakeNextSample(T dataValue, SampleInfo sampleInfo)
        {
            object objectValues = dataValue;
            ReturnCode result = FooDataReader.TakeNextSample(this, ref objectValues, ref sampleInfo);
            dataValue = (T) objectValues;
            return result;
        }

        public ReturnCode TakeWithCondition(
            ref T[] dataValues, ref SampleInfo[] sampleInfos, IReadCondition readCondition)
        {
            return TakeWithCondition(ref dataValues, ref sampleInfos, Length.Unlimited, readCondition);
        }

        public ReturnCode TakeWithCondition(
            ref T[] dataValues, ref SampleInfo[] sampleInfos, int maxSamples, IReadCondition readCondition)
        {
            var objectValues = (object[]) ((object) dataValues);
            ReturnCode result = FooDataReader.TakeWithCondition(
                this, ref objectValues, ref sampleInfos, maxSamples, readCondition);
            dataValues = (T[]) ((object) objectValues);
            return result;
        }

        #endregion IGenericDataReader<T>
    }
}