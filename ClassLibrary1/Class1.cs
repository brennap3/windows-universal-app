using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class Class1
    {
        private NameValueCollection collection;
        private UriBuilder builder;

        public Class1(string uri)
        {
            builder = new UriBuilder(uri);
            collection = System.Web.HttpUtility.ParseQueryString(string.Empty);
        }

        public void AddParameter(string key, string value)
        {
            collection.Add(key, value);
        }

        public Uri Uri
        {
            get
            {
                builder.Query = collection.ToString();
                return builder.Uri;
            }
        }

    }
}
