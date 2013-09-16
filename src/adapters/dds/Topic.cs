using System;
using DDS;
using DDS.OpenSplice;
using communication.dds.api.adapter.qos;
using communication.dds.opensplice.adapters.dds.communication;
using communication.dds.opensplice.adapters.dds.communication.helpers;
using communication.dds.opensplice.adapters.dds.communication.writer;

namespace communication.dds.opensplice.adapters.dds
{
    public class Topic<T> : IDisposable
    {
        private static readonly object _locker = new object();
        private readonly TopicTypeSupport typeSupport;

        public Topic(string topicName, AdapterTopicQos requiredTopicQoS, TypeSupport clientTypeSupport)
        {
            if (clientTypeSupport == null)
            {
                throw new ArgumentNullException("clientTypeSupport");
            }

            string domain = null;

            // just using the default partition, named after the topic name
            TopicName = topicName;
            TopicType = typeof (T).Name;

            // Create a DomainParticipantFactory and a DomainParticipant
            RegisterDomain(this, domain);

            ErrorHandler.CheckHandle(Participant, "DDS.DomainParticipantFactory.CreateParticipant");

            // Register the required datatype.
            typeSupport = new TopicTypeSupport(clientTypeSupport);
            ReturnCode status = typeSupport.RegisterType(Participant, topicName);
            ErrorHandler.CheckStatus(status, "Topic.TopicMessageTypeSupport.RegisterType");

            TopicQos topicQos = null;
            Participant.GetDefaultTopicQos(ref topicQos);
            ApplyQos(topicQos, requiredTopicQoS);

            TopicQos = topicQos;

            TopicMessageTopic = Participant.CreateTopic(topicName, topicName, topicQos);

            Participant.SetDefaultTopicQos(topicQos);
            if (TopicMessageTopic == null)
            {
                throw new Exception("QoS for topic " + TopicName + " FAILED!, please check QoS!");
            }
        }

        public IDomainParticipant Participant { get; private set; }

        public string PartitionName { get; private set; }

        public TopicQos TopicQos { get; private set; }

        public ITopic TopicMessageTopic { get; private set; }

        public string TopicName { get; private set; }

        public string TopicType { get; private set; }

        public void Dispose()
        {
            if (Participant != null)
            {
                try
                {
                    Participant.DeleteTopic(TopicMessageTopic);
                }
                catch
                {
                }
            }
        }

        private void ApplyQos(TopicQos defaultTopicQos, AdapterTopicQos adapterRequiredQos)
        {
            if (defaultTopicQos == null)
            {
                throw new ArgumentNullException("defaultTopicQos");
            }

            if (adapterRequiredQos == null)
            {
                throw new ArgumentNullException("adapterRequiredQos");
            }

            defaultTopicQos.Durability.Kind = adapterRequiredQos.PersistenceType.ConvertPersistence();
            defaultTopicQos.Reliability.Kind = adapterRequiredQos.MessageReliabilityType.ConvertReliability();
        }

        private static void RegisterDomain(Topic<T> topicInstance, string domain)
        {
            lock (_locker)
            {
                DomainParticipantFactory dpf = DomainParticipantFactory.Instance;
                ErrorHandler.CheckHandle(dpf, "DDS.DomainParticipantFactory.Instance");
                var dpQos = new DomainParticipantQos();
                dpf.GetDefaultParticipantQos(ref dpQos);
                topicInstance.Participant = dpf.CreateParticipant(domain, dpQos) ??
                                            (dpf.LookupParticipant(domain) ?? dpf.CreateParticipant(domain, dpQos));

                if (topicInstance.Participant == null)
                {
                    throw new Exception("DDS NOT started, please start / restart your DDS!");
                }
            }
        }

        private class TopicTypeSupport : TypeSupport
        {
            private readonly TypeSupport clientTypeSupport;

            public TopicTypeSupport(TypeSupport clientTypeSupport) : base(typeof (T))
            {
                this.clientTypeSupport = clientTypeSupport;
            }


            public override string TypeName
            {
                get { return clientTypeSupport.TypeName; }
            }

            public override string Description
            {
                get { return clientTypeSupport.Description; }
            }

            public override string KeyList
            {
                get { return clientTypeSupport.KeyList; }
            }

            public override DataWriter CreateDataWriter(IntPtr gapiPtr)
            {
                return new DataWriter<T>(gapiPtr);
            }

            public override DataReader CreateDataReader(IntPtr gapiPtr)
            {
                return new DataReader(gapiPtr);
            }

            public override ReturnCode RegisterType(IDomainParticipant participant, string typeName)
            {
                return clientTypeSupport.RegisterType(participant, typeName);
            }
        }
    }
}