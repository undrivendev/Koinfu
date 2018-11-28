using System;
using System.Collections.Generic;
using System.Text;

namespace Mtd.Koinfu.BLL
{
    public class Exchange : IdEntity,IEquatable<Exchange>
    {
        public string Name { get; protected set; }
        public string RestEndpoint { get; protected set; }
        public string WebsocketEndpoint { get; protected set; }
        public int PollIntervalMs { get; protected set; }
        public bool ReversedCurrencyPairs { get; protected set; }


        public override string ToString()
        {
            return this.Name;
        }

        #region Equals
        public static bool operator ==(Exchange l, Exchange r) => l?.ToString() == r?.ToString();
        public static bool operator !=(Exchange l, Exchange r) => l?.ToString() != r?.ToString();
        public override int GetHashCode() => this.ToString().GetHashCode();
        public override bool Equals(object obj) => (obj as Exchange) == this;
        public bool Equals(Exchange other) => this.ToString() == other.ToString();
        #endregion
    }
}
