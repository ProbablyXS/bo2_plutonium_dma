using AssaultCubeHack;
using System;
using System.Collections.Generic;
using System.Threading;

namespace RL.game
{
    class Player
    {
        public static List<Player> players = new List<Player>();

        public int strPlayerPosition;
        public int strPlayerList;


        //public int PlayerIsAlive;



        public static int GameModeIsNotFFA = 0;
        public static int[] PlayerNotChooseTeam = { 0, 9 }; //Player not choose TEAM (Spectator = 9)

        public static int isAliveTrue1 = 0;
        public static int isAliveTrue2 = 0;

        public Vector3 PositionHead
        {
            get { return Framework.Memory.ReadHEADScatter(Convert.ToUInt32(strPlayerPosition + Offsets.headPos), PlayerCROUCH); }
        }

        public Vector3 PositionFoot
        {
            get { return Framework.Memory.ReadFOOTScatter(Convert.ToUInt32(strPlayerPosition + Offsets.footPos)); }
        }

        public int PlayerTEAM
        {
            get
            {

                if (strPlayerList != 0)
                {
                    return Framework.Memory.ReadScatter<int>(Convert.ToUInt32(strPlayerList + Offsets.PlayerTEAM));
                }
                return default;
            }
        }

        public int PlayerNumberId
        {
            get
            {
                if (strPlayerList != 0)
                {
                    return Framework.Memory.ReadScatter<int>(Convert.ToUInt32(strPlayerList + Offsets.PlayerNumberID));
                }
                return default;
            }
        }

        public int PlayerTEAMForFFA
        {
            get
            {
                if (strPlayerList != 0)
                {
                    return Framework.Memory.ReadScatter<int>(Convert.ToUInt32(strPlayerList + Offsets.PlayerTEAMForFFA));
                }
                return default;
            }
        }

        public int PlayerISALIVE
        {
            get
            {
                if (strPlayerList != 0)
                {
                    return Framework.Memory.ReadScatter<int>(Convert.ToUInt32(strPlayerList + Offsets.PlayerISALIVE));
                }
                return default;
            }
        } //a mettre en cache

        public int PlayerISALIVE2
        {
            get
            {
                if (strPlayerList != 0)
                {
                    return Framework.Memory.ReadScatter<int>(Convert.ToUInt32(strPlayerList + Offsets.PlayerISALIVE2));
                }
                return default;
            }
        } //a mettre en cache

        public int PlayerCROUCH
        {
            get
            {
                if (strPlayerList != 0)
                {
                    return Framework.Memory.ReadScatter<int>(Convert.ToUInt32(strPlayerList + Offsets.PlayerCROUCH));
                }
                return default;
            }
        } //a mettre en cache

        public string PlayerNAME
        {
            get { return Framework.Memory.ReadStringScatter(Convert.ToUInt32(strPlayerList + Offsets.PlayerNAME), 16); }
        }

        public int PlayerWeaponId
        {
            get
            {
                if (strPlayerList != 0)
                {
                    return Framework.Memory.ReadScatter<int>(Convert.ToUInt32(strPlayerList + Offsets.PlayerWeapon));
                }
                return default;
            }
        }

        public int PlayerPing
        {
            get
            {
                if (strPlayerList != 0)
                {
                    return Framework.Memory.ReadScatter<int>(Convert.ToUInt32(strPlayerList + Offsets.PlayerPING));
                }
                return default;
            }
        }

        public static Vector3 SelfPosFoot
        {
            get { return Framework.Memory.ReadFOOTScatter(Convert.ToUInt32(Offsets.SelfLocalPlayer + Offsets.SelfLocalPlayerPOSITION)); }
        }

        public static int SelfPlayerTeam
        {
            //get { return Framework.Memory.Read<int>(Convert.ToUInt32(Offsets.SelfLocalPlayer + Offsets.SelfLocalPlayerTEAM)); }
            get { return Framework.Memory.ReadScatter<int>(Convert.ToUInt32(Offsets.SelfLocalPlayer + Offsets.SelfLocalPlayerTEAM)); }
        }

        public static int SelfPlayerNumberID
        {
            get { return Framework.Memory.ReadScatter<int>(Convert.ToUInt32(Offsets.SelfLocalPlayer + Offsets.SelfLocalPlayerNumberID)); }
        }

        public static bool PlayerIsValid(Player p)
        {

            if (p == null || p.PlayerISALIVE > isAliveTrue1 || p.PlayerISALIVE2 > isAliveTrue2) return false; //ON EST MORT

            if (p.PlayerTEAMForFFA == GameModeIsNotFFA)
            {
                if (p.PlayerTEAM == PlayerNotChooseTeam[0] || p.PlayerTEAM == PlayerNotChooseTeam[1]) return false; //ON EST PAS SUR LA MAP
            }

            return true;
        }

        public static bool PlayerIsValidForAimbot(Player p)
        {

            if (p == null || p.PlayerISALIVE > isAliveTrue1 || p.PlayerISALIVE2 > isAliveTrue2) //ON EST MORT
            {
                return false;
            }

            if (p.PlayerTEAMForFFA == GameModeIsNotFFA)
            {
                if (p.PlayerTEAM == PlayerNotChooseTeam[0] || p.PlayerTEAM == PlayerNotChooseTeam[1] || p.PlayerTEAM == SelfPlayerTeam) return false; //ON EST PAS SUR LA MAP
            }

            return true;
        }

        public Player(int strPlayerPosition, int strPlayerList)
        {
            this.strPlayerPosition = strPlayerPosition;
            this.strPlayerList = strPlayerList;
        }

    }
}
