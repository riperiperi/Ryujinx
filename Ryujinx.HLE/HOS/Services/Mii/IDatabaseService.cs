using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Ipc;
using Ryujinx.HLE.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Ryujinx.HLE.HOS.Services.Mii
{
    class IDatabaseService : IpcService
    {
        enum Source
        {
            Database = 0,
            Default = 1,
            Account = 2,
            Friend = 3
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct CharInfo
        {
            public UInt128 MiiId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
            public string Nickname;
            public byte FontRegion;
            public byte FavoriteColor;
            public byte Gender;
            public byte Height;
            public byte Build;
            public byte Type;
            public byte RegionMove;
            public byte FacelineType;
            public byte FacelineColor;
            public byte FacelineWrinkle;
            public byte FacelineMake;
            public byte HairType;
            public byte HairColor;
            public byte IsHairFlip;
            public byte EyeType;
            public byte EyeColor;
            public byte EyeScale;
            public byte EyeAspect;
            public byte EyeRotate;
            public byte EyeX;
            public byte EyeY;
            public byte EyebrowType;
            public byte EyebrowColor;
            public byte EyebrowScale;
            public byte EyebrowAspect;
            public byte EyebrowRotate;
            public byte EyebrowX;
            public byte EyebrowY;
            public byte NoseType;
            public byte NoseScale;
            public byte NoseY;
            public byte MouthType;
            public byte MouthColor;
            public byte MouthScale;
            public byte MouthAspect;
            public byte MouthY;
            public byte BeardColor;
            public byte BeardType;
            public byte MustacheType;
            public byte MustacheScale;
            public byte MustacheY;
            public byte GlassTyp;
            public byte GlassColor;
            public byte GlassScale;
            public byte GlassY;
            public byte MoleType;
            public byte MoleScale;
            public byte MoleX;
            public byte MoleY;
            public byte Unknown1;
        }

        // IsUpdated(i32) -> bool
        [Command(0)]
        public ResultCode IsUpdated(ServiceCtx context)
        {
            int Key = context.RequestData.ReadInt32();

            Logger.PrintStub(LogClass.ServiceMii, new { Key });

            context.ResponseData.Write(false);

            return ResultCode.Success;
        }

        // Get1(i32) -> (i32, array<nn::mii::CharInfo, 6>)
        [Command(4)]
        public ResultCode Get1(ServiceCtx context)
        {
            int key = context.RequestData.ReadInt32();

            Logger.PrintStub(LogClass.ServiceMii, new { key });

            // We stub as not returning anything for the time being
            context.ResponseData.Write(0);

            return ResultCode.Success;
        }

        // BuildDefault(i32) -> nn::mii::CharInfo
        [Command(7)]
        public ResultCode BuildDefault(ServiceCtx context)
        {
            int key = context.RequestData.ReadInt32();

            Logger.PrintStub(LogClass.ServiceMii, new { key });

            var charInfo = new CharInfo
            {
                MiiId = new UInt128(0, 1),
                Nickname = "no name",
                FontRegion = 0,
                FavoriteColor = 0,
                Gender = 0,
                Height = 0,
                Build = 0,
                Type = 1,
                RegionMove = 0,
                FacelineType = 0,
                FacelineColor = 0,
                FacelineWrinkle = 0,
                FacelineMake = 0,
                HairType = 0,
                HairColor = 0,
                IsHairFlip = 0,
                EyeType = 0,
                EyeColor = 0,
                EyeScale = 0,
                EyeAspect = 0,
                EyeRotate = 0,
                EyeX = 0,
                EyeY = 0,
                EyebrowType = 0,
                EyebrowColor = 0,
                EyebrowScale = 0,
                EyebrowAspect = 0,
                EyebrowRotate = 0,
                EyebrowX = 0,
                EyebrowY = 0,
                NoseType = 0,
                NoseScale = 0,
                NoseY = 0,
                MouthType = 0,
                MouthColor = 0,
                MouthScale = 0,
                MouthAspect = 0,
                MouthY = 0,
                BeardColor = 0,
                BeardType = 0,
                MustacheType = 0,
                MustacheScale = 0,
                MustacheY = 0,
                GlassTyp = 0,
                GlassColor = 0,
                GlassScale = 0,
                GlassY = 0,
                MoleType = 0,
                MoleScale = 0,
                MoleX = 0,
                MoleY = 0,
                Unknown1 = 0
            };

            // nn::mii::CharInfo size is 88 bytes
            byte[] data = new byte[88];
            unsafe
            {
                fixed (byte* dataptr = data)
                {
                    Marshal.StructureToPtr(charInfo, (IntPtr)dataptr, false);
                }
            }

            context.ResponseData.Write(data);

            return 0;
        }

        // SetInterfaceVersion(u32)
        [Command(22)]
        public ResultCode SetInterfaceVersion(ServiceCtx context)
        {
            uint version = context.RequestData.ReadUInt32();

            Logger.PrintStub(LogClass.ServiceMii, new { version });

            return 0;
        }
    }
}