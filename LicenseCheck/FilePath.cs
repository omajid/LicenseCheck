using System.IO;

namespace LicenseCheck
{
    // instance-based wrapper for Path
    public sealed class FilePath
    {
        private string underlyingPath;
        
        public FilePath(string path)
        {
            underlyingPath = path;
        }

        public bool ContainsPath(string subpath)
        {
            return underlyingPath.Contains(subpath);
        }

        public string GetExtension()
        {
            return Path.GetExtension(underlyingPath);
        }

        public string GetFileName()
        {
            return Path.GetFileName(underlyingPath);
        }

        public StreamReader Read()
        {
            return new StreamReader(underlyingPath);
        }

        public override string ToString()
        {
            return underlyingPath;
        }

    }
}
