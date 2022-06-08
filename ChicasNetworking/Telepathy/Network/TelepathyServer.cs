using Telepathy;
using System.Linq;
using Lovatto.Chicas.Internal;

namespace Lovatto.Chicas
{
    public class TelepathyServer : Server
    {
        /// <summary>
        /// 
        /// </summary>      
        public TelepathyServer(int MaxMessageSize) : base(MaxMessageSize) { }
        
    }
}