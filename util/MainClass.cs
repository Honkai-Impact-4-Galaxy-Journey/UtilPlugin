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
        [Description("设置是否启用插件")]
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; }
        [Description("每次自动清理的时间（以秒为单位）")]
        public float Cleanuptime { get; set; } = 300;
    }
    public class UtilPlugin : Plugin<PluginConfig>
    {
        public override void OnEnabled()
        {
            base.OnEnabled();
            EventHandler.Register(true);
            
        }
        public static UtilPlugin Instance { get; private set; }
        public UtilPlugin()
        {
            Instance = this;
        }
    }
}
