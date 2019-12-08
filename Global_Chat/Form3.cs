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
    public partial class Form3 : Form
    {
        GC_WebService.GC_ServiceSoapClient service = new GC_WebService.GC_ServiceSoapClient();
        public int _oda_id;
        public string _oda_ismi;
        public string Girilen_Kullanıcı_Adı;
        public dynamic Response_Kontrol;
        public dynamic Response_Kullanıcı_Ekle;
        public Form3(int oda_id, string oda_ismi)
        {
            //form1'den gelen id'yi ve ismi global oluşturduğumuz değişkenlerimize atıyoruz.
            _oda_id = oda_id;
            _oda_ismi = oda_ismi;
            InitializeComponent();
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
           // Girilen_Kullanıcı_Adı = textBox1.Text;
            //Form2 frm2 = new Form2(_oda_id)
        }
        public void Kullanıcı_Kontrolü()
        {
            //kullanıcı kontrolü ile girilen isim ile varolan veritabanındaki isimleri karşılaştırma yapılyor.
            var response = service.Kullanıcı_Kontrol(Girilen_Kullanıcı_Adı);
            if (response != null)
            {
                //bize servis eğer girilen kullanıcı adında bir kullanıcı varsa id'sini dönderiyor.
                //Eğer dönen bir id varsa demekki kullanıcı adı zaten var
                //Bu sebeple başka kullanıcı adı seçmesi için uyarı veriyoruz.
                Response_Kontrol = JsonConvert.DeserializeObject(response);
                int dönen_kişi_id;

                foreach (var item in Response_Kontrol)
                {
                    dönen_kişi_id = item.Kullanıcı_Id;
                    MessageBox.Show("Kullanıcı adınızı değiştirin");
                    textBox1.Clear();
                }
            }
            else
            {
                //eğer herhangi birşey dönmüyorsa yani öyle bir kullanıcı ve onun bir id'si yoksa kullanıcıyı ekliyoruz.
                Kullanıcıyı_Ekle(_oda_id, Girilen_Kullanıcı_Adı);       
            }
        }
        public void Kullanıcıyı_Ekle(int oda_no, string k_adi)
        {
            //Kullanıcıyı ekliyoruz ve bize servis xx diye bir değer dönderiyor. Eğer kullanıcı eklenmezse null dönderiyor.
            var response = service.Kullanıcı_Ekle(_oda_id, k_adi);
            if (response != null)
            {
                //eğer ekleme başarılıysa oturum işlemi kontrolü için biz eklediğimiz kullanıcının yani kendimizin id'sini almamız lazım.
                //bu sebeple bu fonksiyona gidiyoruz.
                Kendi_Kullanıcı_Id_Alma();
            }
            else
            {
                MessageBox.Show("ekleme hatası");
            }
        }
        public void Kendi_Kullanıcı_Id_Alma()
        {
            //bu fonksiyonda tekrar kullanıcı sorgulayıp kendi ismimizin idsi ni alıyoruz.
            var response = service.Kullanıcı_Kontrol(Girilen_Kullanıcı_Adı);
            if (response != null)
            {
                Response_Kontrol = JsonConvert.DeserializeObject(response);
                int dönen_kişi_id;

                foreach (var item in Response_Kontrol)
                {
                    //aldığımız id'yi chat ekranına yani form2'ye gönderiyoruz.
                    dönen_kişi_id = item.Kullanıcı_Id;
                    Form2 frm2 = new Form2(_oda_id, _oda_ismi, dönen_kişi_id,Girilen_Kullanıcı_Adı);
                    frm2.Show();
                    this.Hide();
                }
            }
            else{}
        }
       

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            //kullanıcı adımızı yazdıktan sonra enter'a bastığımızda kontroller başlıyor.
            if (e.KeyCode == Keys.Enter)
            {
                Girilen_Kullanıcı_Adı = textBox1.Text;
                Kullanıcı_Kontrolü();
            }
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form1 frm1 = new Form1();
            frm1.Show();
            this.Hide();
        }
    }
}
