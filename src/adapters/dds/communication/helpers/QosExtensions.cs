using System;
using DDS;
using communication.dds.api.adapter.qos.enums;

namespace communication.dds.opensplice.adapters.dds.communication.helpers
{
    public static class QosExtensions
    {
        public static ReliabilityQosPolicyKind ConvertReliability(this MessageReliabilityTypeEnum reliabilityType)
        {
            switch (reliabilityType)
            {
                case MessageReliabilityTypeEnum.BestEffort:
                    return ReliabilityQosPolicyKind.BestEffortReliabilityQos;

                case MessageReliabilityTypeEnum.Reliable:
                    return ReliabilityQosPolicyKind.ReliableReliabilityQos;

                default:
                    throw new ArgumentOutOfRangeException("reliabilityType");
            }
        }

        public static DurabilityQosPolicyKind ConvertPersistence(this PersistenceTypeEnum persistenceType)
        {
            switch (persistenceType)
            {
                case PersistenceTypeEnum.Volatile:
                    return DurabilityQosPolicyKind.VolatileDurabilityQos;

                case PersistenceTypeEnum.Transient:
                    return DurabilityQosPolicyKind.TransientDurabilityQos;

                case PersistenceTypeEnum.Persistent:
                    return DurabilityQosPolicyKind.PersistentDurabilityQos;

                default:
                    throw new ArgumentOutOfRangeException("persistenceType");
            }
        }
    }
}