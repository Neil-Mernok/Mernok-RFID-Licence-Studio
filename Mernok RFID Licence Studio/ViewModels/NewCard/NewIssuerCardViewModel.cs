using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MernokPasswords;

namespace Mernok_RFID_Licence_Studio
{
    class NewIssuerCardViewModel : ViewModel
    {
        private NewIssuerCardView _viewInstance;

        RFIDCardInfoRead cardInfoRead = new RFIDCardInfoRead();
        PasswordFinder passwordFinder = new PasswordFinder();
        private const string V = @"C:\Passwords\MernokPasswordMasterList.xml";
        public static MernokPasswordFile mernokPasswordFile = new MernokPasswordFile();


        uint CardinFieldUID = 0;

        public NewIssuerCardViewModel(NewIssuerCardView control) : base(control)
        {
            PassColour = Brushes.OrangeRed;
            mernokPasswordFile = MernokPasswordManager.ReadMernokPasswordFile(V);
            control.DataContext = this;
            _viewInstance = (NewIssuerCardView)control;
        }


        public override void Update(ViewModelReturnData VMReturnData)
        {


            if (VMReturnData.NewCardIssuer_Active)
            {
                this.View.Visibility = Visibility.Visible;

                #region Navigationbar details
                VMReturnData.ViewTitle = VMReturnData.EditCard ? "Edit Card" : "New Card";
                VMReturnData.SubTitle = "New Issuer details";
                VMReturnData.CurrentPageNumber = 1;
                VMReturnData.TotalPageNumber = 4;

                VMReturnData.MenuButtonEnabled = Visibility.Collapsed;
                #endregion

                if (!VMReturnData.CardInField)
                {
                    WarningMessageI = "Present RFID card";
                    MessageColour = Brushes.OrangeRed;
                }
                else
                {

                    VMReturnData.VMCardDetails.IssuerUID = CardinFieldUID = VMReturnData.UID;
                    bool password = passwordFinder.FindPasswordinFile(AdminPassword, mernokPasswordFile);
                    if (password)
                    {
                        PassColour = Brushes.GreenYellow;
                        string[] IssuerDetails =  PasswordDecriptor.PasswordToDetails(AdminPassword);
                        VMReturnData.VMCardDetails.EngineerName = IssuerDetails[3];
                        VMReturnData.VMCardDetails.EngineerUID = UInt32.Parse(IssuerDetails[0]);
                        VMReturnData.NextButtonEnabled = true;
                        WarningMessageI = "Password good, click next to continue";
                        MessageColour = Brushes.White;

                    }
                    else
                    {
                        MessageColour = Brushes.OrangeRed;
                        PassColour = Brushes.OrangeRed;
                        VMReturnData.NextButtonEnabled = false;
                        if(AdminPassword == "")
                            WarningMessageI = "Enter your password";
                        else
                            WarningMessageI = "Enter correct password";
                    }

                }

            }
            else
            {
                //View is not visible, do not update
                //Stop any animations on this vieModel
                this.View.Visibility = Visibility.Collapsed;
                AdminPassword = "";
            }
        }


        #region Binding Properties
        private string _warnMessage;

        public string WarningMessageI
        {
            get { return _warnMessage; }
            set { _warnMessage = value; RaisePropertyChanged("WarningMessageI"); }
        }

        private string _issuerUID;

        public string IssuerUID
        {
            get { return _issuerUID; }
            set { _issuerUID = value; base.RaisePropertyChanged("IssuerUID"); }
        }

        private string _issueDate;
        public string IssueDate
        {
            get { return _issueDate; }
            set { _issueDate = value; base.RaisePropertyChanged("IssueDate"); }
        }

        private uint _engineerUID;
        public uint EngineerUID
        {
            get { return _engineerUID; }
            set { _engineerUID = value; base.RaisePropertyChanged("EngineerUID"); }
        }

        private string _engineerName;

        public string EngineerName
        {
            get { return _engineerName; }
            set { _engineerName = value; base.RaisePropertyChanged("EngineerName"); }
        }

        private string _password;

        public string AdminPassword
        {
            get { return _password; }
            set { _password = value; base.RaisePropertyChanged("AdminPassword"); }
        }

        private Brush _PassColour;

        public Brush PassColour
        {
            get { return _PassColour; }
            set { _PassColour = value; base.RaisePropertyChanged("PassColour"); }
        }

        private Brush _MessageColour;

        public Brush MessageColour
        {
            get { return _MessageColour; }
            set { _MessageColour = value; base.RaisePropertyChanged("MessageColour"); }
        }

        #endregion
    }
}
