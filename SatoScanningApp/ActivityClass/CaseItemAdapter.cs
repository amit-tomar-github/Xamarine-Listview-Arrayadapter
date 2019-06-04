using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using IOCLAndroidApp.Models;

namespace SatoScanningApp.ActivityClass
{

    //#region Frame Scan List Adapter
    //public class CaseItemAdapter : BaseAdapter
    //{

    //    private Context context;
    //    private List<ScanCaseModel> data;
    //    LayoutInflater inflater = null;

    //    public CaseItemAdapter(Context context, List<ScanCaseModel> models)
    //    {
    //        this.context = context;
    //        this.data = models;
    //        inflater = (LayoutInflater)context.
    //                GetSystemService(Context.LayoutInflaterService);
    //    }

    //    public override int Count
    //    {
    //        get
    //        { return data.Count; }
    //    }


    //    public override Java.Lang.Object GetItem(int position)
    //    {
    //        return data[position].ToString();
    //    }

    //    public override long GetItemId(int position)
    //    {
    //        return position;
    //    }

    //    public class ViewHolder
    //    {

    //        public TextView CaseNo;

    //        public ViewHolder(View v)
    //        {
    //           CaseNo = (TextView)v.FindViewById(Resource.Id.listTxtCase);
    //        }
    //    }


    //    public override View GetView(int position, View convertView, ViewGroup parent)
    //    {
    //        ViewHolder holder;

    //        View view = inflater.Inflate(Resource.Layout.view_case_item, parent, false); 
            
    //        holder = new ViewHolder(view);

    //       holder.CaseNo.Text = data[position]._caseNo;

    //        return view;
    //    }
    //}
    //#endregion

    #region Old
    class CaseItemAdapter : BaseAdapter<string>
    {
        List<ScanCaseModel> items;
        Activity context;

        public CaseItemAdapter(Activity context, List<ScanCaseModel> items) : base()
        {
            this.context = context;
            this.items = items;
        }
        public override string this[int position] => items[position]._caseNo;

        public override int Count => items.Count;

        public override long GetItemId(int position) => position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView; // re-use an existing view, if one is available
            if (view == null) // otherwise create a new one
                view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = items[position]._caseNo;
            return view;
        }
    }

    #endregion
}