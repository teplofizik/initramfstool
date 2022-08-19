﻿using CrcSharp;
using Extension.Array;
using Extension.Packet;
using System;
using System.Collections.Generic;
using System.Text;

namespace NyaFs.ImageFormat.Types
{
    class LegacyImage : RawPacket
    {
		public LegacyImage(byte[] Data) : base(Data) { }

		public LegacyImage(string Filename) : base(System.IO.File.ReadAllBytes(Filename)) { }

		/// <summary>
		/// Ключевая последовательность байт для идентификации образа
		/// </summary>
		public uint Magic => ReadUInt32BE(0x00);

		/// <summary>
		/// Контрольная сумма данных
		/// </summary>
		public long Crc
		{
			get { return ReadUInt32BE(0x18); }
			set { WriteUInt32BE(0x18, Convert.ToUInt32(value)); }
		}

		/// <summary>
		/// Контрольная сумма заголовка
		/// </summary>
		public long HeaderCrc
		{
			get { return ReadUInt32BE(0x04); }
			set { WriteUInt32BE(0x04, Convert.ToUInt32(value)); }
		}

		/// <summary>
		/// Корректный ли заголовок?
		/// </summary>
		public bool CorrectHeader
		{
			get
			{
				if (Magic != 0x27051956) return false;

				var Header = ReadArray(0, 0x40);
				Header.WriteUInt32(4, 0);

				return CalcCrc(Header) == HeaderCrc;
			}
		}
		/// <summary>
		/// Являются ли корректными данными
		/// </summary>
		public bool Correct => CalcCrc(Data) == Crc;

		/// <summary>
		/// Длина данных
		/// </summary>
		public long Length
		{
			get { return ReadUInt32BE(0x0C); }
			set { WriteUInt32BE(0x0C, Convert.ToUInt32(value)); }
		}

		/// <summary>
		/// Адрес, куда загружаются данные
		/// </summary>
		public uint DataLoadAddress
		{
			get { return ReadUInt32BE(0x10); }
			set { WriteUInt32BE(0x10, Convert.ToUInt32(value)); }
		}

		/// <summary>
		/// Точка входа
		/// </summary>
		public uint EntryPointAddress
		{
			get { return ReadUInt32BE(0x14); }
			set { WriteUInt32BE(0x14, Convert.ToUInt32(value)); }
		}

		/// <summary>
		/// Тип операционной системы
		/// </summary>
		public OS OperatingSystem
        {
			get { return (OS)ReadByte(0x1C); }
			set { WriteByte(0x1C, Convert.ToByte(value)); }
        }

		/// <summary>
		/// Тип архитектуры
		/// </summary>
		public CPU CPUArchitecture
		{
			get { return (CPU)ReadByte(0x1D); }
			set { WriteByte(0x1D, Convert.ToByte(value)); }
		}

		/// <summary>
		/// Тип образа
		/// </summary>
		public ImageType Type
		{
			get { return (ImageType)ReadByte(0x1E); }
			set { WriteByte(0x1E, Convert.ToByte(value)); }
		}

		/// <summary>
		/// Тип сжатия
		/// </summary>
		public CompressionType Compression
		{
			get { return (CompressionType)ReadByte(0x1F); }
			set { WriteByte(0x1F, Convert.ToByte(value)); }
		}

		/// <summary>
		/// Название образа
		/// </summary>
		public string Name => ReadString(0x20, 0x20);

		/// <summary>
		/// Время сборки образа
		/// </summary>
		public DateTime Timestamp => ConvertFromUnixTimestamp(ReadUInt32BE(0x08));

		public byte[] Data => ReadArray(0x40, Length);

		static UInt32 CalcCrc(byte[] data)
		{
			var crc32 = new Crc(new CrcParameters(32, 0x04c11db7, 0xffffffff, 0xffffffff, true, true));

			return Convert.ToUInt32(crc32.CalculateAsNumeric(data));
		}

		static DateTime ConvertFromUnixTimestamp(long timestamp)
		{
			DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return origin.AddSeconds(timestamp);
		}
	}

	//#define LZ4F_MAGIC	0x184D2204	/* LZ4 Magic Number		*/
	//#define IH_MAGIC	0x27051956	/* Image Magic Number		*/
	//#define IH_NMLEN		32	/* Image Name Length		*/

	/*
	 * Legacy format image header,
	 * all data in network byte order (aka natural aka bigendian).
	 */
	/* typedef struct image_header
	{
		uint32_t ih_magic;          // Image Header Magic Number
		uint32_t ih_hcrc;           // Image Header CRC Checksum
		uint32_t ih_time;           // Image Creation Timestamp
		uint32_t ih_size;           // Image Data Size
		uint32_t ih_load;           // Data	 Load  Address
		uint32_t ih_ep;             // Entry Point Address
		uint32_t ih_dcrc;           // Image Data CRC Checksum
		uint8_t ih_os;              // Operating System
		uint8_t ih_arch;            // CPU architecture
		uint8_t ih_type;            // Image Type
		uint8_t ih_comp;            // Compression Type
		uint8_t ih_name[IH_NMLEN];  // Image Name
	}
	image_header_t; */
}
