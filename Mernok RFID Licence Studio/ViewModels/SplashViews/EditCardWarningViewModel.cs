using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Mernok_RFID_Licence_Studio
{
    public class EditCardWarningViewModel : ViewModel
    {
        private EditCardWarningView _viewInstance;



        public EditCardWarningViewModel(UserControl control) : base(control)
        {

            control.DataContext = this;
            _viewInstance = (EditCardWarningView)control;
        }


        public override void Update(ViewModelReturnData VMReturnData)
        {
            //Only update this view if visible

            if (VMReturnData.EditCardWarn_Active)
            {
                this.View.Visibility = Visibility.Visible;
            }
            else
                this.View.Visibility = Visibility.Collapsed;

        }
    }
}
