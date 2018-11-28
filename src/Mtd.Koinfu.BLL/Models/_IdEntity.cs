using System;
using System.Collections.Generic;
using System.Text;

namespace Mtd.Koinfu.BLL
{
    public abstract class IdEntity
    {
        /// <summary>
        /// persistence-specific property, it should not be here: http://enterprisecraftsmanship.com/2014/12/27/dont-use-ids-domain-entities/
        /// at least it does not participate in equality functions and it's in a separate class
        /// </summary>
        public int Id { get; protected set; }

    }
}
