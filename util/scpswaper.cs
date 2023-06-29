using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem
{
    public class Scpswaper
    {

    }
    public class Scp : ICommand, IUsageProvider
    {
        public string[] Usage => throw new NotImplementedException();

        public string Command => "scp";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "scp互换,使用.scp获取当前可互换scp";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            
        }
    }
}
