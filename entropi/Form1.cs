using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;

namespace entropi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        // alternatif ve kriterleri tut

        public DataTable table;
        public ArrayList alternatifler = new ArrayList();
        public ArrayList kriterler = new ArrayList();
        public double[,] arasbolum,arasbirbolu, lnoncesi, makstablo, sontablo, normalizeedilmis, veri, normalizetablosu, lntablosu, carpilmis, kareli;
        public double ksayisi, tej;
        public double[] arastoplam, toplam, makstablomaks, sira, ntoplam, ktoplam, stoplam, ej, bireksiej, agirlik, karelertop, karekok, lntoplam;

       

        public void alternatiflistele() // Alternatif isimlerini listeleme metodu
        {
            listBox1.Items.Clear();
            for (int i = 0; i < alternatifler.Count; i++)
            {
                listBox1.Items.Add(alternatifler[i]);
            }
        }

        public void kriterlistele() // Kriter isimlerini listele
        {
            listBox2.Items.Clear();
            for (int i = 0; i < kriterler.Count; i++)
            {
                listBox2.Items.Add(kriterler[i]);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            maxbul();
        }

        public void maxbul() // Maksimum değer bulunan alternatif
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            double maks = veri[0,0];
            double min = veri[0,0];
            makstablo = new double[alternatifler.Count+1, kriterler.Count];
            makstablomaks = new double[kriterler.Count];
            for (int i = 0; i < kriterler.Count; i++)
            {
                maks = veri[0,i];
                for (int j = 0; j < alternatifler.Count; j++)
                {
                    if(veri[j, i] > maks)
                    {
                        maks = veri[j, i];
                    }
                }
                makstablomaks[i] = maks;
            }

            for (int i = 0; i < alternatifler.Count; i++)
            {
                for (int j = 0; j < kriterler.Count; j++)
                {
                    makstablo[i, j] = veri[i,j];
                }
            }

            for (int i = 0; i < kriterler.Count; i++)
            {
                makstablo[alternatifler.Count, i] = makstablomaks[i];
            }

            for (int i = 0; i < kriterler.Count; i++)
            {
                dataGridView1.Columns.Add(i.ToString(), kriterler[i].ToString());
            }

            for (int i = 0; i < alternatifler.Count+1; i++)
            {
                var row = new DataGridViewRow();
                for (int j = 0; j < kriterler.Count; j++)
                {
                    row.Cells.Add(new DataGridViewTextBoxCell()
                    {
                        Value = makstablo[i, j]
                    });
                }
                dataGridView1.Rows.Add(row);
            }
            string[] maksalternatifler = new string[alternatifler.Count+1];

            for (int i = 0; i < alternatifler.Count; i++)
            {
                maksalternatifler[i] = alternatifler[i].ToString();
            }

            maksalternatifler[alternatifler.Count] = "Maks";

            for (int i = 0; i < maksalternatifler.Length; i++)
            {
                dataGridView1.Rows[i].HeaderCell.Value = maksalternatifler[i].ToString();
            }

        }
        
        public void listehazirla(DataGridView grid) // Grid e satır ve sütun isimlerini yaz
        {
            grid.ColumnCount = kriterler.Count;
            grid.RowCount = alternatifler.Count;
            for (int i = 0; i < kriterler.Count; i++)
            {
                grid.Columns[i].Name = kriterler[i].ToString();
            }
            for (int i = 0; i < alternatifler.Count; i++)
            {
                grid.Rows[i].HeaderCell.Value = alternatifler[i];
            }
        }

        public void gridtemizle(DataGridView grid) // grid verilerini temizler
        {
            grid.Columns.Clear();
            grid.Rows.Clear();
            grid.Refresh();
            grid.DataSource = null;
        }

        private void button4_Click(object sender, EventArgs e) // Onay butonu - fm kriterine gore NM - NM edilmiş karar matrisi
        {

            normalize();
            lnoncesibul();
            ln();
            carp();
            sutuntoplam();
            kbul();
            kcarp();
            bireksiejbul();
            tejbul();
            agirlikbul();
            tabControl1.SelectedIndex = 1;

        }

        public void datagridoku(DataGridView grid) // Datagridview den veri oku
        {
            veri = new double[alternatifler.Count, kriterler.Count];

            for (int i = 0; i < alternatifler.Count; i++)
            {
                for (int j = 0; j < kriterler.Count; j++)
                {
                    veri[i,j] = Convert.ToDouble(dataGridView1.Rows[i].Cells[j].Value);
                }
            }
        }

        public void griddoldur(DataGridView grid, Double[,] veriler) // verileri istenen grid e dolduran metot
        {
            grid.Rows.Clear();
            grid.Columns.Clear();
            for (int i = 0; i < kriterler.Count; i++)
            {
                grid.Columns.Add(i.ToString(), kriterler[i].ToString());
            }

            for (int i = 0; i < alternatifler.Count; i++)
            {
                var row = new DataGridViewRow();
                for (int j = 0; j < kriterler.Count; j++)
                {
                    row.Cells.Add(new DataGridViewTextBoxCell()
                    {
                        Value = veriler[i, j]
                    });
                }
                grid.Rows.Add(row);
            }

            for (int i = 0; i < alternatifler.Count; i++)
            {
                grid.Rows[i].HeaderCell.Value = alternatifler[i].ToString();
            }
        }

        public void tekligriddoldur(DataGridView grid, Double[] veriler) // tek boyutlu veriyi grid e dolduran metot
        {
            grid.Rows.Clear();
            grid.Columns.Clear();
            for (int i = 0; i < kriterler.Count; i++)
            {
                grid.Columns.Add(i.ToString(), kriterler[i].ToString());
            }
            grid.ColumnCount = kriterler.Count;
            var row = new DataGridViewRow();
            for (int j = 0; j < veriler.Length; j++)
            {
                row.Cells.Add(new DataGridViewTextBoxCell()
                {
                    Value = veriler[j]
                });
            }
            grid.Rows.Add(row);
        }

        

        public void normalize() // normalizasyon işlemleri
        {
            toplam = new double[veri.GetLength(1)];
            normalizetablosu = new double[veri.GetLength(0)+1, veri.GetLength(1)];

            for (int i = 0; i < toplam.Length; i++)
            {
                toplam[i] = 0;
            }
            
            for (int i = 0; i < veri.GetLength(1); i++)
            {
                for (int j = 0; j < veri.GetLength(0); j++)
                {
                    normalizetablosu[j, i] = veri[j, i] / makstablomaks[i];
                }
            }

            

            for (int i = 0; i < veri.GetLength(1); i++)
            {
                for (int j = 0; j < veri.GetLength(0); j++)
                {
                    toplam[i] += normalizetablosu[j, i];
                }
            }

            for (int i = 0; i < kriterler.Count; i++)
            {
                normalizetablosu[alternatifler.Count, i] = toplam[i];
            }

            
            string[] normalizealternatifler = new string[alternatifler.Count + 1];

            for (int i = 0; i < alternatifler.Count; i++)
            {
                normalizealternatifler[i] = alternatifler[i].ToString();
            }

            normalizealternatifler[alternatifler.Count] = "TOPLAM";

            

        }

        private void button5_Click(object sender, EventArgs e) // ln tablosu işlemleri 
        {
            
        }

        private void button11_Click(object sender, EventArgs e) // gözat butonu
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Entropi İçin Excel Dosyası Seçin";
            fdlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            fdlg.Filter = "Excel Dosyaları(.xlsx)| *.xlsx| Excel 2003 Dosyaları(.xls)|*.xls";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                //MessageBox.Show(fdlg.FileName);
                string connStr = "";
                OleDbConnection baglanti = null;
                DataTable dt = null;
                if(Path.GetExtension(fdlg.FileName) == ".XLS" || Path.GetExtension(fdlg.FileName) == ".xls")
                {
                    connStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fdlg.FileName + ";Extended Properties=\"Excel 8.0;HDR=No;IMEX=1\";";
                }
                else
                {
                    connStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source="+ fdlg.FileName + ";Extended Properties='Excel 12.0;HDR=yes'";
                }

                baglanti = new OleDbConnection(@connStr);
                baglanti.Open();
                string sorgu = "SELECT * FROM [Sayfa1$]";
                OleDbDataAdapter da = new OleDbDataAdapter(sorgu, baglanti);
                dt = new DataTable();
                da.Fill(dt);
                dataGridView5.DataSource = dt;
                baglanti.Close();

                excelyazdir(dataGridView5);
            }
        }

        private void dataGridView6_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        public void excelyazdir(DataGridView grid) // excel veri okuma
        {

            int sutunsay = 0;
            int satirsay = 0;
            for (int i = 1; i < grid.Columns.Count; i++)
            {
                if(!string.IsNullOrEmpty(grid.Rows[0].Cells[i].Value as string))
                {
                    sutunsay++;
                }
            }

            for (int i = 1; i < grid.Rows.Count; i++)
            {
                if(!string.IsNullOrEmpty(grid.Rows[i].Cells[0].Value as string))
                {
                    satirsay++;
                }
            }
            veri = new double[satirsay, sutunsay];

            for (int i = 1; i <= satirsay; i++)
            {
                alternatifler.Add(grid.Rows[i].Cells[0].Value);
            }

            for (int i = 1; i <= sutunsay; i++)
            {
                kriterler.Add(grid.Rows[0].Cells[i].Value);
            }

            for (int i = 0; i < satirsay; i++)
            {
                listBox1.Items.Add(alternatifler[i]);
            }

            for (int i = 0; i < sutunsay; i++)
            {
                listBox2.Items.Add(kriterler[i]);
            }

            for (int i = 1; i <= satirsay; i++)
            {
                for (int j = 1; j <= sutunsay; j++)
                {
                        veri[i - 1, j - 1] = Convert.ToDouble(grid.Rows[i].Cells[j].Value);
                }
            }

          
        }

        private void dataGridView5_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e) // Aras tablosu normalize etme butonu
        {

            // 1/değerler

            arasnormalize();

            arasbolum = new double[alternatifler.Count, kriterler.Count];

            for (int i = 0; i < alternatifler.Count; i++)
            {
                for (int j = 0; j < kriterler.Count; j++)
                {
                    arasbolum[i, j] = arasbirbolu[i, j] / arastoplam[j];
                }
            }

            // Ağırlık çarpımı

            arasagirlikcarpilmis = new double[veri.GetLength(0), veri.GetLength(1)];

            for (int i = 0; i < veri.GetLength(1); i++)
            {
                for (int j = 0; j < veri.GetLength(0); j++)
                {
                    arasagirlikcarpilmis[j, i] = arasbolum[j, i] * agirlik[i];
                }
            }

            arastoplanmis = new double[veri.GetLength(0), veri.GetLength(1) + 2];
            for (int i = 0; i < veri.GetLength(1); i++)
            {
                for (int j = 0; j < veri.GetLength(0); j++)
                {
                    arastoplanmis[j, i] = arasagirlikcarpilmis[j, i];
                }
            }
            double top = 0;
            for (int i = 0; i < veri.GetLength(0); i++)
            {
                top = 0;
                for (int j = 0; j < veri.GetLength(1); j++)
                {
                    top += arastoplanmis[i, j];
                }
                arastoplanmis[i, 4] = top;
            }

            double enbuyuk = arastoplanmis[0, 4];

            for (int i = 1; i < veri.GetLength(0); i++)
            {
                if (arastoplanmis[i, 4] > enbuyuk)
                {
                    enbuyuk = arastoplanmis[i, 4];
                }
            }

            for (int i = 0; i < veri.GetLength(0); i++)
            {
                arastoplanmis[i, 4] = arastoplanmis[i, 4] / enbuyuk;
            }

            ///////

            double[] anasira = new double[veri.GetLength(0)];
            double[] sira = new double[veri.GetLength(0)];

            for (int i = 0; i < veri.GetLength(0); i++)
            {
                anasira[i] = arastoplanmis[i, 4];
                sira[i] = arastoplanmis[i, 4];
            }

            // Aras İçin Baloncuk sıralaması
            double gecici = 0;
            for (int i = 0; i < sira.Length; i++)
            {
                for (int j = i; j < sira.Length; j++)
                {
                    if (sira[i] < sira[j])
                    {
                        gecici = sira[i];
                        sira[i] = sira[j];
                        sira[j] = gecici;
                    }
                }
            }

            double[] siralama = new double[veri.GetLength(0)];
            for (int i = 0; i < veri.GetLength(0); i++)
            {
                for (int j = 0; j < veri.GetLength(0); j++)
                {
                    if (sira[i] == anasira[j])
                    {
                        siralama[j] = i + 1;
                        break;
                    }
                }
            }

            for (int i = 0; i < veri.GetLength(0); i++)
            {
                arastoplanmis[i, 5] = siralama[i];
            }



            
            arasgriddoldur(dataGridView13, arastoplanmis);
            
            tabControl1.SelectedIndex = 3;
            
        }

        public void arasnormalizeyaz(DataGridView grid, Double[,] veriler) // verilen tabloyu istenilen grid e yaz
        {
            grid.ColumnCount = kriterler.Count;
            for (int i = 0; i < kriterler.Count; i++)
            {
                grid.Columns[i].Name = kriterler[i].ToString();
            }
            for (int i = 0; i < alternatifler.Count; i++)
            {
                var row = new DataGridViewRow();
                for (int j = 0; j < kriterler.Count; j++)
                {
                    row.Cells.Add(new DataGridViewTextBoxCell()
                    {
                        Value = veriler[i, j]
                    });
                }
                grid.Rows.Add(row);
            }

            for (int i = 0; i < alternatifler.Count; i++)
            {
                grid.Rows[i].HeaderCell.Value = alternatifler[i];
            }
            
        }

        public void arasgriddoldur(DataGridView grid, Double[,] veriler) // verileri istenen grid e dolduran metot
        {
            grid.Rows.Clear();
            grid.Columns.Clear();
            for (int i = 0; i < kriterler.Count; i++)
            {
                grid.Columns.Add(i.ToString(), kriterler[i].ToString());
            }

            grid.Columns.Add("4","Toplam/İlk");
            grid.Columns.Add("5", "Sıra");

            for (int i = 0; i < alternatifler.Count; i++)
            {
                var row = new DataGridViewRow();
                for (int j = 0; j < kriterler.Count+2; j++)
                {
                    row.Cells.Add(new DataGridViewTextBoxCell()
                    {
                        Value = veriler[i, j]
                    });
                }
                grid.Rows.Add(row);
            }

            for (int i = 0; i < alternatifler.Count; i++)
            {
                grid.Rows[i].HeaderCell.Value = alternatifler[i].ToString();
            }
        }


        public double[,] arasagirlikcarpilmis, arastoplanmis;
        public double[] aatoplam, toplambolum;
        private void button2_Click(object sender, EventArgs e)// aras ağırlık çarp
        {
            
        }

        public void arasnormalize() // aras için verileri normaize et ilk adım
        {
            arasbirbolu = new double[alternatifler.Count, kriterler.Count];

            for (int i = 0; i < alternatifler.Count; i++)
            {
                for (int j = 0; j < kriterler.Count; j++)
                {
                    arasbirbolu[i, j] = veri[i, j];
                }
            }

            arastoplam = new double[kriterler.Count];

            for (int i = 0; i < arastoplam.Length; i++)
            {
                arastoplam[i] = 0;
            }

            for (int i = 0; i < kriterler.Count; i++)
            {
                for (int j = 0; j < alternatifler.Count; j++)
                {
                    arastoplam[i] += arasbirbolu[j, i];
                }
            }

            

        }

        public void arasbaslik(DataGridView grid) // grid basliklarını yaz
        {
            grid.Columns.Clear();
            grid.ColumnCount = kriterler.Count;
            for (int i = 0; i < kriterler.Count; i++)
            {
                grid.Columns[i].Name = kriterler[i].ToString() + " (" + agirlik[i].ToString() + ")";
            }
        }

        public void aras(DataGridView grid, double[,] veriler) // verilen tabloyu grid e yaz aras için
        {
            grid.Rows.Clear();
            for (int i = 0; i < alternatifler.Count; i++)
            {
                var row = new DataGridViewRow();
                for (int j = 0; j < kriterler.Count; j++)
                {
                    row.Cells.Add(new DataGridViewTextBoxCell()
                    {
                        Value = veriler[i, j]
                    });
                }
                grid.Rows.Add(row);
            }

            for (int i = 0; i < alternatifler.Count; i++)
            {
                grid.Rows[i].HeaderCell.Value = alternatifler[i];
            }
        }

        private void button9_Click(object sender, EventArgs e) // aras için verileri yaz butonu
        {
            arasbaslik(dataGridView7);
            aras(dataGridView7, veri);
            tabControl1.SelectedIndex = 2;
        }

        private String[] GetExcelSheetNames(string excelFile) // excel oku
        {
            OleDbConnection objConn = null;
            System.Data.DataTable dt = null;

            try
            {
                // Connection String. Change the excel file to the file you
                // will search.
                String connString = "Provider=Microsoft.Jet.OLEDB.4.0;" +
                "Data Source=" + excelFile + ";Extended Properties=Excel 8.0;";

                // Create connection object by using the preceding connection string.
                objConn = new OleDbConnection(connString);
                // Open connection with the database.
                objConn.Open();
                // Get the data table containg the schema guid.
                dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                if (dt == null)
                {
                    return null;
                }

                String[] excelSheets = new String[dt.Rows.Count];
                int i = 0;

                // Add the sheet name to the string array.
                foreach (DataRow row in dt.Rows)
                {
                    excelSheets[i] = row["TABLE_NAME"].ToString();
                    i++;
                }

                // Loop through all of the sheets if you want too...
                for (int j = 0; j < excelSheets.Length; j++)
                {
                    // Query each excel sheet.
                }

                return excelSheets;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                // Clean up.
                if (objConn != null)
                {
                    objConn.Close();
                    objConn.Dispose();
                }
                if (dt != null)
                {
                    dt.Dispose();
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e) // datagridview5 i gizle
        {
            dataGridView5.Visible = false;
        }

        private void button6_Click(object sender, EventArgs e) // sonuc tablosu işlemleri
        {
            
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {


        }


       


        private void button7_Click(object sender, EventArgs e) // snormalize metodunu çalıştır
        {
            snormalize();
            agirliktoplam();
        }

        // KARELERİNİ AL
        public void snormalize() 
        {
            kareli = new double[alternatifler.Count,kriterler.Count];

            for (int i = 0; i < alternatifler.Count; i++)
            {
                for (int j = 0; j < kriterler.Count; j++)
                {
                    kareli[i, j] = Math.Pow(veri[i,j],2);
                }
            }

            karelertop = new double[kriterler.Count];

            for (int i = 0; i < karelertop.Length; i++)
            {
                karelertop[i] = 0;
            }

            for (int i = 0; i < kriterler.Count; i++)
            {
                for (int j = 0; j < alternatifler.Count; j++)
                {
                    karelertop[i] += kareli[j, i];
                }
            }

            karekok = new double[kriterler.Count];

            for (int i = 0; i < karekok.Length; i++)
            {
                karekok[i] = Math.Sqrt(karelertop[i]);
            }



            normalizeedilmis = new double[alternatifler.Count, kriterler.Count];

            for (int i = 0; i < kriterler.Count; i++)
            {
                for (int j = 0; j < alternatifler.Count; j++)
                {
                    normalizeedilmis[j,i] = veri[j,i] / karekok[i] * agirlik[i];
                }
            }

            ntoplam = new double[alternatifler.Count];
            for (int i = 0; i < ntoplam.Length; i++)
            {
                ntoplam[i] = 0;
            }

            for (int i = 0; i < alternatifler.Count; i++)
            {
                for (int j = 0; j < kriterler.Count; j++)
                {
                    ntoplam[i] += normalizeedilmis[i,j];
                }
            }

            sira = new double[alternatifler.Count];
            for (int i = 0; i < sira.Length; i++)
            {
                sira[i] = ntoplam[i];
            }

            double[] sira2 = new double[alternatifler.Count];
            for (int i = 0; i < sira2.Length; i++)
            {
                sira2[i] = sira[i];
            }

            // Baloncuk sıralaması
            double gecici = 0;
            for (int i = 0; i < sira.Length; i++)
            {
                for (int j = i; j < sira.Length; j++)
                {
                    if (sira[i] < sira[j])
                    {
                        gecici = sira[i];
                        sira[i] = sira[j];
                        sira[j] = gecici;
                    }
                }
            }

            double[] siralama = new double[alternatifler.Count];
            for (int i = 0; i < sira.Length; i++)
            {
                for (int j = 0; j < sira2.Length; j++)
                {
                    if(sira[i] == sira2[j])
                    {
                        siralama[j] = i+1;
                        break;
                    }
                }
            }

            sontablo = new double[alternatifler.Count, kriterler.Count+2];

            for (int i = 0; i < alternatifler.Count; i++)
            {
                for (int j = 0; j < kriterler.Count; j++)
                {
                    sontablo[i, j] = normalizeedilmis[i, j];
                }
            }

            for (int i = 0; i < alternatifler.Count; i++)
            {
                sontablo[i, kriterler.Count] = ntoplam[i];
            }

            for (int i = 0; i < alternatifler.Count; i++)
            {
                sontablo[i, kriterler.Count+1] = siralama[i];
            }

            

            tabControl1.SelectedIndex = 4;
        }

        public void lnoncesibul() // LN
        {
            lnoncesi = new double[veri.GetLength(0), veri.GetLength(1)];

            for (int i = 0; i < veri.GetLength(1); i++)
            {
                for (int j = 0; j < veri.GetLength(0); j++)
                {
                    lnoncesi[j, i] = normalizetablosu[j, i] / toplam[i];
                }
            }

            


            
        }

        public void ln() // ln hesaplamaları (log)
        {
        	lntablosu = new double[alternatifler.Count, kriterler.Count];

        	for (int i = 0; i < alternatifler.Count; i++)
            {
                for (int j = 0; j < kriterler.Count; j++)
                {
                    lntablosu[i,j] = Math.Log(lnoncesi[i, j])*lnoncesi[i,j];
                }
            }

           

            
        }

        public void carp()
        {
        	
        }

        public void sutuntoplam() // ln tablosu sutunlarını topla
        {
        	stoplam = new double[kriterler.Count];

            for (int i = 0; i < stoplam.Length; i++)
            {
                stoplam[i] = 0;
            }

            for (int i = 0; i < kriterler.Count; i++)
            {
                for (int j = 0; j < alternatifler.Count; j++)
                {
                    stoplam[i] += lntablosu[j,i];
                }
            }


        }

        public void kbul() // k burada bulunuyor
        {
        	ksayisi = 1 / Math.Log(alternatifler.Count);
            label2.Text = "K Sayısı: " + ksayisi.ToString();
        }

        public void kcarp() // ej değer burada bulunuyor
        {
        	ej = new double[kriterler.Count];
        	for(int i = 0; i < stoplam.Length; i++)
        	{
        		ej[i] = -1 * stoplam[i] * ksayisi; 
        	}

        }

        public void bireksiejbul() // ej 1 - olarak bul
        {
        	bireksiej = new double[kriterler.Count];

        	for(int i = 0; i < ej.Length; i++)
        	{
        		bireksiej[i] = 1 - ej[i];
        	}
        }

        public void tejbul() // toplam ej
        {
        	tej = 0;
        	for(int i = 0; i < bireksiej.Length; i++)
        	{
        		tej += bireksiej[i];
        	}
        }

        public void agirlikbul() // agirlik bul
        {
        	agirlik = new double[kriterler.Count];

            for (int i = 0; i < agirlik.Length; i++)
            {
                agirlik[i] = 0;
            }

            for (int i = 0; i < agirlik.Length; i++)
        	{
        		agirlik[i] = bireksiej[i]/ tej;
        	}
            tekligriddoldur(dataGridView8, agirlik);
        }

        public void agirliktoplam() // agirlik toplamları
        {
            // label5
            double toplam = 0;
            for (int i = 0; i < agirlik.Length; i++)
            {
                toplam += agirlik[i];
            }

            
        }
        
    }
}
