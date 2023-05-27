using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled;
using Exiled.API;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Events;

namespace util
{
    public class PluginConfig : IConfig
    {
        public bool IsEnabled { get; set; }
        public bool Debug { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
    public class UtilPlugin : Plugin<PluginConfig>
    {
        public override void OnEnabled()
        {
            base.OnEnabled();
            
        }
    }
}
