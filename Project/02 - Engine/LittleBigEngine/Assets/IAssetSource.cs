using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ionic.Zip;
using Ionic.Crc;

namespace LBE.Assets
{
    public interface IAssetSource
    {
        String GetFullPath(String path);
        bool Exists(String path);
        Stream Open(String path);

        IAssetDependency CreateDependency(String path);
    }

    public class FileSystemSource : IAssetSource
    {
        String m_contentRoot;
        public String ContentRoot
        {
            get { return m_contentRoot; }
        }

        public FileSystemSource(String root)
        {
            m_contentRoot = Path.GetFullPath(root);
        }

        public bool Exists(string path)
        {
            var filePath = Path.Combine(m_contentRoot, path);
            return File.Exists(filePath);
        }

        public Stream Open(string path)
        {
            var filePath = Path.Combine(m_contentRoot, path);
            return File.OpenRead(filePath);
        }

        public IAssetDependency CreateDependency(string path)
        {
            var filePath = Path.Combine(m_contentRoot, path);
            return Engine.AssetManager.GetFileDependency(filePath);
        }

        public string GetFullPath(string path)
        {
            var filePath = Path.Combine(m_contentRoot, path);
            return filePath;
        }
    }

    public class ZipFileSource : IAssetSource
    {
        class DummyDependency : IAssetDependency
        {
            public event OnChange OnAssetChanged;
            public void Reload() {}
        }

        ZipFile m_zipFile;

        public ZipFileSource(String zipFilePath)
        {
            m_zipFile = ZipFile.Read(zipFilePath);
        }

        public bool Exists(string path)
        {
            return m_zipFile.ContainsEntry(path);
        }

        public Stream Open(string path)
        {
            return new SeekableZipStream(m_zipFile[path]);
        }

        public IAssetDependency CreateDependency(string path)
        {
            return new DummyDependency();
        }

        public string GetFullPath(string path)
        {
            throw new NotImplementedException();
        }
    }

    public class SeekableZipStream : Stream, IDisposable
    {
        ZipEntry m_entry;
        CrcCalculatorStream m_currentStream;

        public SeekableZipStream(ZipEntry entry)
        {
            m_entry = entry;
            m_currentStream = m_entry.OpenReader();
        }

        public override bool CanRead
        {
            get { return m_currentStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return m_currentStream.CanWrite; }
        }

        public override long Length
        {
            get { return m_currentStream.Length; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return m_currentStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (origin == SeekOrigin.Begin)
            {
                m_currentStream.Close();
                m_currentStream = m_entry.OpenReader();
            }

            for (int i = 0; i < offset; i++)
                ReadByte();

            return Position;
        }

        public override void Flush()
        {
            m_currentStream.Flush();
        }

        void IDisposable.Dispose()
        {
            m_currentStream.Close();
        }

        public override void Close()
        {
            m_currentStream.Close();
        }

        public override long Position
        {
            get { return m_currentStream.Position; }
            set { Seek(value, SeekOrigin.Begin); }
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
