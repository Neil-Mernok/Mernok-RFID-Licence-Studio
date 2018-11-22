using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Mernok_RFID_Licence_Studio
{
    public class MainViewModel : ViewModel
    {
        public MainView MyWindow;

        public ICommand Backbtn { get; private set; }
        public ICommand Nextbtn { get; private set; }

        private bool BackbtnPressed = false;
        private bool NextbtnPressed = false;

        public MainViewModel(Window window) : base(window)
        {
            MyWindow = (MainView)window;
            

            Backbtn = new DelegateCommand(BackbtnHandler);
            Nextbtn = new DelegateCommand(NextbtnHandler);

            MyWindow.DataContext = this;
            MyWindow.Show();

        }

        public override void Update(ViewModelReturnData VMReturnData)
        {

            if (!RFID.MernokRFID.IsOpen())
            {
                if (RFID.MernokRFID.OpenRFID(RFID.Mode.Mifare))
                {
                    VMReturnData.RWD_connected = true;
                }
                else
                {
                    VMReturnData.RWD_connected = false;
                }
            }
            else
            {
                VMReturnData.UID = RFID.MernokRFID_interface.read_UID();
                if (VMReturnData.UID != 0)
                {
                    VMReturnData.CardInField = true;
                }
                else
                {
                    VMReturnData.CardInField = false;
                }

                if (VMReturnData.EditCard && VMReturnData.CopiedVMCardDetails.cardUID != VMReturnData.UID && VMReturnData.NewCardWindow>=1)
                {
                    VMReturnData.EditCardWarn_Active = true;
                }
                else
                    VMReturnData.EditCardWarn_Active = false;
            }

            VMReturnData.StartUpView_Active = VMReturnData.RWD_connected;

            #region Navigation Bar Button Setter

            if (VMReturnData.LicenceView_Active)
            {
                VMReturnData.NavigationBar_Active = true;
                VMReturnData.NextButtonEnabled = false;
                VMReturnData.BackButtonEnabled = false;
            }
            else if(VMReturnData.NewCardAccess_Active)
            {
                VMReturnData.NavigationBar_Active = true;
                VMReturnData.BackButtonEnabled = true;
            }
            else
            {
                VMReturnData.NavigationBar_Active = false;
            }

            #endregion


            if(NextbtnPressed)
            {
                VMReturnData.NextWindow();
                NextbtnPressed = false;
            }
            if(BackbtnPressed)
            {
                VMReturnData.BackWindow();
                BackbtnPressed = false;
            }

        }

#region Properties
        private string _cardInfield;

        public string CardInField
        {
            get { return _cardInfield; }
            set { _cardInfield = value;
                base.RaisePropertyChanged("CardInField");
            }
        }
        #endregion

        public void BackbtnHandler()
        {
            BackbtnPressed = true;
        }

        public void NextbtnHandler()
        {
            NextbtnPressed = true;
        }


    }
}
