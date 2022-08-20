using Extension.Array;
using RamFsLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace NyaFs.ImageFormat.Fs.Reader
{
    public class LegacyFsReader : Reader
    {
        bool Loaded = false;
        Types.LegacyImage Image;

        public LegacyFsReader(string Filename)
        {
            Image = new Types.LegacyImage(Filename);

            if (!Image.CorrectHeader)
            {
                Log.Error(0, $"Invalid legacy header in file {Filename}.");
                return;
            }
            if (!Image.Correct)
            {
                Log.Error(0, $"Invalid data in file {Filename}.");
                return;
            }

            // Выберем метод обработки согласно данным заголовка.
            Log.Ok(1, $"Loaded Legacy image");
            Log.Write(1, $"  Operating System: {GetOS(Image.OperatingSystem)}");
            Log.Write(1, $"      Architecture: {GetArch(Image.CPUArchitecture)}");
            Log.Write(1, $"       Compression: {GetCompression(Image.Compression)}");
            Log.Write(1, $"         Timestamp: {Image.Timestamp}");
            Log.Write(1, $" Data Load address: {Image.DataLoadAddress:x08}");
            Log.Write(1, $"EntryPoint address: {Image.EntryPointAddress:x08}");

            if (Image.Type != Types.ImageType.IH_TYPE_RAMDISK)
            {
                Log.Error(0, $"File {Filename} is not ramdisk file.");
                return;
            }

            Loaded = true;
        }

        public override Types.ImageInfo GetImageInfo()
        {
            if (Loaded)
            {
                var Info = new Types.ImageInfo();

                Info.Architecture = Image.CPUArchitecture;
                Info.OperatingSystem = Image.OperatingSystem;
                Info.Name = Image.Name;
                Info.DataLoadAddress = Image.DataLoadAddress;
                Info.EntryPointAddress = Image.EntryPointAddress;
                Info.Type = Image.Type;

                return Info;
            }
            else
                return null;
        }

        /// <summary>
        /// Читаем в файловую систему из cpio-файла
        /// </summary>
        /// <param name="Dst"></param>
        public override void ReadToFs(Filesystem Dst)
        {
            if (!Loaded) return;

            var Data = GetDecompressedData(Image.Data, Image.Compression);

            var FilesystemType = FilesystemDetector.DetectFs(Data);
            Log.Write(1, $"      Filesystem: {GetFilesystemType(FilesystemType)}");

            if (FilesystemType == Types.FsType.Cpio)
            {
                var Reader = new CpioReader(Data);
                Reader.ReadToFs(Dst);
            }
            //else if (FilesystemType == Types.FsType.Ext4)
            //{
            //    var Reader = new ExtReader(Data);
            //    Reader.ReadToFs(Dst);
            //}
            else
                Log.Error(0, "Unsupported filesystem...");
        }

        string GetFilesystemType(Types.FsType Type)
        {
            switch(Type)
            {
                case Types.FsType.Cpio: return "CPIO (ASCII)";
                case Types.FsType.Ext4: return "Ext4";
                default: return "Unknown";
            }
        }

        byte[] GetDecompressedData(byte[] Source, Types.CompressionType Compression)
        {
            switch(Compression)
            {
                case Types.CompressionType.IH_COMP_GZIP: return Compressors.Gzip.Decompress(Source);
                case Types.CompressionType.IH_COMP_NONE: return Source;
                default:
                    Log.Error(0, $"Unsupported compression type: {Compression}");
                    throw new ArgumentException($"Unsupported compression type: {Compression}");
            }
        }

        /// <summary>
        /// Тип ОС
        /// </summary>
        /// <param name="OS"></param>
        /// <returns></returns>
        private string GetOS(Types.OS OS)
        {
            switch(OS)
            {
                case Types.OS.IH_OS_LINUX: return "Linux";
                default: return $"{OS}";
            }
        }

        /// <summary>
        /// Тип архитектуры
        /// </summary>
        /// <param name="CPU"></param>
        /// <returns></returns>
        private string GetArch(Types.CPU CPU)
        {
            switch (CPU)
            {
                case Types.CPU.IH_ARCH_ARM: return "ARM";
                case Types.CPU.IH_ARCH_ARM64: return "ARM64";
                case Types.CPU.IH_ARCH_I386: return "I386";
                case Types.CPU.IH_ARCH_MIPS: return "MIPS";
                case Types.CPU.IH_ARCH_MIPS64: return "MIPS64";
                case Types.CPU.IH_ARCH_X86_64: return "X86_64";
                default: return $"{CPU}";
            }
        }

        /// <summary>
        /// Тип сжатия
        /// </summary>
        /// <param name="Compr"></param>
        /// <returns></returns>
        private string GetCompression(Types.CompressionType Compr)
        {
            switch (Compr)
            {
                case Types.CompressionType.IH_COMP_GZIP: return "gzip";
                case Types.CompressionType.IH_COMP_NONE: return "none";
                default: return $"{Compr}";
            }
        }

        private string GetType(Types.ImageType Type)
        {
            switch (Type)
            {
                case Types.ImageType.IH_TYPE_KERNEL: return "kernel";
                case Types.ImageType.IH_TYPE_MULTI: return "multi";
                case Types.ImageType.IH_TYPE_SCRIPT: return "script";
                case Types.ImageType.IH_TYPE_RAMDISK: return "ramdisk";
                default: return $"{Type}";
            }
        }
    }
}
