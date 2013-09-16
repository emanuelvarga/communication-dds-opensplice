using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using communication.dds.api.adapter;
using communication.dds.api.adapter.callback;
using communication.dds.api.adapter.publisher;
using communication.dds.api.adapter.qos;
using communication.dds.api.adapter.query;
using communication.dds.opensplice.adapters.configuration;
using communication.dds.opensplice.adapters.dds;
using communication.dds.opensplice.adapters.dds.communication.reader.listener;

namespace communication.dds.opensplice.adapters
{
    public class DDSAdapter<TDataType> : Adapter<TDataType>
    {
        private readonly object gate = new object();

        private readonly List<Subscription> subscribers = new List<Subscription>();
        private readonly Topic<TDataType> topic;

        public DDSAdapter(Topic<TDataType> topic)
        {
            if (topic == null)
            {
                throw new ArgumentNullException("topic");
            }

            this.topic = topic;
        }

        public override bool Subscribe(AdapterReaderQos qos, QueryParameters queryParameters,
                                       AdapterDataCallback<TDataType> clientCallback)
        {
            if (queryParameters == null)
            {
                throw new ArgumentNullException("queryParameters");
            }
            try
            {
                Callback callback = null;
                if (clientCallback != null)
                {
                    callback = new Callback(clientCallback);
                }

                var subcriber = new Subscriber<TDataType>(topic, queryParameters.Partition, qos,
                                                          callback == null ? null : callback.Listener, 10);
                lock (gate)
                {
                    subscribers.Add(new Subscription(subcriber, callback));
                }

                return true;
            }
            catch (Exception exception)
            {
                Trace.TraceError("Failed to subscribe to topic: {0} with params: {1} and qos: {2}, error: {3}", topic,
                                 queryParameters, qos, exception);
            }

            return false;
        }

        public override IEnumerable<TDataType> Snapshot(AdapterReaderQos qos, QueryParameters queryParameters)
        {
            var waitHandle = new ManualResetEvent(false);
            using (var callback = new SnapshotCallback(waitHandle))
            {
                using (
                    var subcriber = new Subscriber<TDataType>(topic, queryParameters.Partition, qos, callback.Listener,
                                                              1000))
                {
                    waitHandle.WaitOne(Constants.RequestTimeout);
                    return callback.DataItems;
                }
            }
        }

        public override DataPublisher<TDataType> CreatePublisher(AdapterWriterQos qos, string partitionName)
        {
            return new Publisher<TDataType>(topic, partitionName, qos);
        }

        public override void Dispose()
        {
            lock (gate)
            {
                foreach (
                    Subscription subscription in subscribers.Where(subscriber => subscriber.Callback != null).ToArray())
                {
                    subscription.Callback.Dispose();
                    subscription.Subscriber.Dispose();
                    subscribers.Remove(subscription);
                }
            }
        }

        private class Callback : AdapterDataCallback<TDataType>
        {
            private readonly AdapterDataCallback<TDataType> clientCallback;

            private readonly DataReaderListener<TDataType> listener;
            private bool isDisposed;

            public Callback(AdapterDataCallback<TDataType> clientCallback)
            {
                if (isDisposed)
                {
                    return;
                }

                if (clientCallback == null)
                {
                    throw new ArgumentNullException("clientCallback");
                }

                this.clientCallback = clientCallback;
                listener = new DataReaderListener<TDataType>(this, false);
            }

            public DataReaderListener<TDataType> Listener
            {
                get { return listener; }
            }

            public override void OnDataReceived(TDataType data)
            {
                if (isDisposed)
                {
                    return;
                }

                clientCallback.OnDataReceived(data);
            }

            public override void OnError(Exception exception)
            {
            }

            public override void OnCompleted()
            {
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    isDisposed = true;
                }

                base.Dispose(disposing);
            }
        }

        private class SnapshotCallback : AdapterDataCallback<TDataType>
        {
            private readonly List<TDataType> dataItems = new List<TDataType>();
            private readonly DataReaderListener<TDataType> listener;
            private readonly WaitHandle waitHandle;
            private bool isDisposed;

            public SnapshotCallback(WaitHandle waitHandle)
            {
                this.waitHandle = waitHandle;
                if (waitHandle == null)
                {
                    throw new ArgumentNullException("waitHandle");
                }

                listener = new DataReaderListener<TDataType>(this, true);
            }

            public DataReaderListener<TDataType> Listener
            {
                get { return listener; }
            }

            public List<TDataType> DataItems
            {
                get { return dataItems; }
            }

            public override void OnError(Exception exception)
            {
            }

            public override void OnCompleted()
            {
                waitHandle.Close();
            }

            public override void OnDataReceived(TDataType data)
            {
                DataItems.Add(data);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    isDisposed = true;
                }

                base.Dispose(disposing);
            }
        }

        private class Subscription
        {
            public Subscription(Subscriber<TDataType> subscriber, Callback callback)
            {
                Subscriber = subscriber;
                Callback = callback;
            }

            public Subscriber<TDataType> Subscriber { get; private set; }

            public Callback Callback { get; private set; }
        }
    }
}