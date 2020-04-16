using System;
using System.IO;

namespace FBase.Foundations
{
    public static class UriHelpers
    {
        /// <summary>
        /// Will get the sequence of characters after the last /, even if it's not 
        /// in filename.ext format.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string GetFilenameOnly(this Uri uri)
        {
            return Path.GetFileName(uri.AbsolutePath);
        }

        /// <summary>
        /// Will get the sequence of characters after the last /, even if it's not 
        /// in filename.ext format.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string GetFilenameOnlyWithoutExtension(this Uri uri)
        {
            return Path.GetFileNameWithoutExtension(uri.AbsolutePath);
        }

        /// <summary>
        /// This function will return true if the last string after the last / exists without a final /.
        /// Always end the uri with a / if you intend to target a folder and not a file.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static bool HasFilename(this Uri uri)
        {
            return !string.IsNullOrEmpty(Path.GetFileName(uri.AbsolutePath));
        }


        public static string GetFullUriWithoutFilename(this Uri uri)
        {
            if (!uri.HasFilename())
            {
                return uri.AbsoluteUri;
            }

            int fileNameLength = uri.GetFilenameOnly().Length;
            return uri.AbsoluteUri.Substring(0, uri.AbsoluteUri.Length - fileNameLength);

        }

        /// <summary>
        /// Gets everything between http:// and the first forward slash, including the port if it was specified.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string GetDomainPortionIncludingPort(this Uri uri)
        {
            return uri.Authority;
        }
        /// <summary>
        /// Gets everything between http:// and the first forward slash, without the port if one was specified.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string GetDomainPortionWithoutPort(this Uri uri)
        {
            return uri.Host;
        }

    }
}
