using Ryujinx.Graphics.Shader.Instructions;

namespace Ryujinx.Graphics.Shader.Decoders
{
    enum ClampMode
    {
        Ignore = 0,
        Trap = 2
    }

    class OpCodeSured : OpCodeTextureBase
    {
        public Register Ra { get; }
        public Register Rb { get; }
        public Register Rc { get; }

        public AtomicOp AtomicOp { get; }

        public ImageDimensions Dimensions { get; }

        public ClampMode ClampMode { get; }

        public bool UseComponents { get; }
        public bool IsBindless { get; }

        public new static OpCode Create(InstEmitter emitter, ulong address, long opCode) => new OpCodeSured(emitter, address, opCode);

        public OpCodeSured(InstEmitter emitter, ulong address, long opCode) : base(emitter, address, opCode)
        {
            Ra = new Register(opCode.Extract(8, 8), RegisterType.Gpr);
            Rb = new Register(opCode.Extract(0, 8), RegisterType.Gpr);
            Rc = new Register(opCode.Extract(39, 8), RegisterType.Gpr);

            AtomicOp = (AtomicOp)opCode.Extract(24, 3);
            Dimensions = (ImageDimensions)opCode.Extract(33, 3);
            ClampMode = (ClampMode)opCode.Extract(49, 2);

            IsBindless = !opCode.Extract(51);
            UseComponents = !opCode.Extract(52);
        }
    }
}
