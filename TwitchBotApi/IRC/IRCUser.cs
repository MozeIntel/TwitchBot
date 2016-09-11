using System;
using System.Collections.Generic;

namespace TwitchBotApi.IRC
{
    public enum IRCUserType
    {
        NORMAL,
        MOD,
        BROADCASTER,
        STAFF,
        GLOBAL_MOD,
        ADMIN
    }

    public class IRCUser
    {
        /// <summary>The IRC name (should equal to the <see cref="IRCUser.DisplayName"/>, only in lower-case.</summary>
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public bool Subscriber { get; set; }

        public bool Turbo { get; set; }

        public IRCUserType UserType { get; set; }

        /// <summary>The user's RRGGBB color code (in hex). If user never set it, defaults to <see cref="string.Empty"/>.</summary>
        public string ChatColor { get; set; }

        /// <summary>
        /// Minimum information needed to construct a <see cref="IRCUser"/> object.
        /// </summary>
        /// <param name="name">The IRC name for this user.</param>
        /// <param name="channel">The channel this user is a part of.</param>
        public IRCUser(string name, string channel)
        {
            Name = name;
            DisplayName = name;
            Subscriber = false;
            Turbo = false;
            UserType = name == channel ? IRCUserType.BROADCASTER : IRCUserType.NORMAL;
        }

        //TODO: Replace with better parsing method (maybe from an IRCMessage directly)
        public IRCUser(string name, string channel, IDictionary<string, string> tags)
        {
            Name = name;
            DisplayName = tags["display-name"];
            Subscriber = tags["subscriber"] == "1";
            Turbo = tags["turbo"] == "1";
            UserType = ParseUserType(tags["user-type"]);
            ChatColor = tags["color"];
        }

        /// <summary>
        /// Updates this object with a new <see cref="IRCUser"/> object.
        /// </summary>
        /// <param name="newInfo">Updated object.</param>
        public void Update(IRCUser newInfo)
        {
            DisplayName = newInfo.DisplayName;
            Subscriber = newInfo.Subscriber;
            Turbo = newInfo.Turbo;
            UserType = newInfo.UserType;
            ChatColor = newInfo.ChatColor;
        }

        private static IRCUserType ParseUserType(string userTypeTag)
        {
            return userTypeTag == string.Empty ? IRCUserType.NORMAL : (IRCUserType)Enum.Parse(typeof(IRCUserType), userTypeTag, true);
        }

        private static IRCUserType ParseUserType(string user, string channel, string userTypeTag)
        {
            return user == channel ? IRCUserType.BROADCASTER : ParseUserType(userTypeTag);
        }

        /// <summary>
        /// Does not count global mods, admin or staff as moderars./>
        /// </summary>
        /// <returns>Whether the user has moderator permissions.</returns>
        public bool IsMod()
        {
            return UserType == IRCUserType.MOD || UserType == IRCUserType.BROADCASTER;
        }

        public override bool Equals(object obj)
        {
            IRCUser user = obj as IRCUser;
            return user == null ? false : user.Name == Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
