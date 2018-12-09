using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mernok_RFID_Licence_Studio
{
    public class CardDetails
    {
        public uint cardUID;
        public int CommanderRFIDCardMemoryBlock = 176;

        //Sector35 Block1
        public uint AccessLevel;                              //adress B0[0]
        public DateTime Warning_Date;                            //adress B0[1..3]
        public DateTime Expiry_Date;                             //adress B0[4..6]
        public uint Client_Group;                            //adress B0[7..8]
        public uint Client_Site;                             //adress B0[9..10]
        public UInt16 OperationalArea;                          //adress B0[11..12]
        public uint ByPassBits;                                //adress B0[13]
        public int Options;                                   //adress B0[14]

        //Sector35 Block[2..3]
        public uint[] VehicleLicenceType = new uint[30];   //adress B1..B2

        //sector35 Block[4..5]
        public UInt16[] VID = new UInt16[15];              //adress B3..B4

        //sector35 Block6
        public string OperatorName;                        //adress B5

        //sector35 Block7
        public UInt32 IssuerUID;                            //adress B6[0..3]
        public DateTime Training_Date;                       //adress B6[4..6]
        public DateTime Issue_Date;                          //adress B6[7..9]
        public UInt32 EngineerUID;                         //adress B6[10..14]

        //sector35 Block8
        public string EngineerName;         //adress B7

        //sector35 Block9
        public DateTime HotFlagedDate;                       //adress B8[0..2]
        public UInt16 HotFlagedVID;                        //adress B8[3..4]
        public bool Hotflaged_status;                      //adress B8[5]

        //sector35 Block10
        public byte[] VehicleGroup = new byte[15];         //adress B9
        public string[] VehicleGroupstr = new string[15];

        //sector36 Block11
        public uint ProductCode;                            //adress BA[0]



        //sector36 Block[17..31]
        public string[] VehicleNames = new string[15];     //adress [C0..CE]

    }

    public class CardDetailsFile
    {
        public CardDetails FCardDetails;
        public UInt16 version;
        public DateTime dateCreated;
        public string createdBy;            //Name of file creator
    }
}
