using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content.PM;
using Android.Views;
using IOCLAndroidApp;
using Android.Content;
using System;
using System.IO;
using SatoScanningApp.ActivityClass;
using System.Collections.Generic;
using Honda_Device_Android;
using Android.Media;
using IOCLAndroidApp.Models;

namespace SatoScanningApp
{
    [Activity(Label = "Sato OnlineApp", MainLauncher = true, WindowSoftInputMode = SoftInput.StateHidden, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        clsGlobal clsGLB;
        clsNetwork oNetwork;
        int _iScanCount = 0;
        List<string> _listScanCase;
        ArrayAdapter<string> ItemAdapter;

        EditText txtScanCase;
        TextView txtScanCount;
        ListView listViewScanCase;

        public MainActivity()
        {
            try
            {
                clsGLB = new clsGlobal();
                oNetwork = new clsNetwork();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                // Set our view from the "main" layout resource
                SetContentView(Resource.Layout.activity_main);

                Button btnServerSetting = FindViewById<Button>(Resource.Id.btnServerSetting);
                btnServerSetting.Click += BtnServerSetting_Click;

                txtScanCase = FindViewById<EditText>(Resource.Id.txtScanCase);
                txtScanCase.KeyPress += TxtScanCase_KeyPress;

                txtScanCount = FindViewById<TextView>(Resource.Id.txtScanCount);

                listViewScanCase = FindViewById<ListView>(Resource.Id.listViewScanCase);
                _listScanCase = new List<string>();

                ItemAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, _listScanCase);
                listViewScanCase.Adapter = ItemAdapter;

                if (ReadSettingFile() == false)
                    OpenActivity(typeof(SettingActivity));

                ReadCaseFile();
                txtScanCase.RequestFocus();
            }
            catch (Exception ex)
            {
                clsGLB.ShowMessage(ex.Message, this, MessageTitle.ERROR);
            }
        }

        private void BtnServerSetting_Click(object sender, EventArgs e)
        {
            try
            {
                OpenActivity(typeof(SettingActivity));
            }
            catch (Exception ex)
            {
                clsGLB.ShowMessage(ex.Message, this, MessageTitle.ERROR);
            }
        }

        private void TxtScanCase_KeyPress(object sender, View.KeyEventArgs e)
        {
            try
            {
                if (e.Event.Action == KeyEventActions.Down)
                {
                    if (e.KeyCode == Keycode.Enter)
                    {

                        if (!string.IsNullOrEmpty(txtScanCase.Text))
                        {
                           // SaveCase(txtScanCase.Text.Trim());
                            WriteFile(txtScanCase.Text.Trim());
                            txtScanCase.Text = "";
                            txtScanCase.RequestFocus();
                        }
                        else
                        {
                            Toast.MakeText(this, "Scan case", ToastLength.Long).Show();
                            txtScanCase.RequestFocus();
                        }
                    }
                    else
                        e.Handled = false;
                }
            }
            catch (Exception ex)
            {
                clsGLB.ShowMessage(ex.Message, this, MessageTitle.ERROR);
            }
        }

        #region Methods

        private void SaveCase(string CaseBarcode)
        {
            try
            {
                string _MESSAGE = "VALIDATE_MAPPED_CASE~" + CaseBarcode + "}";
                string[] _RESPONSE = oNetwork.fnSendReceiveData(_MESSAGE).Split('~');
                switch (_RESPONSE[0])
                {
                    case "VALID":
                        WriteFile(CaseBarcode);
                        break;

                    case "INVALID":
                        Toast.MakeText(this, _RESPONSE[1], ToastLength.Long).Show();
                        break;

                    case "ERROR":
                        Toast.MakeText(this, _RESPONSE[1], ToastLength.Long).Show();
                        break;

                    case "NO_CONNECTION":
                        Toast.MakeText(this, "Communication server not connected", ToastLength.Long).Show();
                        break;

                    default:
                        Toast.MakeText(this, "No option match from comm server", ToastLength.Long).Show();
                        break;

                }

              
            }
            catch (Exception ex)
            {
                clsGLB.ShowMessage(ex.Message, this, MessageTitle.ERROR);
            }
        }

        private void WriteFile(string CaseBarcode)
        {
            StreamWriter sw = null;
            try
            {
                string folderPath = Path.Combine(clsGlobal.FilePath, clsGlobal.FileFolder);
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string filename = Path.Combine(folderPath, clsGlobal.CaseFileName);
                sw = new StreamWriter(filename, true);

                sw.WriteLine(CaseBarcode + "'");

                _listScanCase.Add(CaseBarcode);
                ItemAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, _listScanCase);
                listViewScanCase.Adapter = ItemAdapter;

                _iScanCount++;
                txtScanCount.Text = "Scan Count : " + _iScanCount;

                MediaScannerConnection.ScanFile(this, new String[] { filename }, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Flush();
                    sw.Close();
                    sw = null;
                }
            }
        }

        private bool ReadSettingFile()
        {
            StreamReader sr = null;
            try
            {
                string folderPath = Path.Combine(clsGlobal.FilePath, clsGlobal.FileFolder);
                string filename = Path.Combine(folderPath, clsGlobal.ServerIpFileName);

                if (File.Exists(filename))
                {
                    sr = new StreamReader(filename);
                    clsGlobal.mSockIp = sr.ReadLine();
                    clsGlobal.mSockPort = Convert.ToInt32(sr.ReadLine());

                    sr.Close();
                    sr.Dispose();
                    sr = null;

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            { throw ex; }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                    sr = null;
                }
            }
        }

        private void ReadCaseFile()
        {
            StreamReader sr = null;
            try
            {
                string folderPath = Path.Combine(clsGlobal.FilePath, clsGlobal.FileFolder);
                string filename = Path.Combine(folderPath, clsGlobal.CaseFileName);

                if (File.Exists(filename))
                {
                    sr = new StreamReader(filename);
                    while (!sr.EndOfStream)
                    {
                        string str = sr.ReadLine().TrimEnd('\'');
                        _listScanCase.Add(str);
                        _iScanCount++;
                    }
                    ItemAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, _listScanCase);
                    listViewScanCase.Adapter = ItemAdapter;

                    txtScanCount.Text = "Scan Count : " + _iScanCount;

                    sr.Close();
                    sr.Dispose();
                    sr = null;
                }

            }
            catch (Exception ex)
            { throw ex; }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                    sr = null;
                }
            }
        }

        public void ShowConfirmBox(string msg, Activity activity)
        {
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(activity);
            builder.SetTitle("Message");
            builder.SetMessage(msg);
            builder.SetCancelable(false);
            builder.SetPositiveButton("Yes", handllerOkButton);
            builder.SetNegativeButton("No", handllerCancelButton);
            builder.Show();
        }
        void handllerOkButton(object sender, DialogClickEventArgs e)
        {
            this.FinishAffinity();
        }
        void handllerCancelButton(object sender, DialogClickEventArgs e)
        {

        }
        public void OpenActivity(Type t)
        {
            try
            {
                Intent MenuIntent = new Intent(this, t);
                StartActivity(MenuIntent);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        public override void OnBackPressed()
        {
            ShowConfirmBox("Do you want to exit", this);
        }
    }
}