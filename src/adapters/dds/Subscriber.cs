using System;
using DDS;
using communication.dds.api.adapter.qos;
using communication.dds.opensplice.adapters.dds.communication;
using communication.dds.opensplice.adapters.dds.communication.helpers;
using communication.dds.opensplice.adapters.dds.communication.reader.listener;

namespace communication.dds.opensplice.adapters.dds
{
    internal class Subscriber<T> : IDisposable
    {
        private readonly Topic<T> currentTopic;
        private readonly DataReaderListener<T> listener;

        private readonly string partitionName;

        private IDataReader dataReader;

        private ISubscriber topicSubscriber;

        public Subscriber(Topic<T> topic, string partitionName, AdapterReaderQos desiredQos,
                          DataReaderListener<T> listener, int waitForHistoricalData)
        {
            currentTopic = topic;
            this.partitionName = partitionName;
            this.listener = listener;
            IDomainParticipant participant = topic.Participant;
            var subQos = new SubscriberQos();

            ReturnCode status = participant.GetDefaultSubscriberQos(ref subQos);
            ErrorHandler.CheckStatus(
                status, "Subscriber.GetDefaultSubscriberQos (Subscriber)");
            subQos.Partition.Name = new string[1];
            subQos.Partition.Name[0] = partitionName;

            // Create a Subscriber for the application.
            topicSubscriber = participant.CreateSubscriber(subQos);
            ErrorHandler.CheckHandle(
                topicSubscriber, "Subscriber.CreateSubscriber (Subscriber)");

            CreateDataReader(desiredQos, listener, waitForHistoricalData);
        }

        public DataReaderListener<T> Listener
        {
            get { return listener; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool SetQueryCondition(string expression, string[] parameters)
        {
            IQueryCondition condition = dataReader.CreateQueryCondition(
                SampleStateKind.Any, ViewStateKind.Any, InstanceStateKind.Any, expression, parameters);

            listener.QueryCondition = condition;

            if (condition != null || !string.IsNullOrEmpty(expression))
            {
                return true;
            }

            return false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    if (topicSubscriber != null)
                    {
                        RemoveListener();
                        ReturnCode status = topicSubscriber.DeleteContainedEntities();


                        status = currentTopic.Participant.DeleteSubscriber(topicSubscriber);
                        ErrorHandler.CheckStatus(
                            status, "Subscriber.TopicSubscribers.DeleteSubscriber (Dispose)");

                        topicSubscriber = null;
                    }
                }
                catch (Exception e)
                {
                    return;
                }
            }
        }

        private void CreateDataReader(AdapterReaderQos desiredQos, DataReaderListener<T> listener,
                                      int waitForHistoricalData)
        {
            if (partitionName.Contains("*"))
            {
                partitionName.Replace("*", string.Empty);
            }

            if (dataReader != null)
            {
                if (listener != null)
                {
                    ReturnCode status = dataReader.SetListener(
                        listener, StatusKind.DataAvailable | StatusKind.SampleLost | StatusKind.SampleRejected);
                    ErrorHandler.CheckStatus(status, "Subscriber.SetListener (CreateDataReader)");
                }
            }
            else
            {
                // Create a DataReader for the Topic
                // (using the appropriate QoS) and attach a listener to the data reader on new data available
                var dataReaderQos = new DataReaderQos();
                topicSubscriber.CopyFromTopicQos(ref dataReaderQos, currentTopic.TopicQos);

                if (desiredQos != null)
                {
                    dataReaderQos.Durability.Kind = desiredQos.PersistenceType.ConvertPersistence();
                    dataReaderQos.Reliability.Kind = desiredQos.MessageReliabilityType.ConvertReliability();
                }

                dataReader = topicSubscriber.CreateDataReader(
                    currentTopic.TopicMessageTopic,
                    dataReaderQos,
                    listener,
                    StatusKind.DataAvailable);

                ErrorHandler.CheckHandle(
                    dataReader, "Subscriber.CreateDatareader (CreateDataReader)");

                dataReader.WaitForHistoricalData(new Duration(waitForHistoricalData, 0));
            }
        }

        /// <summary>
        ///     Removes the listener.
        /// </summary>
        private void RemoveListener()
        {
            if (dataReader != null)
            {
                ReturnCode status = dataReader.DeleteContainedEntities();
                ErrorHandler.CheckStatus(
                    status, "Subscriber.DataReader.DeleteContainedEntities (RemoveListener)");

                if (topicSubscriber != null)
                {
                    status = topicSubscriber.DeleteDataReader(dataReader);
                    ErrorHandler.CheckStatus(status, "Subscriber.TopicSubscribers.DeleteDataReader (RemoveListener)");
                    dataReader = null;
                }
            }
        }
    }
}