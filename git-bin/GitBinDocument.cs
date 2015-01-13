using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.RepresentationModel.Serialization;

namespace GitBin
{
    public class GitBinDocument
    {
        public string Filename { get; private set; }
        public List<string> ChunkHashes { get; private set; }

        public GitBinDocument()
        {
            this.ChunkHashes = new List<string>();            
        }

        public GitBinDocument(string filename): this()
        {
            this.Filename = filename;
        }

        public void RecordChunk(string hash)
        {
            this.ChunkHashes.Add(hash);
        }

        public static string ToYaml(GitBinDocument document)
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);

            var serializer = new YamlSerializer<GitBinDocument>();
            serializer.Serialize(stringWriter, document);

            if (Environment.OSVersion.Platform == PlatformID.MacOSX ||
                Environment.OSVersion.Platform == PlatformID.Unix)
            {
                sb.Replace("\n", "\r\n");
            }

            return sb.ToString();
        }

        public static GitBinDocument FromYaml(TextReader textReader ) {
            int numchars = 9;
            char[] startChars = new char[numchars];
            textReader.ReadBlock(startChars, 0, numchars);
            string start = new String(startChars);
            if (start != "Filename:")
            {
                throw new YamlDotNet.Core.SyntaxErrorException();
            }

            var yaml = start + textReader.ReadToEnd();
            
            GitBinDocument document;
            var serializer = new YamlSerializer<GitBinDocument>();

            try
            {
                document = serializer.Deserialize(new StringReader(yaml));
            }
            catch (YamlDotNet.Core.SyntaxErrorException e)
            {
                GitBinConsole.WriteLine("Syntax error in YAML file: {0}\n\n", e.Message);
                throw;
            }

            return document;
        }
    }
}