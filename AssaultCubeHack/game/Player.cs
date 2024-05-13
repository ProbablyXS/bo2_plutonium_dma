using AssaultCubeHack;
using System;
using System.Collections.Generic;

namespace RL.game
{
    class Player
    {
        public static List<Player> players = new List<Player>();

        public int strPlayerPosition;
        public int strPlayerList;

        public static int GameModeIsNotFFA = 0;
        public static int[] PlayerNotChooseTeam = { 0, 9 }; //Player not choose TEAM (Spectator = 9)

        public static int isAliveTrue1 = 0;
        public static int isAliveTrue2 = 0;

        public Vector3 PositionHead
        {
            get { return Framework.Memory.ReadHEAD(Convert.ToUInt32(strPlayerPosition + Offsets.headPos), PlayerCROUCH, Framework.Offsets.vmm, Framework.Offsets.processPid); }
        }

        public Vector3 PositionFoot
        {
            get { return Framework.Memory.ReadFOOT(Convert.ToUInt32(strPlayerPosition + Offsets.footPos), Framework.Offsets.vmm, Framework.Offsets.processPid); }
        }

        public int PlayerTEAM
        {
            get { return Framework.Memory.Read<int>(Convert.ToUInt32(strPlayerList + Offsets.PlayerTEAM), Framework.Offsets.vmm, Framework.Offsets.processPid); }
        }

        public int PlayerNumberId
        {
            get { return Framework.Memory.Read<int>(Convert.ToUInt32(strPlayerList + Offsets.PlayerNumberID), Framework.Offsets.vmm, Framework.Offsets.processPid); }
        }

        public int PlayerTEAMForFFA
        {
            get { return Framework.Memory.Read<int>(Convert.ToUInt32(strPlayerList + Offsets.PlayerTEAMForFFA), Framework.Offsets.vmm, Framework.Offsets.processPid); }
        }

        public int PlayerISALIVE
        {
            get { return Framework.Memory.Read<int>(Convert.ToUInt32(strPlayerList + Offsets.PlayerISALIVE), Framework.Offsets.vmm, Framework.Offsets.processPid); }
        }

        public int PlayerISALIVE2
        {
            get { return Framework.Memory.Read<int>(Convert.ToUInt32(strPlayerList + Offsets.PlayerISALIVE2), Framework.Offsets.vmm, Framework.Offsets.processPid); }
        }

        public int PlayerCROUCH
        {
            get { return Framework.Memory.Read<int>(Convert.ToUInt32(strPlayerList + Offsets.PlayerCROUCH), Framework.Offsets.vmm, Framework.Offsets.processPid); }
        }

        public string PlayerNAME
        {
            get { return Memory.ReadString(strPlayerList + Offsets.PlayerNAME, 16); }
        }

        public int PlayerWeaponId
        {
            get { return Framework.Memory.Read<int>(Convert.ToUInt32(strPlayerList + 0x5B8), Framework.Offsets.vmm, Framework.Offsets.processPid); }
        }

        public int PlayerPing
        {
            get { return Framework.Memory.Read<int>(Convert.ToUInt32(strPlayerList + Offsets.PlayerPING), Framework.Offsets.vmm, Framework.Offsets.processPid); }
        }

        public static Vector3 SelfPosFoot
        {
            get { return Framework.Memory.ReadFOOT(Convert.ToUInt32(Offsets.SelfLocalPlayer + Offsets.SelfLocalPlayerPOSITION), Framework.Offsets.vmm, Framework.Offsets.processPid); }
        }

        public static int SelfPlayerTeam
        {
            get { return Framework.Memory.Read<int>(Convert.ToUInt32(Offsets.SelfLocalPlayer + Offsets.SelfLocalPlayerTEAM), Framework.Offsets.vmm, Framework.Offsets.processPid); }
        }

        public static int SelfPlayerNumberID
        {
            get { return Framework.Memory.Read<int>(Convert.ToUInt32(Offsets.SelfLocalPlayer + Offsets.SelfLocalPlayerNumberID), Framework.Offsets.vmm, Framework.Offsets.processPid); }
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
