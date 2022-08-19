using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace BmuTool
{
    class UbootDumpParser
    {
        private UInt32? GetAddress(string Line)
        {
            var Start = Line.IndexOf(':');
            if (Start > 0)
            {
                return Convert.ToUInt32(Line.Substring(0,Start), 16);
            }

            return null;
        }

        public byte[] ConvertToBytes(string[] Lines)
        {
            var Res = new List<byte>();

            UInt32 LastAddress = 0;
            bool AddressSetted = false;
            var Sep = new char[] { ' ' };
            foreach (var L in Lines)
            {
                var Start = L.IndexOf(':');
                var End = L.IndexOf("    ");
                if ((Start > 0) && (End > 0))
                {
                    var Address = GetAddress(L);
                    if (Address.HasValue)
                    {
                        if (!AddressSetted)
                        {
                            LastAddress = Address.Value;
                            AddressSetted = true;
                        }
                        else
                        {
                            var Diff = (Address.Value - LastAddress);
                            if (Diff != 0x10)
                            {
                                Debug.WriteLine($"Diff: {Diff:X02} after {LastAddress:X08}");
                            }
                            LastAddress = Address.Value;
                        }
                        var Part = L.Substring(Start + 1, End - Start).Trim();

                        var Parts = Part.Split(Sep);
                        foreach (var P in Parts)
                        {
                            if (P.Length > 0)
                            {
                                Res.Add(Convert.ToByte(P, 16));
                            }
                        }
                    }
                }
            }

            return Res.ToArray();
        }

        private bool IsEnded(string S)
        {
            S = S.Trim();
            if (S.Length > 0)
            {
                var Last = S[S.Length - 1];

                return (S.Length < 20) && ((Last == '#') || (Last == '>'));
            }
            else
                return false;
        }

    }
}
