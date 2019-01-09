using RFID;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Mernok_RFID_Licence_Studio
{
    class MenuViewModel : ViewModel
    {
        private MenuView _viewInstance;

        public ICommand AboutBtn { get; private set; }

        public ICommand HotFlag { get; private set; }

        public ICommand IssuerCardBtn { get; private set; }

        public ICommand IssuerFileCardBtn { get; private set; }

        public ICommand Exitbtn { get; private set; }

        public ICommand Formatbtn { get; private set; }

        public ICommand AdvancedSet { get; private set; }

        private bool AboutBtnPressed = false;

        private bool IssuerCardBtnPressed = false;

        private bool IssuerFileCardBtnPressed = false;

        private bool _AdvancedSet = false; // to do for creating mernok cards

        private bool _buttonExitPressed = false;

        private bool _HotflagBtnPressed = false;

        private bool _FormatbtnPressed = false;


        public MenuViewModel(UserControl control) : base(control)
        {
            AboutBtn = new DelegateCommand(AboutBtnHandler);
            IssuerCardBtn = new DelegateCommand(IssuerCardBtnHandler);
            IssuerFileCardBtn = new DelegateCommand(IssuerFileCardBtnHandler);
            Exitbtn = new DelegateCommand(ExitBtnHandler);
            HotFlag = new DelegateCommand(HotFlagBtnHandler);
            Formatbtn = new DelegateCommand(Formatbtnhandler);
            control.DataContext = this;
            _viewInstance = (MenuView)control;
        }

        private void IssuerFileCardBtnHandler()
        {
            IssuerFileCardBtnPressed = true;
        }

        private void Formatbtnhandler()
        {
            _FormatbtnPressed = true;
        }

        private void HotFlagBtnHandler()
        {
            _HotflagBtnPressed = true;
        }

        public void AboutBtnHandler()
        {
            AboutBtnPressed = true;
        }

        public void IssuerCardBtnHandler()
        {
            IssuerCardBtnPressed = true;
        }

        private void ExitBtnHandler()
        {
            _buttonExitPressed = true;
        }

        public override void Update(ViewModelReturnData VMReturnData)
        {
            //Only update this view if visible

            if (VMReturnData.MenuView_Active)
            {
                VMReturnData.OptionsPressed = true;
                this.View.Visibility = Visibility.Visible;
 //               VMReturnData.NavigationBar_Active = false;
                EditCardVis = VMReturnData.MenuEditBtnEnabled;
                IssueCardVis = VMReturnData.MenuIssueBtnEnabled;


                if (AboutBtnPressed)
                {
                    AboutBtnPressed = false;
                    VMReturnData.AboutWindow_Active = true;

                }

                if(_HotflagBtnPressed)
                {
                    CardDetails WriteCardDetails = new CardDetails() { Hotflaged_status = true, HotFlagedDate = DateTime.Now, HotFlagedVID = 123 };
                    RFIDCardInfoWrite rFIDCardInfoWrite = new RFIDCardInfoWrite();
                    _HotflagBtnPressed = false;
                    byte[] temp = rFIDCardInfoWrite.Block9(WriteCardDetails);
                    if (MernokRFID_interface.Mifare_Write_Block(Mifare_key.A, 0, WriteCardDetails.CommanderRFIDCardMemoryBlock + 8, temp))
                    {

                    }
                }

                if(_FormatbtnPressed)
                {
                    _FormatbtnPressed = false;
                    RFIDCardInfoWrite rFIDCardInfoWrite = new RFIDCardInfoWrite();
                    CardDetails WriteCardDetails = new CardDetails()
                    {
                        cardUID = 0,
                        AccessLevel = 0,
                        ByPassBits = 0,
                        Client_Group = 0,
                        Client_Site = 0,
                        EngineerName = "",
                        EngineerUID = 0,
                        Expiry_Date = DateTime.MinValue,
                        HotFlagedDate = DateTime.MinValue,
                        HotFlagedVID = 0,
                        Hotflaged_status = false,
                        IssuerUID = 0,
                        Issue_Date = DateTime.MinValue,
                        OperationalArea = 0,
                        OperatorName = "",
                        Options = 0,
                        ProductCode = 0,
                        Training_Date = DateTime.MinValue,
                        VehicleGroup = Enumerable.Repeat((byte)0, 16).ToArray(),
                        VID = Enumerable.Repeat((UInt16)0, 16).ToArray(),
                        VehicleNames = Enumerable.Repeat("", 16).ToArray(),
                        VehicleLicenceType = Enumerable.Repeat((uint)0, 32).ToArray(),
                        Warning_Date = DateTime.MinValue
                    };
                    rFIDCardInfoWrite.WriteInfoToCard(WriteCardDetails);
                }

                if (IssuerCardBtnPressed)
                {
                    IssuerCardBtnPressed = false;
                    VMReturnData.NewIssuerPresent_Active = true;
                    VMReturnData.NewIssuerCard = true;
                    VMReturnData.MenuView_Active = false;


                }

                if(IssuerFileCardBtnPressed)
                {
                    IssuerFileCardBtnPressed = false;
                    VMReturnData.NewCardIssuer_Active = true;
 //                   VMReturnData.MenuView_Active = false;
                }
                

                #region Exit button
                else if (_buttonExitPressed)
                {
                    _buttonExitPressed = false;
                    VMReturnData.MenuView_Active = false;
                }
                #endregion
            }
            else
            {
                VMReturnData.OptionsPressed = false;
                this.View.Visibility = Visibility.Collapsed;
            }
                

        }

        private Visibility _editCardVis;

        public Visibility EditCardVis
        {
            get { return _editCardVis; }
            set { _editCardVis = value; base.RaisePropertyChanged("EditCardVis"); }
        }

        private Visibility _issueCardVis;
        

        public Visibility IssueCardVis
        {
            get { return _issueCardVis; }
            set { _issueCardVis = value; base.RaisePropertyChanged("IssueCardVis"); }
        }

    }
}
