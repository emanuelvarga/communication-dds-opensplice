using System;
using System.Diagnostics;
using DDS;
using communication.dds.api.adapter.callback;

namespace communication.dds.opensplice.adapters.dds.communication.reader.listener
{
    public class DataReaderListener<T> : IDataReaderListener
    {
        private readonly AdapterDataCallback<T> callback;
        private readonly bool snapshot;
        private T lastMessageReceived;

        public DataReaderListener(AdapterDataCallback<T> callback, bool snapshot)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            this.callback = callback;
            this.snapshot = snapshot;
        }

        public T LastMessageReceived
        {
            get { return lastMessageReceived; }
        }

        public IQueryCondition QueryCondition { get; set; }

        private void OnMessageReceived(T msg)
        {
            try
            {
                callback.OnDataReceived(msg);
            }
            catch (Exception exception)
            {
                callback.OnError(exception);
                Trace.TraceError("{0}.OnMessageReceived failed for message: {1} with error: {2}", GetType().Name, msg,
                                 exception);
            }
        }

        private void OnBufferEmpty()
        {
            try
            {
                callback.OnCompleted();
            }
            catch (Exception exception)
            {
                callback.OnError(exception);
                Trace.TraceError("{0}.OnBufferEmpty failed with error: {1}", GetType().Name, exception);
            }
        }

        #region IDataReaderListener

        public void OnDataAvailable(IDataReader entityInterface)
        {
            var topicAdmin = (DataReader<T>) entityInterface;

            var messages = new T[0];
            var infos = new SampleInfo[0];

            if (QueryCondition == null)
            {
                topicAdmin.Read(ref messages, ref infos, Length.Unlimited, SampleStateKind.NotRead, ViewStateKind.Any,
                                InstanceStateKind.Any);
            }
            else
            {
                // Take with a condition
                topicAdmin.TakeWithCondition(ref messages, ref infos, QueryCondition);
            }

            int msgNo = 0;
            foreach (T msg in messages)
            {
                if (!infos[msgNo++].ValidData)
                {
                    continue;
                }

                lastMessageReceived = msg;
                OnMessageReceived(lastMessageReceived);
            }

            if (snapshot && messages.Length == 0 && lastMessageReceived != null &&
                !lastMessageReceived.Equals(default(T)))
            {
                OnBufferEmpty();
                lastMessageReceived = default(T);
            }

            topicAdmin.ReturnLoan(ref messages, ref infos);
        }


        public void OnLivelinessChanged(IDataReader entityInterface, LivelinessChangedStatus status)
        {
        }

        public void OnRequestedDeadlineMissed(IDataReader entityInterface, RequestedDeadlineMissedStatus status)
        {
        }

        public void OnRequestedIncompatibleQos(IDataReader entityInterface, RequestedIncompatibleQosStatus status)
        {
        }

        public void OnSampleLost(IDataReader entityInterface, SampleLostStatus status)
        {
        }

        public void OnSampleRejected(IDataReader entityInterface, SampleRejectedStatus status)
        {
        }

        public void OnSubscriptionMatched(IDataReader entityInterface, SubscriptionMatchedStatus status)
        {
        }

        #endregion IDataReaderListener
    }
}