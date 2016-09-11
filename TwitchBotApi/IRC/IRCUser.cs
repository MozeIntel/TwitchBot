using System;
using System.Collections.Generic;

namespace TwitchBotApi.IRC
{
    public enum IRCUserType
    {
        NORMAL,
        MOD,
        BROADCASTER,
        GLOBAL_MOD,
        ADMIN,
        STAFF
    }

    public class IRCUser
    {
        //The IRC sender name. Should be equal to the Display Name, only lowercase
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public bool Subscriber { get; set; }

        public bool Turbo { get; set; }

        public IRCUserType UserType { get; set; }

        //The user's RRGGBB color code (in hex). If user never set it, defaults to string.empty
        public string ChatColor { get; set; }

        /*
         * The minimum needed to construct a user object.
         * Subscriber, Turbo and user-type will start out at the default (False, False, Normal)
         */ 
        public IRCUser(string name, string channel)
        {
            Name = name;
            DisplayName = name;
            Subscriber = false;
            Turbo = false;
            UserType = name == channel ? IRCUserType.BROADCASTER : IRCUserType.NORMAL;
        }

        //Parse IRC message data into an IRC user
        public IRCUser(string name, string channel, IDictionary<string, string> tags)
        {
            Name = name;
            DisplayName = tags["display-name"];
            Subscriber = tags["subscriber"] == "1";
            Turbo = tags["turbo"] == "1";
            UserType = ParseUserType(tags["user-type"]);
            ChatColor = tags["color"];
        }

        //Update this instance of the object with a more updated version (possibly gained from a PRIVMSG)
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
