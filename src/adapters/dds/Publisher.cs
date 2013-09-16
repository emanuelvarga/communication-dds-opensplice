using System;
using System.Diagnostics;
using DDS;
using communication.dds.api.adapter.publisher;
using communication.dds.api.adapter.qos;
using communication.dds.opensplice.adapters.dds.communication;
using communication.dds.opensplice.adapters.dds.communication.helpers;
using communication.dds.opensplice.adapters.dds.communication.writer;

namespace communication.dds.opensplice.adapters.dds
{
    internal class Publisher<T> : DataPublisher<T>, IDisposable
    {
        private static readonly object padLock = new object();

        private readonly Topic<T> currentTopic;
        private readonly AdapterWriterQos desiredQos;

        private DataWriter<T> dataWriter;

        private IPublisher topicPublisher;

        public Publisher(Topic<T> topic, string partitionName, AdapterWriterQos desiredQos)
        {
            currentTopic = topic;

            IDomainParticipant participant = topic.Participant;
            var publisherQos = new PublisherQos();

            ReturnCode status = participant.GetDefaultPublisherQos(ref publisherQos);
            ErrorHandler.CheckStatus(status, "Publisher.GetDefaultSubscriberQos (Publisher)");
            publisherQos.Partition.Name = new string[1];
            publisherQos.Partition.Name[0] = partitionName;

            // Create a Subscriber for the application.
            topicPublisher = participant.CreatePublisher(publisherQos);
            ErrorHandler.CheckHandle(
                topicPublisher, "Publisher.CreateSubscriber (Publisher)");

            InitializeWriter(desiredQos);
        }

        public DataWriter<T> DataWriter
        {
            get
            {
                if (dataWriter == null)
                {
                    lock (padLock)
                    {
                        if (dataWriter == null)
                        {
                            InitializeWriter(desiredQos);
                        }
                    }
                }

                return dataWriter;
            }
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public override string Publish(T data)
        {
            DataWriter<T> writter = DataWriter;
            if (writter == null)
            {
                throw new Exception("Data writter was null, cannot publish data!");
            }

            try
            {
                writter.Write(data);
                return data.ToString();
            }
            catch (Exception exception)
            {
                Trace.TraceError("{0}.Publish failed to publish data: {1}, error: {2}", GetType().Name, data.ToString(),
                                 exception);
            }

            return null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    if (topicPublisher != null)
                    {
                        RemoveDataWriter();

                        ReturnCode status = topicPublisher.DeleteContainedEntities();
                        ErrorHandler.CheckStatus(status, "TopicPublisher.DeleteContainedEntities (Dispose)");

                        status = currentTopic.Participant.DeletePublisher(topicPublisher);
                        ErrorHandler.CheckStatus(status, "TopicPublisher.DeletePublisher (Dispose)");

                        topicPublisher = null;
                    }
                }
                catch (Exception e)
                {
                }
            }
        }

        private void InitializeWriter(AdapterWriterQos desiredQos)
        {
            var writerQos = new DataWriterQos();
            topicPublisher.GetDefaultDataWriterQos(ref writerQos);
            topicPublisher.CopyFromTopicQos(ref writerQos, currentTopic.TopicQos);
            if (desiredQos != null)
            {
                writerQos.WriterDataLifecycle.AutodisposeUnregisteredInstances = false;
                writerQos.Reliability.Kind = desiredQos.MessageReliabilityType.ConvertReliability();
                writerQos.Durability.Kind = desiredQos.PersistenceType.ConvertPersistence();
            }

            // Create a DataWritter for the current Topic
            dataWriter = topicPublisher.CreateDataWriter(currentTopic.TopicMessageTopic, writerQos) as DataWriter<T>;
            ErrorHandler.CheckHandle(
                DataWriter, "Publisher.CreateDatawriter (InitializeWriter)");
        }

        private void RemoveDataWriter()
        {
            if (DataWriter == null)
            {
                return;
            }

            ReturnCode status = topicPublisher.DeleteDataWriter(DataWriter);
            ErrorHandler.CheckStatus(
                status, "topicPublisher.DeleteDataWriter (RemoveDataWriter)");
        }
    }
}