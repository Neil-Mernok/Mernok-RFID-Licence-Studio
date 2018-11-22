using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using MernokAssets;
using MernokClients;
using MernokProducts;

namespace Mernok_RFID_Licence_Studio
{
    
    public class NewCardDetailsViewModel : ViewModel
    {

        private NewCardDetails1View _viewInstance;

        private ObservableCollection<string> _AccessLevelList;

        private ObservableCollection<string> _VehicleAccessList;

        private RFIDCardInfoRead rFIDCardInfoRead = new RFIDCardInfoRead();

        public MernokClientFile mernokClientFile = MernokClientManager.ReadMernokClientFile();

        public MernokProductFile mernokProductFile = MernokProductManager.ReadMernokProductFile();

        GeneralFunctions generalFunctions = new GeneralFunctions();

        bool onetimeread = false;
        bool onetimeWrite = false;


        public NewCardDetailsViewModel(UserControl control) : base(control)
        {
            WarningDate = DateTime.Now;
            ExpiryDate = DateTime.Now;
            TrainingDateMax = DateTime.Now;
            ExpiryDateMax = DateTime.Now.AddYears(1).AddDays(1);
            WarningDateMax = DateTime.Now.AddYears(1);
            DateStart = DateTime.Now;

        control.DataContext = this;
            
            _viewInstance = (NewCardDetails1View)control;
        }

