using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
namespace Global_Chat
{
    public partial class Form1 : Form
    {
        //servis nesnesi oluşturuyoruz.
        GC_WebService.GC_ServiceSoapClient service = new GC_WebService.GC_ServiceSoapClient();

        //servisten dönen değerleri tutmak ve formun diğer alanlarında kullanmak için global dinamik değişkenlerimizi oluşturduk.
        //Dinamik oluşturmamızda ki amaç, dinamik olarak tanımlanan değişkenin unboxing gibi tip belirleme işlemlerine tabi tutulmamasıdır.
        //Dezavantajı ise şu olacaktır, eğer kod içerisinde gerkeli koşulları sağlamazsak program çalışırken bize hata verecektir.
        public dynamic Response_Odalar;
        public dynamic Response_Alan;
        public dynamic Response_AltAlan;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //İlk formumuz bize oda isimlerini getireceği için servis ten oda isimlerimizi çektik.
            var response = service.Odaları_Listele();
            Response_Odalar = JsonConvert.DeserializeObject(response);
            int oda_id;
            string oda_adi;
            string ful_oda_ismi;     
            foreach (var item in Response_Odalar)
            {
                oda_id = item.Oda_Id;
                oda_adi = item.Oda_Ismi;
                //Burada oda adı ve id'sini 'x' stringi ile birleştiriyoruz (Yani id'si ile)
                ful_oda_ismi = oda_adi + "x" + oda_id;               
                listBox1.Items.Add(ful_oda_ismi);               
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            string select_Odaisim = listBox1.SelectedItem.ToString();
            //bize kullanıcı adı belirleme formunda kullancağımız için oda id'si lazım.
            //bu sebeple split edip 'x' stringinden sonraki bölümü yani id'yi alıyoruz.
            string selected_Odaid = select_Odaisim.Split('x').Last();

            if (listBox1.SelectedItem != null)
            {
                //kullanıcı adı belirleme formuna oda id'sini ve oda ismini gönderiyoruz.
                Form3 frm3 = new Form3(Convert.ToInt32(selected_Odaid),select_Odaisim);
                frm3.Show();
                this.Hide();               
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
