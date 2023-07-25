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

namespace UtilPlugin
{
    public class PluginConfig : IConfig
    {
        [Description("设置是否启用插件")]
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; }
        [Description("每次自动清理的时间（以秒为单位）")]
        public float Cleanuptime { get; set; } = 500;
        [Description("是否启用系统核弹")]
        public bool SystemWarheadEnabled { get; set; } = true;
        [Description("系统核弹触发时间")]
        public float SysteamWarheadTime { get; set; } = 1200;
        [Description("系统核弹触发时公告")]
        public string SystemWarheadBroadcast { get; set; } = "注意，系统核弹已经启动";
    }
    public class UtilPlugin : Plugin<PluginConfig>
    {
        public override void OnEnabled()
        {
            base.OnEnabled();
            EventHandler.Register(true);
        }
        public override string Author => "Silver Wolf";
        public override string Name => "UtilPlugin";
        public override Version Version => new Version(1,1,2);
        public static UtilPlugin Instance { get; private set; }
        public UtilPlugin()
        {
            Instance = this;
        }
    }
}