        public override void Update(ViewModelReturnData VMReturnData)
        {
   
            if (VMReturnData.NewCardDetail_Active)
            {
                this.View.Visibility = Visibility.Visible;

                #region Navigation bar
                VMReturnData.ViewTitle = VMReturnData.EditCard ? "Edit Card" : "New Card";
                VMReturnData.SubTitle = "Operator details";
                VMReturnData.CurrentPageNumber = 2;
                VMReturnData.TotalPageNumber = 4;
                VMReturnData.MenuButtonEnabled = Visibility.Collapsed;
                #endregion

                //Only update this viewModel when this view is visible
                if (VMReturnData.CardInField)
                {
                    UID = rFIDCardInfoRead.UIDtoString(VMReturnData.UID);
                    CardImage = new BitmapImage(new Uri(@"/Resources/Images/CardValid.png", UriKind.Relative));
                }
                else
                {
                    UID = "Card not in field";
                    CardImage = new BitmapImage(new Uri(@"/Resources/Images/PresentCard.png", UriKind.Relative));
                }

                if (VMReturnData.VMCardDetails.IssuerUID == VMReturnData.UID)
                {
                    VMReturnData.CardStillIssuer = true;
                    UID = "Issuer Card in field";
                }
                else
                {
                    VMReturnData.CardStillIssuer = false;
                }
               

                if (!onetimeread)
                {
                    ClientCode = new ObservableCollection<string>();
                    ClientSite = new ObservableCollection<string>();
                    AccessLevelList = new ObservableCollection<string>();
                    VehicleAccessList = new ObservableCollection<string>();
                    ProductCode = new ObservableCollection<string>();

                    foreach (RFIDCardInfoRead.AccessLevel_enum item in Enum.GetValues(typeof(RFIDCardInfoRead.AccessLevel_enum)))
                    {
                        AccessLevelList.Add(item.ToString().Replace("_", " "));
                        AccessLevelnum = 0;
                    }

                    foreach (RFIDCardInfoRead.VehicleAccessType item in Enum.GetValues(typeof(RFIDCardInfoRead.VehicleAccessType)))
                    {
                        VehicleAccessList.Add(item.ToString().Replace("_", " "));
                        VehicleAccessType_ret = 0;
                    }

                    ClientCode = mernokClientFile.mernokClientList.Select(l => l.ClientGroupName).Distinct().ToList();
                    ClientCodenum = 0;
                    ClientSite = mernokClientFile.mernokClientList.Where(l => l.Group == ClientCodenum).Select(t=> t.ClientSiteName).ToList();
                    ClientSitenum = 0;
                    ProductCode = mernokProductFile.mernokProductList.Select(t => t.ProductName).ToList();
                    ProductList_ret = 0;


                    VMReturnData.VMCardDetails.Issue_Date = DateTime.Now;
                    ExpiryDate = DateTime.Now;
                    WarningDate = DateTime.Now;
                    TrainDate = DateTime.Now;
                    onetimeread = true;
                    OperatorName = "";
                    OperationalArea = "";
                }

                if (VMReturnData.EditCard && !onetimeWrite)
                {
                    OperatorName = VMReturnData.CopiedVMCardDetails.OperatorName.Replace(" ","");
                    ClientCodenum = (int)VMReturnData.CopiedVMCardDetails.Client_Group;
                    ClientSitenum = Math.Abs(ClientSite.IndexOf(ClientSite.Where(T => T == mernokClientFile.mernokClientList[(int)VMReturnData.CopiedVMCardDetails.Client_Site].ClientSiteName).First()));
                    ProductList_ret = (int)VMReturnData.CopiedVMCardDetails.ProductCode;
                    OperationalArea = VMReturnData.CopiedVMCardDetails.OperationalArea.ToString();
                    TrainDate = VMReturnData.CopiedVMCardDetails.Training_Date;
                    AccessLevelnum = (uint)Math.Abs(AccessLevelList.IndexOf( ((RFIDCardInfoRead.AccessLevel_enum)VMReturnData.CopiedVMCardDetails.AccessLevel).ToString().Replace("_", " ")));
                    VehicleAccessType_ret = VMReturnData.CopiedVMCardDetails.Options;
                    ExpiryDate = VMReturnData.CopiedVMCardDetails.Expiry_Date;
                    WarningDate = VMReturnData.CopiedVMCardDetails.Warning_Date;
                    onetimeWrite = true;
                }

                if((OperatorName!=null&&OperatorName!="")&&(ClientCode!=null)&& ClientCodenum > -1 && (ClientSitenum > -1)&& (OperationalArea != null && OperationalArea != ""))
                {
                    VMReturnData.NextButtonEnabled = true;
                    VMReturnData.VMCardDetails.OperatorName = OperatorName;
                    VMReturnData.VMCardDetails.Client_Group = (uint)ClientCodenum;
                    VMReturnData.VMCardDetails.Client_Site = (uint)ClientSitenum;
                    VMReturnData.VMCardDetails.ProductCode = (uint)ProductList_ret;
                    VMReturnData.VMCardDetails.Client_Site = mernokClientFile.mernokClientList.Where(t => t.ClientSiteName == ClientSite[ClientSitenum]).First().Client;
                    VMReturnData.VMCardDetails.OperationalArea = Convert.ToUInt16(OperationalArea);
                    VMReturnData.VMCardDetails.Warning_Date = WarningDate;
                    VMReturnData.VMCardDetails.Training_Date = TrainDate;
                    VMReturnData.VMCardDetails.Expiry_Date = ExpiryDate;
                    VMReturnData.VMCardDetails.AccessLevel = (byte)(RFIDCardInfoRead.AccessLevel_enum)Enum.Parse(typeof(RFIDCardInfoRead.AccessLevel_enum), AccessLevelList[(int)AccessLevelnum].Replace(" ", "_"));
                    VMReturnData.VMCardDetails.Options = VehicleAccessType_ret;
                }
                else
                {
                    VMReturnData.NextButtonEnabled = false;
                }

            }
            else
            {
                //View is not visible, do not update
                //Stop any animations on this vieModel
                this.View.Visibility = Visibility.Collapsed;
                onetimeread = false;
                onetimeWrite = false;
            }
        }



        #region Binding properties

        private string _operatorName;

        public string OperatorName
        {
            get { return _operatorName; }
            set { _operatorName = generalFunctions.StringConditioner(value); RaisePropertyChanged("OperatorName"); }
        }

        private IList<string> _clientCode;

        public IList<string> ClientCode
        {
            get { return _clientCode; }
            set
            {
                _clientCode = value;
                RaisePropertyChanged("ClientCode");               
            }
        }

        private IList<string> _productCode;

        public IList<string> ProductCode
        {
            get { return _productCode; }
            set { _productCode = value; RaisePropertyChanged("ProductCode"); }
        }

