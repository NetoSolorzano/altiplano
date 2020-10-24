using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TransCarga
{
    class publico
    {
        public void sololee(Form lfrm)
        {
            foreach (Control oControls in lfrm.Controls)
            {
                if (oControls is TextBox)
                {
                    oControls.Enabled = false;
                }
                if (oControls is ComboBox)
                {
                    oControls.Enabled = false;
                }
                if (oControls is RadioButton)
                {
                    oControls.Enabled = false;
                }
                if (oControls is DateTimePicker)
                {
                    oControls.Enabled = false;
                }
                if (oControls is MaskedTextBox)
                {
                    oControls.Enabled = false;
                }
                if (oControls is GroupBox)
                {
                    oControls.Enabled = false;
                }
                if (oControls is CheckBox)
                {
                    oControls.Enabled = false;
                }
            }
        }
        public void escribe(Form efrm)
        {
            foreach (Control oControls in efrm.Controls)
            {
                if (oControls is TextBox)
                {
                    oControls.Enabled = true;
                }
                if (oControls is ComboBox)
                {
                    oControls.Enabled = true;
                }
                if (oControls is RadioButton)
                {
                    oControls.Enabled = true;
                }
                if (oControls is DateTimePicker)
                {
                    oControls.Enabled = true;
                }
                if (oControls is MaskedTextBox)
                {
                    oControls.Enabled = true;
                }
                if (oControls is GroupBox)
                {
                    oControls.Enabled = true;
                }
                if (oControls is CheckBox)
                {
                    oControls.Enabled = true;
                }
            }
        }
        public void limpiar(Form ofrm)
        {
            foreach (Control oControls in ofrm.Controls)
            {
                if (oControls is TextBox)
                {
                    oControls.Text = "";
                }
            }
        }
        public void limpia_chk(Form oForm)
        {
            foreach (Control oControls in oForm.Controls)
            {
                if (oControls is CheckBox)
                {
                    CheckBox chk = oControls as CheckBox;
                    chk.Checked = false;
                }
            }
        }
        public void limpia_cmb(Form oForm)
        {
            foreach (Control oControls in oForm.Controls)
            {
                if (oControls is ComboBox)
                {
                    ComboBox cmb = oControls as ComboBox;
                    cmb.SelectedIndex = -1;
                }
            }
        }
        public void limpiapag(TabPage pag)
        {
            foreach (Control oControls in pag.Controls)
            {
                if (oControls is TextBox)
                {
                    oControls.Text = "";
                }
                if (oControls is CheckBox)
                {
                    CheckBox chk = oControls as CheckBox;
                    chk.Checked = false;
                }
                if (oControls is ComboBox)
                {
                    ComboBox cmb = oControls as ComboBox;
                    cmb.SelectedIndex = -1;
                }
            }
        }
        public void limpiagbox(GroupBox gbox)
        {
            foreach(Control oControls in gbox.Controls)
            {
                if (oControls is TextBox)
                {
                    oControls.Text = "";
                }
                if (oControls is CheckBox)
                {
                    CheckBox chk = oControls as CheckBox;
                    chk.Checked = false;
                }
                if (oControls is ComboBox)
                {
                    ComboBox cmb = oControls as ComboBox;
                    cmb.SelectedIndex = -1;
                }
            }
        }
        public void limpiasplit(SplitContainer split)
        {
            foreach(Control oControls in split.Panel1.Controls)
            {
                if (oControls is TextBox)
                {
                    oControls.Text = "";
                }
                if (oControls is CheckBox)
                {
                    CheckBox chk = oControls as CheckBox;
                    chk.Checked = false;
                }
                if (oControls is ComboBox)
                {
                    ComboBox cmb = oControls as ComboBox;
                    cmb.SelectedIndex = -1;
                }
            }
            foreach (Control oControls in split.Panel2.Controls)
            {
                if (oControls is TextBox)
                {
                    oControls.Text = "";
                }
                if (oControls is CheckBox)
                {
                    CheckBox chk = oControls as CheckBox;
                    chk.Checked = false;
                }
                if (oControls is ComboBox)
                {
                    ComboBox cmb = oControls as ComboBox;
                    cmb.SelectedIndex = -1;
                }
            }
        }
        // varios
        public int CentimeterToPixel(Form oForm, double Centimeter)
        {
            double pixel = -1;
            using (Graphics g = oForm.CreateGraphics())
            {
                pixel = Centimeter * g.DpiY / 2.54d;
            }
            return (int)pixel;
        }

    }
    public class CacheManager
    {
        static System.Collections.Hashtable ht = new System.Collections.Hashtable();
        public static void AddItem(string key, object value, uint timeToCache)
        {
            if (timeToCache > 3600)
                throw new ArgumentOutOfRangeException("Cache time cannot be more than 1 hour.");
            System.Threading.Timer t = new System.Threading.Timer(new TimerCallback(TimerProc));
            t.Change(timeToCache * 1000, System.Threading.Timeout.Infinite);
            ht.Add(t, key);
            AppDomain.CurrentDomain.SetData(key, value);
        }
        public static object GetItem(string key)
        {
            return AppDomain.CurrentDomain.GetData(key);
        }
        private static void TimerProc(object state)
        {
            System.Threading.Timer t = state as System.Threading.Timer;
            if (t != null)
            {
                object key = ht[t];
                ht.Remove(t);
                t.Dispose();

                if (key != null)
                    AppDomain.CurrentDomain.SetData(key.ToString(), null);
            }
        }
    }
}
