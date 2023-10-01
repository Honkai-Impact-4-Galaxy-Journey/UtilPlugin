//Copyright 2023 Silver Wolf,All Rights Reserved.
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
using PlayerRoles;

namespace UtilPlugin
{
    public class PluginConfig : IConfig
    {
        [Description("设置是否启用插件")]
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; }
        [Description("是否启用系统核弹")]
        public bool AutoCleanupEnabled { get; set; } = true;
        [Description("是否启用自动清理")]
        public bool EnableAutoCleanup { get; set; } = true;
        [Description("每次自动清理的时间（以秒为单位）")]
        public float Cleanuptime { get; set; } = 500;
        [Description("是否启用系统核弹")]
        public bool SystemWarheadEnabled { get; set; } = true;
        [Description("系统核弹触发时间")]
        public float SystemWarheadTime { get; set; } = 1200;
        [Description("系统核弹触发时公告")]
        public string SystemWarheadBroadcast { get; set; } = "注意，系统核弹已经启动";
        [Description("回合结束后是否自动引爆核弹")]
        public bool DetonateOnRoundEnded { get; set; } = false;
        [Description("启用管理员预留位")]
        public bool ReserveSlotEnabled { get; set; } = false;
        [Description("管理员预留位数量")]
        public int Slots { get; set; } = 5;
        [Description("预留位踢出理由")]
        public string ReserveSlotKickReason { get; set; } = "服务器已满人";
        [Description("允许投票提前开启系统核(Disabled)")]
        public bool AllowVoteSystemWarhead { get; set; } = true;
        [Description("投票时长")]
        public int VotingTime { get; set; } = 90;
        [Description("允许投票开启娱乐")]
        public bool ArrowVoteFunny { get; set; } = true;
        [Description("角色最高血量设置")]
        public Dictionary<RoleTypeId, int> HealthValues { get; set; } = new Dictionary<RoleTypeId, int>() { [RoleTypeId.Scp173] = 4300 };
        [Description("角色杀人回血量")]
        public Dictionary<RoleTypeId, int> HealHps { get; set; } = new Dictionary<RoleTypeId, int>() { [RoleTypeId.Scp939] = 20 };
        [Description("是否启用MySQL支持")]
        public bool MysqlEnabled { get; set; } = false;
        [Description("MySQL 连接字符串")]
        public string MysqlConnectstring { get; set; } = "Server=127.0.0.1;Database=scp5;User=root;Password=awaawa;Charset=utf8";
    }
    public class UtilPlugin : Plugin<PluginConfig>
    {
        public override void OnEnabled()
        {
            base.OnEnabled();
            EventHandler.Register(Config.AutoCleanupEnabled);
            ReserveSlot.Register(Config.ReserveSlotEnabled);
            SystemWarhead.Register();
            Gamemode.Register();
            Voting.OnEnabled(true);
            if (Config.MysqlEnabled)
            {
                Database.Update(Config.MysqlConnectstring);
            }
        }
        public override string Author => "Silver Wolf";
        public override string Name => "UtilPlugin";
        public override Version Version => new Version(1,3,1);
        public static UtilPlugin Instance { get; private set; }
        public UtilPlugin()
        {
            Instance = this;
        }
    }
}