        private int _ProductList_ret;

        public int ProductList_ret
        {
            get { return _ProductList_ret; }
            set { _ProductList_ret = value; RaisePropertyChanged("ProductList_ret"); }
        }



        private int _ClientCodenum;

        public int ClientCodenum
        {
            get { return _ClientCodenum; }
            set {
                _ClientCodenum = value;
                RaisePropertyChanged("ClientCodenum");
                ClientSite = mernokClientFile.mernokClientList.Where(l => l.Group == ClientCodenum).Select(t => t.ClientSiteName).ToList();
                ClientSitenum = 0;
            }
        }


        private IList<string> _clientsite;

        public IList<string> ClientSite
        {
            get { return _clientsite; }
            set { _clientsite = value; RaisePropertyChanged("ClientSite"); }
        }

        private int _ClientSitenum;

        public int ClientSitenum
        {
            get { return _ClientSitenum; }
            set { _ClientSitenum = value; RaisePropertyChanged("ClientSitenum"); }
        }

        private string _operationalArea;

        public string OperationalArea
        {
            get { return _operationalArea; }
            set { _operationalArea = generalFunctions.StringNumConditioner(value); RaisePropertyChanged("OperationalArea"); }
        }



        private int _vehicleAccesstype;

        public int VehicleAccessType
        {
            get { return _vehicleAccesstype; }
            set { _vehicleAccesstype = value; RaisePropertyChanged("VehicleAccessType"); }
        }

        private DateTime _warnDate;

        public DateTime WarningDate
        {
            get { return _warnDate.Date; }
            set { _warnDate = value; RaisePropertyChanged("WarningDate"); }
        }

        private DateTime _expiryDate;

        public DateTime ExpiryDate
        {
            get { return _expiryDate.Date; }
            set { _expiryDate = value; RaisePropertyChanged("ExpiryDate"); }
        }

        private DateTime _trainDate;

        public DateTime TrainDate
        {
            get { return _trainDate; }
            set { _trainDate = value; RaisePropertyChanged("TrainDate"); }
        }

        private DateTime _TrainingDateMax;

        public DateTime TrainingDateMax
        {
            get { return _TrainingDateMax; }
            set { _TrainingDateMax = value; RaisePropertyChanged("TrainingDateMax"); }
        }

        private DateTime _ExpiryDateMax;

        public DateTime ExpiryDateMax
        {
            get { return _ExpiryDateMax; }
            set { _ExpiryDateMax = value; RaisePropertyChanged("ExpiryDateMax"); }
        }

        private DateTime _WarningDateMax;

        public DateTime WarningDateMax
        {
            get { return _WarningDateMax; }
            set { _WarningDateMax = value; RaisePropertyChanged("WarningDateMax"); }
        }

        private DateTime _DateStart;

        public DateTime DateStart
        {
            get { return _DateStart; }
            set { _DateStart = value; RaisePropertyChanged("DateStart"); }
        }



        private uint _AccessLevelnum;

        public uint AccessLevelnum
        {
            get { return _AccessLevelnum; }
            set { _AccessLevelnum = value; RaisePropertyChanged("AccessLevelnum"); }
        }

        private int _vehicleAccessType;

        public int VehicleAccessType_ret
        {
            get { return _vehicleAccessType; }
            set { _vehicleAccessType = value; RaisePropertyChanged("VehicleAccessType_ret"); }
        }

        public ObservableCollection<string> AccessLevelList
        {
            get { return _AccessLevelList; }
            set { _AccessLevelList = value; RaisePropertyChanged("AccessLevelList"); }
        }

        public ObservableCollection<string> VehicleAccessList
        {
            get { return _VehicleAccessList; }
            set { _VehicleAccessList = value; RaisePropertyChanged("VehicleAccessList"); }
        }

        private BitmapImage bitmapImage;
        public BitmapImage CardImage
        {
            get { return bitmapImage; }
            set
            {
                bitmapImage = value;
                RaisePropertyChanged("CardImage");
            }
        }

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
        #endregion
    }



}
