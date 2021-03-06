using Ryujinx.Graphics.Shader.Instructions;

namespace Ryujinx.Graphics.Shader.Decoders
{
    class OpCodeSuatom : OpCodeTextureBase
    {
        public Register Rd { get; }
        public Register Ra { get; }
        public Register Rb { get; }

        public AtomicOp AtomicOp { get; }

        public ImageDimensions Dimensions { get; }

        public ClampMode ClampMode { get; }

        public bool ByteAddress { get; }
        public bool UseComponents { get; }
        public bool IsBindless { get; }

        public new static OpCode Create(InstEmitter emitter, ulong address, long opCode) => new OpCodeSuatom(emitter, address, opCode);

        public OpCodeSuatom(InstEmitter emitter, ulong address, long opCode) : base(emitter, address, opCode)
        {
            Rd = new Register(opCode.Extract(0,  8), RegisterType.Gpr);
            Ra = new Register(opCode.Extract(8,  8), RegisterType.Gpr);
            Rb = new Register(opCode.Extract(20, 8), RegisterType.Gpr);

            ByteAddress = opCode.Extract(23);
            AtomicOp = (AtomicOp)opCode.Extract(29, 4); // only in 0xea6 and 0xea0
            Dimensions = (ImageDimensions)opCode.Extract(33, 3);
            ClampMode = (ClampMode)opCode.Extract(49, 2);

            IsBindless = !opCode.Extract(51); // only in 0xea6 and 0xeac
            UseComponents = !opCode.Extract(52);
        }
    }
}
