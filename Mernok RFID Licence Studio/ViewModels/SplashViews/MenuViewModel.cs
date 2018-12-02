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

        public ICommand IssuerCardBtn { get; private set; }

        public ICommand Exitbtn { get; private set; }

        private bool AboutBtnPressed = false;

        private bool IssuerCardBtnPressed = false;

        private bool _buttonExitPressed = false;


        public MenuViewModel(UserControl control) : base(control)
        {
            AboutBtn = new DelegateCommand(AboutBtnHandler);
            IssuerCardBtn = new DelegateCommand(IssuerCardBtnHandler);
            Exitbtn = new DelegateCommand(ExitBtnHandler);
            control.DataContext = this;
            _viewInstance = (MenuView)control;
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
                this.View.Visibility = Visibility.Visible;
                VMReturnData.NavigationBar_Active = false;
                EditCardVis = VMReturnData.MenuEditBtnEnabled;
                IssueCardVis = VMReturnData.MenuIssueBtnEnabled;


                if (AboutBtnPressed)
                {
                    AboutBtnPressed = false;
                    VMReturnData.AboutWindow_Active = true;

                }

                if (IssuerCardBtnPressed)
                {
                    IssuerCardBtnPressed = false;
                    VMReturnData.NewCardIssuer_Active = true;
                    VMReturnData.MenuView_Active = false;


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
                this.View.Visibility = Visibility.Collapsed;

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
