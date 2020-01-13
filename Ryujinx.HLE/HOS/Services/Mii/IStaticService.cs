using Ryujinx.Common.Logging;

namespace Ryujinx.HLE.HOS.Services.Mii
{
    [Service("mii:e")]
    [Service("mii:u")]
    class IStaticService : IpcService
    {
        public IStaticService(ServiceCtx context) { }

        [Command(0)]
        public ResultCode GetDatabaseService(ServiceCtx context)
        {
            uint unknown = context.RequestData.ReadUInt32();

            Logger.PrintStub(LogClass.ServiceMii, new { unknown });

            MakeObject(context, new IDatabaseService());

            return ResultCode.Success;
        }
    }
}
