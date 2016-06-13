using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TLRPResourceEditor.Data
{
    /// <summary>
    /// Read objects from upk files; those are the Unreal Engine 3 packages used by The Last Remnant.
    /// All game data is located in those files: models, textures, sound, music, text, scripts, 
    /// attributes.
    /// Only those files and objects that are supported by this tool are currently read, the rest
    /// will be ignored.
    /// The data used is usually either in the export or the chunks list. Exports usually contain such
    /// things as map data, textures, and models. Chunks usually contains such things as music.
    /// For the complete upk file format see the correspoding documencation such as
    ///    https://wiki.beyondunreal.com/Unreal_package
    /// or Gildor's Forums at http://www.gildor.org/smf/index.php
    /// </summary>
    public class UPKFile
    {
        /// <summary>
        /// The raw bytes of the file.
        /// </summary>
        public byte[] Data { get; set; }
        /// <summary>
        /// The path and file name used by this file.
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// List of names inside this package.
        /// </summary>
        public List<String> Names { get; set; }
        /// <summary>
        /// List of all Imports used by this package.
        /// </summary>
        public List<String> Imports { get; set; }
        /// <summary>
        /// List of all export objects used by this package.
        /// </summary>
        public List<UPKObject> Exports { get; set; }
        /// <summary>
        /// List of all chunk data used by this package.
        /// </summary>
        public List<byte[]> Chunks { get; set; }
        /// <summary>
        /// Flag for file compression. If set, it usually denotates a lzo compression.
        /// </summary>
        private int Compression;

        /// <summary>
        /// Initializes this object with a upk file. upk files usually end with .upk; in some cases
        /// TLR uses the ending .tlr
        /// The whole file is automatically read into memory and analyzed.
        /// </summary>
        /// <param name="fileToRead"></param>
        public UPKFile(string fileToRead)
        {
            Contract.Requires(fileToRead != null);
            Contract.Requires(fileToRead.Length > 0);
            Contract.Requires(fileToRead.ToLower().EndsWith("upk") || fileToRead.ToLower().EndsWith("tlr"));
            Contract.Requires(File.Exists(fileToRead));

            FileName    = fileToRead;
            Data        = File.ReadAllBytes(fileToRead);
            Compression = BitConverter.ToInt32(Data, 93);
            Names       = ReadNames();
            Imports     = ReadImports();
            Exports     = ReadExports();
            //Chunks = ReadChunks();
        }

        private List<string> ReadNames()
        {
            var names = new List<string>();
            var nameOffset = BitConverter.ToInt32(Data, 29);
            for (int i = 0; i < BitConverter.ToInt32(Data, 25); i++)
            {
                var length = BitConverter.ToInt32(Data, nameOffset);
                names.Add(System.Text.Encoding.UTF8.GetString(Data, nameOffset + 4, length - 1));
                nameOffset += length + 12;
            }
            return names;
        }

        private List<String> ReadImports()
        {
            var imports = new List<string>();
            var importOffset = BitConverter.ToInt32(Data, 45);
            for (int i = 0; i < BitConverter.ToInt32(Data, 41); i++)
            {
                var id = BitConverter.ToInt32(Data, importOffset + 20);
                imports.Add(Names[BitConverter.ToInt32(Data, importOffset + 20)]);
                importOffset += 28;
            }
            return imports;
        }

        private List<UPKObject> ReadExports()
        {
            var exports = new List<UPKObject>();
            var exportCount = BitConverter.ToInt32(Data, 33);
            var exportOffset = BitConverter.ToInt32(Data, 37);

            for (var i = 0; i < exportCount; i++)
            {
                var upk = ReadExportData(exportOffset);

                if (upk.Type == "Texture2D")
                {
                    upk.Attributes = ReadExportAttributes(upk);
                    upk.BulkData = ReadExportBlocks(upk);
                }

                exports.Add(upk);
                exportOffset += (upk.additionalFields * 4) + 72;
            }
            return exports;
        }

        private UPKObject ReadExportData(int exportOffset)
        {
            var nameId = BitConverter.ToInt32(Data, exportOffset + 12);
            var name = Names[nameId];

            var dataSize = BitConverter.ToInt32(Data, exportOffset + 32);
            var dataOffset = BitConverter.ToInt32(Data, exportOffset + 36);
            var additionalFields = BitConverter.ToInt32(Data, exportOffset + 44);

            if (dataSize < 1)
            {
                return new UPKObject { Name = name, Type = "None" };
            }

            var id = BitConverter.ToInt32(Data, exportOffset);
            var type = Names[BitConverter.ToInt32(Data, exportOffset + 12)];
            var data = new byte[dataSize];
            type = Names[Names.Count - 1 + id];
            Array.Copy(Data, dataOffset, data, 0, dataSize);
            return new UPKObject { Name = name, Type = type, ExportData = data, dataOffset = dataOffset, additionalFields = additionalFields };
        }

        private Dictionary<string, int> ReadExportAttributes(UPKObject upk)
        {
            var result = new Dictionary<string, int>();
            var attributeOffset = 4;
            var attrName = "";
            while (attrName != "SourceFileTimestamp" && attrName != "None")
            {
                attrName = Names[BitConverter.ToInt32(Data, upk.dataOffset + attributeOffset)];
                var attrType = Names[BitConverter.ToInt32(Data, upk.dataOffset + attributeOffset + 8)];
                var attrLength = BitConverter.ToInt32(Data, upk.dataOffset + attributeOffset + 16);
                var attrValue = BitConverter.ToInt32(Data, upk.dataOffset + attributeOffset + 24);
                result[attrName] = attrValue;
                attributeOffset += 24;
                attributeOffset += attrLength;
                if (attrLength == 0)
                    attributeOffset += 4;
            }
            upk.attributeOffset = attributeOffset;
            return result;
        }

        private List<BulkDataInfo> ReadExportBlocks(UPKObject upk)
        {
            var rawDataStart = upk.dataOffset + upk.attributeOffset + 20;

            var blockOffset = BitConverter.ToInt32(Data, rawDataStart);
            var numberOfBlocks = BitConverter.ToInt32(Data, blockOffset);
            var bulkdata = new List<BulkDataInfo>();
            var bulkoffset = blockOffset + 4;

            for (var i = 0; i < numberOfBlocks; i++)
            {
                var newBulk = new BulkDataInfo
                {
                    Format = BitConverter.ToInt32(Data, bulkoffset),
                    UncompressedSize = BitConverter.ToInt32(Data, bulkoffset + 4),
                    CompressedSize = BitConverter.ToInt32(Data, bulkoffset + 8),
                    OffsetToData = BitConverter.ToInt32(Data, bulkoffset + 12)
                };

                if (newBulk.Format == 16 || newBulk.Format == 0)
                {
                    bulkoffset = newBulk.OffsetToData + newBulk.CompressedSize;
                    newBulk.Width = BitConverter.ToInt32(Data, bulkoffset + 0);
                    newBulk.Height = BitConverter.ToInt32(Data, bulkoffset + 4);
                    newBulk.CompressedData = new byte[newBulk.CompressedSize];
                    Array.Copy(Data, newBulk.OffsetToData, newBulk.CompressedData, 0, newBulk.CompressedSize);
                    bulkoffset += 8;
                }
                else
                {
                    newBulk.Width = BitConverter.ToInt32(Data, bulkoffset + 16);
                    newBulk.Height = BitConverter.ToInt32(Data, bulkoffset + 20);
                    bulkoffset += 24;
                }

                bulkdata.Add(newBulk);
            }
            return bulkdata;
        }
    }

    /// <summary>
    /// Used in export objects to store the raw data inside them. Usually used by types
    /// like textures and models.
    /// </summary>
    public class BulkDataInfo
    {
        public int Format { get; set; }
        public int UncompressedSize { get; set; }
        public int CompressedSize { get; set; }
        public int OffsetToData { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public byte[] CompressedData { get; set; }
    }

    /// <summary>
    /// Represents an export object in the upk package file.
    /// Export object have more attributes than just the raw bytes themselves.
    /// </summary>
    public class UPKObject
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public byte[] ExportData { get; set; }
        public Dictionary<string, int> Attributes { get; set; } = new Dictionary<string, int>();
        public List<BulkDataInfo> BulkData { get; set; }
        public int dataOffset { get; set; }
        public int rawDataOffset { get; set; }
        public int attributeOffset { get; set; }
        public int additionalFields { get; set; }

        public override string ToString()
        {
            return Name;
        }

        #region Image decompression removed for the time being until licencing issues with lzo are solved
        ///// <summary>
        ///// Decompresses and converts the raw texture data into a usable BitmapSource.
        ///// </summary>
        ///// <returns></returns>
        //public BitmapSource Texture2D()
        //{
        //    using (var bmp = new Bitmap(BulkData[0].Width, BulkData[0].Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
        //    {
        //        var imageBuffer = DecompressTextureData(BulkData[0]);
        //        var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
        //        var data = bmp.LockBits(rect, ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        //        Marshal.Copy(imageBuffer, 0, data.Scan0, imageBuffer.Length);
        //        bmp.UnlockBits(data);
        //        var source = BitmapSource.Create(bmp.Width, bmp.Height, bmp.HorizontalResolution, bmp.VerticalResolution, PixelFormats.Bgra32, null, data.Scan0, rect.Width * rect.Height * 4, data.Stride);
        //        return source;
        //    }
        //}

        //private static byte[] DecompressTextureData(BulkDataInfo bulkDataInfo)
        //{
        //    var decompressedData = DecompressLZOChunk(bulkDataInfo.CompressedData);
        //    var textureBuffer = DxtTools.DecompressImage(bulkDataInfo.Width, bulkDataInfo.Height, decompressedData);
        //    return textureBuffer;
        //}

        //private static byte[] DecompressLZOChunk(byte[] data)
        //{
        //    var blockSize = BitConverter.ToUInt32(data, 4);
        //    var uncompressedSize = BitConverter.ToUInt32(data, 12);
        //    var numBlocks = (int)Math.Ceiling((float)uncompressedSize / (float)blockSize);

        //    var offset = 16 + numBlocks * 8;

        //    var result = new byte[0];

        //    for (int i = 0; i < numBlocks; i++)
        //    {
        //        var compressedSizeBlock = BitConverter.ToInt32(data, 16 + i * 8);
        //        var uncompressedSizeBlock = BitConverter.ToInt32(data, 16 + i * 8 + 4);
        //        var compressedBlock = new byte[compressedSizeBlock];

        //        Array.Copy(data, offset, compressedBlock, 0, compressedSizeBlock);
        //        var uncompressedBlock = Lzo.Decompress(compressedBlock, uncompressedSizeBlock);

        //        var resultTemp = new byte[result.Length + uncompressedSizeBlock];
        //        result.CopyTo(resultTemp, 0);
        //        uncompressedBlock.CopyTo(resultTemp, result.Length);
        //        result = resultTemp;

        //        offset += compressedSizeBlock;
        //    }
        //    return result;
        //}
        #endregion
    }
}
