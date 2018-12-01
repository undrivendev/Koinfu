using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mds.Koinfu.BLL
{
    public static class Helper
    {
        public static Uri CombineUrlsAsStrings(params string[] urls)
        {
            var tempPaths = urls.Select(a => a.ToString().TrimStart('/').TrimEnd('/'));
            var finalString = String.Join("/", tempPaths);
            return new Uri(finalString);

        }


        /// <summary>
        /// copies the properties with the same name and type form the objectFrom to the objectTo. 
        /// </summary>
        public static void CopyObjectPropertyValues(object objectFrom, object objectTo)
        {
            var propertiesFrom = objectFrom.GetType().GetProperties();
            var propertiesTo = objectTo.GetType().GetProperties();

            foreach (var propFrom in propertiesFrom)
            {
                foreach (var propTo in propertiesTo)
                {
                    if (propFrom.Name == propTo.Name && propFrom.PropertyType == propTo.PropertyType)
                    {
                        propTo.SetValue(objectTo, propFrom.GetValue(objectFrom));
                        break;
                    }
                }
            }

        }
    }

}
