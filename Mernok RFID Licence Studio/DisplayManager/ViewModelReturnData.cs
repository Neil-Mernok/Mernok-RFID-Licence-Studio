using MernokPasswords;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Mernok_RFID_Licence_Studio
{
    public class ViewModelReturnData 
    {
        //Operation 
        public uint UID = 0;
        public uint IssuerAccess = 0;
        public uint prevUID = 0;
        public uint EditCardUID = 0;
        public uint IssuerUID = 0;
        public uint NewCardUID = 0;
        public int NumberofRWD = 0;
        public bool cardChanged = false;
        public bool RWD_connected = false;
 //       public bool CardInField = false;
        private bool _CardInField;
        public bool CardInField
        {
            get { return _CardInField; }
            set { _CardInField = value; }
        }
        public bool CardRead_Done = false;
        public int NewCardWindow = 0;
        public bool EditCard = false;
        public bool IssuerCardTime = false;
        public CardDetails VMCardDetails = new CardDetails();
        public CardDetails CopiedVMCardDetails = new CardDetails();
        public RFIDCardInfoWrite CardInfoWrite = new RFIDCardInfoWrite();
        public RFIDCardInfoRead cardInfoRead = new RFIDCardInfoRead();

        //Navigation bar details
        public string ViewTitle;
        public string PreviousTitle;
        public string NextTitle;
        public string SubTitle;
        public int CurrentPageNumber;
        public int TotalPageNumber;
        public int NavBarProgress;
        public bool NavigationBar_Active = false;
        public bool NextButton_pressed = false;
        public bool BackButton_pressed = false;
        public bool MenuButton_pressed = false;
        public bool BackArrow_pressed = false;
        public bool NextButtonEnabled = false;
        public bool BackButtonEnabled = false;
        public Visibility MenuButtonEnabled = Visibility.Collapsed;
        public Visibility HelpButtonEnabled = Visibility.Collapsed;

        public Visibility MenuEditBtnEnabled = Visibility.Collapsed;
        public Visibility MenuIssueBtnEnabled = Visibility.Collapsed;

        //main windows       
        public bool StartUpView_Active = false;
        public bool LicenceView_Active = false;
        public bool NewCardAccess_Active = false;
        public bool NewCardIssuer_Active = false;
        public bool NewCardDetail_Active = false;
        public bool NewCardVID_Active = false;
        public bool NewCardVNames_Active = false;
        public bool NewCardType_Active = false;
        public bool NewCardGroup_Active = false;

        //alternate functions
        public bool ExitPromtView_Active = false;
        public bool MenuView_Active = false;

        //descriptive windows
        public bool AboutWindow_Active = false;
        public bool HelpWindow_Active = false;
        public string HelpMessage = "";
        

        //warnings
        public bool CardStillIssuer = false;
        public bool CardFormatError = false;
        public bool LicenceComplete = false;
        public bool CardProgramFail = false;
        public bool CardStillIssuer_Active = false;
        public bool EditCardWarn_Active = false;

        //messages
        public bool CardProramed_done = false;



        //global button events
        public void NextWindow()
        {
            NewCardWindow++;
            if (NewCardWindow==1)
            {
                NewCardIssuer_Active = false;
                NewCardDetail_Active = true;
            }
            if (NewCardWindow==2)
            {

                switch (VMCardDetails.Options)
                {           

                    case 1:
                        NewCardVID_Active = true;
                        break;

                    case 0:
                        NewCardType_Active = true;
                        break;
                    case 2:
                        NewCardVNames_Active = true;
                        break;

                    case 3:
                        NewCardGroup_Active = true;
                        break;

                    default:
                        break;
                }
            }
            if(NewCardWindow==3)
            {
                if(CardStillIssuer)
                {
                    CardStillIssuer_Active = true;
                    NewCardWindow--;
                }
                else if (CardInfoWrite.WriteInfoToCard(VMCardDetails) == 100)
                {
                    CardProramed_done = true;
                    App_datareset();
                }
                else
                {
                    CardProgramFail = true;
                    NewCardWindow--;
                }
                    

            }

        }

        public void BackWindow()
        {
            
            if(NewCardWindow==0)
            {
                NewCardAccess_Active = NewCardIssuer_Active = false;
                EditCard = false;
            }
            if (NewCardWindow==1)
            {
                NewCardDetail_Active = false;
                //VMCardDetails = new CardDetails();
            }
            if (NewCardWindow==2)
            {
                NewCardVNames_Active = NewCardType_Active = NewCardGroup_Active = NewCardVID_Active = false;               
            }

            NewCardWindow--;

        }

        public void MenuButton()
        {
            if(LicenceView_Active)
            {
                EditCard = true;
                EditCardUID = cardInfoRead.cardDetails.cardUID;
                CopiedVMCardDetails = cardInfoRead.cardDetails;
                LicenceView_Active = false;
                NewCardAccess_Active = true;
                NewCardWindow = 0;
            }
        }

        public void BackApp()
        {
            App_datareset();
            //if (LicenceView_Active)
            //{
            //    LicenceView_Active = false;
            //}

            //if (NewCardAccess_Active||NewCardIssuer_Active)
            //{
            //    NewCardAccess_Active = NewCardIssuer_Active = false;
            //}
            //if (NewCardDetail_Active)
            //{
            //    NewCardDetail_Active = false;
            //    NewCardAccess_Active = false;
            //}
            //if (NewCardVID_Active || NewCardVNames_Active || NewCardType_Active || NewCardGroup_Active)
            //{
            //    NewCardVNames_Active = NewCardType_Active = NewCardGroup_Active = NewCardVID_Active = false;
            //    NewCardDetail_Active = true;
            //    NewCardAccess_Active = false;
            //}

            //if(CardFormatError)
            //{
            //    CardFormatError = false;
            //}
        }

        public void App_datareset()
        {
            VMCardDetails = new CardDetails();
            CardRead_Done = false;
            NavigationBar_Active = false;
            NewCardAccess_Active = LicenceView_Active = NewCardDetail_Active = NewCardVID_Active = NewCardVNames_Active = NewCardGroup_Active = NewCardType_Active = NewCardIssuer_Active = false;
            EditCard = false;
            NewCardWindow = 0;
        }

        public void HelpButton()
        {
            HelpWindow_Active = true;
            if(NewCardAccess_Active)
            {
                HelpMessage = "This window requires a valid Issuer card to continue. A valid Issuer card requires:" +
                    " \n 1. The card Expiry date to be one day in the future from today" +
                    " \n 2. The card Access level to be either a Training Officer or a Mernok Engineer" +
                    " \n 3. A Training Officer is only allowed to edit Operator, Trainee Operator, and Temporary Operator cards." +
                    "\n \n If you do not have such a card, you can request an Issuer card from the options menu at the begining of the application.";
            }

            if (NewCardWindow == 1)
            {
                HelpMessage = "This window requires a valid Issuer card to continue. A valid Issuer card requires:" +
                     " \n 1. The card Expiry date to be one day in the future from today" +
                     " \n 2. The card Access level to be either a Training Officer or a Mernok Engineer" +
                     " \n 3. A Training Officer is only allowed to edit Operator, Trainee Operator, and Temporary Operator cards." +
                     "\n \n If you do not have such a card, you can request an Issuer card from the options menu at the begining of the application.";
            }
            if (NewCardWindow == 2)
            {
                switch (VMCardDetails.Options)
                {
                    case 1:
                        //NewCardVID_Active = true;
                        break;

                    case 0:
                        //NewCardType_Active = true;
                        break;
                    case 2:
                        //NewCardVNames_Active = true;
                        break;

                    case 3:
                        //NewCardGroup_Active = true;
                        break;

                    default:
                        break;
                }
            }
            if (NewCardWindow == 3)
            {

            }
        }
        #region General Functions
        public string StringConditioner(string value)
        {
            string string2 = value;
            string[] name2;
            Regex regexItem = new Regex(@"[^A-Z0-9 _]");
            if (string2.Length <= 15)
            {
                string2 = string2.ToUpper();
            }
            else
            {
                string2 = string2.Substring(0, 15).ToUpper();
                //               MessageBox.Show("Tag name my not exceed a length of 15");
            }


            if (!regexItem.IsMatch(string2))
            {
                value = string2;
            }
            else
            {
                //                MessageBox.Show("Tag name my not not contain any special charcters");
                name2 = regexItem.Split(string2);
                string2 = "";
                for (int i = 0; i < name2.Length; i++)
                {
                    if (name2[i] != "")
                    {
                        string2 = string2 + name2[i];
                    }

                }
                //name = regexItem.Replace(name, "$");
                value = string2;
            }

            return value;
        }
        public string StringNumConditioner(string value)
        {
            string string2 = value;
            string[] name2;
            Regex regexItem = new Regex(@"[^0-9]");
            if (string2.Length <= 15)
            {
                string2 = string2.ToUpper();
            }
            else
            {
                string2 = string2.Substring(0, 15).ToUpper();
                //               MessageBox.Show("Tag name my not exceed a length of 15");
            }


            if (!regexItem.IsMatch(string2))
            {
                value = string2;
            }
            else
            {
                //                MessageBox.Show("Tag name my not not contain any special charcters");
                name2 = regexItem.Split(string2);
                string2 = "";
                for (int i = 0; i < name2.Length; i++)
                {
                    if (name2[i] != "")
                    {
                        string2 = string2 + name2[i];
                    }

                }
                //name = regexItem.Replace(name, "$");
                value = string2;
            }

            ulong test;

            if (value != "")
            {
                test = Convert.ToUInt64(value);
                if (test <= 0)
                    test = 1;
                else if (test > ushort.MaxValue)
                    test = ushort.MaxValue;

                value = test.ToString();
            }



            return value;
        }

        #endregion
    }


}
