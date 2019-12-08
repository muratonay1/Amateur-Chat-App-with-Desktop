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
    public partial class Form2 : Form
    {
        GC_WebService.GC_ServiceSoapClient service = new GC_WebService.GC_ServiceSoapClient();
        public int _oda_id;
        public string _oda_ismi;
        public int _kullanıcı_id;
        public string _kullanıcı_adi;
        public dynamic Response_Odadaki_Kisiler;
        public dynamic Response_Oda_Mesajlari;
        public string Mesajim;
        private int tick;
        public Form2(int oda_id, string oda_ismi, int kullanıcı_id,string kullanıcı_adı)
        {
            _oda_id = oda_id;
            _oda_ismi = oda_ismi;
            _kullanıcı_id = kullanıcı_id;
            _kullanıcı_adi = kullanıcı_adı;
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            //Son gönderilen mesaja odaklanmak için son elemanı sürekli seçili tutup ona odaklanıyoruz.
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
            timer1.Enabled = true;
            timer1.Start();
            //form başlığı oda ismi oluyor.
            this.Text = _oda_ismi;
            //her odaya girdiğimizde mesajlar ve kişiler temizleniyor. Sonra tekrar dolduruluyor.Böylece yığılmayı önlüyoruz.
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            Mesajları_Listele();
            Kişileri_Listele();                   
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {            
            Form1 frm1 = new Form1();
            frm1.Show();
            this.Hide();
        }
        public void Mesajları_Listele()
        {
            //Oda id'ye göre mesajları listeletiyoruz.
            listBox1.Items.Clear();
            var response = service.Odadaki_Mesajları_Listele(_oda_id);            
            if (response != null)
            {
                Response_Oda_Mesajlari = JsonConvert.DeserializeObject(response);
                string mesaj_kullanıcı_adı;
                string mesaj;

                foreach (var item in Response_Oda_Mesajlari)
                {
                    mesaj_kullanıcı_adı = item.Kullanıcı_Adı;
                    mesaj = item.Mesaj;
                    listBox1.Items.Add("->" + mesaj_kullanıcı_adı + "-Dedi ki : " + mesaj);
                }               
            }
            else{}
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Mesajim = textBox1.Text;
                Mesaj_Gonder(Mesajim);
                textBox1.Clear();
            }
        }
        public void Mesaj_Gonder(string mesaj)
        {
            //mesajı gönderiyoruz.
            var response = service.Mesaj_Ekle(_oda_id, _kullanıcı_id, _kullanıcı_adi, mesaj);

            if (response != null)
            {
                //bize mesaj gönderildiğine dair mesaj dönüyor ve bu dönen değeri boş olmadğı anlamına geliyor.
                //Mesajları listeliyoruz.
                Mesajları_Listele();
            }
            else { }
        }
        public void Kişileri_Listele()
        {
            //odaya kayıtlı olan kişileri listeliyoruz.
            //ben yapmadım ama siz bir tablo daha oluşturup (online kililer ve online olmayan kişiler olarak) kontrolleri tutup
            //buna göre listeleme işlemi yapabilirsiniz.
            //Bu kontrolü form closing eventi ile bağlayabilirsiniz. Form kapatılırsa servise x id'li kullanıcı oturumu kapattı denir ve online tablosundan kullanıcıyı siler.
            //Deneyebilirsiniz işe yarayacaktır.
            var response = service.Odadaki_Kişileri_Listele(_oda_id);
            if (response != null)
            {
                Response_Odadaki_Kisiler = JsonConvert.DeserializeObject(response);
                int odadaki_kisi_id;
                string odadaki_kisi_adi;
                foreach (var item in Response_Odadaki_Kisiler)
                {
                    odadaki_kisi_id = item.Kullanıcı_Id;
                    odadaki_kisi_adi = item.Kullanıcı_Adı;
                    listBox2.Items.Add(odadaki_kisi_adi);
                }
            }
            else { }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //timer koymamda ki amaç realtime özelliği katmaktı.
            //her 2.5 saniye de bir mesajları ve kişileri güncelliyor.
            //Daha optimize bir yöntem bulursanız realtime hissiyatını daha verimli hale getirebilirsiniz.
            tick++;
            if(tick % 25==0)
            {
                tick = 0;
                Mesajları_Listele();
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                listBox2.Items.Clear();
                Kişileri_Listele();
            }
        }
    }
}
