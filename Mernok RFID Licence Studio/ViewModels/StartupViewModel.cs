﻿#region windows defines
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Media;

#endregion 

namespace Mernok_RFID_Licence_Studio
{
    public class StartupViewModel : ViewModel
    {
        #region ViewModel Global varaibles
        private StartupView _viewInstance;
        RFIDCardInfoRead RFIDCardInfoRead = new RFIDCardInfoRead();
        DispatcherTimer ProgressBarTimer = new DispatcherTimer();

        public ICommand ReadCardInfobtn { get; private set; }
        public ICommand NewCardInfobtn { get; private set; }
        public ICommand Exitbtn { get; private set; }
        public ICommand Aboutbtn { get; private set; }

        private bool ReadCardPressed = false;
        private bool NewCardPressed = false;
        private bool AboutPressed = false;
        private bool formated;
        private bool oneRead = false;

        public bool doneProgress = false;
        int default_max = 5;

        #endregion

        #region View Model Constructor
        public StartupViewModel(UserControl control) : base(control)
        {
            #region Button defines
            ReadCardInfobtn = new DelegateCommand(ReadCardInfoHandler);
            NewCardInfobtn = new DelegateCommand(NewCardInfoHandler);
            Exitbtn = new DelegateCommand(ExitBtnHandler);
            Aboutbtn = new DelegateCommand(AboutbtnHandler);
            #endregion

            #region Progressbar init
            ProgressBarTimer.Tick += new EventHandler(ProgressBarTimer_Tick);
            ProgressBarTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            ReadProgress = default_max;
            #endregion

            formated = false;
            control.DataContext = this;
            _viewInstance = (StartupView)control;
        }
        #endregion

        #region Progressbar run
        private void ProgressBarTimer_Tick(object sender, EventArgs e)
        {
            ReadProgress = default_max - ProgressBarValue++;
            if (ProgressBarValue > default_max)
            {
                ProgressBarTimer.Stop();
                doneProgress = true;
            }
        }
        #endregion

        public override void Update(ViewModelReturnData VMReturnData)
        {
            //Only update this view if visible                        
            if (VMReturnData.StartUpView_Active)
            {
                this.View.Visibility = Visibility.Visible;
                #region Startup operation
                CardPresent = formated & VMReturnData.CardInField;
                if (VMReturnData.CardInField)
                {
                    UID = RFIDCardInfoRead.UIDtoString(VMReturnData.UID);
                    if(!oneRead)
                    {
                        RFIDCardInfoRead.UID = VMReturnData.UID;
                        formated = RFIDCardInfoRead.Block1Info();
                        oneRead = true;
                    }
                                      
                    if (formated)
                        CardImage = new BitmapImage(new Uri(@"/Resources/Images/CardValid.png", UriKind.Relative));
                    else
                        CardImage = new BitmapImage(new Uri(@"/Resources/Images/CardFormatError.png", UriKind.Relative));

                    if (ReadCardPressed == true)
                    {
                        ReadCardPressed = false;
                        doneProgress = false;
                        ProgressBarTimer.Start();
                        VMReturnData.cardInfoRead.ReadInfo(VMReturnData.UID);                                                                                
                    }                    
                }
                else
                {
                    UID = "Present RFID card";
                    ProgressBarValue = 0;
                    ReadProgress = default_max;
                    doneProgress = false;
                    oneRead = false;
                    this.CardImage = new BitmapImage(new Uri(@"/Resources/Images/PresentCard.png", UriKind.Relative));
                    ProgressBarTimer.Stop();
                }

                if (AboutPressed)
                {
                    AboutPressed = false;
                    VMReturnData.AboutWindow_Active = true;
                }

                if (NewCardPressed == true)
                {
                    VMReturnData.NewCardAccess_Active = true;
                    VMReturnData.EditCard = false;
                    VMReturnData.NewCardWindow = 0;
                    VMReturnData.NewCardUID = 0;
                    NewCardPressed = false;
                }

                if (doneProgress)
                {
                    if (!VMReturnData.cardInfoRead.formatted)
                    {
                        VMReturnData.CardFormatError = true;
                    }
                    else
                    {
                        VMReturnData.LicenceView_Active = true;
                    }

                    doneProgress = false;
                    ProgressBarValue = 2;
                    ReadProgress = default_max;
                }
                #endregion

                #region Exit button
                else if (_buttonExitPressed)
                {
                    _buttonExitPressed = false;
                    VMReturnData.ExitPromtView_Active = true;
                }
                #endregion

            }
            else
                this.View.Visibility = Visibility.Collapsed;

        }

        #region Button Handlers
        public void ReadCardInfoHandler()
        {
            ReadCardPressed = true;
        }

        public void NewCardInfoHandler()
        {
            NewCardPressed = true;
        }

        private void ExitBtnHandler()
        {
            _buttonExitPressed = true;
        }

        private void AboutbtnHandler()
        {
            AboutPressed = true;
        }
        #endregion

        #region Binding Properties
        private string _UID;

        public string UID
        {
            get { return _UID; }
            set
            {
                _UID = value;
                base.RaisePropertyChanged("UID");
            }
        }

        private Visibility _PresentCard;

        public Visibility PresentCard
        {
            get { return _PresentCard; }
            set { _PresentCard = value; base.RaisePropertyChanged("PresentCard"); }
        }

        private BitmapImage _CardImage;

        public BitmapImage CardImage
        {
            get { return _CardImage; }
            set { _CardImage = value; base.RaisePropertyChanged("CardImage"); }
        }

        private Brush _ProgressBarColour;

        public Brush ProgressBarColour
        {
            get { return _ProgressBarColour; }
            set { _ProgressBarColour = value; base.RaisePropertyChanged("ProgressBarColour"); }
        }

        private int _ReadProgress;
        private bool _buttonExitPressed;

        public int ReadProgress
        {
            get { return _ReadProgress; }
            set
            {
                _ReadProgress = value;
                base.RaisePropertyChanged("ReadProgress");
            }
        }

        private bool _cardPresent;

        public bool CardPresent
        {
            get { return _cardPresent; }
            set { _cardPresent = value; base.RaisePropertyChanged("CardPresent"); }
        }


        public int ProgressBarValue { get; set; } = 2;


        public int ProgressBarMax
        {
            get { return default_max; }
            set { default_max = value; base.RaisePropertyChanged("ProgressBarMax"); }
        }

        #endregion

    }

}
