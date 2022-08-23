﻿using System;

namespace NyaFsTest
{
    class Program
    {
        static void Main(string[] args)
        {
            TestScript();
        }

        static NyaFs.Processor.Scripting.ScriptBase GetBase()
        {
            var B = new NyaFs.Processor.Scripting.ScriptBase();
            B.Add(new NyaFs.Processor.Scripting.Commands.Load());
            B.Add(new NyaFs.Processor.Scripting.Commands.Store());
            B.Add(new NyaFs.Processor.Scripting.Commands.Set());
            B.Add(new NyaFs.Processor.Scripting.Commands.Fs.Dir());
            B.Add(new NyaFs.Processor.Scripting.Commands.Fs.File());
            B.Add(new NyaFs.Processor.Scripting.Commands.Fs.Rm());

            return B;
        }

        static void TestScript()
        {
            var Processor = new NyaFs.Processor.ImageProcessor();

            var Base = GetBase();
            var Script = new NyaFs.Processor.Scripting.ScriptParser(Base, "test", new string[] {
               // "load initramfs.bin.SD ramfs legacy",
                "load test.fit ramfs fit",
                "load test.fit devtree fit",
                "load test.fit kernel fit",
                "set ramfs name TestImageARM64",
                "set ramfs os linux",
                "set ramfs arch arm64",
                "dir . rwxr-xr-x 0 0",
                "rm tmp",
                "file etc/test.txt test.txt rwxr--r-- 0 0",
                "store initramfs.bin.SD.modified ramfs legacy"
            }).Script;

            if (!Script.HasErrors)
                Processor.Process(Script);
            else
                Console.WriteLine("Errors in script.");
        }
    }
}
