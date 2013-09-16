using System.Diagnostics;
using DDS;

namespace communication.dds.opensplice.adapters.dds.communication
{
    public static class ErrorHandler
    {
        #region Constants and Fields

        private static readonly string[] RetCodeName = new[]
                                                           {
                                                               "DDS_RETCODE_OK", "DDS_RETCODE_ERROR",
                                                               "DDS_RETCODE_UNSUPPORTED", "DDS_RETCODE_BAD_PARAMETER",
                                                               "DDS_RETCODE_PRECONDITION_NOT_MET",
                                                               "DDS_RETCODE_OUT_OF_RESOURCES", "DDS_RETCODE_NOT_ENABLED"
                                                               ,
                                                               "DDS_RETCODE_IMMUTABLE_POLICY",
                                                               "DDS_RETCODE_INCONSISTENT_POLICY",
                                                               "DDS_RETCODE_ALREADY_DELETED",
                                                               "DDS_RETCODE_TIMEOUT", "DDS_RETCODE_NO_DATA",
                                                               "DDS_RETCODE_ILLEGAL_OPERATION"
                                                           };

        public static void CheckHandle(object handle, string info)
        {
            if (handle == null)
            {
                Trace.TraceWarning("Error in " + info + ": Creation failed: invalid handle");
            }
        }

        public static void CheckStatus(ReturnCode status, string info)
        {
            if (status != ReturnCode.Ok && status != ReturnCode.NoData)
            {
                Trace.TraceWarning("Error in " + info + ": " + GetErrorName(status));
            }
        }

        public static string GetErrorName(ReturnCode status)
        {
            return RetCodeName[(int) status];
        }

        #endregion Public Methods
    }
}