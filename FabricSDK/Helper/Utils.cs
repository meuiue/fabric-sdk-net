/*
 *  Copyright 2017 DTCC, Fujitsu Australia Software Technology, IBM - All Rights Reserved.
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *        http://www.apache.org/licenses/LICENSE-2.0
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Hyperledger.Fabric.SDK.Logging;
using Hyperledger.Fabric.SDK.NetExtensions;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using SharpCompress.Archives.Tar;
using SharpCompress.Common;
using SharpCompress.Writers;

namespace Hyperledger.Fabric.SDK.Helper
{
    public static class Utils
    {
        private static readonly ILog logger = LogProvider.GetLogger(typeof(Utils));
        private static readonly bool TRACE_ENABED = logger.IsTraceEnabled();
        private static readonly Config config = Config.GetConfig();
        private static readonly int MAX_LOG_STRING_LENGTH = config.MaxLogStringLength();

        /**
         * Generate hash of the given input using the given Digest.
         *
         * @param input  input data.
         * @param digest the digest to use for hashing
         * @return hashed data.
         */
        public static byte[] Hash(byte[] input, IDigest digest)
        {
            byte[] retValue = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(input, 0, input.Length);
            digest.DoFinal(retValue, 0);
            return retValue;
        }


        private static readonly int NONONCE_LENGTH = 24;

        private static RNGCryptoServiceProvider RANDOM = new RNGCryptoServiceProvider();

        public static byte[] GenerateNonce()
        {

            byte[] values = new byte[NONONCE_LENGTH];
            RANDOM.GetBytes(values);
            return values;
        }

        /**
         * Generate parameter hash for the given chaincode path,func and args
         *
         * @param path Chaincode path
         * @param func Chaincode function name
         * @param args List of arguments
         * @return hash of path, func and args
         */
        public static string GenerateParameterHash(string path, string func, List<string> args)
        {
            logger.Debug($"GenerateParameterHash : path={path}, func={func}, args={string.Join(",",args)}");
                
            // Append the arguments
            StringBuilder param = new StringBuilder(path.Replace("\\","/"));
            param.Append(func);
            args.ForEach(a=>param.Append(a));

            // Compute the hash
            return Hash(Encoding.UTF8.GetBytes(param.ToString()), new Sha3Digest()).ToHexString();
        }

        /**
         * Generate hash of a chaincode directory
         *
         * @param rootDir      Root directory
         * @param chaincodeDir Channel code directory
         * @param hash         Previous hash (if any)
         * @return hash of the directory
         * @throws IOException
         */
        public static string GenerateDirectoryHash(string rootDir, string chaincodeDir, string hash)
        {
            // Generate the project directory
            string projectPath = null;
            if (rootDir == null)
            {
                projectPath = Path.Combine(chaincodeDir);
            }
            else
            {
                projectPath = Path.Combine(rootDir, chaincodeDir);
            }
            DirectoryInfo dir=new DirectoryInfo(projectPath);
            if (!dir.Exists)
                throw new IOException($"The chaincode path \"{projectPath}\" is invalid");
            FileInfo[] files = dir.GetFiles().Where(a => (a.Attributes & (FileAttributes.Normal | FileAttributes.ReadOnly)) != 0).OrderBy(a => a.Name).ToArray();
            string finalhash = hash;
            foreach (FileInfo f in files)
            {
                try
                {
                    byte[] buf = File.ReadAllBytes(f.FullName);
                    byte[] toHash = buf.Union(Encoding.UTF8.GetBytes(finalhash)).ToArray();
                    finalhash=Hash(toHash, new Sha3Digest()).ToHexString();
                }
                catch (IOException ex)
                {
                    throw new Exception($"Error while reading file {f.FullName}", ex);
                }
            }


        // If original hash and final hash are the same, it indicates that no new contents were found
        if (finalhash.Equals(hash)) {
            throw new IOException($"The chaincode directory \"{projectPath}\" has no files");
        }

            return finalhash;
    }

    /**
     * Compress the contents of given directory using Tar and Gzip to an in-memory byte array.
     *
     * @param sourceDirectory  the source directory.
     * @param pathPrefix       a path to be prepended to every file name in the .tar.gz output, or {@code null} if no prefix is required.
     * @param chaincodeMetaInf
     * @return the compressed directory contents.
     * @throws IOException
     */
        public static byte[] GenerateTarGz(string sourceDirectory, string pathPrefix, string chaincodeMetaInf)
        {
            logger.Trace($"generateTarGz: sourceDirectory: {sourceDirectory}, pathPrefix: {pathPrefix}, chaincodeMetaInf: {chaincodeMetaInf}");
            string sourcePath = sourceDirectory;
            using (MemoryStream bos = new MemoryStream())
            using (var writer = WriterFactory.Open(bos, ArchiveType.Tar, CompressionType.GZip))
            {
                writer.WriteAll(sourceDirectory, "*", SearchOption.AllDirectories);
                if (null != chaincodeMetaInf)
                    writer.WriteAll(chaincodeMetaInf, "*", SearchOption.AllDirectories);
                bos.Flush();
                return bos.ToArray();
            }
        }



    /**
     * Generate a v4 UUID
     *
     * @return String representation of {@link UUID}
     */
    public static string GenerateUUID()
    {
        return Guid.NewGuid().ToString().Replace("-", string.Empty);
    }

    /**
     * Create a new {@link Timestamp} instance based on the current time
     *
     * @return timestamp
     */
    public static DateTime GenerateTimestamp()
    {
        return DateTime.UtcNow;
    }

    /**
     * Delete a file or directory
     *
     * @param file {@link File} representing file or directory
     * @throws IOException
     */
        public static void DeleteFileOrDirectory(string name)
        {
            try
            {
                if (Directory.Exists(name))
                {
                    Directory.Delete(name, true);
                }
                else if (File.Exists(name))
                    File.Delete(name);
            }
            catch (Exception e)
            {
                throw new Exception("File or directory does not exist",e);
            }

        }
        public static (string Protocol, string Host, int Port) ParseGrpcUrl(string url)
        {
            (string Protocol, string Host, int Port) ret;
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("URL cannot be null or empty");
            Dictionary<string, string> props = new Dictionary<string, string>();
            Regex p = new Regex("([^:]+)[:]//([^:]+)[:]([0-9]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Match m = p.Match(url);
            if (m.Success)
            {
                ret.Protocol = m.Groups[1].Value;
                ret.Host = m.Groups[2].Value;
                string ports = m.Groups[3].Value;
                int port;
                if (!int.TryParse(ports,out port))
                    throw new ArgumentException("Invalid port");
                ret.Port = port;
                if (!"grpc".Equals(ret.Protocol,StringComparison.InvariantCultureIgnoreCase) && !"grpcs".Equals(ret.Protocol, StringComparison.InvariantCultureIgnoreCase))
                    throw new ArgumentException($"Invalid protocol expected grpc or grpcs and found {ret.Protocol}.");
                return ret;
            }
            throw new ArgumentException("URL must be of the format protocol://host:port");

            // TODO: allow all possible formats of the URL
        }
        /**
 * Check if the strings Grpc url is valid
 *
 * @param url
 * @return Return the exception that indicates the error or null if ok.
 */
        public static Exception CheckGrpcUrl(String url)
        {
            try
            {

                ParseGrpcUrl(url);
                return null;

            }
            catch (Exception e)
            {
                return e;
            }
        }


        /**
         * Read a file from classpath
         *
         * @param fileName
         * @return byte[] data
         * @throws IOException
         */
        /*
        public static byte[] readFileFromClasspath(String fileName) throws IOException
{
    InputStream is = Utils.class.getClassLoader().getResourceAsStream(fileName);
byte[] data = ByteStreams.toByteArray(is);
        try {
            is.close();
        } catch (IOException ex) {
        }
        return data;
    }
    */
   






        /**
         * Private constructor to prevent instantiation.
         */


    }

}
